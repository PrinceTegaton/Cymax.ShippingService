using ParagoLogistics.API.Models;

namespace ParagoLogistics.API.DTO
{
    public class DeliveryFeeCalculationRequestDTO
    {
        public string ContactAddress { get; set; }
        public string WarehouseAddress { get; set; }
        public IEnumerable<Dimension> Dimensions { get; set; }
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