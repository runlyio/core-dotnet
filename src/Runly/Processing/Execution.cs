using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Runly.Processing
{
	public class Execution<TConfig> : ExecutionBase<TConfig, string>
		where TConfig : Config
	{
		readonly Job<TConfig> job;

		public override IJob Job => job;

		public Execution(Job<TConfig> job, IServiceProvider provider)
			: base(new SingleItemSource(), provider)
		{
			this.job = job ?? throw new ArgumentNullException(nameof(job));
		}

		protected async override Task<Result> CallProcess(IServiceScope scope, string item)
		{
			return await job.ProcessAsync();
		}
	}

	public class Execution<TConfig, TItem> : ExecutionBase<TConfig, TItem>
		where TConfig : Config
	{
		readonly Job<TConfig, TItem> job;

		public override IJob Job => job;

		public Execution(Job<TConfig, TItem> job, IItemSource<TItem> source, IServiceProvider provider)
			: base(source, provider)
		{
			this.job = job ?? throw new ArgumentNullException(nameof(job));
		}

		protected async override Task<Result> CallProcess(IServiceScope scope, TItem item)
		{
			return await job.ProcessAsync(item);
		}
	}

	public class Execution<TConfig, TItem, T1> : ExecutionBase<TConfig, TItem>
		where TConfig : Config
	{
		readonly Job<TConfig, TItem, T1> job;

		public override IJob Job => job;

		public Execution(Job<TConfig, TItem, T1> job, IItemSource<TItem> source, IServiceProvider provider)
			: base(source, provider)
		{
			this.job = job ?? throw new ArgumentNullException(nameof(job));
		}

		protected async override Task<Result> CallProcess(IServiceScope scope, TItem item)
		{
			var arg1 = scope.ServiceProvider.GetRequiredService<T1>();

			return await job.ProcessAsync(item, arg1);
		}
	}

	public class Execution<TConfig, TItem, T1, T2> : ExecutionBase<TConfig, TItem>
		where TConfig : Config
	{
		readonly Job<TConfig, TItem, T1, T2> job;

		public override IJob Job => job;

		public Execution(Job<TConfig, TItem, T1, T2> job, IItemSource<TItem> source, IServiceProvider provider)
			: base(source, provider)
		{
			this.job = job ?? throw new ArgumentNullException(nameof(job));
		}

		protected async override Task<Result> CallProcess(IServiceScope scope, TItem item)
		{
			var arg1 = scope.ServiceProvider.GetRequiredService<T1>();
			var arg2 = scope.ServiceProvider.GetRequiredService<T2>();

			return await job.ProcessAsync(item, arg1, arg2);
		}
	}

	public class Execution<TConfig, TItem, T1, T2, T3> : ExecutionBase<TConfig, TItem>
		where TConfig : Config
	{
		readonly Job<TConfig, TItem, T1, T2, T3> job;

		public override IJob Job => job;

		public Execution(Job<TConfig, TItem, T1, T2, T3> job, IItemSource<TItem> source, IServiceProvider provider)
			: base(source, provider)
		{
			this.job = job ?? throw new ArgumentNullException(nameof(job));
		}

		protected async override Task<Result> CallProcess(IServiceScope scope, TItem item)
		{
			var arg1 = scope.ServiceProvider.GetRequiredService<T1>();
			var arg2 = scope.ServiceProvider.GetRequiredService<T2>();
			var arg3 = scope.ServiceProvider.GetRequiredService<T3>();

			return await job.ProcessAsync(item, arg1, arg2, arg3);
		}
	}

	public class Execution<TConfig, TItem, T1, T2, T3, T4> : ExecutionBase<TConfig, TItem>
		where TConfig : Config
	{
		readonly Job<TConfig, TItem, T1, T2, T3, T4> job;

		public override IJob Job => job;

		public Execution(Job<TConfig, TItem, T1, T2, T3, T4> job, IItemSource<TItem> source, IServiceProvider provider)
			: base(source, provider)
		{
			this.job = job ?? throw new ArgumentNullException(nameof(job));
		}

		protected async override Task<Result> CallProcess(IServiceScope scope, TItem item)
		{
			var arg1 = scope.ServiceProvider.GetRequiredService<T1>();
			var arg2 = scope.ServiceProvider.GetRequiredService<T2>();
			var arg3 = scope.ServiceProvider.GetRequiredService<T3>();
			var arg4 = scope.ServiceProvider.GetRequiredService<T4>();

			return await job.ProcessAsync(item, arg1, arg2, arg3, arg4);
		}
	}

	public class Execution<TConfig, TItem, T1, T2, T3, T4, T5> : ExecutionBase<TConfig, TItem>
		where TConfig : Config
	{
		readonly Job<TConfig, TItem, T1, T2, T3, T4, T5> job;

		public override IJob Job => job;

		public Execution(Job<TConfig, TItem, T1, T2, T3, T4, T5> job, IItemSource<TItem> source, IServiceProvider provider)
			: base(source, provider)
		{
			this.job = job ?? throw new ArgumentNullException(nameof(job));
		}

		protected async override Task<Result> CallProcess(IServiceScope scope, TItem item)
		{
			var arg1 = scope.ServiceProvider.GetRequiredService<T1>();
			var arg2 = scope.ServiceProvider.GetRequiredService<T2>();
			var arg3 = scope.ServiceProvider.GetRequiredService<T3>();
			var arg4 = scope.ServiceProvider.GetRequiredService<T4>();
			var arg5 = scope.ServiceProvider.GetRequiredService<T5>();

			return await job.ProcessAsync(item, arg1, arg2, arg3, arg4, arg5);
		}
	}

	public class Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6> : ExecutionBase<TConfig, TItem>
		where TConfig : Config
	{
		readonly Job<TConfig, TItem, T1, T2, T3, T4, T5, T6> job;

		public override IJob Job => job;

		public Execution(Job<TConfig, TItem, T1, T2, T3, T4, T5, T6> job, IItemSource<TItem> source, IServiceProvider provider)
			: base(source, provider)
		{
			this.job = job ?? throw new ArgumentNullException(nameof(job));
		}

		protected async override Task<Result> CallProcess(IServiceScope scope, TItem item)
		{
			var arg1 = scope.ServiceProvider.GetRequiredService<T1>();
			var arg2 = scope.ServiceProvider.GetRequiredService<T2>();
			var arg3 = scope.ServiceProvider.GetRequiredService<T3>();
			var arg4 = scope.ServiceProvider.GetRequiredService<T4>();
			var arg5 = scope.ServiceProvider.GetRequiredService<T5>();
			var arg6 = scope.ServiceProvider.GetRequiredService<T6>();

			return await job.ProcessAsync(item, arg1, arg2, arg3, arg4, arg5, arg6);
		}
	}

	public class Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7> : ExecutionBase<TConfig, TItem>
		where TConfig : Config
	{
		readonly Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7> job;

		public override IJob Job => job;

		public Execution(Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7> job, IItemSource<TItem> source, IServiceProvider provider)
			: base(source, provider)
		{
			this.job = job ?? throw new ArgumentNullException(nameof(job));
		}

		protected async override Task<Result> CallProcess(IServiceScope scope, TItem item)
		{
			var arg1 = scope.ServiceProvider.GetRequiredService<T1>();
			var arg2 = scope.ServiceProvider.GetRequiredService<T2>();
			var arg3 = scope.ServiceProvider.GetRequiredService<T3>();
			var arg4 = scope.ServiceProvider.GetRequiredService<T4>();
			var arg5 = scope.ServiceProvider.GetRequiredService<T5>();
			var arg6 = scope.ServiceProvider.GetRequiredService<T6>();
			var arg7 = scope.ServiceProvider.GetRequiredService<T7>();

			return await job.ProcessAsync(item, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		}
	}

	public class Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8> : ExecutionBase<TConfig, TItem>
		where TConfig : Config
	{
		readonly Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8> job;

		public override IJob Job => job;

		public Execution(Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8> job, IItemSource<TItem> source, IServiceProvider provider)
			: base(source, provider)
		{
			this.job = job ?? throw new ArgumentNullException(nameof(job));
		}

		protected async override Task<Result> CallProcess(IServiceScope scope, TItem item)
		{
			var arg1 = scope.ServiceProvider.GetRequiredService<T1>();
			var arg2 = scope.ServiceProvider.GetRequiredService<T2>();
			var arg3 = scope.ServiceProvider.GetRequiredService<T3>();
			var arg4 = scope.ServiceProvider.GetRequiredService<T4>();
			var arg5 = scope.ServiceProvider.GetRequiredService<T5>();
			var arg6 = scope.ServiceProvider.GetRequiredService<T6>();
			var arg7 = scope.ServiceProvider.GetRequiredService<T7>();
			var arg8 = scope.ServiceProvider.GetRequiredService<T8>();

			return await job.ProcessAsync(item, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
		}
	}

	public class Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9> : ExecutionBase<TConfig, TItem>
		where TConfig : Config
	{
		readonly Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9> job;

		public override IJob Job => job;

		public Execution(Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9> job, IItemSource<TItem> source, IServiceProvider provider)
			: base(source, provider)
		{
			this.job = job ?? throw new ArgumentNullException(nameof(job));
		}

		protected async override Task<Result> CallProcess(IServiceScope scope, TItem item)
		{
			var arg1 = scope.ServiceProvider.GetRequiredService<T1>();
			var arg2 = scope.ServiceProvider.GetRequiredService<T2>();
			var arg3 = scope.ServiceProvider.GetRequiredService<T3>();
			var arg4 = scope.ServiceProvider.GetRequiredService<T4>();
			var arg5 = scope.ServiceProvider.GetRequiredService<T5>();
			var arg6 = scope.ServiceProvider.GetRequiredService<T6>();
			var arg7 = scope.ServiceProvider.GetRequiredService<T7>();
			var arg8 = scope.ServiceProvider.GetRequiredService<T8>();
			var arg9 = scope.ServiceProvider.GetRequiredService<T9>();

			return await job.ProcessAsync(item, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
		}
	}

	public class Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : ExecutionBase<TConfig, TItem>
		where TConfig : Config
	{
		readonly Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> job;

		public override IJob Job => job;

		public Execution(Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> job, IItemSource<TItem> source, IServiceProvider provider)
			: base(source, provider)
		{
			this.job = job ?? throw new ArgumentNullException(nameof(job));
		}

		protected async override Task<Result> CallProcess(IServiceScope scope, TItem item)
		{
			var arg1 = scope.ServiceProvider.GetRequiredService<T1>();
			var arg2 = scope.ServiceProvider.GetRequiredService<T2>();
			var arg3 = scope.ServiceProvider.GetRequiredService<T3>();
			var arg4 = scope.ServiceProvider.GetRequiredService<T4>();
			var arg5 = scope.ServiceProvider.GetRequiredService<T5>();
			var arg6 = scope.ServiceProvider.GetRequiredService<T6>();
			var arg7 = scope.ServiceProvider.GetRequiredService<T7>();
			var arg8 = scope.ServiceProvider.GetRequiredService<T8>();
			var arg9 = scope.ServiceProvider.GetRequiredService<T9>();
			var arg10 = scope.ServiceProvider.GetRequiredService<T10>();

			return await job.ProcessAsync(item, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
		}
	}

	public class Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : ExecutionBase<TConfig, TItem>
		where TConfig : Config
	{
		readonly Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> job;

		public override IJob Job => job;

		public Execution(Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> job, IItemSource<TItem> source, IServiceProvider provider)
			: base(source, provider)
		{
			this.job = job ?? throw new ArgumentNullException(nameof(job));
		}

		protected async override Task<Result> CallProcess(IServiceScope scope, TItem item)
		{
			var arg1 = scope.ServiceProvider.GetRequiredService<T1>();
			var arg2 = scope.ServiceProvider.GetRequiredService<T2>();
			var arg3 = scope.ServiceProvider.GetRequiredService<T3>();
			var arg4 = scope.ServiceProvider.GetRequiredService<T4>();
			var arg5 = scope.ServiceProvider.GetRequiredService<T5>();
			var arg6 = scope.ServiceProvider.GetRequiredService<T6>();
			var arg7 = scope.ServiceProvider.GetRequiredService<T7>();
			var arg8 = scope.ServiceProvider.GetRequiredService<T8>();
			var arg9 = scope.ServiceProvider.GetRequiredService<T9>();
			var arg10 = scope.ServiceProvider.GetRequiredService<T10>();
			var arg11 = scope.ServiceProvider.GetRequiredService<T11>();

			return await job.ProcessAsync(item, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
		}
	}

	public class Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : ExecutionBase<TConfig, TItem>
		where TConfig : Config
	{
		readonly Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> job;

		public override IJob Job => job;

		public Execution(Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> job, IItemSource<TItem> source, IServiceProvider provider)
			: base(source, provider)
		{
			this.job = job ?? throw new ArgumentNullException(nameof(job));
		}

		protected async override Task<Result> CallProcess(IServiceScope scope, TItem item)
		{
			var arg1 = scope.ServiceProvider.GetRequiredService<T1>();
			var arg2 = scope.ServiceProvider.GetRequiredService<T2>();
			var arg3 = scope.ServiceProvider.GetRequiredService<T3>();
			var arg4 = scope.ServiceProvider.GetRequiredService<T4>();
			var arg5 = scope.ServiceProvider.GetRequiredService<T5>();
			var arg6 = scope.ServiceProvider.GetRequiredService<T6>();
			var arg7 = scope.ServiceProvider.GetRequiredService<T7>();
			var arg8 = scope.ServiceProvider.GetRequiredService<T8>();
			var arg9 = scope.ServiceProvider.GetRequiredService<T9>();
			var arg10 = scope.ServiceProvider.GetRequiredService<T10>();
			var arg11 = scope.ServiceProvider.GetRequiredService<T11>();
			var arg12 = scope.ServiceProvider.GetRequiredService<T12>();

			return await job.ProcessAsync(item, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
		}
	}

	public class Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : ExecutionBase<TConfig, TItem>
		where TConfig : Config
	{
		readonly Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> job;

		public override IJob Job => job;

		public Execution(Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> job, IItemSource<TItem> source, IServiceProvider provider)
			: base(source, provider)
		{
			this.job = job ?? throw new ArgumentNullException(nameof(job));
		}

		protected async override Task<Result> CallProcess(IServiceScope scope, TItem item)
		{
			var arg1 = scope.ServiceProvider.GetRequiredService<T1>();
			var arg2 = scope.ServiceProvider.GetRequiredService<T2>();
			var arg3 = scope.ServiceProvider.GetRequiredService<T3>();
			var arg4 = scope.ServiceProvider.GetRequiredService<T4>();
			var arg5 = scope.ServiceProvider.GetRequiredService<T5>();
			var arg6 = scope.ServiceProvider.GetRequiredService<T6>();
			var arg7 = scope.ServiceProvider.GetRequiredService<T7>();
			var arg8 = scope.ServiceProvider.GetRequiredService<T8>();
			var arg9 = scope.ServiceProvider.GetRequiredService<T9>();
			var arg10 = scope.ServiceProvider.GetRequiredService<T10>();
			var arg11 = scope.ServiceProvider.GetRequiredService<T11>();
			var arg12 = scope.ServiceProvider.GetRequiredService<T12>();
			var arg13 = scope.ServiceProvider.GetRequiredService<T13>();

			return await job.ProcessAsync(item, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
		}
	}

	public class Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : ExecutionBase<TConfig, TItem>
		where TConfig : Config
	{
		readonly Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> job;

		public override IJob Job => job;

		public Execution(Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> job, IItemSource<TItem> source, IServiceProvider provider)
			: base(source, provider)
		{
			this.job = job ?? throw new ArgumentNullException(nameof(job));
		}

		protected async override Task<Result> CallProcess(IServiceScope scope, TItem item)
		{
			var arg1 = scope.ServiceProvider.GetRequiredService<T1>();
			var arg2 = scope.ServiceProvider.GetRequiredService<T2>();
			var arg3 = scope.ServiceProvider.GetRequiredService<T3>();
			var arg4 = scope.ServiceProvider.GetRequiredService<T4>();
			var arg5 = scope.ServiceProvider.GetRequiredService<T5>();
			var arg6 = scope.ServiceProvider.GetRequiredService<T6>();
			var arg7 = scope.ServiceProvider.GetRequiredService<T7>();
			var arg8 = scope.ServiceProvider.GetRequiredService<T8>();
			var arg9 = scope.ServiceProvider.GetRequiredService<T9>();
			var arg10 = scope.ServiceProvider.GetRequiredService<T10>();
			var arg11 = scope.ServiceProvider.GetRequiredService<T11>();
			var arg12 = scope.ServiceProvider.GetRequiredService<T12>();
			var arg13 = scope.ServiceProvider.GetRequiredService<T13>();
			var arg14 = scope.ServiceProvider.GetRequiredService<T14>();

			return await job.ProcessAsync(item, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
		}
	}

	public class Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : ExecutionBase<TConfig, TItem>
		where TConfig : Config
	{
		readonly Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> job;

		public override IJob Job => job;

		public Execution(Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> job, IItemSource<TItem> source, IServiceProvider provider)
			: base(source, provider)
		{
			this.job = job ?? throw new ArgumentNullException(nameof(job));
		}

		protected async override Task<Result> CallProcess(IServiceScope scope, TItem item)
		{
			var arg1 = scope.ServiceProvider.GetRequiredService<T1>();
			var arg2 = scope.ServiceProvider.GetRequiredService<T2>();
			var arg3 = scope.ServiceProvider.GetRequiredService<T3>();
			var arg4 = scope.ServiceProvider.GetRequiredService<T4>();
			var arg5 = scope.ServiceProvider.GetRequiredService<T5>();
			var arg6 = scope.ServiceProvider.GetRequiredService<T6>();
			var arg7 = scope.ServiceProvider.GetRequiredService<T7>();
			var arg8 = scope.ServiceProvider.GetRequiredService<T8>();
			var arg9 = scope.ServiceProvider.GetRequiredService<T9>();
			var arg10 = scope.ServiceProvider.GetRequiredService<T10>();
			var arg11 = scope.ServiceProvider.GetRequiredService<T11>();
			var arg12 = scope.ServiceProvider.GetRequiredService<T12>();
			var arg13 = scope.ServiceProvider.GetRequiredService<T13>();
			var arg14 = scope.ServiceProvider.GetRequiredService<T14>();
			var arg15 = scope.ServiceProvider.GetRequiredService<T15>();

			return await job.ProcessAsync(item, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
		}
	}

	public class Execution<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : ExecutionBase<TConfig, TItem>
		where TConfig : Config
	{
		readonly Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> job;

		public override IJob Job => job;

		public Execution(Job<TConfig, TItem, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> job, IItemSource<TItem> source, IServiceProvider provider)
			: base(source, provider)
		{
			this.job = job ?? throw new ArgumentNullException(nameof(job));
		}

		protected async override Task<Result> CallProcess(IServiceScope scope, TItem item)
		{
			var arg1 = scope.ServiceProvider.GetRequiredService<T1>();
			var arg2 = scope.ServiceProvider.GetRequiredService<T2>();
			var arg3 = scope.ServiceProvider.GetRequiredService<T3>();
			var arg4 = scope.ServiceProvider.GetRequiredService<T4>();
			var arg5 = scope.ServiceProvider.GetRequiredService<T5>();
			var arg6 = scope.ServiceProvider.GetRequiredService<T6>();
			var arg7 = scope.ServiceProvider.GetRequiredService<T7>();
			var arg8 = scope.ServiceProvider.GetRequiredService<T8>();
			var arg9 = scope.ServiceProvider.GetRequiredService<T9>();
			var arg10 = scope.ServiceProvider.GetRequiredService<T10>();
			var arg11 = scope.ServiceProvider.GetRequiredService<T11>();
			var arg12 = scope.ServiceProvider.GetRequiredService<T12>();
			var arg13 = scope.ServiceProvider.GetRequiredService<T13>();
			var arg14 = scope.ServiceProvider.GetRequiredService<T14>();
			var arg15 = scope.ServiceProvider.GetRequiredService<T15>();
			var arg16 = scope.ServiceProvider.GetRequiredService<T16>();

			return await job.ProcessAsync(item, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
		}
	}
}
