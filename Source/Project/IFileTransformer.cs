using RegionOrebroLan.Transforming.Configuration;

namespace RegionOrebroLan.Transforming
{
	public interface IFileTransformer
	{
		#region Methods

		void Transform(string destination, string source, string transformation, FileTransformingOptions options = null);

		#endregion
	}
}