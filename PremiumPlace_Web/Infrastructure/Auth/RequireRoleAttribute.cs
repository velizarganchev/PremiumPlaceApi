using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PremiumPlace_Web.Application.Abstractions.Api;
namespace PremiumPlace_Web.Infrastructure.Auth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class RequireRoleAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _roles;

        public RequireRoleAttribute(params string[] roles)
        {
            _roles = roles ?? Array.Empty<string>();
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var auth = context.HttpContext.RequestServices.GetRequiredService<IAuthApi>();

            var me = await auth.MeAsync(context.HttpContext.RequestAborted);
            if (!me.Success || me.Data?.User is null)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            var user = me.Data.User;

            context.HttpContext.Items["pp_current_user"] = user;

            if (_roles.Length > 0 && !_roles.Any(r =>
                    string.Equals(r, user.Role, StringComparison.OrdinalIgnoreCase)))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
                return;
            }
        }
    }
}
