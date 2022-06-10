using ParagoLogistics.API.DTO;
using ParagoLogistics.API.Helpers;
using ParagoLogistics.API.Models;

namespace ParagoLogistics.API.Services
{
    public interface ILogisticService
    {
        Task<IEnumerable<LocationState>> GetLocations();
        Task<Result<DeliveryFeeCalculationResponseDTO>> CalculateDeliveryFee(DeliveryFeeCalculationRequestDTO request);
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

        public async Task<Result<DeliveryFeeCalculationResponseDTO>> CalculateDeliveryFee(DeliveryFeeCalculationRequestDTO request)
        {
            var locations = await _locationService.GetAll();
            if (!locations.Any())
                return Result<DeliveryFeeCalculationResponseDTO>.Failure("No location found");

            // validate address
            var pickupAddress = await _locationService.CreateAddressFromFullAddress(request.ContactAddress);
            if (!pickupAddress.Succeeded)
                return Result<DeliveryFeeCalculationResponseDTO>.Failure($"Pickup address error: {pickupAddress.Message}");

            var deliveryAddress = await _locationService.CreateAddressFromFullAddress(request.WarehouseAddress);
            if (!deliveryAddress.Succeeded)
                return Result<DeliveryFeeCalculationResponseDTO>.Failure($"Delivery address error: {deliveryAddress.Message}");

            decimal fee = 0;

            foreach (var item in request.Dimensions)
            {
                // call algo to calculate fee **wacky algo for test purpose
                fee += CalculatePackageCostByLocationPoint(item, pickupAddress.Data, deliveryAddress.Data);
            }

            return Result<DeliveryFeeCalculationResponseDTO>.Success(new()
            {
                Fee = fee,
                Note = $"Delivery fee is ${fee:N2}"
            }, "OKAY");
        }

        private static decimal CalculatePackageCostByLocationPoint(Dimension dimension, AddressBlock pickupAddress, AddressBlock deliveryAddress)
        {
            double point = ((dimension.Width + dimension.Height + dimension.Length) * dimension.Weight) / (pickupAddress.Point + deliveryAddress.Point);

            return Math.Round(Convert.ToDecimal(point), 2);
        }
    }
}