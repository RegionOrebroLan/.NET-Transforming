using RegionOrebroLan.Transforming.Configuration;

namespace RegionOrebroLan.Transforming
{
	public interface IPackageTransformer
	{
		#region Methods

		void Transform(string destination, IEnumerable<string> fileToTransformPatterns, IEnumerable<string> pathToDeletePatterns, string source, IEnumerable<string> transformationNames, TransformingOptions options = null);

		#endregion
	}
}