using System.Reflection;
using System.Text;
using RegionOrebroLan.Transforming.Extensions;

namespace RegionOrebroLan.Transforming.Text.Extensions
{
	public static class Utf32EncodingExtension
	{
		#region Fields

		private static readonly FieldInfo _bigEndianField = typeof(UTF32Encoding).GetPrivateInstanceField(_bigEndianFieldName);
		private const string _bigEndianFieldName = "bigEndian";
		private static readonly FieldInfo _isThrowExceptionField = typeof(UTF32Encoding).GetPrivateInstanceField(_isThrowExceptionFieldName);
		private const string _isThrowExceptionFieldName = "isThrowException";

		#endregion

		#region Methods

		public static bool BigEndian(this UTF32Encoding utf32Encoding)
		{
			if(utf32Encoding == null)
				throw new ArgumentNullException(nameof(utf32Encoding));

			if(_bigEndianField == null)
				throw new NullReferenceException($"The \"{_bigEndianFieldName}\" field could not be found for version \"{Environment.Version}\".");

			return (bool)_bigEndianField.GetValue(utf32Encoding);
		}

		public static bool ThrowOnInvalidBytes(this UTF32Encoding utf32Encoding)
		{
			if(utf32Encoding == null)
				throw new ArgumentNullException(nameof(utf32Encoding));

			if(_isThrowExceptionField == null)
				throw new NullReferenceException($"The \"{_isThrowExceptionFieldName}\" field could not be found for version \"{Environment.Version}\".");

			return (bool)_isThrowExceptionField.GetValue(utf32Encoding);
		}

		#endregion
	}
}