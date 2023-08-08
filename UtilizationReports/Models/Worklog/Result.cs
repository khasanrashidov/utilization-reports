namespace UtilizationReports.Models.Worklog
{
    public class Result
    {
		public Issue Issue { get; set; }
		public int TimeSpentSeconds { get; set; }
		public int BillableSeconds { get; set; }
		public required DateTime StartDate { get; set; }
		public required string StartTime { get; set; }
		public string? Description { get; set; }
		public Author Author { get; set; }
	}
}
