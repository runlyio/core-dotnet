using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Runly.Hosting
{
	/// <summary>
	/// A cache of <see cref="JobInfo">JobInfos</see> for the job types found in the assemblies provided.
	/// </summary>
	public class JobCache
	{
		/// <summary>
		/// Gets the jobs found in the assemblies provided.
		/// </summary>
		public IEnumerable<JobInfo> Jobs { get; }

		/// <summary>
		/// Initializes a new <see cref="JobCache"/>.
		/// </summary>
		/// <param name="jobAssemblies">The assemblies to search for jobs.</param>
		public JobCache(IEnumerable<Assembly> jobAssemblies)
		{
			Jobs = FindJobTypes(jobAssemblies);
		}

		IEnumerable<JobInfo> FindJobTypes(IEnumerable<Assembly> jobAssemblies)
		{
			var result = new List<JobInfo>();

			var processTypes = jobAssemblies
				.SelectMany(a => a.GetTypes())
				.Where(t => typeof(IJob).IsAssignableFrom(t) && !t.IsInterface);

			foreach (var type in processTypes)
				result.Add(new JobInfo(type));

			return result;
		}

		/// <summary>
		/// Gets the <see cref="JobInfo"/> for the job type specified.
		/// </summary>
		/// <param name="type">The type to get the <see cref="JobInfo"/> for.</param>
		/// <returns>A <see cref="JobInfo"/> or null if the type cannot be found.</returns>
		public JobInfo Get(Type type)
		{
			return Jobs.FirstOrDefault(pi => pi.JobType == type);
		}

		/// <summary>
		/// Gets the <see cref="JobInfo"/> for the job type specified.
		/// </summary>
		/// <param name="jobType">The type to get the <see cref="JobInfo"/> for.</param>
		/// <returns>A <see cref="JobInfo"/> or null if the type cannot be found.</returns>
		/// <remarks>
		/// An exact case match is attempted first for either the type <see cref="MemberInfo.Name"/>
		/// or <see cref="Type.FullName"/>. If no matches are found a case insensitive match is attempted.
		/// If zero or more than one job type matches, a <see cref="TypeNotFoundException"/> will be thrown.
		/// </remarks>
		public JobInfo Get(string jobType)
		{
			return ResolveJobInfo(jobType);
		}

		/// <summary>
		/// Gets the default <see cref="Config"/> for the job type specified.
		/// </summary>
		/// <param name="jobType">The type to get the default config for.</param>
		/// <remarks>
		/// An exact case match is attempted first for either the type <see cref="MemberInfo.Name"/>
		/// or <see cref="Type.FullName"/> of the job type. If no matches are found a case insensitive 
		/// match is attempted. If zero or more than one job type matches, a <see cref="TypeNotFoundException"/>
		/// will be thrown.
		/// </remarks>
		public Config GetDefaultConfig(string jobType)
		{
			return GetDefaultConfig(ResolveJobInfo(jobType));
		}

		/// <summary>
		/// Gets the default <see cref="Config"/> for the <paramref name="jobInfo"/>.
		/// </summary>
		/// <param name="jobInfo">The <see cref="JobInfo"/> to get the default config for.</param>
		/// <returns>The default config for the job.</returns>
		public Config GetDefaultConfig(JobInfo jobInfo)
		{
			var config = (Config)Activator.CreateInstance(jobInfo.ConfigType);

			config.Job.Type = jobInfo.JobType.FullName;
			config.Job.Package = jobInfo.JobType.Assembly.GetName().Name;
			config.Job.Version = jobInfo.JobType.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

			return config;
		}

		JobInfo ResolveJobInfo(string type)
		{
			var types = Jobs.Where(i => i.JobType.Name == type || i.JobType.FullName == type);

			if (types.Count() == 0)
				types = Jobs.Where(i => i.JobType.Name.ToLower() == type.ToLower() || i.JobType.FullName.ToLower() == type.ToLower());

			if (types.Count() != 1)
				throw new TypeNotFoundException(type);

			return types.Single();
		}
	}
}
