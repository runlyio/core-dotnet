using System;

namespace Runly
{
	[Flags]
	public enum JobLoadErrors
	{
		None = 0,
		IsInterface = 1,
		IsAbstract = 2,
		IsGenericTypeDefinition = 4
	}
}
