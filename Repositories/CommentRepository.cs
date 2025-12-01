using Active_Blog_Service.Models;
using Active_Blog_Service_API.Context;
using Active_Blog_Service_API.Dto;
using Active_Blog_Service_API.Repositories.Contract;

namespace Active_Blog_Service_API.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Comment> GetCommentsOfBlogOrderByDateTime(int blogId)
        {
            return _context.Comments.Where(c => c.BlogId == blogId).OrderBy(c => c.CreatedDateTime).ToList();
        }

        public void AddComment(Comment comment)
        {
            _context.Comments.Add(comment);
            _context.SaveChanges();
        }
        public Comment GetSpecificComment(int blogId, int commentId)
        {
            return GetCommentsOfBlogOrderByDateTime(blogId).FirstOrDefault(c => c.Id == commentId)!;
        }
        public void UpdateComment(EditCommentDto editCommentDto)
        {
            var oldComment = GetSpecificComment(editCommentDto.BlogId, editCommentDto.CommentId);
            oldComment.CommentContent = editCommentDto.CommentContent;

            _context.SaveChanges();
        }
        public void DeleteComment(int blogId , int commentId)
        {
            var comment = GetSpecificComment(blogId, commentId);
            _context.Comments.Remove(comment);
            _context.SaveChanges();
        }
    }
}
