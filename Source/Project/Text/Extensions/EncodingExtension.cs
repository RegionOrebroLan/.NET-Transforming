using System.Text;

namespace RegionOrebroLan.Transforming.Text.Extensions
{
	public static class EncodingExtension
	{
		#region Methods

		public static Encoding WithoutByteOrderMark(this Encoding encoding)
		{
			if(encoding == null)
				throw new ArgumentNullException(nameof(encoding));

			if(!encoding.GetPreamble().Any()) // ASCIIEncoding & UTF7Encoding should have an empty preamble.
				return encoding;

			if(encoding is UTF32Encoding utf32Encoding)
				return new UTF32Encoding(utf32Encoding.BigEndian(), false, utf32Encoding.ThrowOnInvalidBytes());

			if(encoding is UTF8Encoding utf8Encoding)
				return new UTF8Encoding(false, utf8Encoding.ThrowOnInvalidBytes());

			if(encoding is UnicodeEncoding unicodeEncoding)
				return new UnicodeEncoding(unicodeEncoding.BigEndian(), false, unicodeEncoding.ThrowOnInvalidBytes());

			// If we get here and the encoding is not a standard one we might want to add logic here.
			return encoding;
		}

		#endregion
	}
}