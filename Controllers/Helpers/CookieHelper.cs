namespace PremiumPlace_API.Controllers.Helpers
{
    public class CookieHelper
    {
        public static void SetAuthCookies(
            HttpResponse response,
            string accessToken,
            string refreshToken)
        {
            var accessOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,              // API е https
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            };

            var refreshOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(14)
            };

            response.Cookies.Append("pp_access", accessToken, accessOptions);
            response.Cookies.Append("pp_refresh", refreshToken, refreshOptions);
        }

        public static void ClearAuthCookies(HttpResponse response)
        {
            response.Cookies.Delete("pp_access");
            response.Cookies.Delete("pp_refresh");
        }
    }
}
