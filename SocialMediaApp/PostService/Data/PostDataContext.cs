using Microsoft.EntityFrameworkCore;
using PostService.Entities;

namespace PostService.Data
{
    public class PostDataContext: DbContext 
    {
        public PostDataContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=localhost; Initial Catalog=Posts; Integrated Security=true");
        }
    }
}
