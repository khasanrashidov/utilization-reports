namespace UtilizationReports.Models.Team
{
    public class Active
    {
        public string Self { get; set; }
        public int Id { get; set; }
        public int CommitmentPercent { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public Role Role { get; set; }
    }
}