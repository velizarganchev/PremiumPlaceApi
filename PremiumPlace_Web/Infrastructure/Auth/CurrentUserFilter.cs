using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PremiumPlace.DTO.Auth;
using PremiumPlace_Web.Application.Abstractions.Api;

namespace PremiumPlace_Web.Infrastructure.Auth
{
    public sealed class CurrentUserFilter : IAsyncActionFilter 
    {
        private const string ItemsKey = "pp_current_user";
        private readonly IAuthApi _auth;

        public CurrentUserFilter(IAuthApi auth)
        {
            _auth = auth;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // If already loaded in this request, reuse it
            if (context.HttpContext.Items.ContainsKey(ItemsKey))
            {
                await next();
                return;
            }

            var me = await _auth.MeAsync(context.HttpContext.RequestAborted);

            if (!me.Success || me.Data?.User is null)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            // Store for this request
            context.HttpContext.Items[ItemsKey] = me.Data.User;

            // Also push to views (optional convenience)
            if (context.Controller is Controller controller)
                controller.ViewData["CurrentUser"] = me.Data.User;

            await next();
        }

        public static AuthUserDTO? GetCurrentUser(HttpContext ctx)
            => ctx.Items.TryGetValue(ItemsKey, out var v) ? v as AuthUserDTO : null;
    }
}
