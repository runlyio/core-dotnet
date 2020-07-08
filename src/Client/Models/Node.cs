using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Runly.Models
{
	public class Node
	{
		public Guid Id { get; set; }
		public string ClientVersion { get; set; }

		public string Machine { get; set; }
		public int Pid { get; set; }
		public string RunningAs { get; set; }

		public DateTime ConnectedAt { get; set; }
		public DateTime? PingedAt { get; set; }     
		/// <summary>
		/// The UTC date/time shutdown was requested by a user.
		/// </summary>
		public DateTime? ShutdownRequestedAt { get; set; }
		/// <summary>
		/// The username of who requsted the shutdown.
		/// </summary>
		public string ShutdownRequestedBy { get; set; }
		public DateTime? DisconnectedAt { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public DisconnectType? DisconnectType { get; set; }

		public bool IsOnline { get; set; }

		public string Organization { get; set; }
		public string Cluster { get; set; }
	}
}
