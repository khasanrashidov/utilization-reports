using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using UtilizationReports.Models.Team;
using UtilizationReports.Models.Worklog;
using UtilizationReports.Services;

namespace UtilizationReports.UnitTests
{
	public class AccountIdMapperServiceUnitTests
	{
		#region Initialization

		private readonly Mock<ITeamMembersService> _teamMembersServiceMock;
		private readonly Mock<ILogger<AccountIdMapperService>> _loggerMock;
		private readonly AccountIdMapperService _accountIdMapperService; // SUT

		public AccountIdMapperServiceUnitTests()
		{
			_teamMembersServiceMock = new Mock<ITeamMembersService>();
			_loggerMock = new Mock<ILogger<AccountIdMapperService>>();
			_accountIdMapperService = new AccountIdMapperService(_teamMembersServiceMock.Object, _loggerMock.Object);
		}

		#endregion

		#region Tests

		[Theory]
		[MemberData(nameof(TeamTestData.GetTeamTestData), MemberType = typeof(TeamTestData))]
		public async Task GetMemberFromAccountNameAsync_WhenCalledWithCorrectAccountName_ReturnsMember(Team membersOfATeam)
		{
			// Arrange
			var accountName = "tEsTaCcOuNtNaMe"; // method is case insensitive

			var expectedMember = new Member
			{
				Self = "Self",
				AccountId = "123456789",
				DisplayName = "TestAccountName"
			};

			_teamMembersServiceMock.Setup(x => x.GetMembersOfATeamAsync()).ReturnsAsync(membersOfATeam);

			// Act
			var result = await _accountIdMapperService.GetMemberFromAccountNameAsync(accountName);

			// Assert
			result.Should().Be(expectedMember);
		}

		[Theory]
		[MemberData(nameof(TeamTestData.GetTeamTestData), MemberType = typeof(TeamTestData))]
		public async Task GetMemberFromAccountNameAsync_WhenCalledWithIncorrectAccountName_ThrowsArgumentException(Team membersOfATeam)
		{
			// Arrange
			var accountName = "Some Wrong Name";
			
			_teamMembersServiceMock.Setup(x => x.GetMembersOfATeamAsync()).ReturnsAsync(membersOfATeam);

			// Act
			Func<Task> act = async () => await _accountIdMapperService.GetMemberFromAccountNameAsync(accountName);

			// Assert
			act.Should().NotBeNull();
			await act.Should().ThrowAsync<ArgumentException>();
			await act.Should().ThrowAsync<ArgumentException>().WithMessage($"No team member found with the given account name: {accountName}");
			await act.Should().NotThrowAsync<NullReferenceException>();
			await act.Should().NotThrowAsync<ArgumentNullException>();
		}

		#endregion
	}
}
