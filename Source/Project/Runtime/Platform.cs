using System.Runtime.InteropServices;

namespace RegionOrebroLan.Transforming.Runtime
{
	public class Platform : IPlatform
	{
		#region Properties

		public virtual bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

		#endregion
	}
}