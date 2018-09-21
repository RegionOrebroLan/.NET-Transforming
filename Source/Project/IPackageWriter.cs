namespace RegionOrebroLan.Transforming
{
	public interface IPackageWriter
	{
		#region Methods

		void Write(string destination, string source);

		#endregion
	}
}