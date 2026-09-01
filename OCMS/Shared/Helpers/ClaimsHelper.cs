using OCMS.Shared.Enums;
using System.Security.Claims;

namespace OCMS.Shared.Helpers
{
    public static class ClaimsHelper
    {
        public static Guid GetUserId(ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(AppClaims.UserId);
            return Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }

        public static string GetFullName(ClaimsPrincipal user)
            => user.FindFirstValue(AppClaims.FullName) ?? string.Empty;

        public static string GetEmail(ClaimsPrincipal user)
            => user.FindFirstValue(AppClaims.Email) ?? string.Empty;

        public static bool IsAdmin(ClaimsPrincipal user)
            => user.IsInRole(AppRoles.Admin.ToString());
    }
}
