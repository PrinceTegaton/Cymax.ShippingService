using Microsoft.AspNetCore.Mvc;
using GreenExpress.API.DTO;
using GreenExpress.API.Models;
using GreenExpress.API.Services;
using Microsoft.Extensions.Primitives;

namespace GreenExpress.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DeliveryController : ControllerBase
    {
        private readonly ILogisticService _logisticService;

        public DeliveryController(ILogisticService logisticService)
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
        [HttpPost]
        [ProducesResponseType(typeof(DeliveryFeeCalculationResponseDTO), 200)]
        [ProducesResponseType(typeof(string), 401)]
        public async Task<IActionResult> GetFee(DeliveryFeeCalculationRequestDTO request, [FromHeader] string clientKey)
        {
            // validate auth key
            // authKey is used from the param(header) for easy testing using swagger
            Request.Headers.TryGetValue("Authorization", out StringValues _authKey);
            if (string.IsNullOrEmpty(clientKey ?? _authKey) || (clientKey ?? _authKey) != "a24ac2ab-0626-467a-856b-332ec6a2ba26")
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