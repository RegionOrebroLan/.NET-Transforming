using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RegionOrebroLan.Transforming.Configuration;
using RegionOrebroLan.Transforming.IO;
using RegionOrebroLan.Transforming.IO.Extensions;
using RegionOrebroLan.Transforming.Text.Extensions;

namespace RegionOrebroLan.Transforming
{
	public class PackageTransformer : BasicTransformer, IPackageTransformer
	{
		#region Fields

		private static readonly IEnumerable<char> _pathPatternSeparators = [';'];

		#endregion

		#region Constructors

		public PackageTransformer(IFileSearcher fileSearcher, IFileSystem fileSystem, IFileTransformerFactory fileTransformerFactory, ILoggerFactory loggerFactory, IOptionsMonitor<TransformingOptions> optionsMonitor, IPackageHandlerLoader packageHandlerLoader)
		{
			this.FileSearcher = fileSearcher ?? throw new ArgumentNullException(nameof(fileSearcher));
			this.FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
			this.FileTransformerFactory = fileTransformerFactory ?? throw new ArgumentNullException(nameof(fileTransformerFactory));
			this.Logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
			this.OptionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
			this.PackageHandlerLoader = packageHandlerLoader ?? throw new ArgumentNullException(nameof(packageHandlerLoader));
		}

		#endregion

		#region Properties

		protected internal virtual IFileSearcher FileSearcher { get; }
		protected internal virtual IFileSystem FileSystem { get; }
		protected internal virtual IFileTransformerFactory FileTransformerFactory { get; }
		protected internal virtual ILogger Logger { get; }
		protected internal virtual IOptionsMonitor<TransformingOptions> OptionsMonitor { get; }
		protected internal virtual IPackageHandlerLoader PackageHandlerLoader { get; }
		protected internal virtual IEnumerable<char> PathPatternSeparators => _pathPatternSeparators;

		#endregion

		#region Methods

		protected internal virtual void DeleteItems(string directoryPath, IEnumerable<string> pathToDeletePatterns)
		{
			var directoriesToCheckForDelete = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			foreach(var filePathToDelete in this.GetFilePathsToDelete(directoryPath, pathToDeletePatterns))
			{
				if(this.FileSystem.Directory.Exists(filePathToDelete))
				{
					this.FileSystem.Directory.Delete(filePathToDelete, true);
					continue;
				}

				if(!this.FileSystem.File.Exists(filePathToDelete))
					continue;

				var directoryName = this.FileSystem.Path.GetDirectoryName(filePathToDelete);

				if(!this.PathsAreEqual(directoryName, directoryPath))
					directoriesToCheckForDelete.Add(this.FileSystem.Path.GetDirectoryName(filePathToDelete));

				this.FileSystem.File.Delete(filePathToDelete);
			}

			foreach(var directoryToCheckForDelete in directoriesToCheckForDelete)
			{
				if(!this.FileSystem.Directory.Exists(directoryToCheckForDelete))
					continue;

				if(this.FindFiles(directoryToCheckForDelete, "**/*").Any())
					continue;

				this.FileSystem.Directory.Delete(directoryToCheckForDelete, true);
			}
		}

		protected internal virtual IEnumerable<string> FindFiles(string directoryPath, string includePattern)
		{
			return this.FileSearcher.Find(directoryPath, null, [includePattern]).Select(filePath => filePath.Replace(this.FileSystem.Path.AltDirectorySeparatorChar, this.FileSystem.Path.DirectorySeparatorChar));
		}

		protected internal virtual IEnumerable<string> FindFiles(string action, string directoryPath, IEnumerable<string> includePatterns)
		{
			return this.FindFiles(action, directoryPath, includePatterns, true);
		}

		protected internal virtual IEnumerable<string> FindFiles(string action, string directoryPath, string includePattern, bool validate)
		{
			var pathMatches = new List<string>();

			// ReSharper disable InvertIf
			if(!string.IsNullOrWhiteSpace(includePattern))
			{
				foreach(var part in includePattern.Split(this.PathPatternSeparators.ToArray()))
				{
					if(string.IsNullOrWhiteSpace(part))
						continue;

					var filePaths = this.FindFiles(directoryPath, part).ToArray();

					if(validate)
					{
						foreach(var filePath in filePaths)
						{
							this.ValidateFilePath(action, directoryPath, filePath);
						}
					}

					pathMatches.AddRange(filePaths);
				}
			}
			// ReSharper restore InvertIf

			return pathMatches.ToArray();
		}

		protected internal virtual IEnumerable<string> FindFiles(string action, string directoryPath, IEnumerable<string> includePatterns, bool validate)
		{
			return (includePatterns ?? []).SelectMany(item => this.FindFiles(action, directoryPath, item, validate));
		}

		protected internal virtual IEnumerable<string> GetFilePathsToDelete(string directoryPath, IEnumerable<string> pathToDeletePatterns)
		{
			var filePathsToDelete = this.FindFiles("delete", directoryPath, pathToDeletePatterns).Select(match => this.FileSystem.Path.IsAbsolutePath(match) ? match : this.FileSystem.Path.Combine(directoryPath, match)).ToArray();

			if(filePathsToDelete.Any(path => this.PathsAreEqual(directoryPath, path)))
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "It is not allowed to delete the directory \"{0}\".", directoryPath));

			return filePathsToDelete;
		}

		protected internal virtual IEnumerable<string> GetFilePathsToTransform(string directoryPath, IEnumerable<string> fileToTransformPatterns)
		{
			return this.FindFiles("transform", directoryPath, fileToTransformPatterns);
		}

		protected internal virtual string GetPathWithoutExtension(string path)
		{
			return path?.Substring(0, path.Length - this.FileSystem.Path.GetExtension(path).Length);
		}

		protected internal virtual IEnumerable<string> GetSourcesForTransformation(string path)
		{
			var sources = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

			// ReSharper disable InvertIf
			if(this.FileSystem.File.Exists(path))
			{
				var pathWithoutExtension = this.GetPathWithoutExtension(path);
				var extension = this.FileSystem.Path.GetExtension(path);

				while(pathWithoutExtension.Contains('.'))
				{
					var lastIndexOfDot = pathWithoutExtension.LastIndexOf('.');
					pathWithoutExtension = pathWithoutExtension.Substring(0, lastIndexOfDot);
					var source = $"{pathWithoutExtension}{extension}";

					if(this.FileSystem.File.Exists(source))
						sources.Add(source);
				}
			}
			// ReSharper restore InvertIf

			return sources;
		}

		protected internal virtual IDictionary<string, IDictionary<string, bool>> GetTransformInformation(string directoryPath, IEnumerable<string> fileToTransformPatterns, IEnumerable<string> transformationNames)
		{
			transformationNames = (transformationNames ?? []).ToArray();
			var stringComparer = StringComparer.OrdinalIgnoreCase;
			var transformInformation = new SortedDictionary<string, IDictionary<string, bool>>(stringComparer);

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

		protected internal virtual IDictionary<string, ISet<string>> GetTransformMap(string directoryPath, IEnumerable<string> fileToTransformPatterns)
		{
			var transformMap = new SortedDictionary<string, ISet<string>>(StringComparer.OrdinalIgnoreCase);

			var filePathsInvolvedInTransformation = this.GetFilePathsToTransform(directoryPath, fileToTransformPatterns).Select(path => this.FileSystem.Path.IsAbsolutePath(path) ? path : this.FileSystem.Path.Combine(directoryPath, path)).ToArray();

			foreach(var path in filePathsInvolvedInTransformation)
			{
				foreach(var source in this.GetSourcesForTransformation(path))
				{
					if(!transformMap.TryGetValue(source, out var set))
					{
						set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
						transformMap.Add(source, set);
					}

					set.Add(path);
				}
			}

			// Add paths from the patterns-search that are not in the map already, either as key or in any value collection. So we can fix bom and line-breaks on them later if necessary, even if they are not involved in any transformations.
			var existingPaths = new HashSet<string>(transformMap.Keys.Concat(transformMap.Values.SelectMany(value => value)), StringComparer.OrdinalIgnoreCase);
			var pathsToAdd = filePathsInvolvedInTransformation.Where(path => !existingPaths.Contains(path)).ToArray();

			foreach(var path in pathsToAdd)
			{
				transformMap.Add(path, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
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

		protected internal virtual void Transform(string directoryPath, IEnumerable<string> fileToTransformPatterns, IEnumerable<string> transformationNames, TransformingOptions options)
		{
			foreach(var item in this.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames))
			{
				if(item.Value.Any())
				{
					foreach(var transformFileInformation in item.Value)
					{
						if(!this.FileSystem.File.Exists(transformFileInformation.Key))
							continue;

						if(transformFileInformation.Value)
							this.FileTransformerFactory.Create(item.Key).Transform(item.Key, item.Key, transformFileInformation.Key, options.File);

						this.FileSystem.File.Delete(transformFileInformation.Key);
					}
				}
				else
				{
					string content;
					Encoding encoding;
					var useByteOrderMark = this.UseByteOrderMark(options.File, item.Key);

					using(var streamReader = new StreamReader(item.Key, true))
					{
						content = this.GetFileContent(options.File, streamReader);

						encoding = useByteOrderMark ? streamReader.CurrentEncoding : streamReader.CurrentEncoding.WithoutByteOrderMark();
					}

					this.FileSystem.WriteFile(content, encoding, item.Key);
				}
			}
		}

		public void Transform(string destination, IEnumerable<string> fileToTransformPatterns, IEnumerable<string> pathToDeletePatterns, string source, IEnumerable<string> transformationNames, TransformingOptions options = null)
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

			this.TransformInternal(destination, fileToTransformPatterns, packageExtractor, packageWriter, pathToDeletePatterns, source, transformationNames, options ?? this.OptionsMonitor.CurrentValue);
		}

		protected internal virtual void TransformInternal(string destination, IEnumerable<string> fileToTransformPatterns, IPackageExtractor packageExtractor, IPackageWriter packageWriter, IEnumerable<string> pathToDeletePatterns, string source, IEnumerable<string> transformationNames, TransformingOptions options)
		{
			if(packageExtractor == null)
				throw new ArgumentNullException(nameof(packageExtractor));

			if(packageWriter == null)
				throw new ArgumentNullException(nameof(packageWriter));

			if(options == null)
				throw new ArgumentNullException(nameof(options));

			var temporaryDirectoryPath = this.FileSystem.Path.Combine(this.FileSystem.Path.GetTempPath(), Guid.NewGuid().ToString());

			try
			{
				var temporaryOriginalDirectoryPath = this.FileSystem.Path.Combine(temporaryDirectoryPath, "Original");

				packageExtractor.Extract(temporaryOriginalDirectoryPath, source);

				var temporaryTransformDirectoryPath = this.FileSystem.Path.Combine(temporaryDirectoryPath, "Transform");

				this.FileSystem.CopyDirectory(temporaryTransformDirectoryPath, temporaryOriginalDirectoryPath);

				this.Transform(temporaryTransformDirectoryPath, fileToTransformPatterns, transformationNames, options);

				this.DeleteItems(temporaryTransformDirectoryPath, pathToDeletePatterns);

				packageWriter.Write(destination, temporaryTransformDirectoryPath);
			}
			finally
			{
				if(options.Package.Cleanup && this.FileSystem.Directory.Exists(temporaryDirectoryPath))
					this.FileSystem.Directory.Delete(temporaryDirectoryPath, true);
			}
		}

		protected internal virtual void ValidateFilePath(string action, string directoryPath, string filePath)
		{
			if(string.IsNullOrWhiteSpace(filePath))
				return;

			// If the file-path is a relative path.
			if(!this.FileSystem.Path.IsAbsolutePath(filePath))
				return;

			if(directoryPath == null)
				throw new ArgumentNullException(nameof(directoryPath));

			if(!this.FileSystem.Path.IsAbsolutePath(directoryPath))
				throw new ArgumentException($"The directory-path can not be relative ({directoryPath}).", nameof(directoryPath));

			// We are not allowed to delete files outside the directory-path.
			// We can not transform files outside the directory-path because we can not resolve the destination for those files.
			var directory = new DirectoryInfo(directoryPath);
			var file = new FileInfo(filePath);
			var parentDirectory = file.Directory;

			while(parentDirectory != null)
			{
				if(this.PathsAreEqual(directory.FullName, parentDirectory.FullName))
					return;

				parentDirectory = parentDirectory.Parent;
			}

			throw new InvalidOperationException($"It is not allowed to {action} the file \"{filePath}\". The file is outside the directory-path \"{directoryPath}\".");
		}

		#endregion
	}
}