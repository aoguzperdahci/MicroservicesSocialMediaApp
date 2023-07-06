using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PostService.Entities;
using PostService.Helpers;
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
                var filename = FileHelper.GetUniqueFileName(postRequest.Image.FileName);
                var requestData = new FormUrlEncodedContent(new[]
{
    new KeyValuePair<string, string>("username", username),
    new KeyValuePair<string, string>("filename", filename),
    //postRequest.Image

});

                

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


                // Check the response status
                response.EnsureSuccessStatusCode();




                //await postService.SavePostImageAsync(postRequest);


            }


            var postResponse = await postService.CreatePostAsync(postRequest);


            if (!postResponse.Success)


            {


                return NotFound(postResponse);


            }


            return Ok(postResponse.Post);


        }

        /*[HttpGet("MainFeed/{username}")]
        public IActionResult GetMainFeed(string username)
        {
            // Kullanıcının takip ettiği kullanıcıların postlarını almak için gerekli işlemler yapılır
            //follower servisten alınabilir?
            var followedUsers = GetFollowers(username); // Kullanıcının takip ettiği kullanıcıları almak için bir metot kullanılabilir

            // Takip edilen kullanıcıların postlarından oluşan bir liste elde edilir
            var allPosts = new List<Post>();
            foreach (var user in followedUsers)
            {
                var userPosts = postService.GetPostsByUserId(user); // Kullanıcının postlarını almak için bir metot kullanılabilir
                allPosts.AddRange(userPosts);
            }

            // Postları en yeni olandan en eskiye doğru sıralar
            var sortedPosts = allPosts.OrderByDescending(p => p.Ts);

            // Sayfalama işlemleri yapılır
            

            return Ok(sortedPosts);
        }*/

    }
}
