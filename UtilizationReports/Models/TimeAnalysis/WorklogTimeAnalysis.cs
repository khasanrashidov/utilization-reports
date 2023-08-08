namespace UtilizationReports.Models.TimeAnalysis
{
    public class WorklogTimeAnalysis
    {
        public double TotalSpentTime { get; set; }
        public double ProjectTimeTotal { get; set; }
        public double VacationTimeTotal { get; set; }
        public double BenchTimeTotal { get; set; }
        public double ProjectTimePercentage { get; set; }
        public double VacationTimePercentage { get; set; }
        public double BenchTimePercentage { get; set; }
    }
}
