using Microsoft.AspNetCore.Mvc;
using PremiumPlace.DTO;
using PremiumPlace_Web.Application.Abstractions.Api;

namespace PremiumPlace_Web.Controllers
{
    [Route("Place")]
    public class PlaceController : Controller
    {
        private readonly IPlaceApi _placeApi;
        public PlaceController(IPlaceApi placeApi)
        {
            _placeApi = placeApi;
        }
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
    }
}
