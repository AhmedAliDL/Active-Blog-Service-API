using Active_Blog_Service_API.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Runtime.CompilerServices;
using Active_Blog_Service_API.Helpers;

namespace Active_Blog_Service_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private static Dictionary<string, DateTime> userCooldowns = new();
        private readonly IConfiguration _configuration;

        public ContactController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("contact")]
        public async Task<IActionResult> sendMessage([FromQuery] ContactDto contactDto)
        {
            if (ModelState.IsValid)
            {

                string fromEmail = _configuration["CompanyInfo:email"]!;
                string fromPassword = _configuration["CompanyInfo:password"]!;

                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                var claims = JwtHelper.GetClaimsFromJwt(authHeader);

                var claimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
                var email = claims.FirstOrDefault(c => c.Type == claimType)!.Value;

                var toEmailAddress = email;

                // Check cooldown
                if (userCooldowns.TryGetValue(toEmailAddress, out var cooldownEndTime))
                {
                    if (DateTime.UtcNow < cooldownEndTime)
                    {
                        var remainingCooldown = cooldownEndTime - DateTime.UtcNow;
                        return BadRequest(new { success = false, message = "Cooldown active", remainingTime = remainingCooldown.TotalMilliseconds });
                    }
                }

                // Set new cooldown
                var timeOfMinutes = TimeSpan.FromMinutes(1);
                userCooldowns[toEmailAddress] = DateTime.UtcNow.Add(timeOfMinutes);

                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(fromEmail, fromPassword)
                };

                await client.SendMailAsync(
                     new MailMessage(from: fromEmail,
                     to: toEmailAddress,
                     subject: contactDto.MessageTitle,
                     body: contactDto.MessageBody
                     ));

                return Ok(new { success = true });
            }
            return BadRequest("Data is wrong try again!");
        }
    }
}
