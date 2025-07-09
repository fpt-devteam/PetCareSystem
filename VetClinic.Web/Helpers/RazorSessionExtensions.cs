using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace VetClinic.Web.Helpers
{
    public static class RazorSessionExtensions
    {
        public static bool IsAuthenticated(this PageModel page)
        {
            return SessionHelper.IsAuthenticated(page.HttpContext.Session);
        }

        public static bool IsInRole(this PageModel page, string role)
        {
            return SessionHelper.IsInRole(page.HttpContext.Session, role);
        }

        public static bool IsInAnyRole(this PageModel page, params string[] roles)
        {
            return SessionHelper.IsInAnyRole(page.HttpContext.Session, roles);
        }

        public static int? GetUserId(this PageModel page)
        {
            return SessionHelper.GetUserId(page.HttpContext.Session);
        }

        public static string? GetUserRole(this PageModel page)
        {
            return SessionHelper.GetUserRole(page.HttpContext.Session);
        }

        public static string? GetUserName(this PageModel page)
        {
            return SessionHelper.GetUserName(page.HttpContext.Session);
        }
    }

    // For use in Views
    public static class ViewSessionHelper
    {
        public static bool IsInRole(ISession session, string role)
        {
            return SessionHelper.IsInRole(session, role);
        }

        public static bool IsInAnyRole(ISession session, params string[] roles)
        {
            return SessionHelper.IsInAnyRole(session, roles);
        }

        public static bool IsAuthenticated(ISession session)
        {
            return SessionHelper.IsAuthenticated(session);
        }

        public static int? GetUserId(ISession session)
        {
            return SessionHelper.GetUserId(session);
        }

        public static string? GetUserRole(ISession session)
        {
            return SessionHelper.GetUserRole(session);
        }

        public static string? GetUserName(ISession session)
        {
            return SessionHelper.GetUserName(session);
        }
    }
}