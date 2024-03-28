using System.Text;
using RegionOrebroLan.Transforming.Text.Extensions;

namespace IntegrationTests.Text.Extensions
{
	public class EncodingExtensionTest
	{
		#region Methods

		[Fact]
		public async Task WithoutByteOrderMark_IfAsciiEncoding_Test()
		{
			await Task.CompletedTask;

			var asciiEncoding = new ASCIIEncoding();
			Assert.False(asciiEncoding.GetPreamble().Any());
			Assert.True(ReferenceEquals(asciiEncoding, asciiEncoding.WithoutByteOrderMark()));
			Assert.False(asciiEncoding.WithoutByteOrderMark().GetPreamble().Any());
		}

		[Fact]
		public async Task WithoutByteOrderMark_IfUnicodeEncoding_Test()
		{
			await Task.CompletedTask;

			var unicodeEncoding = new UnicodeEncoding(true, false, true);
			Assert.False(unicodeEncoding.GetPreamble().Any());
			Assert.True(ReferenceEquals(unicodeEncoding, unicodeEncoding.WithoutByteOrderMark()));
			Assert.False(unicodeEncoding.WithoutByteOrderMark().GetPreamble().Any());

			unicodeEncoding = new UnicodeEncoding(true, true, true);
			Assert.Equal(2, unicodeEncoding.GetPreamble().Length);
			Assert.False(ReferenceEquals(unicodeEncoding, unicodeEncoding.WithoutByteOrderMark()));
			Assert.False(unicodeEncoding.WithoutByteOrderMark().GetPreamble().Any());
		}

		[Fact]
		public async Task WithoutByteOrderMark_IfUtf32Encoding_Test()
		{
			await Task.CompletedTask;

			var utf32Encoding = new UTF32Encoding(true, false, true);
			Assert.False(utf32Encoding.GetPreamble().Any());
			Assert.True(ReferenceEquals(utf32Encoding, utf32Encoding.WithoutByteOrderMark()));
			Assert.False(utf32Encoding.WithoutByteOrderMark().GetPreamble().Any());

			utf32Encoding = new UTF32Encoding(true, true, true);
			Assert.Equal(4, utf32Encoding.GetPreamble().Length);
			Assert.False(ReferenceEquals(utf32Encoding, utf32Encoding.WithoutByteOrderMark()));
			Assert.False(utf32Encoding.WithoutByteOrderMark().GetPreamble().Any());
		}

		[Fact]
		public async Task WithoutByteOrderMark_IfUtf7Encoding_Test()
		{
			await Task.CompletedTask;

			var utf7Encoding = new UTF7Encoding();
			Assert.False(utf7Encoding.GetPreamble().Any());
			Assert.True(ReferenceEquals(utf7Encoding, utf7Encoding.WithoutByteOrderMark()));
			Assert.False(utf7Encoding.WithoutByteOrderMark().GetPreamble().Any());

			utf7Encoding = new UTF7Encoding(false);
			Assert.False(utf7Encoding.GetPreamble().Any());
			Assert.True(ReferenceEquals(utf7Encoding, utf7Encoding.WithoutByteOrderMark()));
			Assert.False(utf7Encoding.WithoutByteOrderMark().GetPreamble().Any());

			utf7Encoding = new UTF7Encoding(true);
			Assert.False(utf7Encoding.GetPreamble().Any());
			Assert.True(ReferenceEquals(utf7Encoding, utf7Encoding.WithoutByteOrderMark()));
			Assert.False(utf7Encoding.WithoutByteOrderMark().GetPreamble().Any());
		}

		[Fact]
		public async Task WithoutByteOrderMark_IfUtf8Encoding_Test()
		{
			await Task.CompletedTask;

			var utf8Encoding = new UTF8Encoding(false);
			Assert.False(utf8Encoding.GetPreamble().Any());
			Assert.True(ReferenceEquals(utf8Encoding, utf8Encoding.WithoutByteOrderMark()));
			Assert.False(utf8Encoding.WithoutByteOrderMark().GetPreamble().Any());

			utf8Encoding = new UTF8Encoding(true);
			Assert.Equal(3, utf8Encoding.GetPreamble().Length);
			Assert.False(ReferenceEquals(utf8Encoding, utf8Encoding.WithoutByteOrderMark()));
			Assert.False(utf8Encoding.WithoutByteOrderMark().GetPreamble().Any());
		}

		#endregion
	}
}