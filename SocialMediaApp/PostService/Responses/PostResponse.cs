using PostService.Responses.Models;

namespace PostService.Responses
{
    public class PostResponse : BaseResponse
    {
        public PostModel Post { get; set; }

    }
}
