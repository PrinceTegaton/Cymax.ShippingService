using BluewaveLogistics.API.Models;
using System.Xml.Serialization;

namespace BluewaveLogistics.API.DTO
{
    public class DeliveryFeeCalculationRequestDTO
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        
        [XmlArrayItem]
        public Package[] Packages { get; set; }
    }

    public class DeliveryFeeCalculationResponseDTO
    {
        public decimal Fee { get; set; }
        public string FeeText => $"${Fee:N2}";
        public decimal Discount { get; set; }
        public string DiscountText => $"${Discount:N2}";
        public string Note { get; set; }
    }
}