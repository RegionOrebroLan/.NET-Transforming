using System.Runtime.InteropServices;
using RegionOrebroLan.Transforming.IO.Extensions;
using Xunit;

namespace UnitTests.IO.Extensions
{
	public class PathExtensionTest
	{
		#region Methods

		[Fact]
		public async Task EnsureTrailingDirectorySeparator_IfThePathDoesNotEndWithADirectorySeparator_ShouldReturnAPathWithATrailingDirectorySeparator()
		{
			await Task.CompletedTask;

			const string directoryName = "Some-directory";

			var path = directoryName;
			var expected = $"{directoryName}{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"\" : "/")}";
			Assert.Equal(expected, PathExtension.EnsureTrailingDirectorySeparator(path));

			path = $"{directoryName}/";
			expected = path;
			Assert.Equal(expected, PathExtension.EnsureTrailingDirectorySeparator(path));

			path = $"{directoryName}\\";
			expected = $"{directoryName}{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"\" : @"\/")}";
			Assert.Equal(expected, PathExtension.EnsureTrailingDirectorySeparator(path));
		}

		[Fact]
		public async Task IsPathFullyQualified_Test()
		{
			await Task.CompletedTask;

			Assert.False(PathExtension.IsPathFullyQualified("test"));
			Assert.False(PathExtension.IsPathFullyQualified("test/"));
			Assert.False(PathExtension.IsPathFullyQualified("test.txt"));

			if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				Assert.False(PathExtension.IsPathFullyQualified("/test"));
				Assert.False(PathExtension.IsPathFullyQualified("/test/"));
				Assert.False(PathExtension.IsPathFullyQualified("/test.txt"));
				Assert.False(PathExtension.IsPathFullyQualified(@"\test"));
				Assert.False(PathExtension.IsPathFullyQualified(@"\test\"));
				Assert.False(PathExtension.IsPathFullyQualified(@"\test.txt"));
				Assert.True(PathExtension.IsPathFullyQualified("C:/test"));
				Assert.True(PathExtension.IsPathFullyQualified("C:/test/"));
				Assert.True(PathExtension.IsPathFullyQualified("C:/test.txt"));
				Assert.True(PathExtension.IsPathFullyQualified(@"C:\test"));
				Assert.True(PathExtension.IsPathFullyQualified(@"C:\test\"));
				Assert.True(PathExtension.IsPathFullyQualified(@"C:\test.txt"));
			}
			else
			{
				Assert.True(PathExtension.IsPathFullyQualified("/test"));
				Assert.True(PathExtension.IsPathFullyQualified("/test/"));
				Assert.True(PathExtension.IsPathFullyQualified("/test.txt"));
			}
		}

		#endregion
	}
}