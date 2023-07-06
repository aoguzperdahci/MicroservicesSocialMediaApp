using PostService.Responses.Models;
using PostService.Responses;
using PostService.Entities;

namespace PostService.Services
{
    public interface IPostService
    {
        Task CreatePostAsync(Post post);
        List<Post> GetPostsByUserId(List<string> followedUsers);
        List<Post> GetProfilePosts(string username);
    }
}
