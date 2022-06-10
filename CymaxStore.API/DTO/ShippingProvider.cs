namespace ParagoLogistics.API.DTO
{
    public class ShippingConfig
    {
        public IEnumerable<ShippingProvider> Providers { get; set; }
    }

    public enum ShippingProviderKey
    {
        Parago = 1,
        GreenExpress = 2,
        Bluewave = 3
    }

    public class ShippingProvider
    {
        public ShippingProviderKey Id { get; set; }
        public string Name { get; set; }
        public string EndpointUrl { get; set; }
        public bool IsActive { get; set; }
        public string AuthKey { get; set; }
    }
}