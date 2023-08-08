using UtilizationReports.Models.Team;

namespace UtilizationReports.Services
{
	public interface IAccountIdMapperService
	{
		public Task<Member> GetMemberFromAccountNameAsync(string accountName);
	}
}
