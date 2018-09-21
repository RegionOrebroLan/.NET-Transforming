using System;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;
using RegionOrebroLan.Transforming.IO.Extensions;

namespace RegionOrebroLan.Transforming
{
	public class ZipFileHandler : IPackageHandler
	{
		#region Constructors

		public ZipFileHandler(IFileSystem fileSystem)
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
			ZipFile.ExtractToDirectory(source, destination);
		}

		public virtual void Write(string destination, string source)
		{
			if(!this.FileSystem.Directory.Exists(source))
				throw new DirectoryNotFoundException(string.Format(CultureInfo.InvariantCulture, "The source-directory \"{0}\" does not exist.", source));

			var directoryToDeleteOnError = this.FileSystem.GetRequiredTopAncestorDirectoryForFile(destination);
			var file = new FileInfo(destination);

			if(!this.FileSystem.Directory.Exists(file.DirectoryName))
				this.FileSystem.Directory.CreateDirectory(file.DirectoryName);

			try
			{
				ZipFile.CreateFromDirectory(source, destination);
			}
			catch
			{
				if(directoryToDeleteOnError != null && this.FileSystem.Directory.Exists(directoryToDeleteOnError.FullName))
					this.FileSystem.Directory.Delete(directoryToDeleteOnError.FullName);

				throw;
			}
		}

		#endregion
	}
}