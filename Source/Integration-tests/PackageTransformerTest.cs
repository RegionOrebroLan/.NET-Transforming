using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.IO;
using RegionOrebroLan.Transforming.IntegrationTests.Helpers;

namespace RegionOrebroLan.Transforming.IntegrationTests
{
	[TestClass]
	public class PackageTransformerTest : BasicTransformingTest
	{
		#region Fields

		private static PackageTransformer _packageTransformer;

		#endregion

		#region Properties

		protected internal virtual PackageTransformer PackageTransformer
		{
			get
			{
				// ReSharper disable InvertIf
				if(_packageTransformer == null)
				{
					var fileSystem = new FileSystem();

					_packageTransformer = new PackageTransformer(new FileSystemEntryMatcher(), fileSystem, new FileTransformerFactory(fileSystem), new PackageHandlerLoader(fileSystem));
				}
				// ReSharper restore InvertIf

				return _packageTransformer;
			}
		}

		protected internal virtual Random Random { get; } = new Random(DateTime.Now.Millisecond);

		#endregion

		#region Methods

		protected internal virtual IEnumerable<string> GetFileSystemEntries(string path)
		{
			return Directory.EnumerateFileSystemEntries(path, "*", SearchOption.AllDirectories);
		}

		protected internal virtual string GetRandomPackageName(string name)
		{
			return name + (this.Random.Next(0, 2) != 0 ? ".zip" : string.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Transform_IfAnyPathToDeleteIsOutsideTheTransformDirectory_ShouldThrowAnInvalidOperationException()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName("Transformed-Package"));
			var fileToTransformPatterns = Enumerable.Empty<string>();
			var pathToDeletePatterns = new[] {@"C:\Some-directory\Some-file.txt"};
			var source = this.GetTestResourcePath(this.GetRandomPackageName("Package"));
			var transformationNames = new[] {"Release", "Test"};

			this.PackageTransformer.Transform(true, destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Transform_IfAnyPathToTransformIsOutsideTheTransformDirectory_ShouldThrowAnInvalidOperationException()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName("Transformed-Package"));
			var fileToTransformPatterns = new[] {@"C:\Some-directory\Some-file.txt"};
			var pathToDeletePatterns = Enumerable.Empty<string>();
			var source = this.GetTestResourcePath(this.GetRandomPackageName("Package"));
			var transformationNames = new[] {"Release", "Test"};

			this.PackageTransformer.Transform(true, destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheDestinationParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformDestinationParameterException<ArgumentException>(string.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Transform_IfTheDestinationParameterIsNull_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformDestinationParameterException<ArgumentNullException>(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheDestinationParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformDestinationParameterException<ArgumentException>(" ");
		}

		[TestMethod]
		public void Transform_IfThePathToDeletePatternsContainsAWholeDirectoryToDelete_ShouldTransformCorrectly()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName("Transformed-Package"));
			var fileToTransformPatterns = new[] {@"**\*.config*", @"**\*.json", @"**\*.xml"};
			var pathToDeletePatterns = new[] {@"Directory\Directory-To-Delete", @"Directory\File-To-Delete.text", @"File-To-Delete.txt"};
			var source = this.GetTestResourcePath(this.GetRandomPackageName("Package"));
			var transformationNames = new[] {"Release", "Test"};

			this.PackageTransformer.Transform(true, destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames);

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);
				destination = extractedDestination;
			}

			var expected = this.GetTestResourcePath("Package-Expected");

			var actualItems = this.GetFileSystemEntries(destination).ToArray();
			var expectedItems = this.GetFileSystemEntries(expected).ToArray();

			Assert.IsTrue(actualItems.SequenceEqual(expectedItems, new FileComparer(destination, expected)));
		}

		[TestMethod]
		public void Transform_IfThePathToDeletePatternsIncludeAllEntries_ShouldTransformCorrectly()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName("Transformed-Package"));
			var fileToTransformPatterns = new[] {@"**\*.config*", @"**\*.json", @"**\*.xml"};
			var pathToDeletePatterns = new[] {"Directory", "Views", "AppSettings.json", "File-To-Delete.txt", "Web.config"};
			var source = this.GetTestResourcePath(this.GetRandomPackageName("Package"));
			var transformationNames = new[] {"Release", "Test"};

			this.PackageTransformer.Transform(true, destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames);

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);
				destination = extractedDestination;
			}

			Assert.AreEqual(0, this.GetFileSystemEntries(destination).Count());
		}

		[TestMethod]
		public void Transform_IfTheSourceIsEmpty_ShouldCreateAnEmptyPackageAtTheDestination()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName("Transformed-Empty-Package"));
			var source = this.GetTestResourcePath(this.GetRandomPackageName("Empty"));

			this.PackageTransformer.Transform(true, destination, null, null, source, null);

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);
				destination = extractedDestination;
			}

			var actualItems = this.GetFileSystemEntries(destination).ToArray();
			var expectedItems = this.GetFileSystemEntries(this.GetTestResourcePath("Empty")).ToArray();

			source = this.GetTestResourcePath("Empty");

			Assert.IsTrue(actualItems.SequenceEqual(expectedItems, new FileComparer(destination, source)));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheSourceParameterDoesNotExistAsDirectoryOrZipFile_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformSourceParameterException<ArgumentException>(this.GetTestResourcePath("File.txt"));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheSourceParameterDoesNotExistAsFileSystemEntry_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformSourceParameterException<ArgumentException>(this.GetTestResourcePath("Non-existing-file.txt"));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheSourceParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformSourceParameterException<ArgumentException>(string.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Transform_IfTheSourceParameterIsNull_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformSourceParameterException<ArgumentNullException>(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheSourceParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformSourceParameterException<ArgumentException>(" ");
		}

		[TestMethod]
		public void Transform_ShouldTransformCorrectly()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName("Transformed-Package"));
			var fileToTransformPatterns = new[] {@"**\*.config*", @"**\*.json", @"**\*.xml"};
			var pathToDeletePatterns = new[] {@"**\Directory-To-Delete\*", @"**\File-To-Delete.*"};
			var source = this.GetTestResourcePath(this.GetRandomPackageName("Package"));
			var transformationNames = new[] {"Release", "Test"};

			this.PackageTransformer.Transform(true, destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames);

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);
				destination = extractedDestination;
			}

			var expected = this.GetTestResourcePath("Package-Expected");

			var actualItems = this.GetFileSystemEntries(destination).ToArray();
			var expectedItems = this.GetFileSystemEntries(expected).ToArray();

			Assert.IsTrue(actualItems.SequenceEqual(expectedItems, new FileComparer(destination, expected)));
		}

		[TestMethod]
		public void Transform_ShouldTransformWithTheTransformationNamesInTheDeclaredOrderAndNotAlphabetically()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName("Transformed-Package"));
			var fileToTransformPatterns = new[] {@"**\*.config*", @"**\*.json", @"**\*.xml"};
			var source = this.GetTestResourcePath(this.GetRandomPackageName("Alphabetical-Test"));
			var transformationNames = new[] {"C", "A", "B"};

			this.PackageTransformer.Transform(true, destination, fileToTransformPatterns, null, source, transformationNames);

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);
				destination = extractedDestination;
			}

			var expected = this.GetTestResourcePath("Alphabetical-Test-Expected");

			var actualItems = this.GetFileSystemEntries(destination).ToArray();
			var expectedItems = this.GetFileSystemEntries(expected).ToArray();

			Assert.IsTrue(actualItems.SequenceEqual(expectedItems, new FileComparer(destination, expected)));
		}

		protected internal virtual void ValidateTransformDestinationParameterException<T>(string destination) where T : ArgumentException
		{
			this.ValidateDestinationParameterException<T>(() => { this.PackageTransformer.Transform(true, destination, null, null, this.GetTestResourcePath("Empty-directory"), null); });
		}

		protected internal virtual void ValidateTransformSourceParameterException<T>(string source) where T : ArgumentException
		{
			this.ValidateSourceParameterException<T>(() => { this.PackageTransformer.Transform(true, this.GetOutputPath("Transformed-directory"), null, null, source, null); });
		}

		#endregion
	}
}