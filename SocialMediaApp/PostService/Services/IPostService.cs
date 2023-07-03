using PostService.Requests;
using PostService.Responses.Models;
using PostService.Responses;

namespace PostService.Services
{
    public interface IPostService
    {
        Task SavePostImageAsync(PostRequest postRequest);


        Task<PostResponse> CreatePostAsync(PostRequest postRequest);



        
    }
}
