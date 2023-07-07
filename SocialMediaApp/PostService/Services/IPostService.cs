using PostService.Responses.Models;
using PostService.Responses;
using PostService.Entities;

namespace PostService.Services
{
    public interface IPostService
    {
        Task CreatePostAsync(Post post);
        List<Post> GetPostsByUserId(List<string> followedUsers, int page, int pageSize);
        List<Post> GetProfilePosts(string username, int page, int pageSize);
        Task DeleteUserPostsAsync(string username);

    }
}
