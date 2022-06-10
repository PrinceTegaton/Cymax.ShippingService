using CymaxStore.API.Models;
using System.Xml.Serialization;

namespace CymaxStore.API.DTO
{
    public class ParagoShippingFeeResponse
    {
        public decimal Fee { get; set; }
        public string FeeText => $"${Fee:N2}";
        public decimal Discount { get; set; }
        public string DiscountText => $"${Discount:N2}";
        public string Note { get; set; }
    }

    public class GreenExpressShippingFeeResponse
    {
        public decimal Fee { get; set; }
        public string FeeText => $"${Fee:N2}";
        public decimal Discount { get; set; }
        public string DiscountText => $"${Discount:N2}";
        public string Note { get; set; }
    }

    #region Bluewave 
    public class Package
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double Length { get; set; }
        public double Weight { get; set; }
    }

    [XmlRoot(ElementName = "DeliveryFeeCalculationRequestDTO")]
    public class BluewaveShippingFeeRequest
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public List<Package> Packages { get; set; }
    }

    [XmlRoot(ElementName = "DeliveryFeeCalculationResponseDTO")]
    public class BluewaveShippingFeeResponse
    {
        public decimal Fee { get; set; }
        public string FeeText => $"${Fee:N2}";
        public decimal Discount { get; set; }
        public string DiscountText => $"${Discount:N2}";
        public string Note { get; set; }
    }
    #endregion
}