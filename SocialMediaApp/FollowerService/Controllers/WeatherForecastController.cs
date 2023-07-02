using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
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

    }
}