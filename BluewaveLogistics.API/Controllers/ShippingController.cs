using Microsoft.AspNetCore.Mvc;
using BluewaveLogistics.API.DTO;
using BluewaveLogistics.API.Models;
using BluewaveLogistics.API.Services;
using Microsoft.Extensions.Primitives;

namespace BluewaveLogistics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ShippingController : ControllerBase
    {
        private readonly ILogisticService _logisticService;

        public ShippingController(ILogisticService logisticService)
        {
            _logisticService = logisticService;
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

            return Ok(locations);
        }

        /// <summary>
        /// Get delivery fee and ETA for package by dimension and location, within Canada only
        /// </summary>
        /// <param name="request"></param>
        /// <remarks>Only British Columbia and Ontario provices are supported at the moment. To see all cities, use GetLocations</remarks>
        /// <returns></returns>
        [Produces("application/xml"), Consumes("application/xml")]
        [HttpPost]
        [ProducesResponseType(typeof(DeliveryFeeCalculationResponseDTO), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> CalculateFeeByAddressAndPackage(DeliveryFeeCalculationRequestDTO request)
        {
            // validate auth key
            // authKey is used from the param(header) for easy testing using swagger
            Request.Headers.TryGetValue("Authorization", out StringValues _authKey);
            if (string.IsNullOrEmpty(_authKey) || _authKey != "bae4ba19-4461-4926-8f6f-d8e12692d42d")
            {
                return Unauthorized();
            }


            var result = await _logisticService.CalculateDeliveryFee(request);

            if (result.rsp == null)
                return BadRequest(result.msg);

            return Ok(result.rsp);
        }
    }
}