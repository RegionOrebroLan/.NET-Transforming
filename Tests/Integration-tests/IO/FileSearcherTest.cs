using RegionOrebroLan.Transforming.IO;

namespace IntegrationTests.IO
{
	public class FileSearcherTest
	{
		#region Fields

		private static readonly DirectoryInfo _resourcesDirectory = new(Path.Combine(Global.ProjectDirectory.FullName, "IO", "Resources", "FileSearcherTest"));

		#endregion

		#region Methods

		[Fact]
		public void Find_IfAnExcludePatternIsAbsolute_ItShouldBeIgnored()
		{
			var fileSearcher = new FileSearcher();
			var directoryPath = _resourcesDirectory.FullName;
			const int numberOfFiles = 22;

			const string includePattern = "**/*";
			Assert.Equal(numberOfFiles, fileSearcher.Find(directoryPath, null, [includePattern]).Count());

			const string relativeExcludePattern = "Web.config";
			Assert.Equal(numberOfFiles - 1, fileSearcher.Find(directoryPath, [relativeExcludePattern], [includePattern]).Count());

			var absoluteExcludePattern = Path.Combine(directoryPath, "Web.config");
			Assert.True(File.Exists(absoluteExcludePattern));
			Assert.Equal(numberOfFiles, fileSearcher.Find(directoryPath, [absoluteExcludePattern], [includePattern]).Count());
		}

		[Fact]
		public void Find_IncludePattern_Test()
		{
			var directoryPath = _resourcesDirectory.FullName;
			var fileSearcher = new FileSearcher();

			var filePaths = fileSearcher.Find(directoryPath, null, ["Directory-to-delete"]).ToArray();
			Assert.False(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, ["/Directory-to-delete"]).ToArray();
			Assert.False(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, ["Directory-to-delete/**"]).ToArray();
			Assert.False(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, ["/Directory-to-delete/**"]).ToArray();
			Assert.False(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, ["Directory-to-delete/**/*"]).ToArray();
			Assert.False(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, ["/Directory-to-delete/**/*"]).ToArray();
			Assert.False(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, ["**/Directory-to-delete"]).ToArray();
			Assert.False(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, ["/**/Directory-to-delete"]).ToArray();
			Assert.False(filePaths.Any());

			filePaths = fileSearcher.Find(directoryPath, null, ["**/Directory-to-delete/**"]).ToArray();
			Assert.Equal(2, filePaths.Length);
			Assert.Equal("Directory/Directory-to-delete/File-to-delete-1.txt", filePaths[0]);
			Assert.Equal("Directory/Directory-to-delete/File-to-delete-2.txt", filePaths[1]);

			filePaths = fileSearcher.Find(directoryPath, null, ["/**/Directory-to-delete/**"]).ToArray();
			Assert.Equal(2, filePaths.Length);
			Assert.Equal("Directory/Directory-to-delete/File-to-delete-1.txt", filePaths[0]);
			Assert.Equal("Directory/Directory-to-delete/File-to-delete-2.txt", filePaths[1]);

			filePaths = fileSearcher.Find(directoryPath, null, ["**/Directory-to-delete/**/*"]).ToArray();
			Assert.Equal(2, filePaths.Length);
			Assert.Equal("Directory/Directory-to-delete/File-to-delete-1.txt", filePaths[0]);
			Assert.Equal("Directory/Directory-to-delete/File-to-delete-2.txt", filePaths[1]);

			filePaths = fileSearcher.Find(directoryPath, null, ["/**/Directory-to-delete/**/*"]).ToArray();
			Assert.Equal(2, filePaths.Length);
			Assert.Equal("Directory/Directory-to-delete/File-to-delete-1.txt", filePaths[0]);
			Assert.Equal("Directory/Directory-to-delete/File-to-delete-2.txt", filePaths[1]);
		}

		[Fact]
		public void Find_ShouldNotReturnAnyHitsForAnAbsoluteIncludePattern()
		{
			var fileSearcher = new FileSearcher();
			var directoryPath = _resourcesDirectory.FullName;

			var includePattern = Path.Combine(directoryPath, "Web.config");
			Assert.True(File.Exists(includePattern));
			Assert.False(fileSearcher.Find(directoryPath, null, [includePattern]).Any());

			includePattern = @"C:\Some-directory\Some-file.txt";
			Assert.False(fileSearcher.Find(directoryPath, null, [includePattern]).Any());
		}

		#endregion
	}
}