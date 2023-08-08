using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace UtilizationReports.IntegrationTests
{
	public class TeamWorklogsControllerIntegrationTests
	{
		public class AccountWorklogsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
		{
			#region Initialization

			private readonly WebApplicationFactory<Program> _webApplicationFactory;

			public AccountWorklogsControllerIntegrationTests(WebApplicationFactory<Program> webApplicationFactory)
			{
				_webApplicationFactory = webApplicationFactory;
			}

			#endregion

			#region Tests

			[Fact]
			public async Task GetWorklogsForAllTeamMembersAsync_WithCorrectInputDataWithoutSpecifyingOutputFormat_ShouldReturnOk()
			{
				// Arrange
				var client = _webApplicationFactory.CreateClient();

				// Act
				var response = await client.GetAsync($"/api/Worklogs/team?dateFrom=2022-01-01&dateTo=2023-01-01");

				// Assert
				Assert.Equal(HttpStatusCode.OK, response.StatusCode);
				Assert.Equal("application/json", response.Content.Headers.ContentType!.ToString());
			}

			[Fact]
			public async Task GetWorklogsForAllTeamMembersAsync_WithWrongInputDate_ShouldReturnBadRequest()
			{
				// Arrange
				var client = _webApplicationFactory.CreateClient();

				// Act
				var response = await client.GetAsync($"/api/Worklogs/team?dateFrom=2023-05-01&dateTo=2023-02-01");

				// Assert
				Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			}

			[Fact]
			public async Task GetWorklogsForAllTeamMembersAsync_WithNoDataSpecified_ShouldReturnBadRequest()
			{
				// Arrange
				var client = _webApplicationFactory.CreateClient();

				// Act
				var response = await client.GetAsync($"/api/Worklogs/team");

				// Assert
				Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
				Assert.Contains("One or more validation errors occurred", response.Content.ReadAsStringAsync().Result);
				Assert.Contains("The dateTo field is required", response.Content.ReadAsStringAsync().Result);
				Assert.Contains("The dateFrom field is required", response.Content.ReadAsStringAsync().Result);
			}

			#endregion
		}
	}
}
