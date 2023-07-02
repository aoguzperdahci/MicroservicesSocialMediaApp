using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using Neo4jClient.Cypher;
using System.Xml.Linq;
using UserService.Entities;

namespace FollowerService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IGraphClient _graphClient;


        public WeatherForecastController(IGraphClient graphClient)
        {
            

            // Connect to the Neo4j database
            _graphClient = graphClient;
          
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var followers = await _graphClient.Cypher.Match("(n:User)")
                .Return(nameof => nameof.As<User>()).ResultsAsync;
            return Ok(followers);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User usr)
        {
            await _graphClient.Cypher.Create("(u:User $usr)")
                .WithParam("usr", usr)
                .ExecuteWithoutResultsAsync();

            return Ok();
        }

       [HttpDelete("{followerUsername}")]/*(Name = "GetWeatherForecast")*/
        public async Task<IActionResult> Unfollow(string followerUsername, string followeeUsername)
        {

            var query = _graphClient.Cypher
                .Match("(follower:User)-[r:is_following]->(followee:User)")
                .Where((User follower) => follower.Username == followerUsername)
                .AndWhere((User followee) => followee.Username == followeeUsername)
                .Delete("r");

            await query.ExecuteWithoutResultsAsync();

            return Ok();

        }


        [HttpPost]
        [Route("api/users/{followerUsername}/follow")]
        
        public async Task<IActionResult> FollowUser(string followerUsername, string followeeUsername)
        {
            var query = _graphClient.Cypher
    .Match("(follower:User {Username: $followerUsername})")
    .Match("(followee:User {Username: $followeeUsername})")
    .Create("(follower)-[:is_following]->(followee)");

            var parameters = new
            {
                followerUsername,
                followeeUsername
            };

            await query.WithParams(parameters).ExecuteWithoutResultsAsync();

            return Ok();

        }



        [HttpGet("{username}/isfollowing")]
        
        public async Task<ActionResult> IsFollowing(string username)
        {
            var query = _graphClient.Cypher
    .Match("(follower:User {Username: $username})-[:is_following]->(followed:User)")
    .WithParam("username", username)
    .Return(followed => followed.As<User>())
    .ResultsAsync;

            var results = await query;

            return Ok(results);

        }



        [HttpGet("{username}/followers")]
        public async Task<IActionResult> GetFollowers(string username)
        {
            var query =  _graphClient.Cypher
                .Match("(follower:User)-[:is_following]->(followed:User {Username: $username})")
                .WithParam("username", username)
                .Return(follower => follower.As<User>())
                .ResultsAsync;



            var results = await query;

            return Ok(results);
        }




    }
}



