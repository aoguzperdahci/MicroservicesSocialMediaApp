using System.Security.Claims;

namespace UserService.Helpers
{
    public static class HttpContextExtension
    {
        public static string GetUserId(this HttpContext context)
        {
            var claim = context.User.Claims.Single(c => c.Type == ClaimTypes.Actor);
            return claim.Value;
        }
    }
}
