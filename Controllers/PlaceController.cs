using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PremiumPlace_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlacesController : ControllerBase
    {
        [HttpGet]
        public string GetPlaces()
        {
            return "List of places";
        }

        [HttpGet("{id:int}")]
        public string GetPlaceById(int id)
        {
            return $"Get Place: {id}";
        }
    }
}
