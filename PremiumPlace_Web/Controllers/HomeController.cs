using Microsoft.AspNetCore.Mvc;
using PremiumPlace.DTO;
using PremiumPlace_Web.Application.Abstractions.Api;
using PremiumPlace_Web.Infrastructure.Auth;
using PremiumPlace_Web.Models;
using System.Diagnostics;

namespace PremiumPlace_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPlaceApi _placeApi;
        public HomeController(IPlaceApi placeApi)
        {
            _placeApi = placeApi;
        }

        [ServiceFilter(typeof(CurrentUserFilter))]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var result = await _placeApi.GetAllAsync(ct);

            if (!result.Success)
            {
                TempData["error"] = result.Message ?? "Could not load places.";
                return View(Array.Empty<PlaceDTO>());
            }

            return View(result.Data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
