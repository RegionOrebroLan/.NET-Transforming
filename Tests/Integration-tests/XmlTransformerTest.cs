using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using IntegrationTests.Fixtures;
using IntegrationTests.Helpers;
using RegionOrebroLan.Transforming.IO.Extensions;
using Xunit;

namespace IntegrationTests
{
	[Collection(FixtureCollection.Name)]
	public class XmlTransformerTest(Fixture fixture)
	{
		#region Fields

		private readonly Fixture _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

		#endregion

		#region Methods

		private string GetOutputPath(params string[] paths)
		{
			return this._fixture.GetOutputPath(ResolvePaths(paths));
		}

		private string GetResourcePath(params string[] paths)
		{
			return this._fixture.GetResourcePath(ResolvePaths(paths));
		}

		private static string[] ResolvePaths(params string[] paths)
		{
			return new[] { "XmlTransformerTest" }.Concat(paths).ToArray();
		}

		[Fact]
		public async Task ResourceFiles_PrerequisiteTest()
		{
			foreach(var file in Directory.GetFileSystemEntries(this.GetResourcePath("bom", "crlf"), "*.config"))
			{
				await ResourceFilesPrerequisiteTest(file, true, false, true);
			}

			foreach(var file in Directory.GetFileSystemEntries(this.GetResourcePath("bom", "lf"), "*.config"))
			{
				await ResourceFilesPrerequisiteTest(file, true, true, false);
			}

			foreach(var file in Directory.GetFileSystemEntries(this.GetResourcePath("no-bom", "crlf"), "*.config"))
			{
				await ResourceFilesPrerequisiteTest(file, false, false, true);
			}

			foreach(var file in Directory.GetFileSystemEntries(this.GetResourcePath("no-bom", "lf"), "*.config"))
			{
				await ResourceFilesPrerequisiteTest(file, false, true, false);
			}
		}

		[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Maybe we can fix this sometime.")]
		private static async Task ResourceFilesPrerequisiteTest(string file, bool hasByteOrderMark, bool hasUnixLineBreaks, bool hasWindowsLineBreaks)
		{
			using(var streamReader = new StreamReader(file, true))
			{
				Assert.Equal(hasByteOrderMark, streamReader.HasByteOrderMark());
				var content = await streamReader.ReadToEndAsync();
				/*
					Even if we try to control the line-breaks it seem to always be lf on non-windows and crlf on windows.
					- /Resources/XmlTransformerTest/bom/lf/.editorconfig
					- /Resources/XmlTransformerTest/no-bom/lf/.editorconfig
				*/
				if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					//Assert.Equal(hasUnixLineBreaks, content.HasUnixLineBreaks());
					//Assert.Equal(hasWindowsLineBreaks, content.HasWindowsLineBreaks());
					Assert.False(content.HasUnixLineBreaks());
					Assert.True(content.HasWindowsLineBreaks());
				}
				else
				{
					//Assert.Equal(hasUnixLineBreaks, content.HasUnixLineBreaks());
					//Assert.Equal(hasWindowsLineBreaks, content.HasWindowsLineBreaks());
					Assert.True(content.HasUnixLineBreaks());
					Assert.False(content.HasWindowsLineBreaks());
				}
			}
		}

		[Fact]
		public void Transform_IfTheTransformationContentIsEmpty_ShouldTransformCorrectly()
		{
			const string fileName = "Web.config";
			var destination = this.GetOutputPath(fileName);
			var source = this.GetResourcePath(fileName);
			var transformation = this.GetResourcePath("Web.No-Transformation.config");

			this._fixture.XmlTransformer.Transform(destination, source, transformation);

			var expectedContent = File.ReadAllText(this.GetResourcePath("Web.No-Transformation.Expected.config"));
			var actualContent = File.ReadAllText(destination);

			Assert.Equal(expectedContent, actualContent);
		}

		[Fact]
		public void Transform_ShouldTransformCorrectly()
		{
			const string fileName = "Web.config";
			var destination = this.GetOutputPath(fileName);
			var source = this.GetResourcePath(fileName);
			var transformation = this.GetResourcePath("Web.Transformation.config");

			this._fixture.XmlTransformer.Transform(destination, source, transformation);

			var expectedContent = File.ReadAllText(this.GetResourcePath("Web.Expected.config"));
			var actualContent = File.ReadAllText(destination);

			Assert.Equal(expectedContent, actualContent);
		}

		#endregion
	}
}