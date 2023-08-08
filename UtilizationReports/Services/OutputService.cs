using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using UtilizationReports.Models.Reports;
using UtilizationReports.Models.Team;
using UtilizationReports.Models.TimeAnalysis;

namespace UtilizationReports.Services
{
    public class OutputService : IOutputService
	{
		public (string fileContent, string contentType) GetOutputByType(OutputFormat format,
															List<Member> membersOfATeam,
															DateTime dateFrom,
															DateTime dateTo,
															List<WorklogTimeAnalysis> worklogTimeAnalysisModels)
		{
			// separating members that have worklogs in the specified date range (total spent time is not zero)
			var normalMembersOfATeam = membersOfATeam.Where((member, index) => worklogTimeAnalysisModels[index].TotalSpentTime != 0).ToList();
			// separating worklogs that have total spent time in the specified date range (that are not zero)
			var normalWorklogTimeAnalysisModels = worklogTimeAnalysisModels.Where((worklog, index) => worklog.TotalSpentTime != 0).ToList();

			return format switch
			{
				OutputFormat.JSON => (ToJson(normalMembersOfATeam, dateFrom, dateTo, normalWorklogTimeAnalysisModels),
													  "application/json"),
				OutputFormat.CSV => (ToCsv(normalMembersOfATeam, dateFrom, dateTo, normalWorklogTimeAnalysisModels),
													  "text/csv"),
				OutputFormat.XML => (ToXml(normalMembersOfATeam, dateFrom, dateTo, normalWorklogTimeAnalysisModels),
													  "text/xml"),
				_ => (ToJson(normalMembersOfATeam, dateFrom, dateTo, normalWorklogTimeAnalysisModels),
													  "application/json"),
			};
		}

		private static string ToCsv(List<Member> membersOfATeam, DateTime dateFrom, DateTime dateTo,
						List<WorklogTimeAnalysis> worklogTimeAnalysisModels)
		{
			var sb = new StringBuilder();
			sb.AppendLine("AccountName,DateFrom,DateTo,TotalSpentTime,ProjectTimeTotal,VacationTimeTotal,BenchTimeTotal,ProjectTimePercentage,VacationTimePercentage,BenchTimePercentage");

			for (var i = 0; i < membersOfATeam.Count; i++)
			{
				var report = new AccountWorklogReport()
				{
					AccountName = membersOfATeam[i].DisplayName,
					DateFrom = dateFrom,
					DateTo = dateTo,
					TotalSpentTime = worklogTimeAnalysisModels[i].TotalSpentTime,
					ProjectTimeTotal = worklogTimeAnalysisModels[i].ProjectTimeTotal,
					VacationTimeTotal = worklogTimeAnalysisModels[i].VacationTimeTotal,
					BenchTimeTotal = worklogTimeAnalysisModels[i].BenchTimeTotal,
					ProjectTimePercentage = worklogTimeAnalysisModels[i].ProjectTimePercentage,
					VacationTimePercentage = worklogTimeAnalysisModels[i].VacationTimePercentage,
					BenchTimePercentage = worklogTimeAnalysisModels[i].BenchTimePercentage
				};
				sb.AppendLine($"{report.AccountName},{report.DateFrom},{report.DateTo},{report.TotalSpentTime},{report.ProjectTimeTotal},{report.VacationTimeTotal},{report.BenchTimeTotal},{report.ProjectTimePercentage}%,{report.VacationTimePercentage}%,{report.BenchTimePercentage}%");
			}

			return sb.ToString();
		}

		private static string ToJson(List<Member> membersOfATeam, DateTime dateFrom, DateTime dateTo,
									List<WorklogTimeAnalysis> worklogTimeAnalysisModels)
		{
			var reports = new List<AccountWorklogReport>();

			for (var i = 0; i < membersOfATeam.Count; i++)
			{
				var report = new AccountWorklogReport()
				{
					AccountName = membersOfATeam[i].DisplayName,
					DateFrom = dateFrom,
					DateTo = dateTo,
					TotalSpentTime = worklogTimeAnalysisModels[i].TotalSpentTime,
					ProjectTimeTotal = worklogTimeAnalysisModels[i].ProjectTimeTotal,
					VacationTimeTotal = worklogTimeAnalysisModels[i].VacationTimeTotal,
					BenchTimeTotal = worklogTimeAnalysisModels[i].BenchTimeTotal,
					ProjectTimePercentage = worklogTimeAnalysisModels[i].ProjectTimePercentage,
					VacationTimePercentage = worklogTimeAnalysisModels[i].VacationTimePercentage,
					BenchTimePercentage = worklogTimeAnalysisModels[i].BenchTimePercentage
				};
				reports.Add(report);
			}

			var options = new JsonSerializerOptions
			{
				NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
			};

			return JsonSerializer.Serialize(reports, options);
		}

		private static string ToXml(List<Member> membersOfATeam, DateTime dateFrom, DateTime dateTo,
												List<WorklogTimeAnalysis> worklogTimeAnalysisModels)
		{
			var reports = new List<AccountWorklogReport>();

			for (var i = 0; i < membersOfATeam.Count; i++)
			{
				var report = new AccountWorklogReport()
				{
					AccountName = membersOfATeam[i].DisplayName,
					DateFrom = dateFrom,
					DateTo = dateTo,
					TotalSpentTime = worklogTimeAnalysisModels[i].TotalSpentTime,
					ProjectTimeTotal = worklogTimeAnalysisModels[i].ProjectTimeTotal,
					VacationTimeTotal = worklogTimeAnalysisModels[i].VacationTimeTotal,
					BenchTimeTotal = worklogTimeAnalysisModels[i].BenchTimeTotal,
					ProjectTimePercentage = worklogTimeAnalysisModels[i].ProjectTimePercentage,
					VacationTimePercentage = worklogTimeAnalysisModels[i].VacationTimePercentage,
					BenchTimePercentage = worklogTimeAnalysisModels[i].BenchTimePercentage
				};
				reports.Add(report);
			}

			var serializer = new XmlSerializer(typeof(List<AccountWorklogReport>));
			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb))
			{
				serializer.Serialize(writer, reports);
			}

			return sb.ToString();
		}
	}
}
