using System.Runtime.InteropServices;

namespace RegionOrebroLan.Transforming.Configuration
{
	public class FileTransformingOptions
	{
		#region Properties

		public virtual bool AvoidByteOrderMark { get; set; } = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
		public virtual ReplacementOptions Replacement { get; set; } = new();

		#endregion
	}
}