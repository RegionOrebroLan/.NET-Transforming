using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Transforming.IO;

namespace IntegrationTests.IO
{
	[TestClass]
	public class FileSearcherTest
	{
		#region Fields

		// ReSharper disable PossibleNullReferenceException
		private static readonly DirectoryInfo _testResourceDirectory = new DirectoryInfo(Path.Combine(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "Test-resources"));
		// ReSharper restore PossibleNullReferenceException

		#endregion

		#region Properties

		protected internal virtual DirectoryInfo TestResourceDirectory => _testResourceDirectory;

		#endregion

		#region Methods

		[TestMethod]
		public void Find_IfAnExcludePatternIsAbsolute_ItShouldBeIgnored()
		{
			var fileSearcher = new FileSearcher();
			var directoryPath = Path.Combine(this.TestResourceDirectory.FullName, "Package");
			const int numberOfFiles = 22;

			const string includePattern = "**/*";
			Assert.AreEqual(numberOfFiles, fileSearcher.Find(directoryPath, null, new[] { includePattern }).Count());

			const string relativeExcludePattern = "Web.config";
			Assert.AreEqual(numberOfFiles - 1, fileSearcher.Find(directoryPath, new[] { relativeExcludePattern }, new[] { includePattern }).Count());

			var absoluteExcludePattern = Path.Combine(directoryPath, "Web.config");
			Assert.IsTrue(File.Exists(absoluteExcludePattern));
			Assert.AreEqual(numberOfFiles, fileSearcher.Find(directoryPath, new[] { absoluteExcludePattern }, new[] { includePattern }).Count());
		}

		[TestMethod]
		public void Find_IncludePattern_Test()
		{
			var directoryPath = Path.Combine(this.TestResourceDirectory.FullName, "Package");
			var fileSearcher = new FileSearcher();

			var filePaths = fileSearcher.Find(directoryPath, null, new[] { "Directory-To-Delete" }).ToArray();
			Assert.IsFalse(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, new[] { "/Directory-To-Delete" }).ToArray();
			Assert.IsFalse(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, new[] { "Directory-To-Delete/**" }).ToArray();
			Assert.IsFalse(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, new[] { "/Directory-To-Delete/**" }).ToArray();
			Assert.IsFalse(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, new[] { "Directory-To-Delete/**/*" }).ToArray();
			Assert.IsFalse(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, new[] { "/Directory-To-Delete/**/*" }).ToArray();
			Assert.IsFalse(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, new[] { "**/Directory-To-Delete" }).ToArray();
			Assert.IsFalse(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, new[] { "/**/Directory-To-Delete" }).ToArray();
			Assert.IsFalse(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, new[] { "**/Directory-To-Delete/**" }).ToArray();
			Assert.AreEqual(2, filePaths.Length);
			Assert.AreEqual("Directory/Directory-To-Delete/File-To-Delete-1.txt", filePaths[0]);
			Assert.AreEqual("Directory/Directory-To-Delete/File-To-Delete-2.txt", filePaths[1]);

			filePaths = fileSearcher.Find(directoryPath, null, new[] { "/**/Directory-To-Delete/**" }).ToArray();
			Assert.AreEqual(2, filePaths.Length);
			Assert.AreEqual("Directory/Directory-To-Delete/File-To-Delete-1.txt", filePaths[0]);
			Assert.AreEqual("Directory/Directory-To-Delete/File-To-Delete-2.txt", filePaths[1]);

			filePaths = fileSearcher.Find(directoryPath, null, new[] { "**/Directory-To-Delete/**/*" }).ToArray();
			Assert.AreEqual(2, filePaths.Length);
			Assert.AreEqual("Directory/Directory-To-Delete/File-To-Delete-1.txt", filePaths[0]);
			Assert.AreEqual("Directory/Directory-To-Delete/File-To-Delete-2.txt", filePaths[1]);

			filePaths = fileSearcher.Find(directoryPath, null, new[] { "/**/Directory-To-Delete/**/*" }).ToArray();
			Assert.AreEqual(2, filePaths.Length);
			Assert.AreEqual("Directory/Directory-To-Delete/File-To-Delete-1.txt", filePaths[0]);
			Assert.AreEqual("Directory/Directory-To-Delete/File-To-Delete-2.txt", filePaths[1]);
		}

		[TestMethod]
		public void Find_ShouldNotReturnAnyHitsForAnAbsoluteIncludePattern()
		{
			var fileSearcher = new FileSearcher();
			var directoryPath = Path.Combine(this.TestResourceDirectory.FullName, "Package");

			var includePattern = Path.Combine(directoryPath, "Web.config");
			Assert.IsTrue(File.Exists(includePattern));
			Assert.IsFalse(fileSearcher.Find(directoryPath, null, new[] { includePattern }).Any());

			includePattern = @"C:\Some-directory\Some-file.txt";
			Assert.IsFalse(fileSearcher.Find(directoryPath, null, new[] { includePattern }).Any());
		}

		#endregion
	}
}