using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runly.Tests.Scenarios.ServiceCollectionConfiguration
{
	public class AsyncProcessNoConf : Job
	{
		public AsyncProcessNoConf() : base(new Config()) { }
		public override Task<Result> ProcessAsync() => Task.FromResult(Result.Success());
	}

	public class AsyncProcessConfOnly : Job<Config>
	{
		public AsyncProcessConfOnly() : base(new Config()) { }
		public override Task<Result> ProcessAsync() => Task.FromResult(Result.Success());
	}

	public class AsyncProcess0 : Job<Config, int>
	{
		public AsyncProcess0() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item)
		{
			throw new NotImplementedException();
		}
	}

	public class AsyncProcess1 : Job<Config, int, Dep1>
	{
		public AsyncProcess1() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1)
		{
			throw new NotImplementedException();
		}
	}

	public class AsyncProcess2 : Job<Config, int, Dep1, Dep2>
	{
		public AsyncProcess2() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2)
		{
			throw new NotImplementedException();
		}
	}

	public class AsyncProcess3 : Job<Config, int, Dep1, Dep2, Dep3>
	{
		public AsyncProcess3() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3)
		{
			throw new NotImplementedException();
		}
	}

	public class AsyncProcess4 : Job<Config, int, Dep1, Dep2, Dep3, Dep4>
	{
		public AsyncProcess4() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4)
		{
			throw new NotImplementedException();
		}
	}

	public class AsyncProcess5 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5>
	{
		public AsyncProcess5() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5)
		{
			throw new NotImplementedException();
		}
	}

	public class AsyncProcess6 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6>
	{
		public AsyncProcess6() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6)
		{
			throw new NotImplementedException();
		}
	}

	public class AsyncProcess7 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7>
	{
		public AsyncProcess7() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7)
		{
			throw new NotImplementedException();
		}
	}

	public class AsyncProcess8 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7, Dep8>
	{
		public AsyncProcess8() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7, Dep8 arg8)
		{
			throw new NotImplementedException();
		}
	}

	public class AsyncProcess9 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7, Dep8, Dep9>
	{
		public AsyncProcess9() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7, Dep8 arg8, Dep9 arg9)
		{
			throw new NotImplementedException();
		}
	}

	public class AsyncProcess10 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7, Dep8, Dep9, Dep10>
	{
		public AsyncProcess10() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7, Dep8 arg8, Dep9 arg9, Dep10 arg10)
		{
			throw new NotImplementedException();
		}
	}

	public class AsyncProcess11 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7, Dep8, Dep9, Dep10, Dep11>
	{
		public AsyncProcess11() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7, Dep8 arg8, Dep9 arg9, Dep10 arg10, Dep11 arg11)
		{
			throw new NotImplementedException();
		}
	}

	public class AsyncProcess12 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7, Dep8, Dep9, Dep10, Dep11, Dep12>
	{
		public AsyncProcess12() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7, Dep8 arg8, Dep9 arg9, Dep10 arg10, Dep11 arg11, Dep12 arg12)
		{
			throw new NotImplementedException();
		}
	}

	public class AsyncProcess13 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7, Dep8, Dep9, Dep10, Dep11, Dep12, Dep13>
	{
		public AsyncProcess13() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7, Dep8 arg8, Dep9 arg9, Dep10 arg10, Dep11 arg11, Dep12 arg12, Dep13 arg13)
		{
			throw new NotImplementedException();
		}
	}

	public class AsyncProcess14 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7, Dep8, Dep9, Dep10, Dep11, Dep12, Dep13, Dep14>
	{
		public AsyncProcess14() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7, Dep8 arg8, Dep9 arg9, Dep10 arg10, Dep11 arg11, Dep12 arg12, Dep13 arg13, Dep14 arg14)
		{
			throw new NotImplementedException();
		}
	}

	public class AsyncProcess15 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7, Dep8, Dep9, Dep10, Dep11, Dep12, Dep13, Dep14, Dep15>
	{
		public AsyncProcess15() : base(new Config()) { }

		public override IAsyncEnumerable<int> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync(int item, Dep1 arg1, Dep2 arg2, Dep3 arg3, Dep4 arg4, Dep5 arg5, Dep6 arg6, Dep7 arg7, Dep8 arg8, Dep9 arg9, Dep10 arg10, Dep11 arg11, Dep12 arg12, Dep13 arg13, Dep14 arg14, Dep15 arg15)
		{
			throw new NotImplementedException();
		}
	}

	public class AsyncProcess16 : Job<Config, int, Dep1, Dep2, Dep3, Dep4, Dep5, Dep6, Dep7, Dep8, Dep9, Dep10, Dep11, Dep12, Dep13, Dep14, Dep15, Dep16>
	{
		public AsyncProcess16() : base(new Config()) { }

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
