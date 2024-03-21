using System.Reflection;
using System.Text;
using RegionOrebroLan.Transforming.Extensions;

namespace RegionOrebroLan.Transforming.Text.Extensions
{
	public static class UnicodeEncodingExtension
	{
		#region Fields

		private static readonly FieldInfo _bigEndianField = typeof(UnicodeEncoding).GetPrivateInstanceField(_bigEndianFieldName, string.Empty);
		private const string _bigEndianFieldName = "bigEndian";
		private static readonly FieldInfo _isThrowExceptionField = typeof(UnicodeEncoding).GetPrivateInstanceField(_isThrowExceptionFieldName, string.Empty);
		private const string _isThrowExceptionFieldName = "isThrowException";

		#endregion

		#region Methods

		public static bool BigEndian(this UnicodeEncoding unicodeEncoding)
		{
			if(unicodeEncoding == null)
				throw new ArgumentNullException(nameof(unicodeEncoding));

			if(_bigEndianField == null)
				throw new NullReferenceException($"The \"{_bigEndianFieldName}\" field could not be found for version \"{Environment.Version}\".");

			return (bool)_bigEndianField.GetValue(unicodeEncoding);
		}

		public static bool ThrowOnInvalidBytes(this UnicodeEncoding unicodeEncoding)
		{
			if(unicodeEncoding == null)
				throw new ArgumentNullException(nameof(unicodeEncoding));

			if(_isThrowExceptionField == null)
				throw new NullReferenceException($"The \"{_isThrowExceptionFieldName}\" field could not be found for version \"{Environment.Version}\".");

			return (bool)_isThrowExceptionField.GetValue(unicodeEncoding);
		}

		#endregion
	}
}