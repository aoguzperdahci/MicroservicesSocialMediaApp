using FollowerService.Entities;
using Neo4jClient;

namespace FollowerService.Services
{
    public class Neo4jFollowerService : IFollowerService
    {
        private readonly IGraphClient _graphClient;

        public Neo4jFollowerService(IGraphClient graphClient)
        {
            this._graphClient = graphClient;
        }

        public async Task CreateUser(string username)
        {
            await _graphClient.Cypher
                .Create("(u:User {Username: $username})")
                .WithParam("username", username)
                .ExecuteWithoutResultsAsync();
        }

        public async Task DeleteUser(string username)
        {
            await _graphClient.Cypher
                .Match("(u:User {Username: $username})")
                .WithParam("username", username)
                .DetachDelete("u")
                .ExecuteWithoutResultsAsync();
        }

        public async Task FollowUser(string followerUsername, string followeeUsername)
        {
            await _graphClient.Cypher
                .Match("(follower:User {Username: $followerUsername})")
                .Match("(followee:User {Username: $followeeUsername})")
                .WithParam("followerUsername", followerUsername)
                .WithParam("followeeUsername", followeeUsername)
                .Create("(follower)-[:is_following]->(followee)")
                .ExecuteWithoutResultsAsync();
        }

        public async Task<IEnumerable<string>> GetFollowers(string username)
        {
            var result = await _graphClient.Cypher
                .Match("(follower:User)-[:is_following]->(followed:User {Username: $username})")
                .WithParam("username", username)
                .Return(follower => follower.As<User>().Username)
                .ResultsAsync;

            return result;
        }

        public async Task<IEnumerable<string>> GetFollowing(string username)
        {
            var results = await _graphClient.Cypher
                .Match("(follower:User {Username: $username})-[:is_following]->(followed:User)")
                .WithParam("username", username)
                .Return(followed => followed.As<User>().Username)
                .ResultsAsync;

            return results;
        }

        public async Task UnfollowUser(string followerUsername, string followeeUsername)
        {
            await _graphClient.Cypher
                .Match("(follower:User)-[r:is_following]->(followee:User)")
                .Where((User follower) => follower.Username == followerUsername)
                .AndWhere((User followee) => followee.Username == followeeUsername)
                .Delete("r")
                .ExecuteWithoutResultsAsync();
        }
    }
}
