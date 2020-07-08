using System;
using System.Collections.Generic;
using System.Text;

namespace Runly.Diagnostics
{
	public class TestConfig : Config
	{
		public int NumberOfItems { get; set; }
		/// <summary>
		/// When supplied, overrides NumberOfItems and produces a list of items according to the categories and counts provided.
		/// </summary>
		public Category[] Categories { get; set; }
		public int MillisecondDelayPerItem { get; set; }
		public bool CanCountItems { get; set; }
		public bool WaitForSignalInInitializeAsync { get; set; }
		public bool WaitForSignalInProcessAsync { get; set; }
		public bool WaitForSignalInFinalizeAsync { get; set; }
		public bool ThrowExceptionInInitializeAsync { get; set; }
		public bool ThrowExceptionInGetItemsAsync { get; set; }
		public bool ThrowExceptionInGetEnumerator { get; set; }
		public bool ThrowExceptionInEnumeratorMoveNext { get; set; }
		public bool ThrowExceptionInEnumeratorCurrent { get; set; }
		public bool ThrowExceptionInGetItemIdAsync { get; set; }
		public bool ThrowExceptionInProcessAsync { get; set; }
		public bool ThrowExceptionInFinalizeAsync { get; set; }
		public string MessageToLogInProcessAsync { get; set; }

		public TestConfig()
		{
			NumberOfItems = 100;
			MillisecondDelayPerItem = 100;
			CanCountItems = true;
		}

		public class Category
		{
			public string Name { get; set; }
			public bool IsSuccessful { get; set; }
			public int Count { get; set; }
		}
	}
}
