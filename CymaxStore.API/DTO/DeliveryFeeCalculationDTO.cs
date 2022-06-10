namespace CymaxStore.API.DTO
{
    public class Carton
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double Length { get; set; }
        public double Weight { get; set; }
    }

    public class ShippingFeeRequest
    {
        public string SourceAddress { get; set; }
        public string DestinationAddress { get; set; }
        public IEnumerable<Carton> Cartons { get; set; }
    }

    public class ShippingFeeResponse
    {
        public decimal Fee { get; set; }
        public string FeeText { get; set; }
        public decimal Discount { get; set; }
        public string DiscountText { get; set; }
        public string Note { get; set; }
        public string Provider { get; set; }
    }

    public class ShippingFeeResponseGroup
    {
        public decimal CheapestFee { get; set; }
        public string CheapestText => $"${CheapestFee:N2}";
        public ShippingFeeResponse CheapestProvider { get; set; }
        public IEnumerable<ShippingFeeResponse> Providers { get; set; }
    }
}