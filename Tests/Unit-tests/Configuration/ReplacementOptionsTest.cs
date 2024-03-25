using System.Runtime.InteropServices;
using RegionOrebroLan.Transforming.Configuration;
using Xunit;

namespace UnitTests.Configuration
{
	public class ReplacementOptionsTest
	{
		#region Methods

		[Fact]
		public async Task Enabled_Test()
		{
			await Task.CompletedTask;

			if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				Assert.False(new ReplacementOptions().Enabled);
			else
				Assert.True(new ReplacementOptions().Enabled);
		}

		[Fact]
		public async Task Replace_Test()
		{
			await Task.CompletedTask;

			if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				const string value = "a\r\nb\nc\r\nd";
				Assert.Equal("a\r\nb\r\nc\r\nd", new ReplacementOptions().Replace(value));
			}
			else
			{
				const string value = "a\nb\r\nc\nd";
				Assert.Equal("a\nb\nc\nd", new ReplacementOptions().Replace(value));
			}
		}

		#endregion
	}
}