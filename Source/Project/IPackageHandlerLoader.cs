namespace RegionOrebroLan.Transforming
{
	public interface IPackageHandlerLoader
	{
		#region Methods

		IPackageExtractor GetExtractor(string source);
		IPackageWriter GetWriter(string destination);

		#endregion
	}
}