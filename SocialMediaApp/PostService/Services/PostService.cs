using PostService.Requests;
using PostService.Responses.Models;
using PostService.Responses;
using PostService.Helpers;
using PostService.Data;
using Microsoft.EntityFrameworkCore;
using PostService.Entities;

namespace PostService.Services
{
    public class PostService : IPostService
    {
        private readonly PostDataContext socialDbContext;


        private readonly IWebHostEnvironment environment;


        public PostService(PostDataContext socialDbContext, IWebHostEnvironment environment)


        {


            this.socialDbContext = socialDbContext;


            this.environment = environment;


        }


        public async Task<PostResponse> CreatePostAsync(PostRequest postRequest)


        {


            var post = new Entities.Post


            {
                Id = 1,

                Username = postRequest.Username,


                Description = postRequest.Description,


                Imagepath = postRequest.ImagePath,


                Ts = DateTime.Now,


                Published = true


            };
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;


            // Post=> Posts
            var postEntry = await socialDbContext.Posts.AddAsync(post);


            var saveResponse = await socialDbContext.SaveChangesAsync();


            if (saveResponse < 0)


            {


                return new PostResponse { Success = false, Error = "Issue while saving the post", ErrorCode = "CP01" };


            }


            var postEntity = postEntry.Entity;


            var postModel = new PostModel


            {


                Id = postEntity.Id,


                Description = postEntity.Description,


                Ts = postEntity.Ts,


                Imagepath = Path.Combine(postEntity.Imagepath),


                Username = postEntity.Username


            };


            return new PostResponse { Success = true, Post = postModel };


        }
        public List<Post> GetPostsByUserId(List<string> followedUsers)
        {
            var posts = socialDbContext.Posts.Where(p => followedUsers.Contains(p.Username)).OrderByDescending(p => p.Ts).ToList();

            return posts;
        }

        public List<Post> GetProfilePosts(string username)
        {
            var posts = socialDbContext.Posts
    .Where(p => p.Username == username)
    .OrderByDescending(p => p.Ts)
    .ToList();



            return posts;
            
        }

        /* public async Task SavePostImageAsync(PostRequest postRequest)


         {


             var uniqueFileName = FileHelper.GetUniqueFileName(postRequest.Image.FileName);
             var uploads = Path.Combine(environment.WebRootPath, "users", "posts", postRequest.UserId.ToString());

             var filePath = Path.Combine(uploads, uniqueFileName);

             Directory.CreateDirectory(Path.GetDirectoryName(filePath));


             await postRequest.Image.CopyToAsync(new FileStream(filePath, FileMode.Create));

             postRequest.ImagePath = filePath;


             return;


         }*/
    }
}
