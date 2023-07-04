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


        [Route("{username}")]


        [RequestSizeLimit(5 * 1024 * 1024)]


        public async Task<IActionResult> SubmitPost([FromForm] PostRequest postRequest, string username)


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
               

                // Send a request to the Media service to save the selected image
                using (var httpClient = new HttpClient())
                {
                    var mediaServiceUrl = "http://localhost:7040/api/media"; // Replace with the actual Media service URL
                    var formData = new MultipartFormDataContent();
                    formData.Add(new StringContent(postRequest.ImagePath),"file");
                    formData.Add(new StringContent(username), "username");

                    var response = await httpClient.PostAsync($"{mediaServiceUrl}/api/media", formData);

                    if (!response.IsSuccessStatusCode)
                    {
                        return StatusCode((int)response.StatusCode, "Error occurred while saving the image in the Media service.");
                    }
                }


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
