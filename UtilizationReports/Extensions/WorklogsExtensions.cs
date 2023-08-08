using UtilizationReports.Models.TimeAnalysis;
using UtilizationReports.Models.Worklog;

namespace UtilizationReports.Extensions
{
	public static class WorklogsExtensions
	{
		public static List<WorklogTimeAnalysis> MapToWorklogTimeAnalysisModel(this List<Worklog> worklogsList)
		{
			// if description is null, set it to empty string
			foreach (var worklog in worklogsList.SelectMany(x => x.Results))
			{
				if (worklog.Description == null)
				{
					worklog.Description = "";
				}
			}
			var projectTimeList = worklogsList.SelectMany(x => x.Results).Where(x => x.BillableSeconds != 0 && !x.Issue.Key.Contains("ANONWORK") && (!x.Description!.Contains("vacation") || !x.Description.Contains("Vacation"))).ToList();
			var vacationTimeList = worklogsList.SelectMany(x => x.Results).Where(x => x.BillableSeconds == 0 && x.Issue.Key.Contains("ANONWORK") && (x.Description!.Contains("vacation") || x.Description.Contains("Vacation"))).ToList();
			var benchTimeList = worklogsList.SelectMany(x => x.Results).Where(x => x.BillableSeconds == 0 && x.Issue.Key.Contains("ANONWORK") && !x.Description!.Contains("vacation") && !x.Description.Contains("Vacation")).ToList();
			double projectTimeTotal = projectTimeList.Sum(x => x.TimeSpentSeconds);
			double vacationTimeTotal = vacationTimeList.Sum(x => x.TimeSpentSeconds);
			double benchTimeTotal = benchTimeList.Sum(x => x.TimeSpentSeconds);
			double totalSpentTime = projectTimeTotal + vacationTimeTotal + benchTimeTotal;
			double projectTimePercentage = Math.Round(projectTimeTotal / totalSpentTime * 100, 3);
			double vacationTimePercentage = Math.Round(vacationTimeTotal / totalSpentTime * 100, 3);
			double benchTimePercentage = Math.Round(benchTimeTotal / totalSpentTime * 100, 3);

			return new List<WorklogTimeAnalysis>() {
				new WorklogTimeAnalysis() {
					TotalSpentTime = totalSpentTime,
					ProjectTimeTotal = projectTimeTotal,
					VacationTimeTotal = vacationTimeTotal,
					BenchTimeTotal = benchTimeTotal,
					ProjectTimePercentage = projectTimePercentage,
					VacationTimePercentage = vacationTimePercentage,
					BenchTimePercentage = benchTimePercentage
				}
			};
		}
	}
}
