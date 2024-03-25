namespace RegionOrebroLan.Transforming.Configuration
{
	public class TransformingOptions
	{
		#region Properties

		public virtual FileTransformingOptions File { get; set; } = new();
		public virtual PackageTransformingOptions Package { get; set; } = new();

		#endregion
	}
}