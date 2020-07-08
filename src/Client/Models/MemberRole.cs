namespace Runly.Models
{
	public enum MemberRole
	{
		// keep these in order of role with least access to role with most access
		Cluster,

		Developer,
		Ops,
		Owner
	}
}
