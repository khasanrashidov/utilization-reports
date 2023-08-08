namespace UtilizationReports
{
	public class RetryPolicyOptions
	{
		public const string RetryPolicy = "RetryPolicy";
		public int MaxRetryAttempts { get; set; } = 3; // Default value
		public int RetryTimeout { get; set; } = 200; // Default value
	}
}
