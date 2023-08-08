using UtilizationReports.Models.Team;

namespace UtilizationReports.Services
{
	public interface ITeamMembersService
	{
		public Task<Team?> GetMembersOfATeamAsync();
	}
}
