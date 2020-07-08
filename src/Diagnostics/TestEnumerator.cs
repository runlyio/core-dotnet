using Runly.Processing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Runly.Diagnostics
{
	public class TestEnumerator<T> : IEnumerator<T>
	{
		private MethodResponse moveNext;
		private MethodResponse getCurrent;

		public TestEnumerator(MethodResponse moveNext, MethodResponse getCurrent)
		{
			this.moveNext = moveNext;
			this.getCurrent = getCurrent;
		}

		public T Current => GetCurrent();

		object IEnumerator.Current
		{
			get
			{
				return GetCurrent();
			}
		}

		private T GetCurrent()
		{
			if (getCurrent == MethodResponse.ThrowException)
				throw new TestJobException(JobMethod.EnumeratorCurrent, "Exception thrown because Config.ThrowExceptionInEnumeratorCurrent is true.");
			else
				return default(T);
		}

		public void Dispose() { }

		public bool MoveNext()
		{
			if (moveNext == MethodResponse.ThrowException)
				throw new TestJobException(JobMethod.EnumeratorMoveNext, "Exception thrown because Config.ThrowExceptionInEnumeratorMoveNext is true.");
			else
				return true;
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}
	}
}
