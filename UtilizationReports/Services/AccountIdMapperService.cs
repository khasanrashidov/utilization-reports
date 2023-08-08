using UtilizationReports.Models.Team;

namespace UtilizationReports.Services
{
	public class AccountIdMapperService : IAccountIdMapperService
	{
		private readonly ITeamMembersService _teamMembersService;
		private readonly ILogger<AccountIdMapperService> _logger;

		public AccountIdMapperService(ITeamMembersService teamMembersService, ILogger<AccountIdMapperService> logger)
		{
			_teamMembersService = teamMembersService;
			_logger = logger;
		}

		public async Task<Member> GetMemberFromAccountNameAsync(string accountName)
		{
			var membersOfATeam = await _teamMembersService.GetMembersOfATeamAsync();

			_logger.LogInformation("Found {MembersOfATeamMetadataCount} team members", membersOfATeam?.Metadata.Count);

			List<MemberResult> teamMembers = membersOfATeam?.Results.ToList()!;

			MemberResult? teamMember = teamMembers.FirstOrDefault(x => string.Equals(x.Member.DisplayName, accountName,
				   StringComparison.OrdinalIgnoreCase))!;

			if (teamMember == null)
			{
				_logger.LogError("No team member found with the given account name: {AccountName}", accountName);
				throw new ArgumentException($"No team member found with the given account name: {accountName}");
			}

			_logger.LogInformation("Getting worklogs for account name: {AccountName}", accountName);

			return teamMember.Member;
		}
	}
}
