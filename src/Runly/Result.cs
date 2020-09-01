using System.Threading;

namespace Runly
{
	/// <summary>
	/// The value returned from ProcessAsync to indicate the result of processing an item.
	/// </summary>
	public class Result
	{
		/// <summary>
		/// A successful result in the "Successful" category, except when cancellation has been requested, changing the category to "Cancelled".
		/// </summary>
		public static Result SuccessOrCancelled(CancellationToken cancellation) => cancellation.IsCancellationRequested ? new Result(Cancelled) : Success();

		/// <summary>
		/// A successful result.
		/// </summary>
		public static Result Success() => new Result();

		/// <summary>
		/// A successful result in a user-defined category.
		/// </summary>
		public static Result Success(string category) => new Result(category);

		/// <summary>
		/// A successful result with output.
		/// </summary>
		public static Result Success(object output) => new Result(output);

		/// <summary>
		/// A successful result in a user-defined category with output.
		/// </summary>
		public static Result Success(string category, object output) => new Result(category, output);

		/// <summary>
		/// An unsuccessful result.
		/// </summary>
		public static Result Failure() => new Result(Failed, false);

		/// <summary>
		/// An unsuccessful result in a user-defined category.
		/// </summary>
		public static Result Failure(string category) => new Result(category, false);

		/// <summary>
		/// An unsuccessful result with output.
		/// </summary>
		public static Result Failure(object output) => new Result(Failed, output, false);

		/// <summary>
		/// An unsuccessful result in a user-defined category with output.
		/// </summary>
		public static Result Failure(string category, object output) => new Result(category, output, false);

		/// <summary>
		/// The default category for successful results.
		/// </summary>
		public const string Successful = "Successful";

		/// <summary>
		/// The default category for failed results.
		/// </summary>
		public const string Failed = "Failed";

		/// <summary>
		/// The default category for cancelled results.
		/// </summary>
		public const string Cancelled = "Cancelled";

		/// <summary>
		/// True when processing was completed successfully.
		/// </summary>
		public bool IsSuccessful { get; private set; }

		/// <summary>
		/// The category to group this result into.
		/// </summary>
		public string Category { get; private set; }

		/// <summary>
		/// Output from processing the item.
		/// </summary>
		public object Output { get; private set; }

		/// <summary>
		/// Creates a successful result.
		/// </summary>
		private Result()
			: this(Successful, null, true) { }

		/// <summary>
		/// Creates a successful result in the <paramref name="category"/> specified.
		/// </summary>
		/// <param name="category">The category to group this result into.</param>
		private Result(string category)
			: this(category, null, true) { }

		/// <summary>
		/// Creates a result in the <paramref name="category"/> specified.
		/// </summary>
		/// <param name="category">The category to group this result into.</param>
		/// <param name="isSuccessful">Whether or not this is a successful result.</param>
		private Result(string category, bool isSuccessful)
			: this(category, null, isSuccessful) { }

		/// <summary>
		/// Creates a successful result with <paramref name="output"/>.
		/// </summary>
		/// <param name="output">Output from processing the item.</param>
		private Result(object output)
			: this(Successful, output, true) { }

		/// <summary>
		/// Creates a successful result in the <paramref name="category"/> specified with <paramref name="output"/>.
		/// </summary>
		/// <param name="category">The category to group this result into.</param>
		/// <param name="output">Output from processing the item.</param>
		private Result(string category, object output)
			: this(category, output, true) { }

		/// <summary>
		/// Creates a result in the <paramref name="category"/> specified with <paramref name="output"/>.
		/// </summary>
		/// <param name="category">The category to group this result into.</param>
		/// <param name="output">Output from processing the item.</param>
		/// <param name="isSuccessful">Whether or not this is a successful result.</param>
		private Result(string category, object output, bool isSuccessful)
		{
			this.Category = !string.IsNullOrWhiteSpace(category) ? category.Trim() : isSuccessful ? Successful : Failed;
			this.Output = output;
			this.IsSuccessful = isSuccessful;
		}
	}
}
