using System;

namespace Runly.Models
{
	public class NodePackages
	{
		public Guid NodeId { get; set; }
		public Package[] Packages { get; set; }
	}
}
