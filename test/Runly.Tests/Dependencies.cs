using System;
using System.Threading.Tasks;

namespace Runly.Tests
{
	public interface IDep1 { }
    public class Dep1 : IDep1, IAsyncDisposable
    {
        public static bool IsDisposed { get; set; }

        public Dep1()
        {
			IsDisposed = false;
        }

		public ValueTask DisposeAsync()
		{
			IsDisposed = true;
			return ValueTask.CompletedTask;
		}
    }
    public interface IDep2 { }
	public class Dep2 : IDep2 { }
	public class Dep3 { }
	public class Dep4 { }
	public class Dep5 { }
	public class Dep6 { }
	public class Dep7 { }
	public class Dep8 { }
	public class Dep9 { }
	public class Dep10 { }
	public class Dep11 { }
	public class Dep12 { }
	public class Dep13 { }
	public class Dep14 { }
	public class Dep15 { }
	public class Dep16 { }
	public class NotADependency { }
}
