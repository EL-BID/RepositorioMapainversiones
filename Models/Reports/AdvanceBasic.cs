namespace IMRepo.Models.Domain
{
    public class AdvanceBasic
    {
        public int id { get; set; }
        public double? actual { get; set; }
        public double? programmed { get; set; }
        public float? percent { get; set; }
        public bool invalid { get; set; } = false;
    }
}
