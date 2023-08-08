using UtilizationReports.Services;

namespace UtilizationReports
{
	public static class ConfigServiceCollectionExtensions
	{
		public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration configuration)
		{
			// Registering options with the dependency injection container
			services.Configure<WorklogCredentialsOptions>(configuration.GetSection(WorklogCredentialsOptions.WorklogCredentials));
			services.Configure<RetryPolicyOptions>(configuration.GetSection(RetryPolicyOptions.RetryPolicy));
			
			return services;
		}

		public static IServiceCollection AddDependencyGroup(this IServiceCollection services)
		{
			// Registering services with the dependency injection container
			services.AddScoped<IWorklogsService, WorklogsService>();
			services.AddScoped<ITeamMembersService, TeamMembersService>();
			services.AddScoped<IOutputService, OutputService>();
			services.AddScoped<IAccountIdMapperService, AccountIdMapperService>();
			
			return services;
		}
	}
}
