namespace RegionOrebroLan.Transforming.Configuration
{
	public class PackageTransformingOptions : FileTransformingOptions
	{
		#region Properties

		/// <summary>
		/// The actual transforming is done under the %temp%-directory. Removing the temporary transform-directories or not. If set to true the temporary transform-directories will be removed, otherwise not.
		/// </summary>
		public virtual bool Cleanup { get; set; } = true;

		#endregion
	}
}