using CymaxStore.API.DTO;
using CymaxStore.API.Helpers;
using CymaxStore.API.Models;
using Newtonsoft.Json;
using ParagoLogistics.API.DTO;
using Rext;

namespace CymaxStore.API.Services
{
    public interface IStoreService
    {
        Task<Result<ShippingFeeResponseGroup>> CalculateDeliveryFee(ShippingFeeRequest request);
    }

    public class StoreService : IStoreService
    {
        private readonly ShippingConfig _shippingConfig;
        private readonly IRextHttpClient _rext;

        public StoreService(ShippingConfig shippingConfig,
                            IRextHttpClient rext)
        {
            _shippingConfig = shippingConfig;
            _rext = rext;
        }

        public async Task<Result<ShippingFeeResponseGroup>> CalculateDeliveryFee(ShippingFeeRequest request)
        {
            if (!_shippingConfig.Providers.Any())
            {
                return Result<ShippingFeeResponseGroup>.Failure("No registered shipping provider");
            }

            var vendorRsp = new List<ShippingFeeResponse>(); // store response from each provider
            var provider = new ShippingProvider(); // load provider config

            // the below conditions are determined by providers who are active
            // see appsetting.json/shippingconfig for more information

            #region PARAGO
            provider = _shippingConfig.Providers.FirstOrDefault(p => p.Id == ShippingProviderKey.Parago);
            if (provider?.IsActive ?? false)
            {
                var paragoRsp = await _rext.PostJSON<Result<ParagoShippingFeeResponse>>(provider.EndpointUrl, new
                {
                    contactAddress = request.SourceAddress,
                    warehouseAddress = request.DestinationAddress,
                    dimensions = request.Cartons
                }, header: new { AuthKey = provider.AuthKey });

                if (paragoRsp.IsSuccess && paragoRsp.Data?.Succeeded == true)
                {
                    vendorRsp.Add(new()
                    {
                        Fee = paragoRsp.Data.Data?.Fee ?? 0,
                        FeeText = paragoRsp.Data.Data?.FeeText,
                        Note = paragoRsp.Data.Data?.Note,
                        Provider = provider.Name
                    });
                }
            }
            #endregion


            #region GREENEXPRESS
            provider = _shippingConfig.Providers.FirstOrDefault(p => p.Id == ShippingProviderKey.GreenExpress);
            if (provider?.IsActive ?? false)
            {
                var greenRsp = await _rext.AddHeader("ClientKey", provider.AuthKey)
                                          .PostJSONForString(provider.EndpointUrl, new
                                          {
                                              consignee = request.SourceAddress,
                                              consignor = request.DestinationAddress,
                                              cartons = request.Cartons
                                          });

                if (greenRsp.IsSuccess)
                {
                    // decode json response in to object
                    var data = JsonConvert.DeserializeObject<GreenExpressShippingFeeResponse>(greenRsp.Data);
                    vendorRsp.Add(new()
                    {
                        Fee = data?.Fee ?? 0,
                        FeeText = data?.FeeText,
                        Note = data?.Note,
                        Provider = provider.Name
                    });
                }
            }
            #endregion

            provider = _shippingConfig.Providers.FirstOrDefault(p => p.Id == ShippingProviderKey.Bluewave);
            if (provider?.IsActive ?? false)
            {
                #region BLUEWAVE
                var bluewaveRsp = await _rext.PostXMLForString(provider.EndpointUrl, new BluewaveShippingFeeRequest
                {
                    Source = request.SourceAddress,
                    Destination = request.DestinationAddress,
                    Packages = request.Cartons.Select(a => new Package
                    {
                        Width = a.Width,
                        Height = a.Height,
                        Length = a.Length,
                        Weight = a.Weight
                    }).ToList()
                }, header: new { Authorization = provider.AuthKey });

                if (bluewaveRsp.IsSuccess)
                {
                    // decode xml response into object
                    var data = StringHelper.FromXml<BluewaveShippingFeeResponse>(bluewaveRsp.Data);
                    vendorRsp.Add(new()
                    {
                        Fee = data?.Fee ?? 0,
                        FeeText = data?.FeeText,
                        Note = data?.Note,
                        Provider = provider.Name
                    });
                }
                #endregion
            }


            if (!vendorRsp.Any())
            {
                return Result<ShippingFeeResponseGroup>.Failure("Unable to get shipping provider response");
            }

            var sortLowestFee = vendorRsp.OrderBy(a => a.Fee); // sort responses by lowest fee
            var cheapest = sortLowestFee.First(); // get the cheapest from all result

            return Result<ShippingFeeResponseGroup>.Success(new()
            {
                CheapestFee = cheapest.Fee,
                CheapestProvider = cheapest,
                Providers = sortLowestFee
            });
        }
    }
}