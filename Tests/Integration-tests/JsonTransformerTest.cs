using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using IntegrationTests.Fixtures;
using IntegrationTests.Helpers;
using RegionOrebroLan.Transforming.Configuration;
using RegionOrebroLan.Transforming.IO.Extensions;
using Xunit;

namespace IntegrationTests
{
	[Collection(FixtureCollection.Name)]
	public class JsonTransformerTest(Fixture fixture)
	{
		#region Fields

		private readonly Fixture _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

		#endregion

		#region Methods

		private string GetBomCrlfResourcePath(params string[] paths)
		{
			return this._fixture.GetResourcePath(ResolvePaths(new[] { "bom", "crlf" }.Concat(paths).ToArray()));
		}

		//private string GetBomLfResourcePath(params string[] paths)
		//{
		//	return this._fixture.GetResourcePath(ResolvePaths(new[] { "bom", "lf" }.Concat(paths).ToArray()));
		//}

		private string GetNoBomCrlfResourcePath(params string[] paths)
		{
			return this._fixture.GetResourcePath(ResolvePaths(new[] { "no-bom", "crlf" }.Concat(paths).ToArray()));
		}

		//private string GetNoBomLfResourcePath(params string[] paths)
		//{
		//	return this._fixture.GetResourcePath(ResolvePaths(new[] { "no-bom", "lf" }.Concat(paths).ToArray()));
		//}

		private string GetOutputPath(params string[] paths)
		{
			return this._fixture.GetOutputPath(ResolvePaths(paths));
		}

		private string GetResourcePath(params string[] paths)
		{
			return this._fixture.GetResourcePath(ResolvePaths(paths));
		}

		private static string GetUniqueSuffix()
		{
			return $"-{Guid.NewGuid().ToString().Replace("-", string.Empty)}";
		}

		private static string[] ResolvePaths(params string[] paths)
		{
			return new[] { "JsonTransformerTest" }.Concat(paths).ToArray();
		}

		[Fact]
		public async Task ResourceFiles_PrerequisiteTest()
		{
			foreach(var file in Directory.GetFileSystemEntries(this.GetResourcePath("bom", "crlf"), "*.json"))
			{
				await ResourceFilesPrerequisiteTest(file, true, false, true);
			}

			foreach(var file in Directory.GetFileSystemEntries(this.GetResourcePath("bom", "lf"), "*.json"))
			{
				await ResourceFilesPrerequisiteTest(file, true, true, false);
			}

			foreach(var file in Directory.GetFileSystemEntries(this.GetResourcePath("no-bom", "crlf"), "*.json"))
			{
				await ResourceFilesPrerequisiteTest(file, false, false, true);
			}

			foreach(var file in Directory.GetFileSystemEntries(this.GetResourcePath("no-bom", "lf"), "*.json"))
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
					- /Resources/JsonTransformerTest/bom/lf/.editorconfig
					- /Resources/JsonTransformerTest/no-bom/lf/.editorconfig
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
		public void Transform_IfTheSourceHasABom_And_IfTheOptionsAreDefault_ShouldResultInADestinationWithABomOnWindows_And_ShouldResultInADestinationWithNoBomOnNonWindows()
		{
			var destination = this.GetOutputPath($"appsettings{GetUniqueSuffix()}.json");
			var source = this.GetBomCrlfResourcePath("appsettings.json");
			var transformation = this.GetBomCrlfResourcePath("appsettings.Transformation.json");

			this._fixture.JsonTransformer.Transform(destination, source, transformation);

			using(var streamReader = new StreamReader(destination, true))
			{
				if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
					Assert.True(streamReader.HasByteOrderMark());
				else
					Assert.False(streamReader.HasByteOrderMark());
			}
		}

		[Fact]
		public void Transform_IfTheSourceHasABom_And_IfTheOptionsAreSetToAvoidBom_ShouldResultInADestinationWithNoBom()
		{
			var destination = this.GetOutputPath($"appsettings{GetUniqueSuffix()}.json");
			var source = this.GetBomCrlfResourcePath("appsettings.json");
			var transformation = this.GetBomCrlfResourcePath("appsettings.Transformation.json");

			this._fixture.JsonTransformer.Transform(destination, source, transformation, new FileTransformingOptions { AvoidByteOrderMark = true });

			using(var streamReader = new StreamReader(destination, true))
			{
				Assert.False(streamReader.HasByteOrderMark());
			}
		}

		[Fact]
		public void Transform_IfTheSourceHasNoBom_ShouldResultInADestinationWithNoBom()
		{
			var destination = this.GetOutputPath($"appsettings{GetUniqueSuffix()}.json");
			var source = this.GetNoBomCrlfResourcePath("appsettings.json");
			var transformation = this.GetBomCrlfResourcePath("appsettings.Transformation.json");

			this._fixture.JsonTransformer.Transform(destination, source, transformation);

			using(var streamReader = new StreamReader(destination, true))
			{
				Assert.False(streamReader.HasByteOrderMark());
			}
		}

		[Fact]
		public void Transform_IfTheTransformationContentIsEmpty_ShouldTransformCorrectly()
		{
			var destination = this.GetOutputPath($"appsettings{GetUniqueSuffix()}.json");
			var source = this.GetResourcePath("appsettings.json");
			var transformation = this.GetResourcePath("appsettings.No-Transformation.json");

			this._fixture.JsonTransformer.Transform(destination, source, transformation);

			var expectedContent = File.ReadAllText(this.GetResourcePath("appsettings.No-Transformation.Expected.json"));
			var actualContent = File.ReadAllText(destination);

			Assert.Equal(expectedContent, actualContent);
		}

		[Fact]
		public void Transform_ShouldTransformCorrectly()
		{
			var destination = this.GetOutputPath($"appsettings{GetUniqueSuffix()}.json");
			var source = this.GetResourcePath("appsettings.json");
			var transformation = this.GetResourcePath("appsettings.Transformation.json");

			this._fixture.JsonTransformer.Transform(destination, source, transformation);

			var expectedContent = File.ReadAllText(this.GetResourcePath("appsettings.Expected.json"));
			var actualContent = File.ReadAllText(destination);

			Assert.Equal(expectedContent, actualContent);
		}

		#endregion
	}
}