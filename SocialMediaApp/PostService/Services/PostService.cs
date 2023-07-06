using PostService.Responses.Models;
using PostService.Responses;
using PostService.Helpers;
using PostService.Data;
using Microsoft.EntityFrameworkCore;
using PostService.Entities;

namespace PostService.Services
{
    public class PostService : IPostService
    {
        private readonly PostDataContext dbContext;

        public PostService(PostDataContext socialDbContext)
        {
            this.dbContext = socialDbContext;
        }


        public async Task CreatePostAsync(Post post)
        {
            var postEntry = await dbContext.Posts.AddAsync(post);
            var saveResponse = await dbContext.SaveChangesAsync();
        }

        public List<Post> GetPostsByUserId(List<string> followedUsers, int page, int pageSize)
        {
            var posts = dbContext.Posts
                .Skip(page * pageSize)
                .Where(p => followedUsers
                .Contains(p.Username))
                .OrderByDescending(p => p.PublishTime)
                .Take(pageSize)
                .ToList();

            return posts;
        }

        public List<Post> GetProfilePosts(string username, int page, int pageSize)
        {
            var posts = dbContext.Posts
                .Where(p => p.Username == username)
                .Skip(page * pageSize)
                .OrderByDescending(p => p.PublishTime)
                .Take(pageSize)
                .ToList();

            return posts;
        }
    }
}
