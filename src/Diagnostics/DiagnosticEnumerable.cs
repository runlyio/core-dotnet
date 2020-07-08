using Runly.Processing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Runly.Diagnostics
{
	public class DiagnosticEnumerable<T> : IEnumerable<T>
	{
		private MethodResponse getEnumerator;
		private MethodResponse moveNext;
		private MethodResponse getCurrent;

		public IEnumerator<T> GetEnumerator()
		{
			if (getEnumerator == MethodResponse.ThrowException)
				throw new DiagnosticJobException(JobMethod.GetEnumerator, "Exception thrown because Config.ThrowExceptionInGetEnumerator is true.");
			else if (getEnumerator == MethodResponse.Null)
				return null;
			else
				return new DiagnosticEnumerator<T>(moveNext, getCurrent);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Throws an exception or returns null in GetEnumerator, depending on whether throwException is true.
		/// </summary>
		public DiagnosticEnumerable(MethodResponse getEnumerator, MethodResponse moveNext, MethodResponse getCurrent)
		{
			this.getEnumerator = getEnumerator;
			this.moveNext = moveNext;
			this.getCurrent = getCurrent;
		}
	}
}
