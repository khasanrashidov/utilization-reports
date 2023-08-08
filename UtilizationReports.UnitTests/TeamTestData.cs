using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilizationReports.Models.Team;
using UtilizationReports.Models.Worklog;

namespace UtilizationReports.UnitTests
{
	public static class TeamTestData
	{
		public static IEnumerable<object[]> GetTeamTestData()
		{
			var membersOfATeam = new Team
			{
				Self = It.IsAny<string>(),
				Metadata = new Metadata
				{
					Count = 5
				},
				Results = new List<MemberResult>
				{
					new MemberResult
					{
						Self = It.IsAny<string>(),
						Team = new TeamInfo
						{
							Self = It.IsAny<string>()
						},
						Member = new Member
						{
							Self = "Self",
							AccountId = "123456789",
							DisplayName = "TestAccountName"
						},
						Memberships = new Memberships
						{
							Self = It.IsAny<string>(),
							Active = new Active
							{
								Self = It.IsAny<string>(),
								Id = 1899,
								CommitmentPercent = 100,
								From = null,
								To = null,
								Role = new Role
								{
									Self = It.IsAny<string>(),
									Id = 1,
									Name = "Member"
								}
							}
						}
					},
					new MemberResult
					{
						Self = It.IsAny<string>(),
						Team = new TeamInfo
						{
							Self = It.IsAny<string>()
						},
						Member = new Member
						{
							Self = It.IsAny<string>(),
							AccountId = "987654321",
							DisplayName = "OtherAccountName"
						},
						Memberships = new Memberships
						{
							Self = It.IsAny<string>(),
							Active = new Active
							{
								Self = It.IsAny<string>(),
								Id = 1900,
								CommitmentPercent = 100,
								From = null,
								To = null,
								Role = new Role
								{
									Self = It.IsAny<string>(),
									Id = 1,
									Name = "Member"
								}
							}
						}
					},
					new MemberResult
					{
						Self = It.IsAny<string>(),
						Team = new TeamInfo
						{
							Self = It.IsAny<string>()
						},
						Member = new Member
						{
							Self = It.IsAny<string>(),
							AccountId = "987654321",
							DisplayName = "OtherAccountName"
						},
						Memberships = new Memberships
						{
							Self = It.IsAny<string>(),
							Active = new Active
							{
								Self = It.IsAny<string>(),
								Id = 1900,
								CommitmentPercent = 100,
								From = null,
								To = null,
								Role = new Role
								{
									Self = It.IsAny<string>(),
									Id = 1,
									Name = "Member"
								}
							}
						}
					},
					new MemberResult
					{
						Self = It.IsAny<string>(),
						Team = new TeamInfo
						{
							Self = It.IsAny<string>()
						},
						Member = new Member
						{
							Self = It.IsAny<string>(),
							AccountId = "987654321",
							DisplayName = "OtherAccountName"
						},
						Memberships = new Memberships
						{
							Self = It.IsAny<string>(),
							Active = new Active
							{
								Self = It.IsAny<string>(),
								Id = 1900,
								CommitmentPercent = 100,
								From = null,
								To = null,
								Role = new Role
								{
									Self = It.IsAny<string>(),
									Id = 1,
									Name = "Member"
								}
							}
						}
					},
					new MemberResult
					{
						Self = It.IsAny<string>(),
						Team = new TeamInfo
						{
							Self = It.IsAny<string>()
						},
						Member = new Member
						{
							Self = It.IsAny<string>(),
							AccountId = "987654321",
							DisplayName = "OtherAccountName"
						},
						Memberships = new Memberships
						{
							Self = It.IsAny<string>(),
							Active = new Active
							{
								Self = It.IsAny<string>(),
								Id = 1900,
								CommitmentPercent = 100,
								From = null,
								To = null,
								Role = new Role
								{
									Self = It.IsAny<string>(),
									Id = 1,
									Name = "Member"
								}
							}
						}
					}
				}
			};

			yield return new object[] { membersOfATeam };
		}
	}
}
