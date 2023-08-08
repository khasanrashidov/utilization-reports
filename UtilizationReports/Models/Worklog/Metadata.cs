namespace UtilizationReports.Models.Worklog
{
    public class Metadata
    {
        public int? Count { get; set; } = 0;
        public int? Offset { get; set; } = 0;
        public int? Limit { get; set; } = 0;
        public string? Next { get; set; } = "";
        public string? Previous { get; set; } = "";
    }
}
