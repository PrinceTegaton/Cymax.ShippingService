using Microsoft.Extensions.Caching.Memory;
using ParagoLogistics.API.Helpers;
using ParagoLogistics.API.Models;

namespace ParagoLogistics.API.Services
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationState>> GetAll();
        Task<Result<AddressBlock>> CreateAddressFromFullAddress(string address);
    }

    public class LocationService : ILocationService
    {
        private readonly IMemoryCache _cache;

        public LocationService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<IEnumerable<LocationState>> GetAll()
        {
            _cache.TryGetValue<IEnumerable<LocationState>>(CacheKeys.Locations, out var locations);

            if (locations == null || !locations.Any())
            {
                locations = await new JsonStoreUtil<LocationState>("Location.CA").GetAll();
                if(locations == null || !locations.Any()) return new List<LocationState>();

                _cache.Set(CacheKeys.Locations, locations);
            }

            return locations;
        }

        public async Task<Result<AddressBlock>> CreateAddressFromFullAddress(string address)
        {
            var locations = await GetAll();

            if (string.IsNullOrEmpty(address))
                return Result<AddressBlock>.Failure("Address is required");

            var locationNodes = address.ToLower().Split(',', StringSplitOptions.TrimEntries).ToList();
            if (locationNodes == null || !locationNodes.Any())
                return Result<AddressBlock>.Failure("Full address is required");

            if (locationNodes.Count < 3)
                return Result<AddressBlock>.Failure("Address must be be in this order: street, city and state/province");

            var block = new AddressBlock
            {
                Street = locationNodes.FirstOrDefault(),
            };

            locationNodes = locationNodes.Skip(0).ToList();

            var state = locations.FirstOrDefault(a => locationNodes.Contains(a.State.ToLower()));
            if (state == null)
                return Result<AddressBlock>.Failure("State/province not found");

            block.State = state.State;

            var city = state.Cities.FirstOrDefault(a => a.Name.ToLower() == locationNodes[1]);
            if (city == null)
                return Result<AddressBlock>.Failure("City not found");

            block.City = city.Name;
            block.Point = city.Point;

            return Result<AddressBlock>.Success(block);
        }
    }
}