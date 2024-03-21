using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Transforming.IO;

namespace IntegrationTests.IO
{
	[TestClass]
	public class FileSearcherTest
	{
		#region Fields

		private static readonly DirectoryInfo _resourcesDirectory = new(Path.Combine(Global.ProjectDirectory.FullName, "IO", "Resources", "FileSearcherTest"));

		#endregion

		#region Methods

		[TestMethod]
		public void Find_IfAnExcludePatternIsAbsolute_ItShouldBeIgnored()
		{
			var fileSearcher = new FileSearcher();
			var directoryPath = _resourcesDirectory.FullName;
			const int numberOfFiles = 22;

			const string includePattern = "**/*";
			Assert.AreEqual(numberOfFiles, fileSearcher.Find(directoryPath, null, [includePattern]).Count());

			const string relativeExcludePattern = "Web.config";
			Assert.AreEqual(numberOfFiles - 1, fileSearcher.Find(directoryPath, [relativeExcludePattern], [includePattern]).Count());

			var absoluteExcludePattern = Path.Combine(directoryPath, "Web.config");
			Assert.IsTrue(File.Exists(absoluteExcludePattern));
			Assert.AreEqual(numberOfFiles, fileSearcher.Find(directoryPath, [absoluteExcludePattern], [includePattern]).Count());
		}

		[TestMethod]
		public void Find_IncludePattern_Test()
		{
			var directoryPath = _resourcesDirectory.FullName;
			var fileSearcher = new FileSearcher();

			var filePaths = fileSearcher.Find(directoryPath, null, ["Directory-to-delete"]).ToArray();
			Assert.IsFalse(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, ["/Directory-to-delete"]).ToArray();
			Assert.IsFalse(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, ["Directory-to-delete/**"]).ToArray();
			Assert.IsFalse(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, ["/Directory-to-delete/**"]).ToArray();
			Assert.IsFalse(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, ["Directory-to-delete/**/*"]).ToArray();
			Assert.IsFalse(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, ["/Directory-to-delete/**/*"]).ToArray();
			Assert.IsFalse(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, ["**/Directory-to-delete"]).ToArray();
			Assert.IsFalse(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, ["/**/Directory-to-delete"]).ToArray();
			Assert.IsFalse(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, ["**/Directory-to-delete/**"]).ToArray();
			Assert.AreEqual(2, filePaths.Length);
			Assert.AreEqual("Directory/Directory-to-delete/File-to-delete-1.txt", filePaths[0]);
			Assert.AreEqual("Directory/Directory-to-delete/File-to-delete-2.txt", filePaths[1]);

			filePaths = fileSearcher.Find(directoryPath, null, ["/**/Directory-to-delete/**"]).ToArray();
			Assert.AreEqual(2, filePaths.Length);
			Assert.AreEqual("Directory/Directory-to-delete/File-to-delete-1.txt", filePaths[0]);
			Assert.AreEqual("Directory/Directory-to-delete/File-to-delete-2.txt", filePaths[1]);

			filePaths = fileSearcher.Find(directoryPath, null, ["**/Directory-to-delete/**/*"]).ToArray();
			Assert.AreEqual(2, filePaths.Length);
			Assert.AreEqual("Directory/Directory-to-delete/File-to-delete-1.txt", filePaths[0]);
			Assert.AreEqual("Directory/Directory-to-delete/File-to-delete-2.txt", filePaths[1]);

			filePaths = fileSearcher.Find(directoryPath, null, ["/**/Directory-to-delete/**/*"]).ToArray();
			Assert.AreEqual(2, filePaths.Length);
			Assert.AreEqual("Directory/Directory-to-delete/File-to-delete-1.txt", filePaths[0]);
			Assert.AreEqual("Directory/Directory-to-delete/File-to-delete-2.txt", filePaths[1]);
		}

		[TestMethod]
		public void Find_ShouldNotReturnAnyHitsForAnAbsoluteIncludePattern()
		{
			var fileSearcher = new FileSearcher();
			var directoryPath = _resourcesDirectory.FullName;

			var includePattern = Path.Combine(directoryPath, "Web.config");
			Assert.IsTrue(File.Exists(includePattern));
			Assert.IsFalse(fileSearcher.Find(directoryPath, null, [includePattern]).Any());

			includePattern = @"C:\Some-directory\Some-file.txt";
			Assert.IsFalse(fileSearcher.Find(directoryPath, null, [includePattern]).Any());
		}

		#endregion
	}
}