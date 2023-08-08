namespace UtilizationReports.Models.Worklog
{
	public class Worklog
	{
		public string Self { get; set; }
		public Metadata Metadata { get; set; }
		public List<Result> Results { get; set; }
	}
}
