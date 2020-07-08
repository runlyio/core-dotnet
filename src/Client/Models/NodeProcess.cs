namespace Runly.Models
{
	public class NodeProcess
	{
		public string Type { get; set; }
		public string ConfigType { get; set; }
		public object DefaultConfig { get; set; }
		public string Application { get; set; }
		public string Package { get; set; }
		public string Version { get; set; }
	}
}
