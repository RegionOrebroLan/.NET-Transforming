using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Transforming;
using RegionOrebroLan.Transforming.IO;
using RegionOrebroLan.Transforming.Runtime;

namespace IntegrationTests
{
	[TestClass]
	public class FileTransformerFactoryTest : BasicTransformingTest
	{
		#region Fields

		private static FileTransformerFactory _fileTransformerFactory;

		#endregion

		#region Properties

		protected internal virtual FileTransformerFactory FileTransformerFactory => _fileTransformerFactory ??= new FileTransformerFactory(new FileSystem(), new Platform());

		#endregion

		#region Methods

		[TestMethod]
		public void Create_IfTheSourceIsAJsonFile_ShouldReturnAJsonTransformer()
		{
			Assert.IsTrue(this.FileTransformerFactory.Create(this.GetTestResourcePath("AppSettings.json")) is JsonTransformer);
		}

		[TestMethod]
		public void Create_IfTheSourceIsAXmlFile_ShouldReturnAXmlTransformer()
		{
			Assert.IsTrue(this.FileTransformerFactory.Create(this.GetTestResourcePath("Web.config")) is XmlTransformer);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Create_IfTheSourceIsNeitherAJsonFileNorAXmlFile_ShouldThrowAnInvalidOperationException()
		{
			this.FileTransformerFactory.Create(this.GetTestResourcePath("File.txt"));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Create_IfTheSourceParameterDoesNotExistAsFile_ShouldThrowAnArgumentException()
		{
			this.FileTransformerFactory.Create(this.GetTestResourcePath("Non-existing-file.txt"));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Create_IfTheSourceParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			this.FileTransformerFactory.Create(string.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_IfTheSourceParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			this.FileTransformerFactory.Create(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Create_IfTheSourceParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			this.FileTransformerFactory.Create(" ");
		}

		#endregion
	}
}