using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ParagoLogistics.API.DTO;
using ParagoLogistics.API.Models;
using ParagoLogistics.API.Services;

namespace ParagoLogistics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ShippingController : ControllerBase
    {
        private readonly ILogisticService _logisticService;
        private readonly ILogger<ShippingController> _logger;

        public ShippingController(ILogisticService logisticService,
                                  ILogger<ShippingController> logger)
        {
            _logisticService = logisticService;
            _logger = logger;
        }

        /// <summary>
        /// Get all service locations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LocationState>), 200)]
        public async Task<IActionResult> GetLocations()
        {
            var locations = await _logisticService.GetLocations();

            return Ok(Result<IEnumerable<LocationState>>.Success(locations, "Locations retrieved"));
        }

        /// <summary>
        /// Get delivery fee and ETA for package by dimension and location, within Canada only
        /// </summary>
        /// <param name="request"></param>
        /// <param name="authKey"></param>
        /// <remarks>Only British Columbia and Ontario provices are supported at the moment. To see all cities, use GetLocations</remarks>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<LocationState>), 200)]
        public async Task<IActionResult> CalculateFee(DeliveryFeeCalculationRequestDTO request, [FromHeader] string authKey)
        {
            // validate auth key
            // authKey is used from the param(header) for easy testing using swagger
            Request.Headers.TryGetValue("Authorization", out StringValues _authKey);
            if (string.IsNullOrEmpty(authKey ?? _authKey) || (authKey ?? _authKey) != "4bcf25fd-5ade-400b-a080-c2aa0c97e3bf")
            {
                return Unauthorized();
            }


            var result = await _logisticService.CalculateDeliveryFee(request);

            if (result == null)
                return BadRequest(result);

            return Ok(result);
        }
    }
}