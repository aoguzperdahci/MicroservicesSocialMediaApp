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
        string apiGatawayUri = "http://localhost:700/api/Media";


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
                
            var responseContent = await response.Content.ReadAsStringAsync();

            var imgUrl = @"http://localhost:7080/" + username + "/" + filename;

            await postService.CreatePostAsync(new Post { Image = imgUrl, PublisTime = DateTime.Now, Username = username});

            return Ok();
        }

        [HttpGet("MainFeed/{username}")]
        public IActionResult GetMainFeed(string username)
        {
            string followerServiceUri = apiGatawayUri + "/FollowerService/api/";

            //HttpClient httpClient = new HttpClient();

            //HttpResponseMessage response = await httpClient.PostAsync(mediaServiceUri, multipartContent);

            //List<string> followedUsers = GetFollowers(username); // Kullanıcının takip ettiği kullanıcıları almak için bir metot kullanılabilir

            //var userPosts = postService.GetPostsByUserId(followedUsers);

            // Sayfalama işlemleri yapılır

            //return Ok(userPosts);
            return Ok();
        }

        [HttpGet("ProfileFeed/{username}")]
        public IActionResult ProfileFeed(string username)
        {
            var userPosts = postService.GetProfilePosts(username);

            // Sayfalama işlemleri yapılır

            return Ok(userPosts);
        }
    }
}
