using GreenExpress.API.Models;

namespace GreenExpress.API.DTO
{
    public class DeliveryFeeCalculationRequestDTO
    {
        public string Consignee { get; set; }
        public string Consignor { get; set; }
        public IEnumerable<Dimension> Cartons { get; set; }
    }

    public class DeliveryFeeCalculationResponseDTO
    {
        public int EtaPeriod { get; set; }
        public DateTime? EtaDate { get; set; }
        public decimal Fee { get; set; }
        public string FeeText => $"${Fee:N2}";
        public decimal Discount { get; set; }
        public string DiscountText => $"${Discount:N2}";
        public string Note { get; set; }
    }
}