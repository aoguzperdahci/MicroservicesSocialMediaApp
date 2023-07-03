using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using PostService.Requests;
using PostService.Responses;
using PostService.Services;

namespace PostService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ILogger<PostsController> logger;


        private readonly IPostService postService;


        public PostsController(ILogger<PostsController> logger, IPostService postService)


        {


            this.logger = logger;


            this.postService = postService;


        }


        [HttpPost]


        [Route("")]


        [RequestSizeLimit(5 * 1024 * 1024)]


        public async Task<IActionResult> SubmitPost([FromForm] PostRequest postRequest)


        {


            if (postRequest == null)


            {


                return BadRequest(new PostResponse { Success = false, ErrorCode = "S01", Error = "Invalid post request" });


            }


            if (string.IsNullOrEmpty(Request.GetMultipartBoundary()))


            {


                return BadRequest(new PostResponse { Success = false, ErrorCode = "S02", Error = "Invalid post header" });


            }


            if (postRequest.Image != null)


            {


                await postService.SavePostImageAsync(postRequest);


            }


            var postResponse = await postService.CreatePostAsync(postRequest);


            if (!postResponse.Success)


            {


                return NotFound(postResponse);


            }


            return Ok(postResponse.Post);


        }

    }
}
