using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using UtilizationReports.Models.Team;
using UtilizationReports.Services;

namespace UtilizationReports.UnitTests
{
	public class TeamMembersServiceUnitTests
	{
		#region Initialization

		private readonly Mock<ILogger<TeamMembersService>> _loggerMock;
		private readonly IMemoryCache _memoryCache;
		private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
		private readonly Mock<IOptions<WorklogCredentialsOptions>> _optionsMock;
		private readonly Mock<HttpMessageHandler> _msgHandler;

		public TeamMembersServiceUnitTests()
		{
			var services = new ServiceCollection();
			services.AddMemoryCache();
			var serviceProvider = services.BuildServiceProvider();
			_memoryCache = serviceProvider.GetService<IMemoryCache>()!;

			_loggerMock = new Mock<ILogger<TeamMembersService>>();
			_httpClientFactoryMock = new Mock<IHttpClientFactory>();
			_optionsMock = new Mock<IOptions<WorklogCredentialsOptions>>();
			_msgHandler = new Mock<HttpMessageHandler>();
		}

		#endregion

		#region Tests

		[Theory]
		[MemberData(nameof(TeamTestData.GetTeamTestData), MemberType = typeof(TeamTestData))]
		public async Task GetMembersOfATeamAsync_WithValidResponseFromApi_ReturnsTeam(Team expectedTeam)
		{
			// Arrange
			var responseBody = JsonConvert.SerializeObject(expectedTeam);
			var response = new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(responseBody)
			};

			_optionsMock.Setup(x => x.Value).Returns(new WorklogCredentialsOptions
			{
				TeamMembersUrl = "https://test.example.com/members",
				TokenType = "Bearer",
				AccessToken = "1234567890"
			});

			_msgHandler.Protected()
					   .Setup<Task<HttpResponseMessage>>("SendAsync",
						ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
					   .ReturnsAsync(response);

			var httpClient = new HttpClient(_msgHandler.Object);

			_httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

			var sut = new TeamMembersService(_loggerMock.Object,
											_memoryCache,
											_httpClientFactoryMock.Object,
											_optionsMock.Object);

			// Act
			var result = await sut.GetMembersOfATeamAsync();

			// Assert
			result.Should().NotBeNull().And.BeOfType<Team>().And.BeEquivalentTo(expectedTeam);
			result!.Metadata.Count.Should().Be(expectedTeam.Metadata.Count);
			result.Results.Should().NotBeNullOrEmpty().And.HaveCount(expectedTeam.Results.Count);
		}

		#endregion
	}
}
