using Active_Blog_Service.Models;
using Active_Blog_Service_API.Context;
using Active_Blog_Service_API.Dto;
using Active_Blog_Service_API.Repositories.Contract;
using Microsoft.AspNetCore.Identity;

namespace Active_Blog_Service_API.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly AppDbContext _context;

        public BlogRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Blog> GetAllBlogs()
        {
            return _context.Blogs.ToList();
        }

        public Blog GetBlogById(int id)
        {
            return _context.Blogs.FirstOrDefault(b => b.Id == id)!;
        }

        public void AddBlog(Blog blog)
        {
            _context.Blogs.Add(blog);
            _context.SaveChanges();
        }
        public async Task UpdateBlogAsync(int id, EditBlogDto newBlog)
        {
            Blog oldBlog = GetBlogById(id);
            oldBlog.Title = newBlog.Title??oldBlog.Title;
            oldBlog.BlogContent = newBlog.BlogContent??oldBlog.BlogContent;
            oldBlog.Category = newBlog.Category ?? oldBlog.Category;
            if (newBlog.Image is not null)
            {
                var uploadsfolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/BlogImages");
                var fileName = Path.GetFileName(newBlog.Image.FileName);
                var filePath = Path.Combine(uploadsfolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await newBlog.Image.CopyToAsync(stream);
                }
                oldBlog.Image = $"/BlogImages/{fileName}";
            }

            _context.SaveChanges();
        }
        public void DeleteBlog(int id)
        {
            Blog blog = GetBlogById(id);
            _context.Blogs.Remove(blog);
            _context.SaveChanges();
        }

    }
}
