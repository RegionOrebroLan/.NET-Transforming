using System.Runtime.InteropServices;

namespace RegionOrebroLan.Transforming.Configuration
{
	public class ReplacementOptions
	{
		#region Properties

		public virtual bool Enabled { get; set; } = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

		/// <summary>
		/// Replacing new-line character/characters for example.
		/// </summary>
		public virtual Func<string, string> Replace { get; set; } = value => value?.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);

		#endregion
	}
}