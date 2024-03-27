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

		private static readonly FileTransformerFactory _fileTransformerFactory = new(Mock.Of<IFileSystem>(), Mock.Of<ILoggerFactory>(), Mock.Of<IOptionsMonitor<TransformingOptions>>());

		#endregion

		#region Methods

		[Fact]
		public void Create_IfTheSourceParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("source", () => _fileTransformerFactory.Create(string.Empty));
		}

		[Fact]
		public void Create_IfTheSourceParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("source", () => _fileTransformerFactory.Create(null));
		}

		[Fact]
		public void Create_IfTheSourceParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("source", () => _fileTransformerFactory.Create(" "));
		}

		#endregion
	}
}