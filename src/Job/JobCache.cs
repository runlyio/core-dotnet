using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Runly
{
	public class JobCache
	{
		public IEnumerable<JobInfo> Jobs { get; }

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

		public JobInfo Get(Type type)
		{
			return Jobs.FirstOrDefault(pi => pi.JobType == type);
		}

		public JobInfo Get(string type)
		{
			return ResolveJobInfo(type);
		}

		public Config GetDefaultConfig(string processType)
		{
			return GetDefaultConfig(ResolveJobInfo(processType));
		}

		public Config GetDefaultConfig(JobInfo pi)
		{
			var config = (Config)Activator.CreateInstance(pi.ConfigType);

			config.Job.Type = pi.JobType.FullName;
			config.Job.Package = pi.JobType.Assembly.GetName().Name;
			config.Job.Version = pi.JobType.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

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
