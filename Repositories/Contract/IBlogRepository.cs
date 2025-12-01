using Active_Blog_Service.Models;
using Active_Blog_Service_API.Dto;

namespace Active_Blog_Service_API.Repositories.Contract
{
    public interface IBlogRepository : IAddScoped
    {
        List<Blog> GetAllBlogs();
        Blog GetBlogById(int id);
        void AddBlog(Blog blog);
        Task UpdateBlogAsync(int id, EditBlogDto newBlog);
        void DeleteBlog(int id);
    }
}
