using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using UtilizationReports.Models.Team;

namespace UtilizationReports.Services
{
	public class TeamMembersService : ITeamMembersService
	{
		private readonly WorklogCredentialsOptions _worklogCredentialsOptions;
		private readonly ILogger<TeamMembersService> _logger;
		private readonly IMemoryCache _memoryCache;
		private readonly IHttpClientFactory _httpClientFactory;

		public TeamMembersService(ILogger<TeamMembersService> logger,
									IMemoryCache memoryCache,
									IHttpClientFactory httpClientFactory,
									IOptions<WorklogCredentialsOptions> options)
		{
			_logger = logger;
			_memoryCache = memoryCache;
			_httpClientFactory = httpClientFactory;
			_worklogCredentialsOptions = options.Value;
		}

		public async Task<Team?> GetMembersOfATeamAsync()
		{
			if (string.IsNullOrEmpty(_worklogCredentialsOptions?.TeamMembersUrl)) throw new ArgumentException("URL is missing");

			if (_memoryCache.TryGetValue("TeamMembers", out Team? cachedTeamMembers))
			{
				_logger.LogInformation("Fetching team members from cache");
				return cachedTeamMembers;
			}

			UriBuilder urlBuilder = new(_worklogCredentialsOptions.TeamMembersUrl);

			var cacheKey = $"Worklogs_{urlBuilder.Uri}";

			using var _httpClient = _httpClientFactory.CreateClient();
			try
			{
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_worklogCredentialsOptions.TokenType, _worklogCredentialsOptions.AccessToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while setting up authorization header: {ExMessage}", ex.Message);
			}

			_logger.LogInformation("Fetching team members");

			HttpResponseMessage response = await _httpClient.GetAsync(urlBuilder.Uri);

			if (response.IsSuccessStatusCode)
			{
				string responseBody = await response.Content.ReadAsStringAsync();

				Team? result = JsonConvert.DeserializeObject<Team>(responseBody);

				_memoryCache.Set(cacheKey, result);

				_logger.LogInformation("Team fetched successfully");

				return result;
			}

			Exception exc = new($"Error occurred while fetching data. Status code: {response.StatusCode}");
			_logger.LogError(exc, "Error occurred while fetching data. Status code: {StatusCode}", response.StatusCode);
			throw exc;
		}
	}
}
