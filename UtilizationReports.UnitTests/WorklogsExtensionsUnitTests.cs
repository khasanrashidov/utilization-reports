using FluentAssertions;
using Moq;
using UtilizationReports.Extensions;
using UtilizationReports.Models.Worklog;

namespace UtilizationReports.UnitTests
{
	public class WorklogsExtensionsUnitTests
	{
		#region Tests

		[Theory]
		[InlineData("ANONWORK01-4", 1000, 0, "Vacation", 2000, 1000, 1000, 0, 50, 50, 0)]
		[InlineData("ANONWORK01-4", 1000, 0, "No projects right now (bench)", 2000, 1000, 0, 1000, 50, 0, 50)]
		[InlineData("EJAROWA01-5696", 5000, 1000, "Work on project", 6000, 6000, 0, 0, 100, 0, 0)]
		[InlineData("ANONWORK01-4", 5000, 0, "Vacation", 6000, 1000, 5000, 0, 16.667, 83.333, 0)]
		public void MapToWorklogTimeAnalysisModel_WhenCalled_ShouldMapWorklogsToWorklogTimeAnalysisModel(string issueKey,
																	int timeSpentSeconds,
																	int billableSeconds,
																	string description,
																	double expectedTotalSpentTime,
																	double expectedProjectTimeTotal,
																	double expectedVacationTimeTotal,
																	double expectedBenchTimeTotal,
																	double expectedProjectTimePercentage,
																	double expectedVacationTimePercentage,
																	double expectedBenchTimePercentage)
		{
			// Arrange
			var workLog = new Worklog
			{
				Self = It.IsAny<string>(),
				Metadata = new Metadata
				{
					Count = It.IsAny<int>(),
					Offset = 0,
					Limit = 50,
					Next = It.IsAny<string>()
				},
				Results = new List<Result>
					{
						new Result
						{
							Issue = new Issue
							{
								Self = It.IsAny<string>(),
								Key = "EJAROWA01-93",
								Id = It.IsAny<int>()
							},
							TimeSpentSeconds = 1000,
							BillableSeconds = 1000,
							StartDate = It.IsAny<DateTime>(),
							StartTime = It.IsAny<string>(),
							Description = "Working on issue EJAROWA01-93",
							Author = new Author
							{
								Self = It.IsAny<string>(),
								AccountId = "12345",
								DisplayName = It.IsAny<string>()
							}
						},
						new Result
						{
							Issue = new Issue
							{
								Self = It.IsAny<string>(),
								Key = issueKey,
								Id = It.IsAny<int>()
							},
							TimeSpentSeconds = timeSpentSeconds,
							BillableSeconds = billableSeconds,
							StartDate = It.IsAny<DateTime>(),
							StartTime = It.IsAny<string>(),
							Description = description,
							Author = new Author
							{
								Self = It.IsAny<string>(),
								AccountId = "12345",
								DisplayName = It.IsAny<string>()
							}
						}
					}
			};

			var worklogsList = new List<Worklog>
			{
				workLog
			};

			// Act
			var analysisResults = worklogsList.MapToWorklogTimeAnalysisModel();

			// Assert
			analysisResults.Should().NotBeNull();
			analysisResults.Should().HaveCount(1);

			var analysis = analysisResults[0];

			analysis.TotalSpentTime.Should().Be(expectedTotalSpentTime);
			analysis.ProjectTimeTotal.Should().Be(expectedProjectTimeTotal);
			analysis.VacationTimeTotal.Should().Be(expectedVacationTimeTotal);
			analysis.BenchTimeTotal.Should().Be(expectedBenchTimeTotal);
			analysis.ProjectTimePercentage.Should().Be(expectedProjectTimePercentage);
			analysis.VacationTimePercentage.Should().Be(expectedVacationTimePercentage);
			analysis.BenchTimePercentage.Should().Be(expectedBenchTimePercentage);
		}

		[Fact]
		public void CreateWorklogTimeAnalysisModelAsync_ShouldReturnEmptyModel_WhenNoWorklogs()
		{
			// Arrange
			var worklogsList = new List<Worklog>();

			// Act
			var analysisResults = worklogsList.MapToWorklogTimeAnalysisModel();

			analysisResults.Should().NotBeNull();
			analysisResults.Should().HaveCount(1); // because there is a model, but with default values

			var result = analysisResults[0];

			result.TotalSpentTime.Should().Be(0.0);
			result.ProjectTimeTotal.Should().Be(0.0);
			result.VacationTimeTotal.Should().Be(0.0);
			result.BenchTimeTotal.Should().Be(0.0);
			result.ProjectTimePercentage.Should().Be(double.NaN); // Because 0/0 is NaN
			result.VacationTimePercentage.Should().Be(double.NaN); // Because 0/0 is NaN
			result.BenchTimePercentage.Should().Be(double.NaN); // Because 0/0 is NaN
		}

		#endregion
	}
}
