using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text;
using UtilizationReports.Extensions;
using UtilizationReports.Models.Team;
using UtilizationReports.Services;

namespace UtilizationReports.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class WorklogsController : ControllerBase
	{
		private readonly IWorklogsService _worklogsService;
		private readonly IOutputService _outputService;
		private readonly ILogger<WorklogsController> _logger;
		private readonly IAccountIdMapperService _accountIdMapperService;
		private readonly ITeamMembersService _teamMembersService;

		public WorklogsController(
			IWorklogsService worklogsService,
			IOutputService outputService,
			ILogger<WorklogsController> logger,
			IAccountIdMapperService accountIdMapperService,
			ITeamMembersService teamMembersService)
		{
			_worklogsService = worklogsService;
			_outputService = outputService;
			_logger = logger;
			_accountIdMapperService = accountIdMapperService;
			_teamMembersService = teamMembersService;
		}

		/// <summary>
		/// This method is used to get worklogs for a specific account (account name) and date range (date from and date to).
		/// </summary>
		/// <param name="accountName">
		/// The account name for which the worklogs are fetched. Account name is case insensitive.
		/// </param>
		/// <param name="dateFrom">
		/// The "date from" parameter of date range for which the worklogs are fetched.
		/// </param>
		/// <param name="dateTo">
		/// The "date to" parameter of date range for which the worklogs are fetched.
		/// </param>
		/// <param name="outputFormat">
		/// The output format of the response. It can be JSON (default option), CSV, or XML.
		/// </param>
		/// <returns>
		/// Returns worklog report (in user-specified format: JSON, CSV, or XML) for a specific account (account name) and date range (date from and date to).
		/// </returns>
		[HttpGet]
		public async Task<IActionResult> GetWorklogByAccountNameAndDateAsync(
			[FromQuery][Required] string accountName,
			[FromQuery][Required] DateTime dateFrom,
			[FromQuery][Required] DateTime dateTo,
			[FromQuery] OutputFormat outputFormat = OutputFormat.JSON)
		{

			if (dateFrom > dateTo)
			{
				_logger.LogError("Date from is greater than date to");
				return BadRequest("Date from is greater than date to");
			}
			if (dateFrom > DateTime.Now || dateTo > DateTime.Now)
			{
				_logger.LogError("Date from or date to is greater than current date");
				return BadRequest("Date from or date to is greater than current date");
			}

			var member = await _accountIdMapperService.GetMemberFromAccountNameAsync(accountName);

			var workLogs = await _worklogsService.GetWorklogsForAccountWithDateIntervalAsync(member.AccountId, dateFrom, dateTo);

			var (fileContent, contentType) = _outputService.GetOutputByType(outputFormat,
																 new List<Member> { member },
																 dateFrom,
																 dateTo,
																 workLogs.ToList().MapToWorklogTimeAnalysisModel());

			return File(Encoding.UTF8.GetBytes(fileContent), contentType);
		}

		/// <summary>
		/// This method is used to get worklogs for all team members and date range (date from and date to).
		/// </summary>
		/// <param name="dateFrom">
		/// The "date from" parameter of date range for which the worklogs are fetched.
		/// </param>
		/// <param name="dateTo">
		/// The "date to" parameter of date range for which the worklogs are fetched.
		/// </param>
		/// <param name="outputFormat">
		/// The output format of the response. It can be JSON (default option), CSV, or XML.
		/// </param>
		/// <returns>
		/// Returns worklog report (in user-specified format: JSON, CSV, or XML) for all team members and date range (date from and date to).
		/// </returns>
		[HttpGet("team")]
		public async Task<IActionResult> GetWorklogsForAllTeamMembersAsync(
			[FromQuery][Required] DateTime dateFrom,
			[FromQuery][Required] DateTime dateTo,
			[FromQuery] OutputFormat outputFormat = OutputFormat.JSON)
		{

			if (dateFrom > dateTo)
			{
				_logger.LogError("Date from is greater than date to");
				return BadRequest("Date from is greater than date to");
			}
			if (dateFrom > DateTime.Now || dateTo > DateTime.Now)
			{
				_logger.LogError("Date from or date to is greater than current date");
				return BadRequest("Date from or date to is greater than current date");
			}

			var membersOfATeam = await _teamMembersService.GetMembersOfATeamAsync();

			var workLogTasks = membersOfATeam?.Results.Select(x => _worklogsService.GetWorklogsForAccountWithDateIntervalAsync(x.Member.AccountId, dateFrom, dateTo));
			var worklogs = await Task.WhenAll(workLogTasks!);

			var worklogTimeAnalysisModels = worklogs.SelectMany(x => x.MapToWorklogTimeAnalysisModel()).ToList();

			var members = membersOfATeam!.Results.Select(x => x.Member).ToList();

			var (fileContent, contentType) = _outputService.GetOutputByType(outputFormat,
																			members,
																			dateFrom,
																			dateTo,
																			worklogTimeAnalysisModels);

			return File(Encoding.UTF8.GetBytes(fileContent), contentType);
		}
	}
}
