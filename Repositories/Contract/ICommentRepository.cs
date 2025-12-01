using Active_Blog_Service.Models;
using Active_Blog_Service_API.Dto;

namespace Active_Blog_Service_API.Repositories.Contract
{
    public interface ICommentRepository : IAddScoped
    {
        List<Comment> GetCommentsOfBlogOrderByDateTime(int blogId);
        void AddComment(Comment comment);
        Comment GetSpecificComment(int blogId, int commentId);
        void UpdateComment(EditCommentDto editCommentDto);
        void DeleteComment(int blogId, int commentId);

    }
}
