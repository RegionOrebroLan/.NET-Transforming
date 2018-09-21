namespace RegionOrebroLan.Transforming
{
	public interface IFileTransformerFactory
	{
		#region Methods

		IFileTransformer Create(string source);

		#endregion
	}
}