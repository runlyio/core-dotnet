using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runly.Tests
{
	public class JobNoConf : Job
	{
		public JobNoConf() : base(new Config()) { }
		public override Task<Result> ProcessAsync() => Task.FromResult(Result.Success());
	}

	public class JobSingleItem : Job<Config>
	{
		public JobSingleItem() : base(new Config()) { }
		public override Task<Result> ProcessAsync() => Task.FromResult(Result.Success());
	}

	public class Job0 : Job<Config, int>
	{
		public Job0() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item)
		{
			throw new NotImplementedException();
		}
	}

	public class Job1 : Job<Config, int, Dep1>
	{
		public Job1() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1)
		{
			throw new NotImplementedException();
		}
	}

	public class Job2 : Job<Config, int, Dep1, Dep2>
	{
		public Job2() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2)
		{
			throw new NotImplementedException();
		}
	}

	public class Job3 : Job<Config, int, Dep1, Dep2, Dep3>
	{
		public Job3() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3)
		{
			throw new NotImplementedException();
		}
	}

	public class Job4 : Job<Config, int, Dep1, Dep2, Dep3, Dep4>
	{
		public Job4() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4)
		{
			throw new NotImplementedException();
		}
	}

	public class Job5 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5>
	{
		public Job5() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5)
		{
			throw new NotImplementedException();
		}
	}

	public class Job6 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6>
	{
		public Job6() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6)
		{
			throw new NotImplementedException();
		}
	}

	public class Job7 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7>
	{
		public Job7() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7)
		{
			throw new NotImplementedException();
		}
	}

	public class Job8 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7, Dep8>
	{
		public Job8() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7, Dep8 arg8)
		{
			throw new NotImplementedException();
		}
	}

	public class Job9 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7, Dep8, Dep9>
	{
		public Job9() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7, Dep8 arg8, Dep9 arg9)
		{
			throw new NotImplementedException();
		}
	}

	public class Job10 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7, Dep8, Dep9, Dep10>
	{
		public Job10() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7, Dep8 arg8, Dep9 arg9, Dep10 arg10)
		{
			throw new NotImplementedException();
		}
	}

	public class Job11 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7, Dep8, Dep9, Dep10, Dep11>
	{
		public Job11() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7, Dep8 arg8, Dep9 arg9, Dep10 arg10, Dep11 arg11)
		{
			throw new NotImplementedException();
		}
	}

	public class Job12 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7, Dep8, Dep9, Dep10, Dep11, Dep12>
	{
		public Job12() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7, Dep8 arg8, Dep9 arg9, Dep10 arg10, Dep11 arg11, Dep12 arg12)
		{
			throw new NotImplementedException();
		}
	}

	public class Job13 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7, Dep8, Dep9, Dep10, Dep11, Dep12, Dep13>
	{
		public Job13() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7, Dep8 arg8, Dep9 arg9, Dep10 arg10, Dep11 arg11, Dep12 arg12, Dep13 arg13)
		{
			throw new NotImplementedException();
		}
	}

	public class Job14 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7, Dep8, Dep9, Dep10, Dep11, Dep12, Dep13, Dep14>
	{
		public Job14() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7, Dep8 arg8, Dep9 arg9, Dep10 arg10, Dep11 arg11, Dep12 arg12, Dep13 arg13, Dep14 arg14)
		{
			throw new NotImplementedException();
		}
	}

	public class Job15 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7, Dep8, Dep9, Dep10, Dep11, Dep12, Dep13, Dep14, Dep15>
	{
		public Job15() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7, Dep8 arg8, Dep9 arg9, Dep10 arg10, Dep11 arg11, Dep12 arg12, Dep13 arg13, Dep14 arg14, Dep15 arg15)
		{
			throw new NotImplementedException();
		}
	}

	public class Job16 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7, Dep8, Dep9, Dep10, Dep11, Dep12, Dep13, Dep14, Dep15, Dep16>
	{
		public Job16() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7, Dep8 arg8, Dep9 arg9, Dep10 arg10, Dep11 arg11, Dep12 arg12, Dep13 arg13, Dep14 arg14, Dep15 arg15, Dep16 arg16)
		{
			throw new NotImplementedException();
		}
	}
}
