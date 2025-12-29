using Microsoft.AspNetCore.Mvc;
using PremiumPlace.DTO;
using PremiumPlace_Web.Application.Abstractions.Api;
using PremiumPlace_Web.Infrastructure.Auth;

namespace PremiumPlace_Web.Controllers
{
    [Route("Place")]
    [RequireRole("Admin")]
    public class PlaceController : Controller
    {
        private readonly IPlaceApi _placeApi;
        public PlaceController(IPlaceApi placeApi)
        {
            _placeApi = placeApi;
        }

        [HttpGet("Index")]
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

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlaceCreateDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _placeApi.CreateAsync(dto, ct);
            if (!result.Success)
            {
                TempData["error"] = result.Message ?? "Could not create place.";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await _placeApi.GetByIdAsync(id, ct);

            if (!result.Success || result.Data is null)
            {
                TempData["error"] = result.Message ?? "Could not load place.";
                return RedirectToAction("Index");
            }

            var dto = new PlaceUpdateDTO
            {
                Id = id,
                Name = result.Data.Name,
                Details = result.Data.Details,
                Rate = result.Data.Rate,
                SquareFeet = result.Data.SquareFeet,
                Occupancy = result.Data.Occupancy,
                ImageUrl = result.Data.ImageUrl
            };
            return View(dto);
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PlaceUpdateDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _placeApi.UpdateAsync(id, dto, ct);
            if (!result.Success)
            {
                TempData["error"] = result.Message ?? "Could not create place.";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _placeApi.GetByIdAsync(id, ct);
            if (!result.Success || result.Data is null)
            {
                TempData["error"] = result.Message ?? "Could not load place.";
                return RedirectToAction("Index");
            }
            return View(result.Data);
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(PlaceDTO dto, CancellationToken ct)
        {
            var result = await _placeApi.DeleteAsync(dto.Id, ct);
            if (!result.Success)
            {
                TempData["error"] = result.Message ?? "Could not delete place.";
            }
            return RedirectToAction("Index");
        }
    }
}