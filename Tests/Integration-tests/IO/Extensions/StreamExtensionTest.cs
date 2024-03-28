using RegionOrebroLan.Transforming.IO.Extensions;

namespace IntegrationTests.IO.Extensions
{
	public class StreamExtensionTest
	{
		#region Methods

		[Fact]
		public async Task CopyToAndResetPosition_ShouldCopyTo()
		{
			await Task.CompletedTask;

			using(var stream = CreateMemoryStream())
			{
				using(var memoryStream = new MemoryStream())
				{
					// ReSharper disable MethodHasAsyncOverload
					stream.CopyToAndResetPosition(memoryStream);
					// ReSharper restore MethodHasAsyncOverload

					Assert.Equal(stream.Length, memoryStream.Length);
				}
			}
		}

		[Fact]
		public async Task CopyToAndResetPosition_ShouldResetThePosition()
		{
			await Task.CompletedTask;

			using(var stream = CreateMemoryStream())
			{
				Assert.Equal(0, stream.Position);

				using(var memoryStream = new MemoryStream())
				{
					// ReSharper disable MethodHasAsyncOverload
					stream.CopyToAndResetPosition(memoryStream);
					// ReSharper restore MethodHasAsyncOverload
				}

				Assert.Equal(0, stream.Position);
			}
		}

		[Fact]
		public async Task CopyToAndResetPositionAsync_ShouldCopyTo()
		{
			using(var stream = CreateMemoryStream())
			{
				using(var memoryStream = new MemoryStream())
				{
					await stream.CopyToAndResetPositionAsync(memoryStream);

					Assert.Equal(stream.Length, memoryStream.Length);
				}
			}
		}

		[Fact]
		public async Task CopyToAndResetPositionAsync_ShouldResetThePosition()
		{
			using(var stream = CreateMemoryStream())
			{
				Assert.Equal(0, stream.Position);

				using(var memoryStream = new MemoryStream())
				{
					await stream.CopyToAndResetPositionAsync(memoryStream);
				}

				Assert.Equal(0, stream.Position);
			}
		}

		private static MemoryStream CreateMemoryStream()
		{
			return new MemoryStream([1, 2, 3, 4, 5]);
		}

		#endregion
	}
}