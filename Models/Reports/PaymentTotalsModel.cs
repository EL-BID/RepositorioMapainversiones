namespace IMRepo.Models.Reports
{
    public class PaymentTotalsModel
    {
        public int id { get; set; }
        public int? totalQty { get; set; }
        public double? totalValue { get; set; }
        public int? requestedQty { get; set; }
        public double? requestedValue { get; set; }
        public int? approvedQty { get; set; }
        public double? approvedValue { get; set; }
        public double? available { get; set; }
    }
}
