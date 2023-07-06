using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PostService.Entities;
using PostService.Helpers;
using PostService.Responses;
using PostService.Services;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;

namespace PostService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IPostService postService;
        string apiGatawayUri = "http://localhost:7000";


        public PostsController(IPostService postService)
        {
            this.postService = postService;
        }

        [HttpPost]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> SubmitPost([FromForm] PostRequest post)
        {
            var username = HttpContext.GetUserId();
            var filename = FileHelper.GetUniqueFileName(post.Image.FileName);

            using MultipartFormDataContent multipartContent = new();
            multipartContent.Add(new StringContent(username, Encoding.UTF8, MediaTypeNames.Text.Plain), "Username");
            multipartContent.Add(new StringContent(filename, Encoding.UTF8, MediaTypeNames.Text.Plain), "Filename");

            byte[] data;
            using (var br = new BinaryReader(post.Image.OpenReadStream()))
                data = br.ReadBytes((int)post.Image.OpenReadStream().Length);
            
            ByteArrayContent bytes = new ByteArrayContent(data);
            multipartContent.Add(bytes, "file", "File");

            string mediaServiceUri = apiGatawayUri + "/MediaService/api/Media";

            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.PostAsync(mediaServiceUri, multipartContent);
                
            var imgUrl = @"http://localhost:7080/" + username + "/" + filename;

            await postService.CreatePostAsync(new Post { Image = imgUrl, PublishTime = DateTime.Now, Username = username});

            return Ok();
        }

        [HttpGet("main-feed")]
        public async Task<IActionResult> GetMainFeedAsync([FromQuery] int page, [FromQuery] int pageSize)
        {
            var username = HttpContext.GetUserId();
            string followerServiceUri = apiGatawayUri + "/FollowerService/api/User/following/" + username;

            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.GetAsync(followerServiceUri);

            var content = await response.Content.ReadAsStringAsync();

            List<string> followedUsers = JsonConvert.DeserializeObject<List<string>>(content);

            var userPosts = postService.GetPostsByUserId(followedUsers, page, pageSize);

            return Ok(userPosts);
        }

        [HttpGet("profile-feed/{username}")]
        public IActionResult ProfileFeed(string username, [FromQuery] int page, [FromQuery] int pageSize)
        {
            var userPosts = postService.GetProfilePosts(username, page, pageSize);
            return Ok(userPosts);
        }
    }
}
