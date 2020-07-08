using MessagePack;
using System;
using System.Collections.Generic;

namespace Runly
{
	public class ItemResult
	{
		public string Id { get; private set; }
		public bool IsSuccessful { get; private set; }
		[IgnoreMember]
		public bool FailedDueToException => (ProcessAsync?.Exception != null || GetItemIdAsync?.Exception != null || EnumeratorCurrent?.Exception != null || EnumeratorMoveNext?.Exception != null);

		private string category;
		public string Category
		{
			get
			{
				if (category != null)
					return category;
				else if (ProcessAsync?.Exception != null)
					return ProcessAsync.Exception.GetType().Name;
				else if (GetItemIdAsync?.Exception != null)
					return GetItemIdAsync.Exception.GetType().Name;
				else if (EnumeratorCurrent?.Exception != null)
					return EnumeratorCurrent.Exception.GetType().Name;
				else if (EnumeratorMoveNext?.Exception != null)
					return EnumeratorMoveNext.Exception.GetType().Name;
				else
					return null;
			}
		}

		public object Output { get; private set; }

		public Dictionary<JobMethod, MethodOutcome> Methods { get; } = new Dictionary<JobMethod, MethodOutcome>();

		[IgnoreMember]
		public MethodOutcome EnumeratorMoveNext => Methods.ValueOrDefault(JobMethod.EnumeratorMoveNext);

		[IgnoreMember]
		public MethodOutcome EnumeratorCurrent => Methods.ValueOrDefault(JobMethod.EnumeratorCurrent);

		[IgnoreMember]
		public MethodOutcome GetItemIdAsync => Methods.ValueOrDefault(JobMethod.GetItemIdAsync);

		[IgnoreMember]
		public MethodOutcome ProcessAsync => Methods.ValueOrDefault(JobMethod.ProcessAsync);

		internal ItemResult()
		{
			this.Id = "Unknown";
		}

		public ItemResult(string id, bool isSuccessful, string category)
			: this(id, isSuccessful, category, null, null) { }

		[SerializationConstructor]
		public ItemResult(string id, bool isSuccessful, string category, object output, Dictionary<JobMethod, MethodOutcome> methods)

		{
			Id = id;
			IsSuccessful = isSuccessful;
			this.category = category;
			Output = output;
			Methods = methods ?? Methods;
		}

		internal void Current(TimeSpan duration, Exception exception)
		{
			Methods.Add(JobMethod.EnumeratorCurrent, new MethodOutcome(JobMethod.EnumeratorCurrent, duration, exception));
		}

		internal void MoveNext(TimeSpan duration, Exception exception)
		{
			Methods.Add(JobMethod.EnumeratorMoveNext, new MethodOutcome(JobMethod.EnumeratorMoveNext, duration, exception));
		}

		internal void SetItem(string id, TimeSpan duration, Exception exception)
		{
			Methods.Add(JobMethod.GetItemIdAsync, new MethodOutcome(JobMethod.GetItemIdAsync, duration, exception));
			this.Id = id ?? Id;
		}

		internal void Complete(Result result, TimeSpan duration, Exception exception)
		{
			Methods.Add(JobMethod.ProcessAsync, new MethodOutcome(JobMethod.ProcessAsync, duration, exception));
			this.IsSuccessful = result?.IsSuccessful ?? false;
			this.category = result?.Category;
			this.Output = result?.Output;
		}
	}
}
