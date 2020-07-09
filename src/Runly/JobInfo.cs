using System;
using System.Collections.Generic;

namespace Runly
{
	public class JobInfo
	{
		public Type JobType { get; private set; }

		public Type ItemType { get; private set; }

		public Type ConfigType { get; private set; }

		public Type[] Dependencies { get; private set; }

		public JobLoadErrors Errors { get; private set; }

		public bool IsValid => Errors == JobLoadErrors.None;

		public JobInfo(Type jobType)
		{
			this.JobType = jobType ?? throw new ArgumentNullException(nameof(jobType));

			if (!typeof(IJob).IsAssignableFrom(jobType))
				throw new ArgumentException($"{jobType.FullName} does not implement {typeof(IJob).FullName}");

			Errors |= jobType.IsInterface ? JobLoadErrors.IsInterface : jobType.IsAbstract ? JobLoadErrors.IsAbstract : JobLoadErrors.None;
			Errors |= jobType.IsGenericTypeDefinition ? JobLoadErrors.IsGenericTypeDefinition : JobLoadErrors.None;

			FindJobBaseType(jobType);
		}

		private void FindJobBaseType(Type jobType)
		{
			// The types we're looking for are abstract so they will
			// be extended at least one time.
			do
			{
				jobType = jobType.BaseType;
			}
			while (!IsJobBaseType(jobType));

			ReadGenericTypeArguments(jobType);
		}

		private bool IsJobBaseType(Type type)
		{
			// Looking for the generic types which will have the generic type 
			// parameters specified, so we can't know the full type at compile time.
			return type.Assembly == typeof(JobInfo).Assembly && type.IsGenericType && type.Name.StartsWith("Job`");
		}

		private void ReadGenericTypeArguments(Type type)
		{
			// This method relies on the generic type parameters for all
			// process base classes being ordered: Config, Item, Dep1, Dep2...DepN

			this.ConfigType = type.GenericTypeArguments[0];

			if (type.GenericTypeArguments.Length > 1)
				this.ItemType = type.GenericTypeArguments[1];

			var types = new List<Type>();

			for (int i = 2; i < type.GenericTypeArguments.Length; i++)
				types.Add(type.GenericTypeArguments[i]);

			this.Dependencies = types.ToArray();
		}
	}
}
