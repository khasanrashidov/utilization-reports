namespace UtilizationReports.Models.Team
{
	public class Member
	{
		public string Self { get; set; }
		public string AccountId { get; set; }
		public string DisplayName { get; set; }

		public override bool Equals(object? obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			Member otherMember = (Member)obj;

			// Compare the properties of the two objects
			return Self == otherMember.Self &&
				   AccountId == otherMember.AccountId &&
				   DisplayName == otherMember.DisplayName;
		}
		public override int GetHashCode()
		{
			return HashCode.Combine(Self, AccountId, DisplayName);
		}
	}
}
