using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using Neo4jClient.Cypher;
using UserService.Entities;

namespace FollowerService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IGraphClient _graphClient;



        //private readonly ILogger<WeatherForecastController> _logger;
        //private readonly IGraphClient _client;

        public WeatherForecastController(IGraphClient graphClient)
        {
            //_logger = logger;

            // Connect to the Neo4j database
            _graphClient = graphClient;
            //_client = client;
        }

        [HttpGet]/*(Name = "GetWeatherForecast")*/
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

        [HttpDelete("{username}")]/*(Name = "GetWeatherForecast")*/
        public async Task<IActionResult> Delete(string usrName)
        {
            await _graphClient.Cypher.Create("(u:User)")
                .Where((User u) => u.Username == usrName)
                .Delete("u")
                .ExecuteWithoutResultsAsync();

            return Ok();
        }
        public async Task<IActionResult> FollowUser(string followerUsername, string followedUsername)
        {
            await _graphClient.Cypher
    .Match("(follower:User)", "(followed:User)")
        .Where((User follower) => follower.Username == followerUsername)
        .AndWhere((User followed) => followed.Username == followedUsername)
        .CreateUnique("(follower)-[:IS_FOLLOWING]->(followed)")
        .ExecuteWithoutResultsAsync();
            return Ok();
        }

    [HttpGet("{username}/isfollowing")]

        public async Task<IActionResult> GetFollowing(string username)
        {
            //MATCH(follower: User) -[:IS_FOLLOWING]->(user: User { name: 'Jane Smith'}) RETURN follower
            var query = _graphClient.Cypher
                .Match("(follower:User {Username: $username})-[:IS_FOLLOWING]->(followed:User)")
                .Return(followed => followed.As<User>())
                .ResultsAsync;

            return Ok(query);
        }

        [HttpGet("{username}/followers")]
        public async Task<IActionResult> GetFollowers(string username)
        {
            var followers =  _graphClient.Cypher
                .Match("(follower:User)-[:IS_FOLLOWING]->(user:User {Username: $username})")
                .Return(follower => follower.As<User>())
                .ResultsAsync;

            return Ok(followers);
        }




    }
}



