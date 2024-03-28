using RegionOrebroLan.Transforming.IO.Extensions;

namespace IntegrationTests.IO.Extensions
{
	public class StreamReaderExtensionTest
	{
		#region Methods

		private static string GetFilePath(string fileName)
		{
			return Path.Combine(Global.ProjectDirectory.FullName, "IO", "Extensions", "Resources", "StreamReaderExtensionTest", fileName);
		}

		[Fact]
		public async Task HasByteOrderMark_IfTheStreamReaderFileHasABom_ShouldReturnTrue()
		{
			await Task.CompletedTask;

			using(var streamReader = new StreamReader(GetFilePath("bom-file.txt")))
			{
				Assert.True(streamReader.HasByteOrderMark());
			}
		}

		[Fact]
		public async Task HasByteOrderMark_IfTheStreamReaderFileNotHasABom_ShouldReturnFalse()
		{
			await Task.CompletedTask;

			using(var streamReader = new StreamReader(GetFilePath("no-bom-file.txt")))
			{
				Assert.False(streamReader.HasByteOrderMark());
			}
		}

		#endregion
	}
}