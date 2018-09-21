using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using RegionOrebroLan.IO;
using RegionOrebroLan.Transforming.IO.Extensions;

namespace RegionOrebroLan.Transforming
{
	public class PackageTransformer : IPackageTransformer
	{
		#region Fields

		private static readonly IEnumerable<char> _pathPatternSeparators = new[] {';'};

		#endregion

		#region Constructors

		public PackageTransformer(IFileSystemEntryMatcher fileSystemEntryMatcher, IFileSystem fileSystem, IFileTransformerFactory fileTransformerFactory, IPackageHandlerLoader packageHandlerLoader)
		{
			this.FileSystemEntryMatcher = fileSystemEntryMatcher ?? throw new ArgumentNullException(nameof(fileSystemEntryMatcher));
			this.FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
			this.FileTransformerFactory = fileTransformerFactory ?? throw new ArgumentNullException(nameof(fileTransformerFactory));
			this.PackageHandlerLoader = packageHandlerLoader ?? throw new ArgumentNullException(nameof(packageHandlerLoader));
		}

		#endregion

		#region Properties

		protected internal virtual IFileSystem FileSystem { get; }
		protected internal virtual IFileSystemEntryMatcher FileSystemEntryMatcher { get; }
		protected internal virtual IFileTransformerFactory FileTransformerFactory { get; }
		protected internal virtual IPackageHandlerLoader PackageHandlerLoader { get; }
		protected internal virtual IEnumerable<char> PathPatternSeparators => _pathPatternSeparators;

		#endregion

		#region Methods

		protected internal virtual void DeleteItems(string directoryPath, IEnumerable<string> pathToDeletePatterns)
		{
			var directoriesToCheckForDelete = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			foreach(var fileSystemEntryPathToDelete in this.GetFileSystemEntryPathsToDelete(directoryPath, pathToDeletePatterns))
			{
				if(this.FileSystem.Directory.Exists(fileSystemEntryPathToDelete))
				{
					this.FileSystem.Directory.Delete(fileSystemEntryPathToDelete, true);
					continue;
				}

				if(!this.FileSystem.File.Exists(fileSystemEntryPathToDelete))
					continue;

				var directoryName = this.FileSystem.Path.GetDirectoryName(fileSystemEntryPathToDelete);

				if(!this.PathsAreEqual(directoryName, directoryPath))
					directoriesToCheckForDelete.Add(this.FileSystem.Path.GetDirectoryName(fileSystemEntryPathToDelete));

				this.FileSystem.File.Delete(fileSystemEntryPathToDelete);
			}

			foreach(var directoryToCheckForDelete in directoriesToCheckForDelete)
			{
				if(!this.FileSystem.Directory.Exists(directoryToCheckForDelete))
					continue;

				if(this.FileSystem.Directory.EnumerateFileSystemEntries(directoryToCheckForDelete).Any())
					continue;

				this.FileSystem.Directory.Delete(directoryToCheckForDelete);
			}
		}

		protected internal virtual IEnumerable<string> GetFilePathsToTransform(string directoryPath, IEnumerable<string> fileToTransformPatterns)
		{
			return this.GetFileSystemEntryPathMatches("transform", directoryPath, fileToTransformPatterns);
		}

		protected internal virtual IEnumerable<string> GetFileSystemEntryPathMatches(string directoryPath, string includePattern)
		{
			return this.FileSystemEntryMatcher.GetPathMatches(directoryPath, null, includePattern);
		}

		protected internal virtual IEnumerable<string> GetFileSystemEntryPathMatches(string action, string directoryPath, IEnumerable<string> includePatterns)
		{
			return this.GetFileSystemEntryPathMatches(action, directoryPath, includePatterns, true);
		}

		protected internal virtual IEnumerable<string> GetFileSystemEntryPathMatches(string action, string directoryPath, string includePattern, bool validate)
		{
			var pathMatches = new List<string>();

			// ReSharper disable InvertIf
			if(!string.IsNullOrWhiteSpace(includePattern))
			{
				foreach(var part in includePattern.Split(this.PathPatternSeparators.ToArray()))
				{
					if(string.IsNullOrWhiteSpace(part))
						continue;

					var fileSystemEntryPathMatches = this.GetFileSystemEntryPathMatches(directoryPath, part).ToArray();

					if(validate)
					{
						foreach(var fileSystemEntryPathMatch in fileSystemEntryPathMatches)
						{
							this.ValidateFileSystemEntryPathMatch(action, directoryPath, fileSystemEntryPathMatch);
						}
					}

					pathMatches.AddRange(fileSystemEntryPathMatches);
				}
			}
			// ReSharper restore InvertIf

			return pathMatches.ToArray();
		}

		protected internal virtual IEnumerable<string> GetFileSystemEntryPathMatches(string action, string directoryPath, IEnumerable<string> includePatterns, bool validate)
		{
			return (includePatterns ?? Enumerable.Empty<string>()).SelectMany(item => this.GetFileSystemEntryPathMatches(action, directoryPath, item, validate));
		}

		protected internal virtual IEnumerable<string> GetFileSystemEntryPathsToDelete(string directoryPath, IEnumerable<string> pathToDeletePatterns)
		{
			var fileSystemEntryPathsToDelete = this.GetFileSystemEntryPathMatches("delete", directoryPath, pathToDeletePatterns).Select(match => this.FileSystem.Path.IsPathRooted(match) ? match : this.FileSystem.Path.Combine(directoryPath, match)).ToArray();

			if(fileSystemEntryPathsToDelete.Any(path => this.PathsAreEqual(directoryPath, path)))
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "It is not allowed to delete the directory \"{0}\".", directoryPath));

			return fileSystemEntryPathsToDelete;
		}

		protected internal virtual string GetPathWithoutExtension(string path)
		{
			return path?.Substring(0, path.Length - this.FileSystem.Path.GetExtension(path).Length);
		}

		protected internal virtual IDictionary<string, IDictionary<string, bool>> GetTransformInformation(string directoryPath, IEnumerable<string> fileToTransformPatterns, IEnumerable<string> transformationNames)
		{
			transformationNames = (transformationNames ?? Enumerable.Empty<string>()).ToArray();
			var stringComparer = StringComparer.OrdinalIgnoreCase;
			var transformInformation = new Dictionary<string, IDictionary<string, bool>>(stringComparer);

			foreach(var mapping in this.GetTransformMap(directoryPath, fileToTransformPatterns))
			{
				var dictionary = new Dictionary<string, bool>(stringComparer);
				transformInformation.Add(mapping.Key, dictionary);

				foreach(var transformationName in transformationNames)
				{
					var transformationFilePath = mapping.Value.FirstOrDefault(path => this.GetPathWithoutExtension(path).EndsWith("." + transformationName, StringComparison.OrdinalIgnoreCase));

					if(transformationFilePath == null)
						continue;

					dictionary.Add(transformationFilePath, true);
					mapping.Value.Remove(transformationFilePath);
				}

				foreach(var remainingItem in mapping.Value)
				{
					dictionary.Add(remainingItem, false);
				}
			}

			return transformInformation;
		}

		protected internal virtual IDictionary<string, IList<string>> GetTransformMap(string directoryPath, IEnumerable<string> fileToTransformPatterns)
		{
			var transformMap = new Dictionary<string, IList<string>>(StringComparer.OrdinalIgnoreCase);

			var filePathsInvolvedInTransformation = this.GetFilePathsToTransform(directoryPath, fileToTransformPatterns).ToArray();

			foreach(var path in filePathsInvolvedInTransformation)
			{
				var fullPath = this.FileSystem.Path.IsPathRooted(path) ? path : this.FileSystem.Path.Combine(directoryPath, path);

				if(!this.TryGetSourceForTransformation(fullPath, out var source))
					continue;

				if(!transformMap.TryGetValue(source, out var list))
				{
					list = new List<string>();
					transformMap.Add(source, list);
				}

				list.Add(fullPath);
			}

			return transformMap;
		}

		protected internal virtual string NormalizePath(string path)
		{
			return path?.TrimEnd(this.FileSystem.Path.AltDirectorySeparatorChar, this.FileSystem.Path.DirectorySeparatorChar).ToUpperInvariant();
		}

		protected internal virtual bool PathsAreEqual(string firstPath, string secondPath)
		{
			return string.Equals(this.NormalizePath(firstPath), this.NormalizePath(secondPath), StringComparison.OrdinalIgnoreCase);
		}

		protected internal virtual void Transform(string directoryPath, IEnumerable<string> fileToTransformPatterns, IEnumerable<string> transformationNames)
		{
			foreach(var item in this.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames))
			{
				foreach(var transformFileInformation in item.Value)
				{
					if(!this.FileSystem.File.Exists(transformFileInformation.Key))
						continue;

					if(transformFileInformation.Value)
						this.FileTransformerFactory.Create(item.Key).Transform(item.Key, item.Key, transformFileInformation.Key);

					this.FileSystem.File.Delete(transformFileInformation.Key);
				}
			}
		}

		public void Transform(bool cleanup, string destination, IEnumerable<string> fileToTransformPatterns, IEnumerable<string> pathToDeletePatterns, string source, IEnumerable<string> transformationNames)
		{
			if(destination == null)
				throw new ArgumentNullException(nameof(destination));

			if(string.IsNullOrWhiteSpace(destination))
				throw new ArgumentException("Destination can not be empty or whitespace.", nameof(destination));

			if(this.FileSystem.EntryExists(destination))
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The destination \"{0}\" already exists.", destination), nameof(destination));

			IPackageWriter packageWriter;

			try
			{
				packageWriter = this.PackageHandlerLoader.GetWriter(destination);
			}
			catch(Exception exception)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The destination \"{0}\" is invalid.", destination), nameof(destination), exception);
			}

			if(source == null)
				throw new ArgumentNullException(nameof(source));

			if(string.IsNullOrWhiteSpace(source))
				throw new ArgumentException("Source can not be empty or whitespace.", nameof(source));

			if(!this.FileSystem.EntryExists(source))
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The source \"{0}\" does not exist.", source), nameof(source));

			IPackageExtractor packageExtractor;

			try
			{
				packageExtractor = this.PackageHandlerLoader.GetExtractor(source);
			}
			catch(Exception exception)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The source \"{0}\" is invalid.", source), nameof(source), exception);
			}

			this.TransformInternal(cleanup, destination, fileToTransformPatterns, packageExtractor, packageWriter, pathToDeletePatterns, source, transformationNames);
		}

		protected internal virtual void TransformInternal(bool cleanup, string destination, IEnumerable<string> fileToTransformPatterns, IPackageExtractor packageExtractor, IPackageWriter packageWriter, IEnumerable<string> pathToDeletePatterns, string source, IEnumerable<string> transformationNames)
		{
			if(packageExtractor == null)
				throw new ArgumentNullException(nameof(packageExtractor));

			if(packageWriter == null)
				throw new ArgumentNullException(nameof(packageWriter));

			var temporaryDirectoryPath = this.FileSystem.Path.Combine(this.FileSystem.Path.GetTempPath(), Guid.NewGuid().ToString());

			try
			{
				var temporaryOriginalDirectoryPath = this.FileSystem.Path.Combine(temporaryDirectoryPath, "Original");

				packageExtractor.Extract(temporaryOriginalDirectoryPath, source);

				var temporaryTransformDirectoryPath = this.FileSystem.Path.Combine(temporaryDirectoryPath, "Transform");

				this.FileSystem.CopyDirectory(temporaryTransformDirectoryPath, temporaryOriginalDirectoryPath);

				this.Transform(temporaryTransformDirectoryPath, fileToTransformPatterns, transformationNames);

				this.DeleteItems(temporaryTransformDirectoryPath, pathToDeletePatterns);

				packageWriter.Write(destination, temporaryTransformDirectoryPath);
			}
			finally
			{
				if(cleanup && this.FileSystem.Directory.Exists(temporaryDirectoryPath))
					this.FileSystem.Directory.Delete(temporaryDirectoryPath, true);
			}
		}

		protected internal virtual bool TryGetSourceForTransformation(string path, out string source)
		{
			source = null;

			if(!this.FileSystem.File.Exists(path))
				return false;

			var filePathWithoutExtension = this.GetPathWithoutExtension(path);

			var lastIndexOfDot = filePathWithoutExtension.LastIndexOf(".", StringComparison.OrdinalIgnoreCase);

			if(lastIndexOfDot < 0)
				return false;

			source = path.Substring(0, lastIndexOfDot) + this.FileSystem.Path.GetExtension(path);

			if(this.FileSystem.File.Exists(source))
				return true;

			source = null;
			return false;
		}

		protected internal virtual void ValidateFileSystemEntryPathMatch(string action, string directoryPath, string fileSystemEntryPathMatch)
		{
			if(string.IsNullOrWhiteSpace(fileSystemEntryPathMatch))
				return;

			if(!this.FileSystem.Path.IsPathRooted(fileSystemEntryPathMatch))
				return;

			// We are not allowed to delete files outside the directory-path.
			// We can not transform files outside the directory-path because we can not resolve the destination for those files.
			if(!new Uri(directoryPath).IsBaseOf(new Uri(fileSystemEntryPathMatch)))
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "It is not allowed to {0} the file \"{1}\". The file is outside the directory-path \"{2}\".", action, fileSystemEntryPathMatch, directoryPath));
		}

		#endregion
	}
}