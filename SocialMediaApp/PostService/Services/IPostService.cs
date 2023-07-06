using PostService.Requests;
using PostService.Responses.Models;
using PostService.Responses;
using PostService.Entities;

namespace PostService.Services
{
    public interface IPostService
    {
        //Task SavePostImageAsync(PostRequest postRequest);


        Task<PostResponse> CreatePostAsync(PostRequest postRequest);
        List<Post> GetPostsByUserId(List<string> followedUsers);
        List<Post> GetProfilePosts(string username);





    }
}
