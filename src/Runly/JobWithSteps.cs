using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runly
{
	public abstract class JobWithSteps<TConfig, T1> : Job<TConfig, Step<T1>, T1>
		where TConfig : Config
	{
		StepBuilder stepBuilder;

		public JobWithSteps(TConfig config)
			: base(config) { }

		public override IAsyncEnumerable<Step<T1>> GetItemsAsync() => stepBuilder.steps.ToAsyncEnumerable();

		public override Task<Result> ProcessAsync(Step<T1> item, T1 arg1) => item.Function(arg1);

		public StepBuilder FirstStep(Func<T1, Task<Result>> step) => stepBuilder = new StepBuilder(step);

		public sealed class StepBuilder
		{
			internal readonly List<Step<T1>> steps = new List<Step<T1>>();

			internal StepBuilder(Func<T1, Task<Result>> firstStep, string name = null) => steps.Add(new Step<T1>(firstStep, name));

			public StepBuilder Then(Func<T1, Task<Result>> step, string name = null)
			{
				steps.Add(new Step<T1>(step, name));
				return this;
			}
		}

	}
	public sealed class Step<T1>
	{
		internal Step(Func<T1, Task<Result>> step, string name = null)
		{
			this.Function = step;
			this.Name = name ?? step.GetType().Name;
		}

		public string Name { get; internal set; }
		public Func<T1, Task<Result>> Function { get; internal set; }
	}

	public class JobWithSteps : JobWithSteps<Config, int>
	{
		public JobWithSteps(Config config)
			: base(config)
		{
			FirstStep(Step1)
			.Then(_ => Step2());
		}


		private Task<Result> Step1(int arg1) => Task.FromResult(Result.Success());
		private Task<Result> Step2() => Task.FromResult(Result.Success());
	}
}
