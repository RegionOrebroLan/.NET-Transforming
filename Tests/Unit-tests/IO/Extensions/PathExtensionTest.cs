using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Transforming.IO.Extensions;

namespace UnitTests.IO.Extensions
{
	[TestClass]
	public class PathExtensionTest
	{
		#region Methods

		[TestMethod]
		public async Task EnsureTrailingDirectorySeparator_IfThePathDoesNotEndWithADirectorySeparator_ShouldReturnAPathWithATrailingDirectorySeparator()
		{
			await Task.CompletedTask;

			const string directoryName = "Some-directory";

			var path = directoryName;
			var expected = $"{directoryName}{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"\" : "/")}";
			Assert.AreEqual(expected, PathExtension.EnsureTrailingDirectorySeparator(path));

			path = $"{directoryName}/";
			expected = path;
			Assert.AreEqual(expected, PathExtension.EnsureTrailingDirectorySeparator(path));

			path = $"{directoryName}\\";
			expected = $"{directoryName}{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"\" : @"\/")}";
			Assert.AreEqual(expected, PathExtension.EnsureTrailingDirectorySeparator(path));
		}

		[TestMethod]
		public async Task IsPathFullyQualified_Test()
		{
			await Task.CompletedTask;

			Assert.IsFalse(PathExtension.IsPathFullyQualified("test"));
			Assert.IsFalse(PathExtension.IsPathFullyQualified("test/"));
			Assert.IsFalse(PathExtension.IsPathFullyQualified("test.txt"));

			if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				Assert.IsFalse(PathExtension.IsPathFullyQualified("/test"));
				Assert.IsFalse(PathExtension.IsPathFullyQualified("/test/"));
				Assert.IsFalse(PathExtension.IsPathFullyQualified("/test.txt"));
				Assert.IsFalse(PathExtension.IsPathFullyQualified(@"\test"));
				Assert.IsFalse(PathExtension.IsPathFullyQualified(@"\test\"));
				Assert.IsFalse(PathExtension.IsPathFullyQualified(@"\test.txt"));
				Assert.IsTrue(PathExtension.IsPathFullyQualified("C:/test"));
				Assert.IsTrue(PathExtension.IsPathFullyQualified("C:/test/"));
				Assert.IsTrue(PathExtension.IsPathFullyQualified("C:/test.txt"));
				Assert.IsTrue(PathExtension.IsPathFullyQualified(@"C:\test"));
				Assert.IsTrue(PathExtension.IsPathFullyQualified(@"C:\test\"));
				Assert.IsTrue(PathExtension.IsPathFullyQualified(@"C:\test.txt"));
			}
			else
			{
				Assert.IsTrue(PathExtension.IsPathFullyQualified("/test"));
				Assert.IsTrue(PathExtension.IsPathFullyQualified("/test/"));
				Assert.IsTrue(PathExtension.IsPathFullyQualified("/test.txt"));
			}
		}

		#endregion
	}
}