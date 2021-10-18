namespace RegionOrebroLan.Transforming.IO
{
	public class FileSystem : IFileSystem
	{
		#region Properties

		public virtual IDirectory Directory { get; } = new DirectoryWrapper();
		public virtual IFile File { get; } = new FileWrapper();
		public virtual IPath Path { get; } = new PathWrapper();

		#endregion
	}
}