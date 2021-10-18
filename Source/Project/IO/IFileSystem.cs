namespace RegionOrebroLan.Transforming.IO
{
	public interface IFileSystem
	{
		#region Properties

		IDirectory Directory { get; }
		IFile File { get; }
		IPath Path { get; }

		#endregion
	}
}