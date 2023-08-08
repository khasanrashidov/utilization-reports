using UtilizationReports.Models.Worklog;

namespace UtilizationReports.Services
{
    public interface IWorklogsService
    {
        public Task<List<Worklog>> GetWorklogsForAccountWithDateIntervalAsync(string accountId, DateTime from, DateTime to);
        public Task<List<Worklog>> GetWorklogsForAccountWithAccountIdOnlyAsync(string accountId);
        public Task<Worklog?> GetWorklogsAsync(int limit, int offset, string accountId, DateTime? from = null, DateTime? to = null);
	}
}
