using System.Text;

namespace RegionOrebroLan.Transforming.IO.Extensions
{
	public static class StreamExtension
	{
		#region Methods

		/// <summary>
		/// Same as CopyTo but then it resets the position to the position it had before CopyTo.
		/// </summary>
		public static void CopyToAndResetPosition(this Stream stream, Stream destination)
		{
			if(stream == null)
				throw new ArgumentNullException(nameof(stream));

			var position = stream.Position;

			try
			{
				stream.CopyTo(destination);
			}
			finally
			{
				stream.Position = position;
			}
		}

		/// <summary>
		/// Same as CopyToAsync but then it resets the position to the position it had before CopyToAsync.
		/// </summary>
		public static async Task CopyToAndResetPositionAsync(this Stream stream, Stream destination)
		{
			if(stream == null)
				throw new ArgumentNullException(nameof(stream));

			var position = stream.Position;

			try
			{
				await stream.CopyToAsync(destination);
			}
			finally
			{
				stream.Position = position;
			}
		}

		public static byte[] GetBytes(this Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException(nameof(stream));

			if(stream is MemoryStream memoryStream)
				return memoryStream.ToArray();

			using(memoryStream = new MemoryStream())
			{
				stream.CopyToAndResetPosition(memoryStream);

				return memoryStream.ToArray();
			}
		}

		public static bool HasByteOrderMark(this Stream stream, Encoding encoding)
		{
			if(stream == null)
				throw new ArgumentNullException(nameof(stream));

			if(encoding == null)
				throw new ArgumentNullException(nameof(encoding));

			var preamble = encoding.GetPreamble();

			if(!preamble.Any())
				return false;

			var bytes = stream.GetBytes();

			if(!bytes.Any())
				return false;

			for(var i = 0; i < preamble.Length; i++)
			{
				if(bytes.Length < i + 1)
					return false;

				if(preamble[i] != bytes[i])
					return false;
			}

			return true;
		}

		#endregion
	}
}