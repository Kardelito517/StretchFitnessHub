using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace StretchFitnessHub.Filters
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public RoleAuthorizeAttribute(string roles)
        {
            _roles = roles.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(r => r.Trim())
                          .ToArray();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userRole = context.HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(userRole) || !_roles.Contains(userRole, StringComparer.OrdinalIgnoreCase))
            {
                context.Result = new RedirectToActionResult("LandingPage", "Account", null);
            }
        }
    }
}
