using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user) //extending claimPrincipal user obejct
        {
            return user?.FindFirst(ClaimTypes.Name)?.Value; //unique name comes in as "Name"
        }

        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value; //unique name comes in as "Name"
        }
    }
}
