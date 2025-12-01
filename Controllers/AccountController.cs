using Active_Blog_Service.Models;
using Active_Blog_Service_API.Dto;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace Active_Blog_Service_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public AccountController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                { 
                    FName = registerDto.FName,
                    Email = registerDto.Email,
                    PasswordHash = registerDto.Password,
                    LName = registerDto.LName,
                    PhoneNumber  = registerDto.Phone,
                    Address = registerDto.Address,
                };
                user.UserName = registerDto.Email;

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserImages");
                var fileName = Path.GetFileName(registerDto.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using(var stream = new FileStream(filePath,FileMode.Create))
                {
                    await registerDto.ImageFile.CopyToAsync(stream);
                }

                user.Image = $"/UserImages/{filePath}";

                var result = await _userManager.CreateAsync(user,registerDto.Password);

                if (result.Succeeded)
                {
                    return Ok(result);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            ModelState.AddModelError("", "Data is worng try again!");
            return BadRequest(ModelState);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromQuery]LoginDto loginDto)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user != null)
                {
                    var found = await _userManager.CheckPasswordAsync(user,loginDto.Password);
                    if(found)
                    {
                        var config = WebApplication.CreateBuilder().Configuration;
                        //create token
                        var claims = new List<Claim>
                        {
                            new(ClaimTypes.Email,user.Email),
                            new(ClaimTypes.NameIdentifier , user.Id),
                            new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                        };

                        //get role
                        var roles = await _userManager.GetRolesAsync(user);
                        foreach(var role in roles)
                        {
                            claims.Add(new(ClaimTypes.Role, role));
                        }

                        SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SecurityKey"]!));

                        var singInCred = new SigningCredentials(
                            algorithm:SecurityAlgorithms.HmacSha256,
                            key: key
                            );
                        var token = new JwtSecurityToken(
                            issuer: config["JWT:issuer"],
                            audience: config["JWT:audience"],
                            claims: claims,
                            expires:DateTime.Now.AddDays(15),
                            signingCredentials: singInCred
                            );

                        return Ok(
                            new
                            {
                                token = new JwtSecurityTokenHandler().WriteToken(token),
                                expireDate =  token.ValidTo
                            }
                            );
                    }

                    return Unauthorized("Password Not correct!");
                }
                return Unauthorized("Not Found Account!");
            }
            return Unauthorized("There`s wrong data!");
        }


    }
}
