using System.IO;
using System.Text;

namespace Runly.Internal
{
	static class TextWriterExtensions
	{
		public static void WriteJson(this TextWriter writer, object toSerialize)
		{
			ConfigWriter.Serializer.Serialize(writer,toSerialize);
		}
	}

	class VoidTextWriter : TextWriter
	{
		public override Encoding Encoding => Encoding.UTF8;
		public override void Write(char value) { }
	}
}
