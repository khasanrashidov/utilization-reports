using FluentAssertions;
using Moq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using UtilizationReports.Models.Reports;
using UtilizationReports.Models.Team;
using UtilizationReports.Models.TimeAnalysis;
using UtilizationReports.Models.Worklog;
using UtilizationReports.Services;

namespace UtilizationReports.UnitTests
{
	public class OutputServiceUnitTests
	{
		#region Initialization

		private readonly OutputService _outputService; // SUT

		public OutputServiceUnitTests()
		{
			_outputService = new OutputService();
		}

		#endregion

		#region Tests

		[Theory]
		[InlineData(OutputFormat.JSON, "application/json")]
		[InlineData(OutputFormat.CSV, "text/csv")]
		[InlineData(OutputFormat.XML, "text/xml")]
		public void GetOutputByType_ForAllSpecifiedFormats_ReturnsCorrectContentTypeAndFileContent(OutputFormat expectedFormat, string expectedContentType)
		{
			// Arrange
			var membersOfATeam = new Mock<List<Member>>();
			var dateFrom = It.IsAny<DateTime>();
			var dateTo = It.IsAny<DateTime>();
			var worklogTimeAnalysisModels = new Mock<List<WorklogTimeAnalysis>>();

			// Act
			var (fileContent, contentType) = _outputService.GetOutputByType(expectedFormat, membersOfATeam.Object, dateFrom, dateTo, worklogTimeAnalysisModels.Object);

			// Assert
			contentType.Should().Be(expectedContentType);
			fileContent.Should().NotBeNullOrEmpty().And.NotBeNullOrWhiteSpace();
		}

		[Theory]
		[MemberData(nameof(TeamTestData.GetTeamTestData), MemberType = typeof(TeamTestData))]
		public void GetOutPutByType_WhenMemberContainsTotalSpentTimeEqualZero_ShouldNotBeIncludedInOutput(Team membersOfATeam)
		{
			// Arrange
			var members = membersOfATeam!.Results.Select(x => x.Member).ToList();

			var worklogTimeAnalysisModels = new List<WorklogTimeAnalysis>
			{
				new WorklogTimeAnalysis
				{
					TotalSpentTime = 0,
					ProjectTimeTotal = 0,
					VacationTimeTotal = 0,
					BenchTimeTotal = 0,
					ProjectTimePercentage = 0,
					VacationTimePercentage = 0,
					BenchTimePercentage = 0
				},
				new WorklogTimeAnalysis
				{
					TotalSpentTime = 0,
					ProjectTimeTotal = 0,
					VacationTimeTotal = 0,
					BenchTimeTotal = 0,
					ProjectTimePercentage = 0,
					VacationTimePercentage = 0,
					BenchTimePercentage = 0
				},
				new WorklogTimeAnalysis
				{
					TotalSpentTime = 1,
					ProjectTimeTotal = 1,
					VacationTimeTotal = 1,
					BenchTimeTotal = 1,
					ProjectTimePercentage = 1,
					VacationTimePercentage = 1,
					BenchTimePercentage = 1
				},
				new WorklogTimeAnalysis
				{
					TotalSpentTime = 2,
					ProjectTimeTotal = 2,
					VacationTimeTotal = 2,
					BenchTimeTotal = 2,
					ProjectTimePercentage = 2,
					VacationTimePercentage = 2,
					BenchTimePercentage = 2
				},
				new WorklogTimeAnalysis
				{
					TotalSpentTime = 3,
					ProjectTimeTotal = 3,
					VacationTimeTotal = 3,
					BenchTimeTotal = 3,
					ProjectTimePercentage = 3,
					VacationTimePercentage = 3,
					BenchTimePercentage = 3
				}
			};

			DateTime from = new(2021, 1, 1);
			DateTime to = new(2021, 1, 31);
			OutputFormat outputFormat = OutputFormat.JSON;

			var membersOfATeamWithTotalSpentTime = new Team
			{
				Self = It.IsAny<string>(),
				Metadata = new Metadata
				{
					Count = 2
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

			var membersWithTotalSpentTime = membersOfATeamWithTotalSpentTime!.Results.Select(x => x.Member).ToList();

			var worklogTimeAnalysisModelsWithTotalSpentTime = new List<WorklogTimeAnalysis>
			{
				new WorklogTimeAnalysis
				{
					TotalSpentTime = 1,
					ProjectTimeTotal = 1,
					VacationTimeTotal = 1,
					BenchTimeTotal = 1,
					ProjectTimePercentage = 1,
					VacationTimePercentage = 1,
					BenchTimePercentage = 1
				},
				new WorklogTimeAnalysis
				{
					TotalSpentTime = 2,
					ProjectTimeTotal = 2,
					VacationTimeTotal = 2,
					BenchTimeTotal = 2,
					ProjectTimePercentage = 2,
					VacationTimePercentage = 2,
					BenchTimePercentage = 2
				},
				new WorklogTimeAnalysis
				{
					TotalSpentTime = 3,
					ProjectTimeTotal = 3,
					VacationTimeTotal = 3,
					BenchTimeTotal = 3,
					ProjectTimePercentage = 3,
					VacationTimePercentage = 3,
					BenchTimePercentage = 3
				}
			};

			var reports = new List<AccountWorklogReport>();

			for (var i = 0; i < membersWithTotalSpentTime.Count; i++)
			{
				var report = new AccountWorklogReport()
				{
					AccountName = membersWithTotalSpentTime[i].DisplayName,
					DateFrom = from,
					DateTo = to,
					TotalSpentTime = worklogTimeAnalysisModelsWithTotalSpentTime[i].TotalSpentTime,
					ProjectTimeTotal = worklogTimeAnalysisModelsWithTotalSpentTime[i].ProjectTimeTotal,
					VacationTimeTotal = worklogTimeAnalysisModelsWithTotalSpentTime[i].VacationTimeTotal,
					BenchTimeTotal = worklogTimeAnalysisModelsWithTotalSpentTime[i].BenchTimeTotal,
					ProjectTimePercentage = worklogTimeAnalysisModelsWithTotalSpentTime[i].ProjectTimePercentage,
					VacationTimePercentage = worklogTimeAnalysisModelsWithTotalSpentTime[i].VacationTimePercentage,
					BenchTimePercentage = worklogTimeAnalysisModelsWithTotalSpentTime[i].BenchTimePercentage
				};
				reports.Add(report);
			}

			var options = new JsonSerializerOptions
			{
				NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
			};

			var expectedJson = JsonSerializer.Serialize(reports, options);

			// Act
			(string fileContent, string contentType) = _outputService.GetOutputByType(outputFormat, members, from, to, worklogTimeAnalysisModels);

			// Assert
			fileContent.Should().BeEquivalentTo(expectedJson);
			contentType.Should().BeEquivalentTo("application/json");
		}

		#endregion
	}
}
