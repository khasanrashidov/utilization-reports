using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using UtilizationReports.Models.Worklog;
using UtilizationReports.Services;

namespace UtilizationReports.UnitTests
{
	public class WorklogsServiceUnitTests
	{
		#region Initialization

		private readonly Mock<ILogger<WorklogsService>> _loggerMock;
		private readonly IMemoryCache _memoryCache;
		private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
		private readonly Mock<IOptions<WorklogCredentialsOptions>> _optionsMock;
		private readonly Mock<IOptions<RetryPolicyOptions>> _retryPolicyOptionsMock;
		private readonly Mock<HttpMessageHandler> _msgHandler;

		public WorklogsServiceUnitTests()
		{
			var services = new ServiceCollection();
			services.AddMemoryCache();
			var serviceProvider = services.BuildServiceProvider();
			_memoryCache = serviceProvider.GetService<IMemoryCache>()!;

			_loggerMock = new Mock<ILogger<WorklogsService>>();
			_httpClientFactoryMock = new Mock<IHttpClientFactory>();
			_optionsMock = new Mock<IOptions<WorklogCredentialsOptions>>();
			_retryPolicyOptionsMock = new Mock<IOptions<RetryPolicyOptions>>();
			_msgHandler = new Mock<HttpMessageHandler>();
		}

		#endregion

		#region Tests

		[Theory]
		[MemberData(nameof(WorklogTestData.GetWorklogTestData), MemberType = typeof(WorklogTestData))]
		public async Task GetWorklogsAsync_WithValidResponseFromApi_ReturnsWorklogs(Worklog expectedWorkLog)
		{
			// Arrange
			string accountId = "12345";
			int limit = 1000;
			int offset = 0;
			DateTime? from = null;
			DateTime? to = null;

			var json = JsonConvert.SerializeObject(expectedWorkLog);
			var response = new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(json)
			};

			_optionsMock.Setup(x => x.Value).Returns(new WorklogCredentialsOptions
			{
				WorklogsForAccountIdUrl = "https://test.example.com/worklogs/",
				TokenType = "Bearer",
				AccessToken = "1234567890",
				WorklogsLimit = 1000
			});

			_retryPolicyOptionsMock.Setup(x => x.Value).Returns(new RetryPolicyOptions
			{
				MaxRetryAttempts = 3,
				RetryTimeout = 200
			});

			_msgHandler.Protected()
					   .Setup<Task<HttpResponseMessage>>("SendAsync",
						ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
					   .ReturnsAsync(response);

			var httpClient = new HttpClient(_msgHandler.Object);

			_httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

			var sut = new WorklogsService(_loggerMock.Object,
										_memoryCache,
										_httpClientFactoryMock.Object,
										_optionsMock.Object,
										_retryPolicyOptionsMock.Object);

			// Act
			var result = await sut.GetWorklogsAsync(limit, offset, accountId, from, to);

			// Assert
			result.Should().NotBeNull().And.BeOfType<Worklog>().And.BeEquivalentTo(expectedWorkLog);
			result!.Results.Should().HaveCount(expectedWorkLog.Results.Count);
			result.Results[0].Issue.Key.Should().Be(expectedWorkLog.Results[0].Issue.Key);
			result.Results[0].Author.AccountId.Should().Be(expectedWorkLog.Results[0].Author.AccountId);
			result.Results[0].Author.DisplayName.Should().Be(expectedWorkLog.Results[0].Author.DisplayName);
			result.Results[0].Description.Should().Be(expectedWorkLog.Results[0].Description);
			result.Results[0].BillableSeconds.Should().Be(expectedWorkLog.Results[0].BillableSeconds);
			result.Results[0].TimeSpentSeconds.Should().Be(expectedWorkLog.Results[0].TimeSpentSeconds);
		}

		[Theory]
		[MemberData(nameof(WorklogTestData.GetWorklogTestData), MemberType = typeof(WorklogTestData))]
		public async Task GetWorklogsForAccountWithDateIntervalAsync_WhenCalled_ShouldReturnWorklogsList(Worklog expectedWorkLog)
		{
			// Arrange
			var expectedWorklogList = new List<Worklog>
			{
				expectedWorkLog
			};

			var accountId = "12345";
			DateTime from = new(2020, 1, 1);
			DateTime to = new(2021, 1, 31);

			var json = JsonConvert.SerializeObject(expectedWorkLog);
			var response = new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(json)
			};

			_optionsMock.Setup(x => x.Value).Returns(new WorklogCredentialsOptions
			{
				WorklogsForAccountIdUrl = "https://test.example.com/worklogs/",
				TokenType = "Bearer",
				AccessToken = "1234567890",
				WorklogsLimit = 1000
			});

			_retryPolicyOptionsMock.Setup(x => x.Value).Returns(new RetryPolicyOptions
			{
				MaxRetryAttempts = 3,
				RetryTimeout = 200
			});

			_msgHandler.Protected()
					   .Setup<Task<HttpResponseMessage>>("SendAsync",
						ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
					   .ReturnsAsync(response);

			var httpClient = new HttpClient(_msgHandler.Object);

			_httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

			var worklogsServiceMethodsMock = new Mock<IWorklogsService>();
			worklogsServiceMethodsMock.Setup(m => m.GetWorklogsAsync(1000, It.IsAny<int>(), accountId, from, to)).ReturnsAsync(expectedWorkLog);
			worklogsServiceMethodsMock.Setup(m => m.GetWorklogsForAccountWithAccountIdOnlyAsync(accountId)).ReturnsAsync(expectedWorklogList);

			var sut = new WorklogsService(_loggerMock.Object,
							_memoryCache,
							_httpClientFactoryMock.Object,
							_optionsMock.Object,
							_retryPolicyOptionsMock.Object);

			// Act
			var result = await sut.GetWorklogsForAccountWithDateIntervalAsync(accountId, from, to);

			// Assert
			result.Should().BeEquivalentTo(expectedWorklogList);
			result.Should().HaveCount(expectedWorklogList.Count);
			result[0].Results.Should().BeEquivalentTo(expectedWorklogList[0].Results);
			result[0].Results[0].Issue.Key.Should().BeEquivalentTo("EJAROWA01-93"); // expectedWorklogList[0].Results[0].Issue.Key
			result[0].Results[1].Issue.Key.Should().BeEquivalentTo("ANONWORK01-4"); // expectedWorklogList[0].Results[1].Issue.Key
		}

		[Fact]
		public async Task GetWorklogsForAccountWithAccountIdOnlyAsync_WhenNoWorklogsFound_ShouldReturnWorklogsListWithEmptyResult()
		{
			// Arrange
			var expectedWorkLog = new Worklog
			{
				Self = It.IsAny<string>(),
				Metadata = new Metadata
				{
					Count = 1,
					Offset = 0,
					Limit = 1000,
					Next = It.IsAny<string>()
				},
				Results = new List<Result> { }
			};

			var expectedWorklogList = new List<Worklog>
			{
				expectedWorkLog
			};

			var accountId = "12345";
			DateTime from = new(2020, 1, 1);
			DateTime to = new(2021, 1, 31);

			var json = JsonConvert.SerializeObject(expectedWorkLog);
			var response = new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(json)
			};

			_optionsMock.Setup(x => x.Value).Returns(new WorklogCredentialsOptions
			{
				WorklogsForAccountIdUrl = "https://test.example.com/worklogs/",
				TokenType = "Bearer",
				AccessToken = "1234567890",
				WorklogsLimit = 1000
			});

			_retryPolicyOptionsMock.Setup(x => x.Value).Returns(new RetryPolicyOptions
			{
				MaxRetryAttempts = 3,
				RetryTimeout = 200
			});

			_msgHandler.Protected()
					   .Setup<Task<HttpResponseMessage>>("SendAsync",
						ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
					   .ReturnsAsync(response);

			var httpClient = new HttpClient(_msgHandler.Object, false);

			_httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

			var worklogsServiceMethodsMock = new Mock<IWorklogsService>();
			worklogsServiceMethodsMock.Setup(m => m.GetWorklogsAsync(1000, It.IsAny<int>(), accountId, from, to)).ReturnsAsync(expectedWorkLog);
			worklogsServiceMethodsMock.Setup(m => m.GetWorklogsForAccountWithDateIntervalAsync(accountId, from, to)).ReturnsAsync(expectedWorklogList);

			var sut = new WorklogsService(_loggerMock.Object,
							_memoryCache,
							_httpClientFactoryMock.Object,
							_optionsMock.Object,
							_retryPolicyOptionsMock.Object);

			// Act
			var result = await sut.GetWorklogsForAccountWithAccountIdOnlyAsync(accountId);

			// Assert
			result.Should().BeEquivalentTo(expectedWorklogList);
			result.Should().HaveCount(expectedWorklogList.Count);
			result[0].Results.Count.Should().Be(0);
			result[0].Results.Should().BeEquivalentTo(expectedWorklogList[0].Results);
		}

		#endregion
	}
}
