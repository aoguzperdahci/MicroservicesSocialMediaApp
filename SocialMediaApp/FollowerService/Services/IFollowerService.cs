namespace FollowerService.Services
{
    public interface IFollowerService
    {
        Task FollowUser(string followerUsername, string followeeUsername);
        Task UnfollowUser(string followerUsername, string followeeUsername);
        Task<IEnumerable<string>> GetFollowing(string username);
        Task<IEnumerable<string>> GetFollowers(string username);
        Task CreateUser(string username);
        Task DeleteUser(string username);
    }
}
