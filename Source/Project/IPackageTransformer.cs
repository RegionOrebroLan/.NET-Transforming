namespace RegionOrebroLan.Transforming
{
	public interface IPackageTransformer
	{
		#region Methods

		void Transform(bool cleanup, string destination, IEnumerable<string> fileToTransformPatterns, IEnumerable<string> pathToDeletePatterns, string source, IEnumerable<string> transformationNames);

		#endregion
	}
}