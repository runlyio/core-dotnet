using System;

namespace Runly.Hosting
{
	/// <summary>
	/// An exception thrown when the job type cannot be resolved.
	/// </summary>
	public class TypeNotFoundException : Exception
	{
		/// <summary>
		/// Initializes a new <see cref="TypeNotFoundException"/>.
		/// </summary>
		/// <param name="typeName"></param>
		public TypeNotFoundException(string typeName)
			: base($"Type {typeName} not found.") { }
	}
}
