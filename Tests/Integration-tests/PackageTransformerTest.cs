using System.IO.Compression;
using System.Runtime.InteropServices;
using IntegrationTests.Fixtures;
using IntegrationTests.Helpers;
using RegionOrebroLan.Transforming.Configuration;
using RegionOrebroLan.Transforming.IO.Extensions;

namespace IntegrationTests
{
	[Collection(FixtureCollection.Name)]
	public class PackageTransformerTest(Fixture fixture)
	{
		#region Fields

		private readonly Fixture _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

		#endregion

		#region Properties

		protected internal virtual Random Random { get; } = new(DateTime.Now.Millisecond);

		#endregion

		#region Methods

		protected internal virtual IEnumerable<string> GetFileSystemEntries(string path)
		{
			return Directory.EnumerateFileSystemEntries(path, "*", SearchOption.AllDirectories).OrderBy(entry => entry, StringComparer.OrdinalIgnoreCase);
		}

		private string GetOutputPath(params string[] paths)
		{
			return this._fixture.GetOutputPath(ResolvePaths(paths));
		}

		protected internal virtual string GetRandomPackageName(string name)
		{
			return name + (this.Random.Next(0, 2) != 0 ? ".zip" : string.Empty);
		}

		private string GetResourcePath(params string[] paths)
		{
			return this._fixture.GetResourcePath(ResolvePaths(paths));
		}

		[Fact]
		public void GetSourcesForTransformation_Test()
		{
			var directoryPath = this.GetResourcePath("My.Simple.Package");

			var path = Path.Combine(directoryPath, "Web.1.2.3.4.5.config");
			var sources = this._fixture.PackageTransformer.GetSourcesForTransformation(path).ToArray();
			Assert.Equal(2, sources.Length);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.config"), sources[0]);
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), sources[1]);

			path = Path.Combine(directoryPath, "Web.1.2.3.config");
			sources = this._fixture.PackageTransformer.GetSourcesForTransformation(path).ToArray();
			Assert.Single(sources);
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), sources[0]);

			path = Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config");
			sources = this._fixture.PackageTransformer.GetSourcesForTransformation(path).ToArray();
			Assert.Single(sources);
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), sources[0]);

			path = Path.Combine(directoryPath, "Web.config");
			sources = this._fixture.PackageTransformer.GetSourcesForTransformation(path).ToArray();
			Assert.Empty(sources);

			path = Path.Combine(directoryPath, "Web.Release.config");
			sources = this._fixture.PackageTransformer.GetSourcesForTransformation(path).ToArray();
			Assert.Single(sources);
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), sources[0]);

			path = Path.Combine(directoryPath, "Web.Test.config");
			sources = this._fixture.PackageTransformer.GetSourcesForTransformation(path).ToArray();
			Assert.Single(sources);
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), sources[0]);

			path = Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config");
			sources = this._fixture.PackageTransformer.GetSourcesForTransformation(path).ToArray();
			Assert.Single(sources);
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), sources[0]);
		}

		[Fact]
		public void GetTransformInformation_Test()
		{
			var manyTransformationNames = new[] { "Release", "Test", "1.2.3.4.5", "AB.CDE.FGHI", "NON.EXISTING.DOT.NAME" };
			var noTransformationNames = Enumerable.Empty<string>().ToArray();

			var directoryPath = this.GetResourcePath("My.Package.With.Multiple.Dots");
			var fileToTransformPatterns = new[] { "**/*.config*" };
			var transformationNames = noTransformationNames;
			var transformInformation = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.Equal(2, transformInformation.Count);
			Assert.Equal(Path.Combine(directoryPath, "Views", "Web.config"), transformInformation.ElementAt(0).Key);
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(1).Key);

			transformationNames = manyTransformationNames;
			transformInformation = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.Equal(2, transformInformation.Count);
			Assert.Equal(Path.Combine(directoryPath, "Views", "Web.config"), transformInformation.ElementAt(0).Key);
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(1).Key);

			// Both back- and forward-slashes works.
			fileToTransformPatterns = ["**/*.config*", @"**\*.json", "**/*.xml"];
			transformationNames = noTransformationNames;
			transformInformation = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.Equal(8, transformInformation.Count);
			Assert.Equal(Path.Combine(directoryPath, "appsettings.json"), transformInformation.ElementAt(0).Key);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Another-json-file.json"), transformInformation.ElementAt(1).Key);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Another-xml-file.xml"), transformInformation.ElementAt(2).Key);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Json-file.json"), transformInformation.ElementAt(3).Key);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Xml-file.xml"), transformInformation.ElementAt(4).Key);
			Assert.Equal(Path.Combine(directoryPath, "Settings.For.json"), transformInformation.ElementAt(5).Key);
			Assert.Equal(Path.Combine(directoryPath, "Views", "Web.config"), transformInformation.ElementAt(6).Key);
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(7).Key);

			transformationNames = manyTransformationNames;
			transformInformation = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.Equal(8, transformInformation.Count);
			Assert.Equal(Path.Combine(directoryPath, "appsettings.json"), transformInformation.ElementAt(0).Key);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Another-json-file.json"), transformInformation.ElementAt(1).Key);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Another-xml-file.xml"), transformInformation.ElementAt(2).Key);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Json-file.json"), transformInformation.ElementAt(3).Key);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Xml-file.xml"), transformInformation.ElementAt(4).Key);
			Assert.Equal(Path.Combine(directoryPath, "Settings.For.json"), transformInformation.ElementAt(5).Key);
			Assert.Equal(Path.Combine(directoryPath, "Views", "Web.config"), transformInformation.ElementAt(6).Key);
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(7).Key);

			// Both back- and forward-slashes works.
			fileToTransformPatterns = [@"**\*.xml", "**/*.json", @"**\*.config*"];
			transformationNames = noTransformationNames;
			transformInformation = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.Equal(8, transformInformation.Count);
			Assert.Equal(Path.Combine(directoryPath, "appsettings.json"), transformInformation.ElementAt(0).Key);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Another-json-file.json"), transformInformation.ElementAt(1).Key);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Another-xml-file.xml"), transformInformation.ElementAt(2).Key);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Json-file.json"), transformInformation.ElementAt(3).Key);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Xml-file.xml"), transformInformation.ElementAt(4).Key);
			Assert.Equal(Path.Combine(directoryPath, "Settings.For.json"), transformInformation.ElementAt(5).Key);
			Assert.Equal(Path.Combine(directoryPath, "Views", "Web.config"), transformInformation.ElementAt(6).Key);
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(7).Key);

			transformationNames = manyTransformationNames;
			transformInformation = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.Equal(8, transformInformation.Count);
			Assert.Equal(Path.Combine(directoryPath, "appsettings.json"), transformInformation.ElementAt(0).Key);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Another-json-file.json"), transformInformation.ElementAt(1).Key);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Another-xml-file.xml"), transformInformation.ElementAt(2).Key);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Json-file.json"), transformInformation.ElementAt(3).Key);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Xml-file.xml"), transformInformation.ElementAt(4).Key);
			Assert.Equal(Path.Combine(directoryPath, "Settings.For.json"), transformInformation.ElementAt(5).Key);
			Assert.Equal(Path.Combine(directoryPath, "Views", "Web.config"), transformInformation.ElementAt(6).Key);
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(7).Key);

			directoryPath = this.GetResourcePath("My.Simple.Package");
			fileToTransformPatterns = ["**/*.config*"];
			transformationNames = noTransformationNames;
			transformInformation = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.Equal(2, transformInformation.Count);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.config"), transformInformation.ElementAt(0).Key);
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(1).Key);

			transformationNames = manyTransformationNames;
			transformInformation = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.Equal(2, transformInformation.Count);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.config"), transformInformation.ElementAt(0).Key);
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(1).Key);

			transformationNames = ["4.5"];
			transformInformation = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.Equal(2, transformInformation.Count);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.config"), transformInformation.ElementAt(0).Key);
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(1).Key);

			fileToTransformPatterns = ["**/*.config*", "**/*.json", "**/*.xml"];
			transformationNames = noTransformationNames;
			transformInformation = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.Equal(2, transformInformation.Count);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.config"), transformInformation.ElementAt(0).Key);
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(1).Key);

			transformationNames = manyTransformationNames;
			transformInformation = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.Equal(2, transformInformation.Count);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.config"), transformInformation.ElementAt(0).Key);
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(1).Key);

			fileToTransformPatterns = ["**/*.config*"];
			transformationNames = manyTransformationNames;
			var transformEntries = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(0).Value;
			Assert.Single(transformEntries);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(0).Key);
			Assert.True(transformEntries.ElementAt(0).Value);

			transformEntries = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(1).Value;
			Assert.Equal(6, transformEntries.Count);
			Assert.Equal(Path.Combine(directoryPath, "Web.Release.config"), transformEntries.ElementAt(0).Key);
			Assert.True(transformEntries.ElementAt(0).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.Test.config"), transformEntries.ElementAt(1).Key);
			Assert.True(transformEntries.ElementAt(1).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(2).Key);
			Assert.True(transformEntries.ElementAt(2).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config"), transformEntries.ElementAt(3).Key);
			Assert.True(transformEntries.ElementAt(3).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.config"), transformEntries.ElementAt(4).Key);
			Assert.False(transformEntries.ElementAt(4).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config"), transformEntries.ElementAt(5).Key);
			Assert.False(transformEntries.ElementAt(5).Value);

			transformationNames = noTransformationNames;
			transformEntries = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(0).Value;
			Assert.Single(transformEntries);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(0).Key);
			Assert.False(transformEntries.ElementAt(0).Value);

			transformEntries = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(1).Value;
			Assert.Equal(6, transformEntries.Count);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(0).Key);
			Assert.False(transformEntries.ElementAt(0).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.config"), transformEntries.ElementAt(1).Key);
			Assert.False(transformEntries.ElementAt(1).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config"), transformEntries.ElementAt(2).Key);
			Assert.False(transformEntries.ElementAt(2).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.Release.config"), transformEntries.ElementAt(3).Key);
			Assert.False(transformEntries.ElementAt(3).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.Test.config"), transformEntries.ElementAt(4).Key);
			Assert.False(transformEntries.ElementAt(4).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config"), transformEntries.ElementAt(5).Key);
			Assert.False(transformEntries.ElementAt(5).Value);

			transformationNames = ["4.5"];
			transformEntries = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(0).Value;
			Assert.Single(transformEntries);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(0).Key);
			Assert.True(transformEntries.ElementAt(0).Value);

			transformEntries = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(1).Value;
			Assert.Equal(6, transformEntries.Count);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(0).Key);
			Assert.True(transformEntries.ElementAt(0).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.config"), transformEntries.ElementAt(1).Key);
			Assert.False(transformEntries.ElementAt(1).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config"), transformEntries.ElementAt(2).Key);
			Assert.False(transformEntries.ElementAt(2).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.Release.config"), transformEntries.ElementAt(3).Key);
			Assert.False(transformEntries.ElementAt(3).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.Test.config"), transformEntries.ElementAt(4).Key);
			Assert.False(transformEntries.ElementAt(4).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config"), transformEntries.ElementAt(5).Key);
			Assert.False(transformEntries.ElementAt(5).Value);

			transformationNames = ["Test"];
			transformEntries = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(0).Value;
			Assert.Single(transformEntries);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(0).Key);
			Assert.False(transformEntries.ElementAt(0).Value);

			transformEntries = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(1).Value;
			Assert.Equal(6, transformEntries.Count);
			Assert.Equal(Path.Combine(directoryPath, "Web.Test.config"), transformEntries.ElementAt(0).Key);
			Assert.True(transformEntries.ElementAt(0).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(1).Key);
			Assert.False(transformEntries.ElementAt(1).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.config"), transformEntries.ElementAt(2).Key);
			Assert.False(transformEntries.ElementAt(2).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config"), transformEntries.ElementAt(3).Key);
			Assert.False(transformEntries.ElementAt(3).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.Release.config"), transformEntries.ElementAt(4).Key);
			Assert.False(transformEntries.ElementAt(4).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config"), transformEntries.ElementAt(5).Key);
			Assert.False(transformEntries.ElementAt(5).Value);

			transformationNames = ["Test", "AB.CDE.FGHI"];
			transformEntries = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(0).Value;
			Assert.Single(transformEntries);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(0).Key);
			Assert.False(transformEntries.ElementAt(0).Value);

			transformEntries = this._fixture.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(1).Value;
			Assert.Equal(6, transformEntries.Count);
			Assert.Equal(Path.Combine(directoryPath, "Web.Test.config"), transformEntries.ElementAt(0).Key);
			Assert.True(transformEntries.ElementAt(0).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config"), transformEntries.ElementAt(1).Key);
			Assert.True(transformEntries.ElementAt(1).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(2).Key);
			Assert.False(transformEntries.ElementAt(2).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.config"), transformEntries.ElementAt(3).Key);
			Assert.False(transformEntries.ElementAt(3).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.Release.config"), transformEntries.ElementAt(4).Key);
			Assert.False(transformEntries.ElementAt(4).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config"), transformEntries.ElementAt(5).Key);
			Assert.False(transformEntries.ElementAt(5).Value);
		}

		[Fact]
		public void GetTransformMap_Test()
		{
			var directoryPath = this.GetResourcePath("My.Package.With.Multiple.Dots");
			var fileToTransformPatterns = new[] { "**/*.config*" };
			var transformMap = this._fixture.PackageTransformer.GetTransformMap(directoryPath, fileToTransformPatterns);
			Assert.Equal(2, transformMap.Count);
			Assert.Equal(Path.Combine(directoryPath, "Views", "Web.config"), transformMap.ElementAt(0).Key);
			Assert.Equal(4, transformMap.ElementAt(0).Value.Count);
			Assert.Equal(Path.Combine(directoryPath, "Views", "Web.1.2.3.4.5.config"), transformMap.ElementAt(0).Value.ElementAt(0));
			Assert.Equal(Path.Combine(directoryPath, "Views", "Web.AB.CDE.FGHI.config"), transformMap.ElementAt(0).Value.ElementAt(1));
			Assert.Equal(Path.Combine(directoryPath, "Views", "Web.Release.config"), transformMap.ElementAt(0).Value.ElementAt(2));
			Assert.Equal(Path.Combine(directoryPath, "Views", "Web.Test.config"), transformMap.ElementAt(0).Value.ElementAt(3));
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), transformMap.ElementAt(1).Key);
			Assert.Equal(5, transformMap.ElementAt(1).Value.Count);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformMap.ElementAt(1).Value.ElementAt(0));
			Assert.Equal(Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config"), transformMap.ElementAt(1).Value.ElementAt(1));
			Assert.Equal(Path.Combine(directoryPath, "Web.Release.config"), transformMap.ElementAt(1).Value.ElementAt(2));
			Assert.Equal(Path.Combine(directoryPath, "Web.Test.config"), transformMap.ElementAt(1).Value.ElementAt(3));
			Assert.Equal(Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config"), transformMap.ElementAt(1).Value.ElementAt(4));

			fileToTransformPatterns = ["**/*.config*", "**/*.json", "**/*.xml"];
			transformMap = this._fixture.PackageTransformer.GetTransformMap(directoryPath, fileToTransformPatterns);
			Assert.Equal(8, transformMap.Count);
			Assert.Equal(Path.Combine(directoryPath, "appsettings.json"), transformMap.ElementAt(0).Key);
			Assert.Equal(4, transformMap.ElementAt(0).Value.Count);
			Assert.Equal(Path.Combine(directoryPath, "appsettings.1.2.3.4.5.json"), transformMap.ElementAt(0).Value.ElementAt(0));
			Assert.Equal(Path.Combine(directoryPath, "appsettings.AB.CDE.FGHI.json"), transformMap.ElementAt(0).Value.ElementAt(1));
			Assert.Equal(Path.Combine(directoryPath, "appsettings.Release.json"), transformMap.ElementAt(0).Value.ElementAt(2));
			Assert.Equal(Path.Combine(directoryPath, "appsettings.Test.json"), transformMap.ElementAt(0).Value.ElementAt(3));
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Another-json-file.json"), transformMap.ElementAt(1).Key);
			Assert.Empty(transformMap.ElementAt(1).Value);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Another-xml-file.xml"), transformMap.ElementAt(2).Key);
			Assert.Empty(transformMap.ElementAt(2).Value);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Json-file.json"), transformMap.ElementAt(3).Key);
			Assert.Equal(2, transformMap.ElementAt(3).Value.Count);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Json-file.Dummy.json"), transformMap.ElementAt(3).Value.ElementAt(0));
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Json-file.Test.json"), transformMap.ElementAt(3).Value.ElementAt(1));
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Xml-file.xml"), transformMap.ElementAt(4).Key);
			Assert.Single(transformMap.ElementAt(4).Value);
			Assert.Equal(Path.Combine(directoryPath, "Directory", "Xml-file.Release.xml"), transformMap.ElementAt(4).Value.ElementAt(0));
			Assert.Equal(Path.Combine(directoryPath, "Settings.For.json"), transformMap.ElementAt(5).Key);
			Assert.Equal(4, transformMap.ElementAt(5).Value.Count);
			Assert.Equal(Path.Combine(directoryPath, "Settings.For.1.2.3.4.5.json"), transformMap.ElementAt(5).Value.ElementAt(0));
			Assert.Equal(Path.Combine(directoryPath, "Settings.For.AB.CDE.FGHI.json"), transformMap.ElementAt(5).Value.ElementAt(1));
			Assert.Equal(Path.Combine(directoryPath, "Settings.For.Release.json"), transformMap.ElementAt(5).Value.ElementAt(2));
			Assert.Equal(Path.Combine(directoryPath, "Settings.For.Test.json"), transformMap.ElementAt(5).Value.ElementAt(3));
			Assert.Equal(Path.Combine(directoryPath, "Views", "Web.config"), transformMap.ElementAt(6).Key);
			Assert.Equal(4, transformMap.ElementAt(6).Value.Count);
			Assert.Equal(Path.Combine(directoryPath, "Views", "Web.1.2.3.4.5.config"), transformMap.ElementAt(6).Value.ElementAt(0));
			Assert.Equal(Path.Combine(directoryPath, "Views", "Web.AB.CDE.FGHI.config"), transformMap.ElementAt(6).Value.ElementAt(1));
			Assert.Equal(Path.Combine(directoryPath, "Views", "Web.Release.config"), transformMap.ElementAt(6).Value.ElementAt(2));
			Assert.Equal(Path.Combine(directoryPath, "Views", "Web.Test.config"), transformMap.ElementAt(6).Value.ElementAt(3));
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), transformMap.ElementAt(7).Key);
			Assert.Equal(5, transformMap.ElementAt(7).Value.Count);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformMap.ElementAt(7).Value.ElementAt(0));
			Assert.Equal(Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config"), transformMap.ElementAt(7).Value.ElementAt(1));
			Assert.Equal(Path.Combine(directoryPath, "Web.Release.config"), transformMap.ElementAt(7).Value.ElementAt(2));
			Assert.Equal(Path.Combine(directoryPath, "Web.Test.config"), transformMap.ElementAt(7).Value.ElementAt(3));
			Assert.Equal(Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config"), transformMap.ElementAt(7).Value.ElementAt(4));

			directoryPath = this.GetResourcePath("My.Simple.Package");
			fileToTransformPatterns = ["**/*.config*"];
			transformMap = this._fixture.PackageTransformer.GetTransformMap(directoryPath, fileToTransformPatterns);
			Assert.Equal(2, transformMap.Count);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.config"), transformMap.ElementAt(0).Key);
			Assert.Single(transformMap.ElementAt(0).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformMap.ElementAt(0).Value.ElementAt(0));
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), transformMap.ElementAt(1).Key);
			Assert.Equal(6, transformMap.ElementAt(1).Value.Count);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformMap.ElementAt(1).Value.ElementAt(0));
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.config"), transformMap.ElementAt(1).Value.ElementAt(1));
			Assert.Equal(Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config"), transformMap.ElementAt(1).Value.ElementAt(2));
			Assert.Equal(Path.Combine(directoryPath, "Web.Release.config"), transformMap.ElementAt(1).Value.ElementAt(3));
			Assert.Equal(Path.Combine(directoryPath, "Web.Test.config"), transformMap.ElementAt(1).Value.ElementAt(4));
			Assert.Equal(Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config"), transformMap.ElementAt(1).Value.ElementAt(5));

			fileToTransformPatterns = ["**/*.config*", "**/*.json", "**/*.xml"];
			transformMap = this._fixture.PackageTransformer.GetTransformMap(directoryPath, fileToTransformPatterns);
			Assert.Equal(2, transformMap.Count);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.config"), transformMap.ElementAt(0).Key);
			Assert.Single(transformMap.ElementAt(0).Value);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformMap.ElementAt(0).Value.ElementAt(0));
			Assert.Equal(Path.Combine(directoryPath, "Web.config"), transformMap.ElementAt(1).Key);
			Assert.Equal(6, transformMap.ElementAt(1).Value.Count);
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformMap.ElementAt(1).Value.ElementAt(0));
			Assert.Equal(Path.Combine(directoryPath, "Web.1.2.3.config"), transformMap.ElementAt(1).Value.ElementAt(1));
			Assert.Equal(Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config"), transformMap.ElementAt(1).Value.ElementAt(2));
			Assert.Equal(Path.Combine(directoryPath, "Web.Release.config"), transformMap.ElementAt(1).Value.ElementAt(3));
			Assert.Equal(Path.Combine(directoryPath, "Web.Test.config"), transformMap.ElementAt(1).Value.ElementAt(4));
			Assert.Equal(Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config"), transformMap.ElementAt(1).Value.ElementAt(5));
		}

		private static string GetUniquelySuffixedValue(string name)
		{
			return $"{name}-{Guid.NewGuid().ToString().Replace("-", string.Empty)}";
		}

		[Fact]
		public void Prerequisite_ZipFile_ExtractToDirectory_WorksDifferentlyDependingOnTargetFramework_Test()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName(Guid.NewGuid().ToString()));
			var source = this.GetResourcePath("Empty.zip");
			ZipFile.ExtractToDirectory(source, destination);

#if NETFRAMEWORK
			Assert.True(Directory.Exists(destination));
#else
			Assert.False(Directory.Exists(destination));
#endif
		}

		private static string[] ResolvePaths(params string[] paths)
		{
			return new[] { "PackageTransformerTest" }.Concat(paths).ToArray();
		}

		[Fact]
		public void Transform_IfTheDestinationDirectoryContainsADot_ShouldTransformCorrectly()
		{
			var destination = this.GetOutputPath("My.Transformed-Package");
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var pathToDeletePatterns = new[] { "**/Directory-To-Delete/*", "**/File-To-Delete.*" };
			var source = this.GetResourcePath(this.GetRandomPackageName("Package"));
			var transformationNames = new[] { "Release", "Test" };

			this._fixture.PackageTransformer.Transform(destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames);

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);
				destination = extractedDestination;
			}

			var expected = this.GetResourcePath("Package-Expected");

			var actualItems = this.GetFileSystemEntries(destination).ToArray();
			var expectedItems = this.GetFileSystemEntries(expected).ToArray();

			Assert.True(actualItems.SequenceEqual(expectedItems, new FileComparer(destination, expected)));
		}

		[Fact]
		public void Transform_IfTheOptionsAreSetToAvoidBom_And_IfTheSourceFilesHaveABom_ShouldResultInADestinationWithNoBomRegardingFilesInvolvedInTheTransformPattern()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName(GetUniquelySuffixedValue("Transformed-Package")));
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var pathToDeletePatterns = new[] { "**/Directory-To-Delete/*", "**/File-To-Delete.*" };
			var source = this.GetResourcePath("bom", this.GetRandomPackageName("Package"));
			var transformationNames = new[] { "Release", "Test" };

			this._fixture.PackageTransformer.Transform(destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames, new TransformingOptions { File = new FileTransformingOptions { AvoidByteOrderMark = true } });

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);
				destination = extractedDestination;
			}

			var sourceFiles = Directory.GetFiles(this.GetResourcePath("bom", "Package"), "*", SearchOption.AllDirectories).ToArray();

			foreach(var file in sourceFiles)
			{
				using(var streamReader = new StreamReader(file, true))
				{
					Assert.True(streamReader.HasByteOrderMark());
				}
			}

			var destinationFiles = Directory.GetFiles(destination, "*", SearchOption.AllDirectories).ToArray();

			foreach(var file in destinationFiles)
			{
				using(var streamReader = new StreamReader(file, true))
				{
					Assert.False(streamReader.HasByteOrderMark());
				}
			}
		}

		[Fact]
		public void Transform_IfTheOptionsAreSetToExplicitReplace_ShouldWorkProperly()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName(GetUniquelySuffixedValue("Transformed-Package")));
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var pathToDeletePatterns = new[] { "**/Directory-To-Delete/*", "**/File-To-Delete.*" };
			var source = this.GetResourcePath(this.GetRandomPackageName("Package"));
			var transformationNames = new[] { "Release", "Test" };

			var options = new TransformingOptions();
			options.File.Replacement.Enabled = true;
			options.File.Replacement.Replace = value => value?.Replace("\r\n", "\n").Replace("\n", "\t");

			this._fixture.PackageTransformer.Transform(destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames, options);

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);
				destination = extractedDestination;
			}

			var destinationFiles = Directory.GetFiles(destination, "*", SearchOption.AllDirectories).ToArray();

			foreach(var file in destinationFiles)
			{
				if(File.ReadAllLines(file).Length <= 1)
					continue;

				var content = File.ReadAllText(file);

				Assert.DoesNotContain("\r\n", content);
				Assert.DoesNotContain("\n", content);
				Assert.Contains("\t", content);
			}
		}

		[Fact]
		public void Transform_IfTheOptionsAreSetToNotAvoidBom_And_IfTheSourceFilesHaveABom_ShouldResultInADestinationWithBomRegardingFilesInvolvedInTheTransformPattern()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName(GetUniquelySuffixedValue("Transformed-Package")));
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var pathToDeletePatterns = new[] { "**/Directory-To-Delete/*", "**/File-To-Delete.*" };
			var source = this.GetResourcePath("bom", this.GetRandomPackageName("Package"));
			var transformationNames = new[] { "Release", "Test" };

			this._fixture.PackageTransformer.Transform(destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames, new TransformingOptions { File = new FileTransformingOptions { AvoidByteOrderMark = false } });

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);
				destination = extractedDestination;
			}

			var sourceFiles = Directory.GetFiles(this.GetResourcePath("bom", "Package"), "*", SearchOption.AllDirectories).ToArray();

			foreach(var file in sourceFiles)
			{
				using(var streamReader = new StreamReader(file, true))
				{
					Assert.True(streamReader.HasByteOrderMark());
				}
			}

			var destinationFiles = Directory.GetFiles(destination, "*", SearchOption.AllDirectories).ToArray();

			foreach(var file in destinationFiles)
			{
				using(var streamReader = new StreamReader(file, true))
				{
					Assert.True(streamReader.HasByteOrderMark());
				}
			}
		}

		[Fact]
		public void Transform_IfTheOptionsAreSetToReplace_ShouldWorkProperlyOnAllPlatforms()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName(GetUniquelySuffixedValue("Transformed-Package")));
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var pathToDeletePatterns = new[] { "**/Directory-To-Delete/*", "**/File-To-Delete.*" };
			var source = this.GetResourcePath(this.GetRandomPackageName("Package"));
			var transformationNames = new[] { "Release", "Test" };

			var options = new TransformingOptions();
			options.File.Replacement.Enabled = true;

			this._fixture.PackageTransformer.Transform(destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames, options);

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);
				destination = extractedDestination;
			}

			var sourceFiles = Directory.GetFiles(this.GetResourcePath("Package"), "*", SearchOption.AllDirectories).ToArray();

			foreach(var file in sourceFiles)
			{
				if(File.ReadAllLines(file).Length <= 1)
					continue;

				var content = File.ReadAllText(file);

				Assert.Contains("\r\n", content);
			}

			var destinationFiles = Directory.GetFiles(destination, "*", SearchOption.AllDirectories).ToArray();

			foreach(var file in destinationFiles)
			{
				if(File.ReadAllLines(file).Length <= 1)
					continue;

				var content = File.ReadAllText(file);

				if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					Assert.Contains("\r\n", content);
				}
				else
				{
					Assert.DoesNotContain("\r\n", content);
					Assert.Contains("\n", content);
				}
			}
		}

		[Fact]
		public void Transform_IfThePathToDeletePatternsContainsAWholeDirectoryWithWildcards_ShouldTransformCorrectly()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName(GetUniquelySuffixedValue("Transformed-Package")));
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var pathToDeletePatterns = new[] { "Directory/Directory-To-Delete/**/*", "Directory/File-To-Delete.text", "File-To-Delete.txt" };
			var source = this.GetResourcePath(this.GetRandomPackageName("Package"));
			var transformationNames = new[] { "Release", "Test" };

			this._fixture.PackageTransformer.Transform(destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames);

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);
				destination = extractedDestination;
			}

			var expected = this.GetResourcePath("Package-Expected");

			var actualItems = this.GetFileSystemEntries(destination).ToArray();
			var expectedItems = this.GetFileSystemEntries(expected).ToArray();

			Assert.True(actualItems.SequenceEqual(expectedItems, new FileComparer(destination, expected)));
		}

		[Fact]
		public void Transform_IfThePathToDeletePatternsIncludeAllEntries_ShouldTransformCorrectly()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName(GetUniquelySuffixedValue("Transformed-Package")));
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var pathToDeletePatterns = new[] { "Directory/**/*", "Views/**/*", "appsettings.json", "File-To-Delete.txt", "Web.config" };
			var source = this.GetResourcePath(this.GetRandomPackageName("Package"));
			var transformationNames = new[] { "Release", "Test" };

			this._fixture.PackageTransformer.Transform(destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames);

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);

				// To handle NET 5.0 and NET Core 3.1.
				// Extracting an empty archive does not create an empty directory.
#if !NETFRAMEWORK
				if(!Directory.Exists(extractedDestination))
					Directory.CreateDirectory(extractedDestination);
#endif

				destination = extractedDestination;
			}

			Assert.Empty(this.GetFileSystemEntries(destination));
		}

		[Fact]
		public void Transform_IfTheSourceDirectoryContainsADot_ShouldTransformCorrectly()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName(GetUniquelySuffixedValue("Transformed-Package")));
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var pathToDeletePatterns = new[] { "**/Directory-To-Delete/*", "**/File-To-Delete.*" };
			var source = this.GetResourcePath("My.Package");
			var transformationNames = new[] { "Release", "Test" };

			this._fixture.PackageTransformer.Transform(destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames);

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);
				destination = extractedDestination;
			}

			var expected = this.GetResourcePath("Package-Expected");

			var actualItems = this.GetFileSystemEntries(destination).ToArray();
			var expectedItems = this.GetFileSystemEntries(expected).ToArray();

			Assert.True(actualItems.SequenceEqual(expectedItems, new FileComparer(destination, expected)));
		}

		[Fact]
		public void Transform_IfTheSourceDirectoryContainsMultipleDotsAndTransformationNamesContainsDots_ShouldTransformCorrectly()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName(GetUniquelySuffixedValue("Transformed-Package")));
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var pathToDeletePatterns = new[] { "**/Directory-To-Delete/*", "**/File-To-Delete.*" };
			var source = this.GetResourcePath("My.Package.With.Multiple.Dots");
			var transformationNames = new[] { "Release", "Test", "1.2.3.4.5", "AB.CDE.FGHI", "NON.EXISTING.DOT.NAME" };

			this._fixture.PackageTransformer.Transform(destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames);

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);
				destination = extractedDestination;
			}

			var expected = this.GetResourcePath("My.Package.With.Multiple.Dots-Expected");

			var actualItems = this.GetFileSystemEntries(destination).ToArray();
			var expectedItems = this.GetFileSystemEntries(expected).ToArray();

			Assert.True(actualItems.SequenceEqual(expectedItems, new FileComparer(destination, expected)));
		}

		[Fact]
		public void Transform_IfTheSourceFilesHaveABom_ShouldResultInADestinationWithBomOnWindows_But_ShouldResultInADestinationWithNoBomOnNotWindows_RegardingFilesInvolvedInTheTransformPattern()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName(GetUniquelySuffixedValue("Transformed-Package")));
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var pathToDeletePatterns = new[] { "**/Directory-To-Delete/*", "**/File-To-Delete.*" };
			var source = this.GetResourcePath("bom", this.GetRandomPackageName("Package"));
			var transformationNames = new[] { "Release", "Test" };

			this._fixture.PackageTransformer.Transform(destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames);

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);
				destination = extractedDestination;
			}

			var sourceFiles = Directory.GetFiles(this.GetResourcePath("bom", "Package"), "*", SearchOption.AllDirectories).ToArray();

			foreach(var file in sourceFiles)
			{
				using(var streamReader = new StreamReader(file, true))
				{
					Assert.True(streamReader.HasByteOrderMark());
				}
			}

			var destinationFiles = Directory.GetFiles(destination, "*", SearchOption.AllDirectories).ToArray();

			foreach(var file in destinationFiles)
			{
				using(var streamReader = new StreamReader(file, true))
				{
					if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
						Assert.True(streamReader.HasByteOrderMark());
					else
						Assert.False(streamReader.HasByteOrderMark());
				}
			}
		}

		[Fact]
		public void Transform_IfTheSourceFilesHaveNoBom_ShouldResultInADestinationWithNoBomRegardingFilesInvolvedInTheTransformPattern()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName(GetUniquelySuffixedValue("Transformed-Package")));
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var pathToDeletePatterns = new[] { "**/Directory-To-Delete/*", "**/File-To-Delete.*" };
			var source = this.GetResourcePath("no-bom", this.GetRandomPackageName("Package"));
			var transformationNames = new[] { "Release", "Test" };

			this._fixture.PackageTransformer.Transform(destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames);

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);
				destination = extractedDestination;
			}

			var sourceFiles = Directory.GetFiles(this.GetResourcePath("no-bom", "Package"), "*", SearchOption.AllDirectories).ToArray();

			foreach(var file in sourceFiles)
			{
				using(var streamReader = new StreamReader(file, true))
				{
					Assert.False(streamReader.HasByteOrderMark());
				}
			}

			var destinationFiles = Directory.GetFiles(destination, "*", SearchOption.AllDirectories).ToArray();

			foreach(var file in destinationFiles)
			{
				using(var streamReader = new StreamReader(file, true))
				{
					Assert.False(streamReader.HasByteOrderMark());
				}
			}
		}

		[Fact]
		public void Transform_IfTheSourceIsEmpty_ShouldCreateAnEmptyPackageAtTheDestination()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName(GetUniquelySuffixedValue("Transformed-Empty-Package")));
			var source = this.GetResourcePath(this.GetRandomPackageName("Empty"));

			this._fixture.PackageTransformer.Transform(destination, null, null, source, null);

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);

				// To handle NET 5.0 and NET Core 3.1.
				// Extracting an empty archive does not create an empty directory.
#if !NETFRAMEWORK
				if(!Directory.Exists(extractedDestination))
					Directory.CreateDirectory(extractedDestination);
#endif

				destination = extractedDestination;
			}

			var actualItems = this.GetFileSystemEntries(destination).ToArray();
			var expectedItems = this.GetFileSystemEntries(this.GetResourcePath("Empty")).ToArray();

			source = this.GetResourcePath("Empty");

			Assert.True(actualItems.SequenceEqual(expectedItems, new FileComparer(destination, source)));
		}

		[Fact]
		public void Transform_IfTheSourceParameterDoesNotExistAsDirectoryOrZipFile_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("source", () => this._fixture.PackageTransformer.Transform(this.GetOutputPath("Transformed-directory"), null, null, "File.txt", null));
		}

		[Fact]
		public void Transform_ShouldTransformCorrectly()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName(GetUniquelySuffixedValue("Transformed-Package")));
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var pathToDeletePatterns = new[] { "**/Directory-To-Delete/*", "**/File-To-Delete.*" };
			var source = this.GetResourcePath(this.GetRandomPackageName("Package"));
			var transformationNames = new[] { "Release", "Test" };

			this._fixture.PackageTransformer.Transform(destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames);

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);
				destination = extractedDestination;
			}

			var expected = this.GetResourcePath("Package-Expected");

			var actualItems = this.GetFileSystemEntries(destination).ToArray();
			var expectedItems = this.GetFileSystemEntries(expected).ToArray();

			Assert.True(actualItems.SequenceEqual(expectedItems, new FileComparer(destination, expected)));
		}

		[Fact]
		public void Transform_ShouldTransformWithTheTransformationNamesInTheDeclaredOrderAndNotAlphabetically()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName(GetUniquelySuffixedValue("Transformed-Package")));
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var source = this.GetResourcePath(this.GetRandomPackageName("Alphabetical-Test"));
			var transformationNames = new[] { "C", "A", "B" };

			this._fixture.PackageTransformer.Transform(destination, fileToTransformPatterns, null, source, transformationNames);

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);
				destination = extractedDestination;
			}

			var expected = this.GetResourcePath("Alphabetical-Test-Expected");

			var actualItems = this.GetFileSystemEntries(destination).ToArray();
			var expectedItems = this.GetFileSystemEntries(expected).ToArray();

			Assert.True(actualItems.SequenceEqual(expectedItems, new FileComparer(destination, expected)));
		}

		[Fact]
		public void ValidateFilePath_IfTheDirectoryIsNotAnAncestorOfTheFile_ShouldThrowAnInvalidOperationException()
		{
			const string action = "test";
			var directoryPaths = new List<string>();
			var exceptions = new List<InvalidOperationException>();
			var filePaths = new List<string>();

			if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				directoryPaths.AddRange([@"C:\Some-directory", @"C:\Some-directory\"]);
				filePaths.AddRange([@"C:\Some-other-directory\Some-file.txt", @"C:\Some-other-directory\", @"C:\Some-other-directory"]);
			}
			else
			{
				directoryPaths.AddRange(["/Some-directory", @"/Some-directory/"]);
				filePaths.AddRange(["/Some-other-directory/Some-file.txt", "/Some-other-directory/", "/Some-other-directory"]);
			}

			foreach(var directoryPath in directoryPaths)
			{
				foreach(var filePath in filePaths)
				{
					try
					{
						this._fixture.PackageTransformer.ValidateFilePath(action, directoryPath, filePath);
					}
					catch(InvalidOperationException invalidOperationException)
					{
						if(string.Equals(invalidOperationException.Message, $"It is not allowed to {action} the file \"{filePath}\". The file is outside the directory-path \"{directoryPath}\".", StringComparison.Ordinal))
							exceptions.Add(invalidOperationException);
					}
				}
			}

			Assert.Throws<InvalidOperationException>(() =>
			{
				if(exceptions.Count == 6)
					throw exceptions.First();
			});
		}

		[Fact]
		public void ValidateFilePath_IfTheDirectoryPathIsNull_And_IfTheFilePathIsAbsolute_ShouldThrowAnArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("directoryPath", () => { this._fixture.PackageTransformer.ValidateFilePath("Test", null, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"C:\Test.txt" : "/Test.txt"); });
		}

		[Fact]
		public void ValidateFilePath_IfTheDirectoryPathIsRelative_ShouldThrowAnArgumentException()
		{
			var exceptions = new List<ArgumentException>();
			var directoryPaths = new List<string>();
			var expectedNumberOfExceptions = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 5 : 2;
			string filePath;

			if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				directoryPaths.AddRange(["Directory", "/Directory", "/Directory/", @"\Directory", @"\Directory\"]);
				filePath = @"C:\Test.txt";
			}
			else
			{
				directoryPaths.AddRange(["Directory", "Directory/"]);
				filePath = "/Test.txt";
			}

			foreach(var directoryPath in directoryPaths)
			{
				try
				{
					this._fixture.PackageTransformer.ValidateFilePath("Test", directoryPath, filePath);
				}
				catch(ArgumentException argumentException)
				{
					if(string.Equals(argumentException.ParamName, nameof(directoryPath), StringComparison.Ordinal) && argumentException.Message.StartsWith($"The directory-path can not be relative ({directoryPath}).", StringComparison.Ordinal))
						exceptions.Add(argumentException);
				}
			}

			Assert.Throws<ArgumentException>("directoryPath", () =>
			{
				if(exceptions.Count == expectedNumberOfExceptions)
					throw exceptions.First();
			});
		}

		[Fact]
		public async Task ValidateFilePath_IfTheFilePathParameterIsARelativePath_ShouldNotThrowAnException()
		{
			await this.ValidateFilePathShouldNotThrowAnException("Text.txt");
			await this.ValidateFilePathShouldNotThrowAnException("./Text.txt");
			await this.ValidateFilePathShouldNotThrowAnException("../Text.txt");
			await this.ValidateFilePathShouldNotThrowAnException("Directory/Text.txt");
			await this.ValidateFilePathShouldNotThrowAnException("./Directory/Text.txt");
			await this.ValidateFilePathShouldNotThrowAnException("../Directory/Text.txt");

			if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				await this.ValidateFilePathShouldNotThrowAnException("/Text.txt");
				await this.ValidateFilePathShouldNotThrowAnException(@"\Text.txt");
				await this.ValidateFilePathShouldNotThrowAnException(@"Directory\Text.txt");
				await this.ValidateFilePathShouldNotThrowAnException("/Directory/Text.txt");
				await this.ValidateFilePathShouldNotThrowAnException(@"\Directory\Text.txt");
			}
		}

		protected internal virtual async Task ValidateFilePathShouldNotThrowAnException(string filePath)
		{
			await Task.CompletedTask;

			this._fixture.PackageTransformer.ValidateFilePath("Test", RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"C:\Some-directory" : "/Some-directory", filePath);
		}

		#endregion
	}
}