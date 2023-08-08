using UtilizationReports.Models.Worklog;

namespace UtilizationReports.Models.Team
{
    public class Team
    {
        public string Self { get; set; }
        public Metadata Metadata { get; set; }
        public List<MemberResult> Results { get; set; }
    }
}
