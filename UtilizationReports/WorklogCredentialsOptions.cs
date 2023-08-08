namespace UtilizationReports
{
	public class WorklogCredentialsOptions
	{
		public const string WorklogCredentials = "WorklogCredentials";
		public string TeamMembersUrl { get; set; } = string.Empty;
		public string WorklogsForAccountIdUrl { get; set; } = string.Empty;
		public string TokenType { get; set; } = string.Empty;
		public string AccessToken { get; set; } = string.Empty;
		public int WorklogsLimit { get; set; } = 1000; // Default value

	}
}
