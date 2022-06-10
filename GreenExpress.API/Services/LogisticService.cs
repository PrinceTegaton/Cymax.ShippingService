using GreenExpress.API.DTO;
using GreenExpress.API.Helpers;
using GreenExpress.API.Models;

namespace GreenExpress.API.Services
{
    public interface ILogisticService
    {
        Task<IEnumerable<LocationState>> GetLocations();
        Task<(DeliveryFeeCalculationResponseDTO rsp, string msg)> CalculateDeliveryFee(DeliveryFeeCalculationRequestDTO request);
    }

    public class LogisticService : ILogisticService
    {
        private readonly ILocationService _locationService;

        public LogisticService(ILocationService locationService)
        {
            _locationService = locationService;
        }

        public async Task<IEnumerable<LocationState>> GetLocations()
        {
            var locations = new JsonStoreUtil<LocationState>("Location.CA");

            return await locations.GetAll();
        }

        public async Task<(DeliveryFeeCalculationResponseDTO rsp, string msg)> CalculateDeliveryFee(DeliveryFeeCalculationRequestDTO request)
        {
            var locations = await _locationService.GetAll();
            if (!locations.Any())
                return (null, "No location found");

            // validate address
            var pickupAddress = await _locationService.CreateAddressFromFullAddress(request.Consignee);
            if (pickupAddress.rsp == null)
                return (null, $"Pickup address error: {pickupAddress.msg}");

            var deliveryAddress = await _locationService.CreateAddressFromFullAddress(request.Consignor);
            if (deliveryAddress.rsp == null)
                return (null, $"Delivery address error: {deliveryAddress.msg}");

            decimal fee = 0;

            foreach (var item in request.Cartons)
            {
                // call algo to calculate fee **wacky algo for test purpose
                fee += CalculatePackageCostByLocationPoint(item, pickupAddress.rsp, deliveryAddress.rsp);
            }

            return (new()
            {
                Fee = fee,
                Note = $"Delivery fee is ${fee:N2}"
            }, "OKAY");
        }

        private static decimal CalculatePackageCostByLocationPoint(Dimension dimension, AddressBlock pickupAddress, AddressBlock deliveryAddress)
        {
            double point = ((dimension.Width + dimension.Height - dimension.Length) * dimension.Weight) / (pickupAddress.Point + deliveryAddress.Point) + 3;

            return Math.Round(Convert.ToDecimal(point), 2);
        }
    }
}