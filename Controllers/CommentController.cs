using Active_Blog_Service.Models;
using Active_Blog_Service_API.Dto;
using Active_Blog_Service_API.Helpers;
using Active_Blog_Service_API.Repositories.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Active_Blog_Service_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly UserManager<User> _userManager;

        public CommentController(ICommentRepository commentRepository ,UserManager<User> userManager)
        {
            _commentRepository = commentRepository;
            _userManager = userManager;
        }
        [HttpGet("index")]
        public IActionResult Index([FromQuery]int blogId)
        {
            var comments = _commentRepository.GetCommentsOfBlogOrderByDateTime(blogId);
            return Ok(comments);
        }
        [HttpPost("addComment")]
        public async Task<IActionResult> AddComment([FromQuery] AddCommentDto addCommentDto)
        {
            if (ModelState.IsValid)
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                var claims = JwtHelper.GetClaimsFromJwt(authHeader);

                var claimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
                var email = claims.FirstOrDefault(c => c.Type == claimType)!.Value;

                Console.WriteLine($"\n\n\n{email}\n\n\n");
                var user = await _userManager.FindByEmailAsync(email);

                var comment = new Comment
                {
                    CommentContent = addCommentDto.CommentContent,
                    UserId = user.Id,
                    BlogId = addCommentDto.BlogId
                };
                _commentRepository.AddComment(comment);

                return Ok("Comment successfuly added!");
            }
            return BadRequest("Data is wrong try again!");
        }
        [HttpPost("editComment")]
        public IActionResult EditComment([FromQuery] EditCommentDto editCommentDto)
        {
            if (ModelState.IsValid)
            {
                _commentRepository.UpdateComment(editCommentDto);
                return Ok("Comment successfuly edited!");
            }
            return BadRequest("Data is wrong try again!");
        }
        [HttpPost("deleteComment")]
        public IActionResult DeleteComment([FromQuery] int blogId, [FromQuery] int commentId)
        {
            if (ModelState.IsValid)
            {
                _commentRepository.DeleteComment(blogId, commentId);
                return Ok("Comment successfuly deleted!");
            }
            return BadRequest("Data is wrong try again!");
        }
    }
}
