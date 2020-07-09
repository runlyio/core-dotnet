using System;

namespace Runly
{
	public class TypeNotFoundException : ApplicationException
	{
		public TypeNotFoundException(string typeName)
			: base($"Type {typeName} not found.") { }
	}
}
