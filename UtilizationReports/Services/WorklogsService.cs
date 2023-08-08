using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using UtilizationReports.Models.Worklog;

namespace UtilizationReports.Services
{
	public class WorklogsService : IWorklogsService
	{
		private readonly WorklogCredentialsOptions _worklogCredentialsOptions;
		private readonly RetryPolicyOptions _retryPolicyOptions;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IMemoryCache _memoryCache;
		private readonly ILogger<WorklogsService> _logger;

		public WorklogsService(
			ILogger<WorklogsService> logger,
			IMemoryCache memoryCache,
			IHttpClientFactory httpClientFactory,
			IOptions<WorklogCredentialsOptions> options,
			IOptions<RetryPolicyOptions> retryPolicyOptions)
		{
			_worklogCredentialsOptions = options.Value;
			_retryPolicyOptions = retryPolicyOptions.Value;
			_httpClientFactory = httpClientFactory;
			_memoryCache = memoryCache;
			_logger = logger;
		}

		public async Task<List<Worklog>> GetWorklogsForAccountWithDateIntervalAsync(string accountId, DateTime from, DateTime to)
		{
			List<Worklog> worklogsList = new();
			int i = 0;
			while (true)
			{
				var result = await GetWorklogsAsync(limit: _worklogCredentialsOptions.WorklogsLimit,
													offset: i * _worklogCredentialsOptions.WorklogsLimit,
													accountId, from, to)!;
				if (result != null)
				{
					worklogsList.Add(result);
				}
				if (result?.Metadata?.Count < _worklogCredentialsOptions.WorklogsLimit)
				{
					break;
				}
				i++;
			}

			var resultCheck = worklogsList.Where(x => x.Results == null || x.Results.Count == 0);
			if (resultCheck.Any())
			{
				_logger.LogWarning("Worklogs for account {AccountId} are empty, so just fetching his all worklogs (if there are any)", accountId);
				worklogsList = await GetWorklogsForAccountWithAccountIdOnlyAsync(accountId);

				// now, even it is empty, we return it
				return worklogsList;
			}

			return worklogsList;
		}

		public async Task<List<Worklog>> GetWorklogsForAccountWithAccountIdOnlyAsync(string accountId)
		{
			List<Worklog> worklogsList = new();
			int i = 0;
			while (true)
			{
				var result = await GetWorklogsAsync(limit: _worklogCredentialsOptions.WorklogsLimit,
													offset: i * _worklogCredentialsOptions.WorklogsLimit,
													accountId)!;
				if (result != null)
				{
					worklogsList.Add(result);
				}
				if (result?.Metadata?.Count < _worklogCredentialsOptions.WorklogsLimit)
				{
					break;
				}
				i++;
			}

			return worklogsList;
		}

		public async Task<Worklog?> GetWorklogsAsync(int limit, int offset, string accountId,
														DateTime? from = null, DateTime? to = null)
		{
			if (string.IsNullOrEmpty(_worklogCredentialsOptions.WorklogsForAccountIdUrl)) throw new ArgumentException("URL is missing");

			UriBuilder urlBuilder = new(_worklogCredentialsOptions.WorklogsForAccountIdUrl);
			urlBuilder.Path += accountId;
			if (from != null && to != null)
			{
				urlBuilder.Query = $"from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}&offset={offset}&limit={limit}";
			}
			else
			{
				urlBuilder.Query = $"offset={offset}&limit={limit}";
			}

			var cacheKey = $"Worklogs_{urlBuilder.Uri}";

			if (_memoryCache.TryGetValue(cacheKey, out Worklog? cachedWorklogs))
			{
				_logger.LogInformation("Fetching worklogs from cache");
				return cachedWorklogs;
			}

			using var _httpClient = _httpClientFactory.CreateClient();
			try
			{
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_worklogCredentialsOptions.TokenType, _worklogCredentialsOptions.AccessToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while setting up authorization header: {ExMessage}", ex.Message);
			}

			_logger.LogInformation("Fetching worklogs");

			int MaxRetryAttempts = _retryPolicyOptions.MaxRetryAttempts;
			bool retry = true;

			while (retry)
			{
				HttpResponseMessage response = await _httpClient.GetAsync(urlBuilder.Uri);

				if (response.IsSuccessStatusCode)
				{
					string responseBody = await response.Content.ReadAsStringAsync();

					Worklog? result = JsonConvert.DeserializeObject<Worklog>(responseBody);

					_memoryCache.Set(cacheKey, result);

					_logger.LogInformation("Worklogs fetched successfully");

					return result;
				}

				if (response.StatusCode == HttpStatusCode.TooManyRequests)
				{
					if (MaxRetryAttempts != 0)
					{
						_logger.LogWarning("Too many requests. Retrying after {timeout} ms", _retryPolicyOptions.RetryTimeout);
						await Task.Delay(_retryPolicyOptions.RetryTimeout);
						MaxRetryAttempts--;
					}
					else
					{
						retry = false; // exceeded maximum retries, exit the loop
					}
				}
				else
				{
					Exception exc = new($"Error occurred while fetching data. Status code: {response.StatusCode}");
					_logger.LogError(exc, "Error occurred while fetching data. Status code: {StatusCode}", response.StatusCode);
					throw exc;
				}
			}

			// Handle the case when the maximum number of retries is reached
			Exception maxRetryExc = new("Maximum number of retries reached.");
			_logger.LogError(maxRetryExc, "Maximum number of retries reached.");
			throw maxRetryExc;
		}
	}
}