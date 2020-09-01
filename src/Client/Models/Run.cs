﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Runly.Client.Models
{
	public enum RunState
	{
		Enqueued,
		Running = 3, // Set to 3 and 5 to match InstanceState
		Successful = 5,
		Failed,
		Cancelled,
		TimedOut,
		Error
	}

	public class Run
	{
		/// <summary>
		/// The unique identifier of the run.
		/// </summary>
		/// <example>215d7714-40b3-406f-88ac-5f6ba2b9755b</example>
		public Guid Id { get; set; }

		public EnvironmentInfo Environment { get; set; }

		public string Template { get; set; }
		public bool IsScheduled { get; set; }

		/// <summary>
		/// The current state of the run. The state is terminal when CompletedAt is not null.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		public RunState State { get; set; }
		/// <summary>
		/// The UTC date/time the job was enqueued.
		/// </summary>
		public DateTime EnqueuedAt { get; set; }
		/// <summary>
		/// The username of who enqueued the job.
		/// </summary>
		/// <example>homer.simpson</example>
		public string EnqueuedBy { get; set; }

		/// <summary>
		/// The UTC date/time the first instance started.
		/// </summary>
		public DateTime? StartedAt { get; set; }
		/// <summary>
		/// The UTC date/time cancellation was requested by a user.
		/// </summary>
		public DateTime? CancellationRequestedAt { get; set; }
		/// <summary>
		/// The username of who requsted the cancellation of the run.
		/// </summary>
		public string CancellationRequestedBy { get; set; }

		/// <summary>
		/// The UTC date/time all instances of the run were confirmed complete.
		/// </summary>
		public DateTime? CompletedAt { get; set; }

		public JobInfo Job { get; set; }

		public ProgressInfo Progress { get; set; }

		public NodeInfo Node { get; set; }

		public class EnvironmentInfo
		{
			public string Id { get; set; }
			public bool IsProduction { get; set; }
		}

		public class JobInfo
		{
			/// <summary>
			/// The organization that owns this package.
			/// </summary>
			/// <example>my-org</example>
			public string Org { get; set; }

			/// <summary>
			/// The package name where the job can be found.
			/// </summary>
			/// <example>MyAwesomePackage</example>
			public string Package { get; set; }
			/// <summary>
			/// The package/job version.
			/// </summary>
			/// <example>1.0.0</example>
			public string Version { get; set; }

			/// <summary>
			/// The fully qualified type name of the job.
			/// </summary>
			/// <example>My.Namespace.HelloWorld</example>
			public string Type { get; set; }

			/// <summary>
			/// The short name of the job.
			/// </summary>
			/// <example>HelloWorld</example>
			public string Name { get; set; }
		}

		public class ProgressInfo
		{
			public int Success { get; set; }
			public int Failed { get; set; }
			public int? Total { get; set; }

			public IEnumerable<ItemProgress> Categories { get; set; } = new ItemProgress[0];
		}

		public class NodeInfo
		{
			public Guid Id { get; set; }
			public string Machine { get; set; }
			public string Cluster { get; set; }
		}
	}
}
