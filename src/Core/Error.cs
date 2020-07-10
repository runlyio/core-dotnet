using System;
using Newtonsoft.Json;

namespace Runly
{
	/// <summary>
	/// A serializable representation an <see cref="Exception"/>. 
	/// </summary>
	public class Error
	{
		/// <summary>
		/// Gets a message describing the exception.
		/// </summary>
		public string Message { get; }

		/// <summary>
		/// Gets a detailed description of the exception.
		/// </summary>
		public string Trace { get; }

		/// <summary>
		/// Initializes a new <see cref="Error"/>.
		/// </summary>
		[JsonConstructor]
		public Error(string message, string trace)
		{
			Message = message;
			Trace = trace;
		}

		/// <summary>
		/// Initializes a new <see cref="Error"/> from an <see cref="Exception"/>.
		/// </summary>
		/// <param name="ex"></param>
		public Error(Exception ex)
		{
			if (ex == null)
				throw new ArgumentNullException(nameof(ex));

			Trace = ex.ToDetailedString();

			while (ex.InnerException != null)
				ex = ex.InnerException;

			Message = ex.Message;
		}

		/// <summary>
		/// Returns the <see cref="Message"/>, which describes the <see cref="Error"/>.
		/// </summary>
		public override string ToString() => Message;
	}
}
