using MessagePack;
using System;
using System.Collections.Generic;

namespace Runly
{
	/// <summary>
	/// The result of a single item being processed by a job.
	/// </summary>
	public class ItemResult
	{
		/// <summary>
		/// Gets the ID of the item.
		/// </summary>
		public string Id { get; private set; }

		/// <summary>
		/// Indicates whether the job successfully processed the item.
		/// </summary>
		public bool IsSuccessful { get; private set; }

		/// <summary>
		/// Indicates whether the item failed because an exception was thrown from ProcessAsync.
		/// </summary>
		[IgnoreMember]
		public bool FailedDueToException => (ProcessAsync?.Exception != null || GetItemIdAsync?.Exception != null || EnumeratorCurrent?.Exception != null || EnumeratorMoveNext?.Exception != null);

		private string category;

		/// <summary>
		/// The category this result is grouped into. If the item <see cref="FailedDueToException"/>, this will be the type name of the exception.
		/// </summary>
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

		/// <summary>
		/// Gets the individual <see cref="MethodOutcome"/> for each <see cref="JobMethod"/> that was called for this item.
		/// </summary>
		public Dictionary<JobMethod, MethodOutcome> Methods { get; } = new Dictionary<JobMethod, MethodOutcome>();

		/// <summary>
		/// Gets the <see cref="MethodOutcome"/> for the call to <see cref="MoveNextAsync"/> on the enumerator.
		/// </summary>
		[IgnoreMember]
		public MethodOutcome EnumeratorMoveNext => Methods.ValueOrDefault(JobMethod.EnumeratorMoveNext);

		/// <summary>
		/// Gets the <see cref="MethodOutcome"/> for the call to <see cref="Current"/> on the enumerator.
		/// </summary>
		[IgnoreMember]
		public MethodOutcome EnumeratorCurrent => Methods.ValueOrDefault(JobMethod.EnumeratorCurrent);

		/// <summary>
		/// Gets the <see cref="MethodOutcome"/> for the call to GetItemIdAsync.
		/// </summary>
		[IgnoreMember]
		public MethodOutcome GetItemIdAsync => Methods.ValueOrDefault(JobMethod.GetItemIdAsync);

		/// <summary>
		/// Gets the <see cref="MethodOutcome"/> for the call to ProcessAsync.
		/// </summary>
		[IgnoreMember]
		public MethodOutcome ProcessAsync => Methods.ValueOrDefault(JobMethod.ProcessAsync);

		internal ItemResult()
		{
			this.Id = "Unknown";
		}

		/// <summary>
		/// Initializes a new <see cref="ItemResult"/>.
		/// </summary>
		public ItemResult(string id, bool isSuccessful, string category)
			: this(id, isSuccessful, category, null, null) { }

		/// <summary>
		/// Initializes a new <see cref="ItemResult"/>.
		/// </summary>
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
