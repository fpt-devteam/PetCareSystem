using Microsoft.AspNetCore.Http;

namespace VetClinic.Web.Helpers
{
    public static class SessionHelper
    {
        public static bool IsAuthenticated(ISession session)
        {
            return session.GetString("IsAuthenticated") == "true";
        }

        public static int? GetUserId(ISession session)
        {
            return session.GetInt32("UserId");
        }

        public static string? GetUserName(ISession session)
        {
            return session.GetString("UserName");
        }

        public static string? GetUserEmail(ISession session)
        {
            return session.GetString("UserEmail");
        }

        public static string? GetUserRole(ISession session)
        {
            return session.GetString("UserRole");
        }

        public static bool IsInRole(ISession session, string role)
        {
            var userRole = session.GetString("UserRole");
            return !string.IsNullOrEmpty(userRole) && userRole.Equals(role, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsInAnyRole(ISession session, params string[] roles)
        {
            var userRole = session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole)) return false;

            return roles.Any(role => userRole.Equals(role, StringComparison.OrdinalIgnoreCase));
        }
    }
}