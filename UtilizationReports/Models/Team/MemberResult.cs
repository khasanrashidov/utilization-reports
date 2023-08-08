namespace UtilizationReports.Models.Team
{
    public class MemberResult
    {
        public string Self { get; set; }
        public TeamInfo Team { get; set; }
        public Member Member { get; set; }
        public Memberships Memberships { get; set; }
    }
}
