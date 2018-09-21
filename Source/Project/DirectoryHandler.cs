using System;
using System.IO.Abstractions;
using RegionOrebroLan.Transforming.IO.Extensions;

namespace RegionOrebroLan.Transforming
{
	public class DirectoryHandler : IPackageHandler
	{
		#region Constructors

		public DirectoryHandler(IFileSystem fileSystem)
		{
			this.FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
		}

		#endregion

		#region Properties

		protected internal virtual IFileSystem FileSystem { get; }

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