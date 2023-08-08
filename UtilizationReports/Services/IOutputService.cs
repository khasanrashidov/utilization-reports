using UtilizationReports.Models;
using UtilizationReports.Models.Team;
using UtilizationReports.Models.TimeAnalysis;

namespace UtilizationReports.Services
{
    public interface IOutputService
	{
		public (string fileContent, string contentType) GetOutputByType(OutputFormat format,
															List<Member> membersOfATeam,
															DateTime dateFrom,
															DateTime dateTo,
															List<WorklogTimeAnalysis> worklogTimeAnalysisModels);
	}
}
