using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PostService.Requests;
using PostService.Responses;
using PostService.Services;
using System.Net.Http;
using System.Text;

namespace PostService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ILogger<PostsController> logger;
        private readonly HttpClient _httpClient;

        private readonly IPostService postService;


        public PostsController(ILogger<PostsController> logger, IPostService postService, HttpClient httpClient)


        {


            this.logger = logger;

            _httpClient = httpClient;
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
                var requestData = new FormUrlEncodedContent(new[]
{
    new KeyValuePair<string, string>("username", username),
    new KeyValuePair<string, string>("filename", postRequest.ImagePath)
});
                /*
                                 * = new
                {
                    Username = username,
                    Filename = postRequest.ImagePath
                };*/
                //var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

                //var username = "John Doe";
                var filename = postRequest.ImagePath;

                var data = new { username, filename };
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                // Send the POST request to the Media service
                //var response = await _httpClient.PostAsJsonAsync("http://localhost:7040/swagger/index.html", requestData);
                string mediaServiceUri = "http://localhost:7040/swagger/index.html";

                    HttpClient httpClient = new HttpClient();
                    //HttpResponseMessage response = await httpClient.GetAsync(mediaServiceUri);
                    HttpResponseMessage response = await httpClient.PostAsync("http://localhost:7040/swagger/index.html", content);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                //var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

                // Send the POST request with the JSON data
                //HttpResponseMessage response = await httpClient.PostAsync(mediaServiceUri, jsonContent);


                // Check the response status
                response.EnsureSuccessStatusCode();

                    // Process the response if needed
                
            
                /*using (var httpClient = new HttpClient())
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
                }*/


                //await postService.SavePostImageAsync(postRequest);


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
