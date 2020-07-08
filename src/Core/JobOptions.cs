namespace Runly
{
	public class JobOptions
	{
		/// <summary>
		/// When true, allows the execution of ProcessAsync on multiple concurrent threads. When false each item will be processed
		/// serially on one thread in the order returned from the enumerator.
		/// </summary>
		/// <remarks>When set to false, <see cref="ExecutionConfig.ParallelTaskCount"/> is ignored.</remarks>
		/// <value>True by default</value>
		public bool CanProcessInParallel { get; set; } = true;
	}
}
