using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RegionOrebroLan.Transforming;
using RegionOrebroLan.Transforming.Configuration;
using RegionOrebroLan.Transforming.IO;

namespace UnitTests
{
	public class FileTransformerFactoryTest
	{
		#region Fields

		private static FileTransformerFactory _fileTransformerFactory;

		#endregion

		#region Properties

		protected internal virtual FileTransformerFactory FileTransformerFactory => _fileTransformerFactory ??= new FileTransformerFactory(Mock.Of<IFileSystem>(), Mock.Of<ILoggerFactory>(), Mock.Of<IOptionsMonitor<TransformingOptions>>());

		#endregion

		#region Methods

		[Fact]
		public void Create_IfTheSourceParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("source", () => this.FileTransformerFactory.Create(string.Empty));
		}

		[Fact]
		public void Create_IfTheSourceParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("source", () => this.FileTransformerFactory.Create(null));
		}

		[Fact]
		public void Create_IfTheSourceParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("source", () => this.FileTransformerFactory.Create(" "));
		}

		#endregion
	}
}