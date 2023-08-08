using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace UtilizationReports.IntegrationTests
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

		[Theory]
		[InlineData("aleh churak", "2022-01-01", "2023-01-01", "JSON", "application/json")]
		[InlineData("viachaslau kitsun", "2022-01-01", "2023-01-01", "CSV", "text/csv")]
		[InlineData("stanislau bychkouski", "2022-01-01", "2023-01-01", "XML", "text/xml")]
		public async Task GetWorklogByAccountNameAndDateAsync_WithCorrectInputData_ShouldReturnOk(string accountName, string dateFrom, string dateTo, string outputFormat, string expectedContentType)
		{
			// Arrange
			var client = _webApplicationFactory.CreateClient();

			// Act
			var response = await client.GetAsync($"/api/Worklogs?accountName={accountName}&dateFrom={dateFrom}&dateTo={dateTo}&outputFormat={outputFormat}");

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal(expectedContentType, response.Content.Headers.ContentType!.ToString());
		}

		[Fact]
		public async Task GetWorklogByAccountNameAndDateAsync_WithCorrectInputData_ShouldReturnOkWithoutSpecifyingOutputFormat()
		{
			// Arrange
			var client = _webApplicationFactory.CreateClient();

			// Act
			var response = await client.GetAsync($"/api/Worklogs?accountName=aleh%20churak&dateFrom=2022-01-01&dateTo=2023-01-01");

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			Assert.Equal("application/json", response.Content.Headers.ContentType!.ToString());
		}

		[Theory]
		[InlineData("viachaslau kitsun", "2023-01-01", "2022-01-01", "CSV")]
		public async Task GetWorklogByAccountNameAndDateAsync_WithIncorrectDateInputData_ShouldReturnBadRequest(string accountName, string dateFrom, string dateTo, string outputFormat)
		{
			// Arrange
			var client = _webApplicationFactory.CreateClient();

			// Act
			var response = await client.GetAsync($"/api/Worklogs?accountName={accountName}&dateFrom={dateFrom}&dateTo={dateTo}&outputFormat={outputFormat}");

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}

		[Theory]
		[InlineData("wrong name", "2022-01-01", "2023-01-01", "JSON")]
		public async Task GetWorklogByAccountNameAndDateAsync_WithIncorrectAccountNameInputData_ShouldReturnBadRequest(string accountName, string dateFrom, string dateTo, string outputFormat)
		{
			// Arrange
			var client = _webApplicationFactory.CreateClient();

			// Act
			var response = await client.GetAsync($"/api/Worklogs?accountName={accountName}&dateFrom={dateFrom}&dateTo={dateTo}&outputFormat={outputFormat}");
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Contains($"No team member found with the given account name: {accountName}", responseString);
		}

		[Theory]
		[InlineData("aleh churak", "2022-01-01", "2023-01-01", "wrong format")]
		public async Task GetWorklogByAccountNameAndDateAsync_WithIncorrectOutputFormatInputData_ShouldReturnBadRequest(string accountName, string dateFrom, string dateTo, string outputFormat)
		{
			// Arrange
			var client = _webApplicationFactory.CreateClient();

			// Act
			var response = await client.GetAsync($"/api/Worklogs?accountName={accountName}&dateFrom={dateFrom}&dateTo={dateTo}&outputFormat={outputFormat}");
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			Assert.Contains($"The value '{outputFormat}' is not valid.", responseString);
		}

		#endregion
	}
}
