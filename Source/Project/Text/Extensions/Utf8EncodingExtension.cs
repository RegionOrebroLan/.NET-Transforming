using System.Reflection;
using System.Text;
using RegionOrebroLan.Transforming.Extensions;

namespace RegionOrebroLan.Transforming.Text.Extensions
{
	public static class Utf8EncodingExtension
	{
		#region Fields

		private static readonly FieldInfo _isThrowExceptionField = typeof(UTF8Encoding).GetPrivateInstanceField(_isThrowExceptionFieldName);
		private const string _isThrowExceptionFieldName = "isThrowException";

		#endregion

		#region Methods

		public static bool ThrowOnInvalidBytes(this UTF8Encoding utf8Encoding)
		{
			if(utf8Encoding == null)
				throw new ArgumentNullException(nameof(utf8Encoding));

			if(_isThrowExceptionField == null)
				throw new NullReferenceException($"The \"{_isThrowExceptionFieldName}\" field could not be found for version \"{Environment.Version}\".");

			return (bool)_isThrowExceptionField.GetValue(utf8Encoding);
		}

		#endregion
	}
}