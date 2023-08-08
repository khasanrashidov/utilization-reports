using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilizationReports.Models.Worklog;

namespace UtilizationReports.UnitTests
{
	public static class WorklogTestData
	{
		public static IEnumerable<object[]> GetWorklogTestData()
		{
			var expectedWorkLog = new Worklog
			{
				Self = It.IsAny<string>(),
				Metadata = new Metadata
				{
					Count = It.IsAny<int>(),
					Offset = 0,
					Limit = 1000,
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
						Key = "ANONWORK01-4",
						Id = It.IsAny<int>()
					},
					TimeSpentSeconds = 1000,
					BillableSeconds = 0,
					StartDate = It.IsAny<DateTime>(),
					StartTime = It.IsAny<string>(),
					Description = "Vacation",
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
						Key = "ANONWORK01-4",
						Id = It.IsAny<int>()
					},
					TimeSpentSeconds = 1000,
					BillableSeconds = 0,
					StartDate = It.IsAny<DateTime>(),
					StartTime = It.IsAny<string>(),
					Description = "No projects right now (bench)",
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
						Key = "EJAROWA01-5696",
						Id = It.IsAny<int>()
					},
					TimeSpentSeconds = 5000,
					BillableSeconds = 1000,
					StartDate = It.IsAny<DateTime>(),
					StartTime = It.IsAny<string>(),
					Description = "Work on project",
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
						Key = "ANONWORK01-4",
						Id = It.IsAny<int>()
					},
					TimeSpentSeconds = 5000,
					BillableSeconds = 0,
					StartDate = It.IsAny<DateTime>(),
					StartTime = It.IsAny<string>(),
					Description = "Vacation",
					Author = new Author
					{
						Self = It.IsAny<string>(),
						AccountId = "12345",
						DisplayName = It.IsAny<string>()
					}
				}
				}
			};

			yield return new object[] { expectedWorkLog };
		}
	}
}
