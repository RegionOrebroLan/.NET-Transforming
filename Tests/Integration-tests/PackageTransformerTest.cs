using System.IO.Compression;
using System.Runtime.InteropServices;
using IntegrationTests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Transforming;
using RegionOrebroLan.Transforming.IO;
using RegionOrebroLan.Transforming.Runtime;

namespace IntegrationTests
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
					var loggerFactory = new NullLoggerFactory();

					_packageTransformer = new PackageTransformer(new FileSearcher(), fileSystem, new FileTransformerFactory(fileSystem, loggerFactory, new Platform()), loggerFactory, new PackageHandlerLoader(fileSystem));
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
		public void GetSourcesForTransformation_Test()
		{
			var directoryPath = this.GetTestResourcePath("My.Simple.Package");

			var path = Path.Combine(directoryPath, "Web.1.2.3.4.5.config");
			var sources = this.PackageTransformer.GetSourcesForTransformation(path).ToArray();
			Assert.AreEqual(2, sources.Length);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.config"), sources[0]);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), sources[1]);

			path = Path.Combine(directoryPath, "Web.1.2.3.config");
			sources = this.PackageTransformer.GetSourcesForTransformation(path).ToArray();
			Assert.AreEqual(1, sources.Length);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), sources[0]);

			path = Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config");
			sources = this.PackageTransformer.GetSourcesForTransformation(path).ToArray();
			Assert.AreEqual(1, sources.Length);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), sources[0]);

			path = Path.Combine(directoryPath, "Web.config");
			sources = this.PackageTransformer.GetSourcesForTransformation(path).ToArray();
			Assert.AreEqual(0, sources.Length);

			path = Path.Combine(directoryPath, "Web.Release.config");
			sources = this.PackageTransformer.GetSourcesForTransformation(path).ToArray();
			Assert.AreEqual(1, sources.Length);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), sources[0]);

			path = Path.Combine(directoryPath, "Web.Test.config");
			sources = this.PackageTransformer.GetSourcesForTransformation(path).ToArray();
			Assert.AreEqual(1, sources.Length);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), sources[0]);

			path = Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config");
			sources = this.PackageTransformer.GetSourcesForTransformation(path).ToArray();
			Assert.AreEqual(1, sources.Length);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), sources[0]);
		}

		[TestMethod]
		public void GetTransformInformation_Test()
		{
			var manyTransformationNames = new[] { "Release", "Test", "1.2.3.4.5", "AB.CDE.FGHI", "NON.EXISTING.DOT.NAME" };
			var noTransformationNames = Enumerable.Empty<string>().ToArray();

			var directoryPath = this.GetTestResourcePath("My.Package.With.Multiple.Dots");
			var fileToTransformPatterns = new[] { "**/*.config*" };
			var transformationNames = noTransformationNames;
			var transformInformation = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.AreEqual(2, transformInformation.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Views", "Web.config"), transformInformation.ElementAt(0).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(1).Key);

			transformationNames = manyTransformationNames;
			transformInformation = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.AreEqual(2, transformInformation.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Views", "Web.config"), transformInformation.ElementAt(0).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(1).Key);

			// Both back- and forward-slashes works.
			fileToTransformPatterns = ["**/*.config*", @"**\*.json", "**/*.xml"];
			transformationNames = noTransformationNames;
			transformInformation = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.AreEqual(6, transformInformation.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "AppSettings.json"), transformInformation.ElementAt(0).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Directory", "JSON-file.json"), transformInformation.ElementAt(1).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Directory", "XML-file.xml"), transformInformation.ElementAt(2).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Settings.For.json"), transformInformation.ElementAt(3).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Views", "Web.config"), transformInformation.ElementAt(4).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(5).Key);

			transformationNames = manyTransformationNames;
			transformInformation = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.AreEqual(6, transformInformation.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "AppSettings.json"), transformInformation.ElementAt(0).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Directory", "JSON-file.json"), transformInformation.ElementAt(1).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Directory", "XML-file.xml"), transformInformation.ElementAt(2).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Settings.For.json"), transformInformation.ElementAt(3).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Views", "Web.config"), transformInformation.ElementAt(4).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(5).Key);

			// Both back- and forward-slashes works.
			fileToTransformPatterns = [@"**\*.xml", "**/*.json", @"**\*.config*"];
			transformationNames = noTransformationNames;
			transformInformation = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.AreEqual(6, transformInformation.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "AppSettings.json"), transformInformation.ElementAt(0).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Directory", "JSON-file.json"), transformInformation.ElementAt(1).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Directory", "XML-file.xml"), transformInformation.ElementAt(2).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Settings.For.json"), transformInformation.ElementAt(3).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Views", "Web.config"), transformInformation.ElementAt(4).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(5).Key);

			transformationNames = manyTransformationNames;
			transformInformation = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.AreEqual(6, transformInformation.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "AppSettings.json"), transformInformation.ElementAt(0).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Directory", "JSON-file.json"), transformInformation.ElementAt(1).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Directory", "XML-file.xml"), transformInformation.ElementAt(2).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Settings.For.json"), transformInformation.ElementAt(3).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Views", "Web.config"), transformInformation.ElementAt(4).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(5).Key);

			directoryPath = this.GetTestResourcePath("My.Simple.Package");
			fileToTransformPatterns = ["**/*.config*"];
			transformationNames = noTransformationNames;
			transformInformation = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.AreEqual(2, transformInformation.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.config"), transformInformation.ElementAt(0).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(1).Key);

			transformationNames = manyTransformationNames;
			transformInformation = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.AreEqual(2, transformInformation.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.config"), transformInformation.ElementAt(0).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(1).Key);

			transformationNames = ["4.5"];
			transformInformation = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.AreEqual(2, transformInformation.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.config"), transformInformation.ElementAt(0).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(1).Key);

			fileToTransformPatterns = ["**/*.config*", "**/*.json", "**/*.xml"];
			transformationNames = noTransformationNames;
			transformInformation = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.AreEqual(2, transformInformation.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.config"), transformInformation.ElementAt(0).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(1).Key);

			transformationNames = manyTransformationNames;
			transformInformation = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames);
			Assert.AreEqual(2, transformInformation.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.config"), transformInformation.ElementAt(0).Key);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), transformInformation.ElementAt(1).Key);

			fileToTransformPatterns = ["**/*.config*"];
			transformationNames = manyTransformationNames;
			var transformEntries = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(0).Value;
			Assert.AreEqual(1, transformEntries.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(0).Key);
			Assert.IsTrue(transformEntries.ElementAt(0).Value);

			transformEntries = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(1).Value;
			Assert.AreEqual(6, transformEntries.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.Release.config"), transformEntries.ElementAt(0).Key);
			Assert.IsTrue(transformEntries.ElementAt(0).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.Test.config"), transformEntries.ElementAt(1).Key);
			Assert.IsTrue(transformEntries.ElementAt(1).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(2).Key);
			Assert.IsTrue(transformEntries.ElementAt(2).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config"), transformEntries.ElementAt(3).Key);
			Assert.IsTrue(transformEntries.ElementAt(3).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.config"), transformEntries.ElementAt(4).Key);
			Assert.IsFalse(transformEntries.ElementAt(4).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config"), transformEntries.ElementAt(5).Key);
			Assert.IsFalse(transformEntries.ElementAt(5).Value);

			transformationNames = noTransformationNames;
			transformEntries = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(0).Value;
			Assert.AreEqual(1, transformEntries.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(0).Key);
			Assert.IsFalse(transformEntries.ElementAt(0).Value);

			transformEntries = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(1).Value;
			Assert.AreEqual(6, transformEntries.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(0).Key);
			Assert.IsFalse(transformEntries.ElementAt(0).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.config"), transformEntries.ElementAt(1).Key);
			Assert.IsFalse(transformEntries.ElementAt(1).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config"), transformEntries.ElementAt(2).Key);
			Assert.IsFalse(transformEntries.ElementAt(2).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.Release.config"), transformEntries.ElementAt(3).Key);
			Assert.IsFalse(transformEntries.ElementAt(3).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.Test.config"), transformEntries.ElementAt(4).Key);
			Assert.IsFalse(transformEntries.ElementAt(4).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config"), transformEntries.ElementAt(5).Key);
			Assert.IsFalse(transformEntries.ElementAt(5).Value);

			transformationNames = ["4.5"];
			transformEntries = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(0).Value;
			Assert.AreEqual(1, transformEntries.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(0).Key);
			Assert.IsTrue(transformEntries.ElementAt(0).Value);

			transformEntries = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(1).Value;
			Assert.AreEqual(6, transformEntries.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(0).Key);
			Assert.IsTrue(transformEntries.ElementAt(0).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.config"), transformEntries.ElementAt(1).Key);
			Assert.IsFalse(transformEntries.ElementAt(1).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config"), transformEntries.ElementAt(2).Key);
			Assert.IsFalse(transformEntries.ElementAt(2).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.Release.config"), transformEntries.ElementAt(3).Key);
			Assert.IsFalse(transformEntries.ElementAt(3).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.Test.config"), transformEntries.ElementAt(4).Key);
			Assert.IsFalse(transformEntries.ElementAt(4).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config"), transformEntries.ElementAt(5).Key);
			Assert.IsFalse(transformEntries.ElementAt(5).Value);

			transformationNames = ["Test"];
			transformEntries = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(0).Value;
			Assert.AreEqual(1, transformEntries.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(0).Key);
			Assert.IsFalse(transformEntries.ElementAt(0).Value);

			transformEntries = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(1).Value;
			Assert.AreEqual(6, transformEntries.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.Test.config"), transformEntries.ElementAt(0).Key);
			Assert.IsTrue(transformEntries.ElementAt(0).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(1).Key);
			Assert.IsFalse(transformEntries.ElementAt(1).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.config"), transformEntries.ElementAt(2).Key);
			Assert.IsFalse(transformEntries.ElementAt(2).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config"), transformEntries.ElementAt(3).Key);
			Assert.IsFalse(transformEntries.ElementAt(3).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.Release.config"), transformEntries.ElementAt(4).Key);
			Assert.IsFalse(transformEntries.ElementAt(4).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config"), transformEntries.ElementAt(5).Key);
			Assert.IsFalse(transformEntries.ElementAt(5).Value);

			transformationNames = ["Test", "AB.CDE.FGHI"];
			transformEntries = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(0).Value;
			Assert.AreEqual(1, transformEntries.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(0).Key);
			Assert.IsFalse(transformEntries.ElementAt(0).Value);

			transformEntries = this.PackageTransformer.GetTransformInformation(directoryPath, fileToTransformPatterns, transformationNames).ElementAt(1).Value;
			Assert.AreEqual(6, transformEntries.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.Test.config"), transformEntries.ElementAt(0).Key);
			Assert.IsTrue(transformEntries.ElementAt(0).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config"), transformEntries.ElementAt(1).Key);
			Assert.IsTrue(transformEntries.ElementAt(1).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformEntries.ElementAt(2).Key);
			Assert.IsFalse(transformEntries.ElementAt(2).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.config"), transformEntries.ElementAt(3).Key);
			Assert.IsFalse(transformEntries.ElementAt(3).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.Release.config"), transformEntries.ElementAt(4).Key);
			Assert.IsFalse(transformEntries.ElementAt(4).Value);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config"), transformEntries.ElementAt(5).Key);
			Assert.IsFalse(transformEntries.ElementAt(5).Value);
		}

		[TestMethod]
		public void GetTransformMap_Test()
		{
			var directoryPath = this.GetTestResourcePath("My.Package.With.Multiple.Dots");
			var fileToTransformPatterns = new[] { "**/*.config*" };
			var transformMap = this.PackageTransformer.GetTransformMap(directoryPath, fileToTransformPatterns);
			Assert.AreEqual(2, transformMap.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Views", "Web.config"), transformMap.ElementAt(0).Key);
			Assert.AreEqual(4, transformMap.ElementAt(0).Value.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Views", "Web.1.2.3.4.5.config"), transformMap.ElementAt(0).Value.ElementAt(0));
			Assert.AreEqual(Path.Combine(directoryPath, "Views", "Web.AB.CDE.FGHI.config"), transformMap.ElementAt(0).Value.ElementAt(1));
			Assert.AreEqual(Path.Combine(directoryPath, "Views", "Web.Release.config"), transformMap.ElementAt(0).Value.ElementAt(2));
			Assert.AreEqual(Path.Combine(directoryPath, "Views", "Web.Test.config"), transformMap.ElementAt(0).Value.ElementAt(3));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), transformMap.ElementAt(1).Key);
			Assert.AreEqual(5, transformMap.ElementAt(1).Value.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformMap.ElementAt(1).Value.ElementAt(0));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config"), transformMap.ElementAt(1).Value.ElementAt(1));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.Release.config"), transformMap.ElementAt(1).Value.ElementAt(2));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.Test.config"), transformMap.ElementAt(1).Value.ElementAt(3));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config"), transformMap.ElementAt(1).Value.ElementAt(4));

			fileToTransformPatterns = ["**/*.config*", "**/*.json", "**/*.xml"];
			transformMap = this.PackageTransformer.GetTransformMap(directoryPath, fileToTransformPatterns);
			Assert.AreEqual(6, transformMap.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Views", "Web.config"), transformMap.ElementAt(0).Key);
			Assert.AreEqual(4, transformMap.ElementAt(0).Value.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Views", "Web.1.2.3.4.5.config"), transformMap.ElementAt(0).Value.ElementAt(0));
			Assert.AreEqual(Path.Combine(directoryPath, "Views", "Web.AB.CDE.FGHI.config"), transformMap.ElementAt(0).Value.ElementAt(1));
			Assert.AreEqual(Path.Combine(directoryPath, "Views", "Web.Release.config"), transformMap.ElementAt(0).Value.ElementAt(2));
			Assert.AreEqual(Path.Combine(directoryPath, "Views", "Web.Test.config"), transformMap.ElementAt(0).Value.ElementAt(3));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), transformMap.ElementAt(1).Key);
			Assert.AreEqual(5, transformMap.ElementAt(1).Value.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformMap.ElementAt(1).Value.ElementAt(0));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config"), transformMap.ElementAt(1).Value.ElementAt(1));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.Release.config"), transformMap.ElementAt(1).Value.ElementAt(2));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.Test.config"), transformMap.ElementAt(1).Value.ElementAt(3));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config"), transformMap.ElementAt(1).Value.ElementAt(4));
			Assert.AreEqual(Path.Combine(directoryPath, "AppSettings.json"), transformMap.ElementAt(2).Key);
			Assert.AreEqual(4, transformMap.ElementAt(2).Value.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "AppSettings.1.2.3.4.5.json"), transformMap.ElementAt(2).Value.ElementAt(0));
			Assert.AreEqual(Path.Combine(directoryPath, "AppSettings.AB.CDE.FGHI.json"), transformMap.ElementAt(2).Value.ElementAt(1));
			Assert.AreEqual(Path.Combine(directoryPath, "AppSettings.Release.json"), transformMap.ElementAt(2).Value.ElementAt(2));
			Assert.AreEqual(Path.Combine(directoryPath, "AppSettings.Test.json"), transformMap.ElementAt(2).Value.ElementAt(3));
			Assert.AreEqual(Path.Combine(directoryPath, "Directory", "JSON-file.json"), transformMap.ElementAt(3).Key);
			Assert.AreEqual(2, transformMap.ElementAt(3).Value.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Directory", "JSON-file.Dummy.json"), transformMap.ElementAt(3).Value.ElementAt(0));
			Assert.AreEqual(Path.Combine(directoryPath, "Directory", "JSON-file.Test.json"), transformMap.ElementAt(3).Value.ElementAt(1));
			Assert.AreEqual(Path.Combine(directoryPath, "Settings.For.json"), transformMap.ElementAt(4).Key);
			Assert.AreEqual(4, transformMap.ElementAt(4).Value.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Settings.For.1.2.3.4.5.json"), transformMap.ElementAt(4).Value.ElementAt(0));
			Assert.AreEqual(Path.Combine(directoryPath, "Settings.For.AB.CDE.FGHI.json"), transformMap.ElementAt(4).Value.ElementAt(1));
			Assert.AreEqual(Path.Combine(directoryPath, "Settings.For.Release.json"), transformMap.ElementAt(4).Value.ElementAt(2));
			Assert.AreEqual(Path.Combine(directoryPath, "Settings.For.Test.json"), transformMap.ElementAt(4).Value.ElementAt(3));
			Assert.AreEqual(Path.Combine(directoryPath, "Directory", "XML-file.xml"), transformMap.ElementAt(5).Key);
			Assert.AreEqual(1, transformMap.ElementAt(5).Value.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Directory", "XML-file.Release.xml"), transformMap.ElementAt(5).Value.ElementAt(0));

			directoryPath = this.GetTestResourcePath("My.Simple.Package");
			fileToTransformPatterns = ["**/*.config*"];
			transformMap = this.PackageTransformer.GetTransformMap(directoryPath, fileToTransformPatterns);
			Assert.AreEqual(2, transformMap.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.config"), transformMap.ElementAt(0).Key);
			Assert.AreEqual(1, transformMap.ElementAt(0).Value.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformMap.ElementAt(0).Value.ElementAt(0));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), transformMap.ElementAt(1).Key);
			Assert.AreEqual(6, transformMap.ElementAt(1).Value.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformMap.ElementAt(1).Value.ElementAt(0));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.config"), transformMap.ElementAt(1).Value.ElementAt(1));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config"), transformMap.ElementAt(1).Value.ElementAt(2));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.Release.config"), transformMap.ElementAt(1).Value.ElementAt(3));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.Test.config"), transformMap.ElementAt(1).Value.ElementAt(4));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config"), transformMap.ElementAt(1).Value.ElementAt(5));

			fileToTransformPatterns = ["**/*.config*", "**/*.json", "**/*.xml"];
			transformMap = this.PackageTransformer.GetTransformMap(directoryPath, fileToTransformPatterns);
			Assert.AreEqual(2, transformMap.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.config"), transformMap.ElementAt(0).Key);
			Assert.AreEqual(1, transformMap.ElementAt(0).Value.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformMap.ElementAt(0).Value.ElementAt(0));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.config"), transformMap.ElementAt(1).Key);
			Assert.AreEqual(6, transformMap.ElementAt(1).Value.Count);
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.4.5.config"), transformMap.ElementAt(1).Value.ElementAt(0));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.1.2.3.config"), transformMap.ElementAt(1).Value.ElementAt(1));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.AB.CDE.FGHI.config"), transformMap.ElementAt(1).Value.ElementAt(2));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.Release.config"), transformMap.ElementAt(1).Value.ElementAt(3));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.Test.config"), transformMap.ElementAt(1).Value.ElementAt(4));
			Assert.AreEqual(Path.Combine(directoryPath, "Web.That-Should.Not-Be.Used.config"), transformMap.ElementAt(1).Value.ElementAt(5));
		}

		[TestMethod]
		public void Prerequisite_ZipFile_ExtractToDirectory_WorksDifferentlyDependingOnTargetFramework_Test()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName(Guid.NewGuid().ToString()));
			var source = this.GetTestResourcePath("Empty.zip");
			ZipFile.ExtractToDirectory(source, destination);

#if NETFRAMEWORK
			Assert.IsTrue(Directory.Exists(destination));
#else
			Assert.IsFalse(Directory.Exists(destination));
#endif
		}

		[TestMethod]
		public void Transform_IfTheDestinationDirectoryContainsADot_ShouldTransformCorrectly()
		{
			var destination = this.GetOutputPath("My.Transformed-Package");
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var pathToDeletePatterns = new[] { "**/Directory-To-Delete/*", "**/File-To-Delete.*" };
			var source = this.GetTestResourcePath(this.GetRandomPackageName("Package"));
			var transformationNames = new[] { "Release", "Test" };

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
		public void Transform_IfThePathToDeletePatternsContainsAWholeDirectoryWithWildcards_ShouldTransformCorrectly()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName("Transformed-Package"));
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var pathToDeletePatterns = new[] { "Directory/Directory-To-Delete/**/*", "Directory/File-To-Delete.text", "File-To-Delete.txt" };
			var source = this.GetTestResourcePath(this.GetRandomPackageName("Package"));
			var transformationNames = new[] { "Release", "Test" };

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
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var pathToDeletePatterns = new[] { "Directory/**/*", "Views/**/*", "AppSettings.json", "File-To-Delete.txt", "Web.config" };
			var source = this.GetTestResourcePath(this.GetRandomPackageName("Package"));
			var transformationNames = new[] { "Release", "Test" };

			this.PackageTransformer.Transform(true, destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames);

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

			Assert.AreEqual(0, this.GetFileSystemEntries(destination).Count());
		}

		[TestMethod]
		public void Transform_IfTheSourceDirectoryContainsADot_ShouldTransformCorrectly()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName("Transformed-Package"));
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var pathToDeletePatterns = new[] { "**/Directory-To-Delete/*", "**/File-To-Delete.*" };
			var source = this.GetTestResourcePath("My.Package");
			var transformationNames = new[] { "Release", "Test" };

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
		public void Transform_IfTheSourceDirectoryContainsMultipleDotsAndTransformationNamesContainsDots_ShouldTransformCorrectly()
		{
			var destination = this.GetOutputPath(this.GetRandomPackageName("Transformed-Package"));
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var pathToDeletePatterns = new[] { "**/Directory-To-Delete/*", "**/File-To-Delete.*" };
			var source = this.GetTestResourcePath("My.Package.With.Multiple.Dots");
			var transformationNames = new[] { "Release", "Test", "1.2.3.4.5", "AB.CDE.FGHI", "NON.EXISTING.DOT.NAME" };

			this.PackageTransformer.Transform(true, destination, fileToTransformPatterns, pathToDeletePatterns, source, transformationNames);

			if(!Directory.Exists(destination))
			{
				var extractedDestination = this.GetOutputPath(Guid.NewGuid().ToString());
				ZipFile.ExtractToDirectory(destination, extractedDestination);
				destination = extractedDestination;
			}

			var expected = this.GetTestResourcePath("My.Package.With.Multiple.Dots-Expected");

			var actualItems = this.GetFileSystemEntries(destination).ToArray();
			var expectedItems = this.GetFileSystemEntries(expected).ToArray();

			Assert.IsTrue(actualItems.SequenceEqual(expectedItems, new FileComparer(destination, expected)));
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

				// To handle NET 5.0 and NET Core 3.1.
				// Extracting an empty archive does not create an empty directory.
#if !NETFRAMEWORK
				if(!Directory.Exists(extractedDestination))
					Directory.CreateDirectory(extractedDestination);
#endif

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
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var pathToDeletePatterns = new[] { "**/Directory-To-Delete/*", "**/File-To-Delete.*" };
			var source = this.GetTestResourcePath(this.GetRandomPackageName("Package"));
			var transformationNames = new[] { "Release", "Test" };

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
			var fileToTransformPatterns = new[] { "**/*.config*", "**/*.json", "**/*.xml" };
			var source = this.GetTestResourcePath(this.GetRandomPackageName("Alphabetical-Test"));
			var transformationNames = new[] { "C", "A", "B" };

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

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
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
				filePaths.AddRange([@"/Some-other-directory/Some-file.txt", "/Some-other-directory/", "/Some-other-directory"]);
			}

			foreach(var directoryPath in directoryPaths)
			{
				foreach(var filePath in filePaths)
				{
					try
					{
						this.PackageTransformer.ValidateFilePath(action, directoryPath, filePath);
					}
					catch(InvalidOperationException invalidOperationException)
					{
						if(string.Equals(invalidOperationException.Message, $"It is not allowed to {action} the file \"{filePath}\". The file is outside the directory-path \"{directoryPath}\".", StringComparison.Ordinal))
							exceptions.Add(invalidOperationException);
					}
				}
			}

			if(exceptions.Count == 6)
				throw exceptions.First();
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ValidateFilePath_IfTheDirectoryPathIsNull_And_IfTheFilePathIsAbsolute_ShouldThrowAnArgumentNullException()
		{
			try
			{
				this.PackageTransformer.ValidateFilePath("Test", null, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"C:\Test.txt" : "/Test.txt");
			}
			catch(ArgumentNullException argumentNullException)
			{
				if(argumentNullException.ParamName!.Equals("directoryPath", StringComparison.Ordinal))
					throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
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
					this.PackageTransformer.ValidateFilePath("Test", directoryPath, filePath);
				}
				catch(ArgumentException argumentException)
				{
					if(string.Equals(argumentException.ParamName, nameof(directoryPath), StringComparison.Ordinal) && argumentException.Message.StartsWith($"The directory-path can not be relative ({directoryPath}).", StringComparison.Ordinal))
						exceptions.Add(argumentException);
				}
			}

			if(exceptions.Count == expectedNumberOfExceptions)
				throw exceptions.First();
		}

		[TestMethod]
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

			this.PackageTransformer.ValidateFilePath("Test", RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"C:\Some-directory" : "/Some-directory", filePath);
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