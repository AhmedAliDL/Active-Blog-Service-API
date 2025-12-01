using Active_Blog_Service.Models;
using Active_Blog_Service_API.Dto;
using Active_Blog_Service_API.Helpers;
using Active_Blog_Service_API.Repositories.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace Active_Blog_Service_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BlogController : ControllerBase
    {
        private readonly IBlogRepository _blogRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly UserManager<User> _userManager;

        public BlogController(IBlogRepository blogRepository, ICommentRepository commentRepository, UserManager<User> userManager)
        {
            _blogRepository = blogRepository;
            _commentRepository = commentRepository;
            _userManager = userManager;
        }
        [HttpGet("index")]
        public IActionResult Index()
        {
            var blogs = _blogRepository.GetAllBlogs();

            return Ok(blogs);
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddBlog([FromForm] AddBlogDto addBlogDto)
        {
            if (ModelState.IsValid)
            {
                Blog blog = new()
                {
                    Title = addBlogDto.Title,
                    Category = addBlogDto.Category,
                    BlogContent = addBlogDto.BlogContent
                };
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                var claims = JwtHelper.GetClaimsFromJwt(authHeader);

                var claimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
                var email = claims.FirstOrDefault(c => c.Type == claimType)!.Value;

                var user = await _userManager.FindByEmailAsync(email);
                blog.UserId = user.Id;

                var uploadsfolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/BlogImages");
                var fileName = Path.GetFileName(addBlogDto.Image.FileName);
                var filePath = Path.Combine(uploadsfolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await addBlogDto.Image.CopyToAsync(stream);
                }

                blog.Image = $"/BlogImages/{fileName}";

                _blogRepository.AddBlog(blog);

                return Ok("Blog Added Successfuly!");
            }
            return BadRequest("Data is wrong try again later!");


        }
        [HttpPost("edit")]
        public async Task<IActionResult> EditBlog([FromQuery] int id, [FromForm] EditBlogDto editBlogDto)
        {
            if (ModelState.IsValid)
            {

                await _blogRepository.UpdateBlogAsync(id, editBlogDto);

                return Ok("Blog Edited Successfuly!");
            }
            return BadRequest("Data is wrong try again later!");

        }
        [HttpGet("details")]
        public async Task<IActionResult> BlogDetail([FromQuery] int id)
        {
            var blog = _blogRepository.GetBlogById(id);
            var blogDetails = new BlogDetailsDto();
            if (blog != null)
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                var claims = JwtHelper.GetClaimsFromJwt(authHeader);

                var claimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
                var email = claims.FirstOrDefault(c => c.Type == claimType)!.Value;

                var user = await _userManager.FindByEmailAsync(email);

                blogDetails.Title = blog.Title;
                blogDetails.CreatedDate = blog.CreatedDate;
                blogDetails.Category = blog.Category;
                blogDetails.BlogImage = blog.Image;
                blogDetails.BlogContent = blog.BlogContent;
                var comments = _commentRepository.GetCommentsOfBlogOrderByDateTime(id);
                var commentsDto = new List<CommentDetailsDto>();
                foreach (var comment in comments)
                {
                    var commentUser = await _userManager.FindByIdAsync(comment.UserId);
                    commentsDto.Add(
                            new CommentDetailsDto
                            {
                                UserName = $"{commentUser.FName} {commentUser.LName}",
                                CommentContent = comment.CommentContent,
                                UserImage = commentUser.Image,
                                CommentDate = comment.CreatedDateTime
                            }

                        );
                }
                blogDetails.BlogComments = commentsDto;
                blogDetails.UserImage = user.Image;
                blogDetails.UserName = $"{user.FName} {user.LName}";

            }
            return Ok(blogDetails);
        }
        [HttpPost("delete")]
        public IActionResult DeleteBlog([FromQuery] int id)
        {
            try
            {
                _blogRepository.DeleteBlog(id);
                return Ok("Blog deleted successfuly!");
            }
            catch
            {
                return BadRequest("Data is wrong try again!");
            }
        }


    }
}
