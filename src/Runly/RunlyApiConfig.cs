﻿using System;

namespace Runly
{
	/// <summary>
	/// Runly API connection information used to report progress and store results of the job.
	/// </summary>
	/// <remarks>The data in this class is used by the Runly API, node and/or job host. This class should not be extended.</remarks>
	public class RunlyApiConfig
	{
		/// <summary>
		/// The URI of the Runly API, https://api.runly.io/ by default.
		/// </summary>
		public string Uri { get; set; }

		/// <summary>
		/// The auth token used to grant the job access to the API.
		/// </summary>
		public string Token { get; set; }

		/// <summary>
		/// The org that the run is queued or executing in.
		/// </summary>
		public string OrganizationId { get; set; }

		/// <summary>
		/// The environment that the run is queued or executing in.
		/// </summary>
		public string EnvironmentId { get; set; }
		
		/// <summary>
		/// The ID of the instance assigned by the API.
		/// </summary>
		public Guid InstanceId { get; set; }

		/// <summary>
		/// The name of the cluster that the run is assigned to.
		/// </summary>
		/// <remarks>If a value is provided when queued, the run with be assigned to
		/// the node specified. If the cluster does not exist the HTTP response will 
		/// be 409 Conflict. If no nodes are connected to the cluster the HTTP response
		/// will be 202 Accepted.</remarks>
		public string Cluster { get; set; }

		/// <summary>
		/// The name of the cluster that the run is assigned to.
		/// </summary>
		/// <remarks>If a value is provided when queued, the run with be assigned to
		/// the cluster specified. If the node does not exist or is not connected the HTTP 
		/// response will be 409 Conflict.</remarks>
		public string Node { get; set; }

		/// <summary>
		/// When true, successful item results will be sent to the API. This is similar to turning on debug logging, it may severely impact the performance of jobs.
		/// </summary>
		public bool LogSuccessfulItemResults { get; set; }

		/// <summary>
		/// Initializes a new <see cref="RunlyApiConfig"/>.
		/// </summary>
		public RunlyApiConfig()
		{
			Uri = "https://api.runly.io/";
		}
	}
}
