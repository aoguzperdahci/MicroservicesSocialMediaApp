using FollowerService.Consumers;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using Neo4jClient.Cypher;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Xml.Linq;
using UserService.Entities;
using System.Text;
using System.Linq;


namespace FollowerService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly IGraphClient _graphClient;


        public WeatherForecastController(IGraphClient graphClient, UserCreatedEventConsumer userCreatedEventConsumer)
        {
            

            // Connect to the Neo4j database
            _graphClient = graphClient;
            

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
                .Return(followed => followed.As<User>().Username)
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
                .Return(follower => follower.As<User>().Username)
                .ResultsAsync;



            var results = await query;

            

            return Ok(results);
        }




    }
}



