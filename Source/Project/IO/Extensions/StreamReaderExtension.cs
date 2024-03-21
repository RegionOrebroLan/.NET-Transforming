namespace RegionOrebroLan.Transforming.IO.Extensions
{
	public static class StreamReaderExtension
	{
		#region Methods

		public static bool HasByteOrderMark(this StreamReader streamReader)
		{
			if(streamReader == null)
				throw new ArgumentNullException(nameof(streamReader));

			return streamReader.BaseStream.HasByteOrderMark(streamReader.CurrentEncoding);
		}

		#endregion
	}
}