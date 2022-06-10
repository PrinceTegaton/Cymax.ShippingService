using Microsoft.AspNetCore.Mvc;
using CymaxStore.API.DTO;
using CymaxStore.API.Models;
using CymaxStore.API.Services;

namespace CymaxStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        /// <summary>
        /// Calculate fee for item(s) delivery
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Result<ShippingFeeResponseGroup>), 200)]
        public async Task<IActionResult> CalculateFee(ShippingFeeRequest request)
        {
            var result = await _storeService.CalculateDeliveryFee(request);

            if (result == null)
                return BadRequest(result);

            return Ok(result);
        }
    }
}