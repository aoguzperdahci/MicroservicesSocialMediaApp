namespace PostService.Entities
{
    public class PostRequest
    {
        public IFormFile Image { get; set; }

        public string Description { get; set; }
    }
}
