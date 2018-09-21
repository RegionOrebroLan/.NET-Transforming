using System;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Text;

namespace RegionOrebroLan.Transforming.IO.Extensions
{
	public static class FileSystemExtension
	{
		#region Methods

		public static void CopyDirectory(this IFileSystem fileSystem, string destination, string source)
		{
			if(fileSystem == null)
				throw new ArgumentNullException(nameof(fileSystem));

			if(!fileSystem.Directory.Exists(source))
				throw new DirectoryNotFoundException(string.Format(CultureInfo.InvariantCulture, "The directory \"{0}\" does not exist.", source));

			if(!fileSystem.Directory.Exists(destination))
				fileSystem.Directory.CreateDirectory(destination);

			foreach(var path in fileSystem.Directory.GetDirectories(source))
			{
				fileSystem.CopyDirectory(fileSystem.Path.Combine(destination, new DirectoryInfo(path).Name), path);
			}

			foreach(var path in fileSystem.Directory.GetFiles(source))
			{
				fileSystem.File.Copy(path, fileSystem.Path.Combine(destination, new FileInfo(path).Name), true);
			}
		}

		public static bool EntryExists(this IFileSystem fileSystem, string path)
		{
			if(fileSystem == null)
				throw new ArgumentNullException(nameof(fileSystem));

			return fileSystem.Directory.Exists(path) || fileSystem.File.Exists(path);
		}

		public static DirectoryInfoBase GetRequiredTopAncestorDirectoryForFile(this IFileSystem fileSystem, string filePath)
		{
			if(fileSystem == null)
				throw new ArgumentNullException(nameof(fileSystem));

			DirectoryInfo requiredTopAncestorDirectory = null;
			var file = new FileInfo(filePath);

			// ReSharper disable All
			if(!fileSystem.Directory.Exists(file.DirectoryName))
			{
				requiredTopAncestorDirectory = new DirectoryInfo(file.DirectoryName);

				while(requiredTopAncestorDirectory.Parent != null && !requiredTopAncestorDirectory.Parent.Exists)
				{
					requiredTopAncestorDirectory = requiredTopAncestorDirectory.Parent;
				}
			}
			// ReSharper restore All

			return requiredTopAncestorDirectory != null ? new DirectoryInfoWrapper(fileSystem, requiredTopAncestorDirectory) : null;
		}

		public static void WriteFile(this IFileSystem fileSystem, string content, Encoding encoding, string path)
		{
			if(fileSystem == null)
				throw new ArgumentNullException(nameof(fileSystem));

			var directoryToDeleteOnError = fileSystem.GetRequiredTopAncestorDirectoryForFile(path);
			var file = new FileInfo(path);

			if(!fileSystem.Directory.Exists(file.DirectoryName))
				fileSystem.Directory.CreateDirectory(file.DirectoryName);

			try
			{
				File.WriteAllText(path, content, encoding);
			}
			catch
			{
				if(directoryToDeleteOnError != null && fileSystem.Directory.Exists(directoryToDeleteOnError.FullName))
					fileSystem.Directory.Delete(directoryToDeleteOnError.FullName);

				throw;
			}
		}

		#endregion
	}
}