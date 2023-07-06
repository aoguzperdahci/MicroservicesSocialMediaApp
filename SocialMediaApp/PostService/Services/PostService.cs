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

        public List<Post> GetPostsByUserId(List<string> followedUsers)
        {
            var posts = dbContext.Posts.Where(p => followedUsers.Contains(p.Username)).OrderByDescending(p => p.PublisTime).ToList();
            return posts;
        }

        public List<Post> GetProfilePosts(string username)
        {
            var posts = dbContext.Posts
                .Where(p => p.Username == username)
                .OrderByDescending(p => p.PublisTime)
                .ToList();

            return posts;
        }
    }
}
