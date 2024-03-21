using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Transforming.IO.Extensions;

namespace IntegrationTests.IO.Extensions
{
	[TestClass]
	public class StreamExtensionTest
	{
		#region Methods

		[TestMethod]
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

					Assert.AreEqual(stream.Length, memoryStream.Length);
				}
			}
		}

		[TestMethod]
		public async Task CopyToAndResetPosition_ShouldResetThePosition()
		{
			await Task.CompletedTask;

			using(var stream = CreateMemoryStream())
			{
				Assert.AreEqual(0, stream.Position);

				using(var memoryStream = new MemoryStream())
				{
					// ReSharper disable MethodHasAsyncOverload
					stream.CopyToAndResetPosition(memoryStream);
					// ReSharper restore MethodHasAsyncOverload
				}

				Assert.AreEqual(0, stream.Position);
			}
		}

		[TestMethod]
		public async Task CopyToAndResetPositionAsync_ShouldCopyTo()
		{
			using(var stream = CreateMemoryStream())
			{
				using(var memoryStream = new MemoryStream())
				{
					await stream.CopyToAndResetPositionAsync(memoryStream);

					Assert.AreEqual(stream.Length, memoryStream.Length);
				}
			}
		}

		[TestMethod]
		public async Task CopyToAndResetPositionAsync_ShouldResetThePosition()
		{
			using(var stream = CreateMemoryStream())
			{
				Assert.AreEqual(0, stream.Position);

				using(var memoryStream = new MemoryStream())
				{
					await stream.CopyToAndResetPositionAsync(memoryStream);
				}

				Assert.AreEqual(0, stream.Position);
			}
		}

		private static MemoryStream CreateMemoryStream()
		{
			return new MemoryStream([1, 2, 3, 4, 5]);
		}

		#endregion
	}
}