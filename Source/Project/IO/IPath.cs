namespace RegionOrebroLan.Transforming.IO
{
	public interface IPath
	{
		#region Properties

		char AltDirectorySeparatorChar { get; }
		char DirectorySeparatorChar { get; }

		#endregion

		#region Methods

		string Combine(params string[] paths);
		string GetDirectoryName(string path);
		string GetExtension(string path);
		string GetTempPath();
		bool IsPathRooted(string path);

		#endregion
	}
}