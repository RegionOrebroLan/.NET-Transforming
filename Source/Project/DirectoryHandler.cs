using RegionOrebroLan.Transforming.IO;
using RegionOrebroLan.Transforming.IO.Extensions;

namespace RegionOrebroLan.Transforming
{
	public class DirectoryHandler(IFileSystem fileSystem) : IPackageHandler
	{
		#region Properties

		protected internal virtual IFileSystem FileSystem { get; } = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

		#endregion

		#region Methods

		public virtual void Extract(string destination, string source)
		{
			this.FileSystem.CopyDirectory(destination, source);
		}

		public virtual void Write(string destination, string source)
		{
			this.FileSystem.CopyDirectory(destination, source);
		}

		#endregion
	}
}