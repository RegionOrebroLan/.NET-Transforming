using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Transforming.Text.Extensions;

namespace IntegrationTests.Text.Extensions
{
	[TestClass]
	public class EncodingExtensionTest
	{
		#region Methods

		[TestMethod]
		public async Task WithoutByteOrderMark_IfAsciiEncoding_Test()
		{
			await Task.CompletedTask;

			var asciiEncoding = new ASCIIEncoding();
			Assert.IsFalse(asciiEncoding.GetPreamble().Any());
			Assert.IsTrue(ReferenceEquals(asciiEncoding, asciiEncoding.WithoutByteOrderMark()));
			Assert.IsFalse(asciiEncoding.WithoutByteOrderMark().GetPreamble().Any());
		}

		[TestMethod]
		public async Task WithoutByteOrderMark_IfUnicodeEncoding_Test()
		{
			await Task.CompletedTask;

			var unicodeEncoding = new UnicodeEncoding(true, false, true);
			Assert.IsFalse(unicodeEncoding.GetPreamble().Any());
			Assert.IsTrue(ReferenceEquals(unicodeEncoding, unicodeEncoding.WithoutByteOrderMark()));
			Assert.IsFalse(unicodeEncoding.WithoutByteOrderMark().GetPreamble().Any());

			unicodeEncoding = new UnicodeEncoding(true, true, true);
			Assert.AreEqual(2, unicodeEncoding.GetPreamble().Length);
			Assert.IsFalse(ReferenceEquals(unicodeEncoding, unicodeEncoding.WithoutByteOrderMark()));
			Assert.IsFalse(unicodeEncoding.WithoutByteOrderMark().GetPreamble().Any());
		}

		[TestMethod]
		public async Task WithoutByteOrderMark_IfUtf32Encoding_Test()
		{
			await Task.CompletedTask;

			var utf32Encoding = new UTF32Encoding(true, false, true);
			Assert.IsFalse(utf32Encoding.GetPreamble().Any());
			Assert.IsTrue(ReferenceEquals(utf32Encoding, utf32Encoding.WithoutByteOrderMark()));
			Assert.IsFalse(utf32Encoding.WithoutByteOrderMark().GetPreamble().Any());

			utf32Encoding = new UTF32Encoding(true, true, true);
			Assert.AreEqual(4, utf32Encoding.GetPreamble().Length);
			Assert.IsFalse(ReferenceEquals(utf32Encoding, utf32Encoding.WithoutByteOrderMark()));
			Assert.IsFalse(utf32Encoding.WithoutByteOrderMark().GetPreamble().Any());
		}

		[TestMethod]
		public async Task WithoutByteOrderMark_IfUtf7Encoding_Test()
		{
			await Task.CompletedTask;

			var utf7Encoding = new UTF7Encoding();
			Assert.IsFalse(utf7Encoding.GetPreamble().Any());
			Assert.IsTrue(ReferenceEquals(utf7Encoding, utf7Encoding.WithoutByteOrderMark()));
			Assert.IsFalse(utf7Encoding.WithoutByteOrderMark().GetPreamble().Any());

			utf7Encoding = new UTF7Encoding(false);
			Assert.IsFalse(utf7Encoding.GetPreamble().Any());
			Assert.IsTrue(ReferenceEquals(utf7Encoding, utf7Encoding.WithoutByteOrderMark()));
			Assert.IsFalse(utf7Encoding.WithoutByteOrderMark().GetPreamble().Any());

			utf7Encoding = new UTF7Encoding(true);
			Assert.IsFalse(utf7Encoding.GetPreamble().Any());
			Assert.IsTrue(ReferenceEquals(utf7Encoding, utf7Encoding.WithoutByteOrderMark()));
			Assert.IsFalse(utf7Encoding.WithoutByteOrderMark().GetPreamble().Any());
		}

		[TestMethod]
		public async Task WithoutByteOrderMark_IfUtf8Encoding_Test()
		{
			await Task.CompletedTask;

			var utf8Encoding = new UTF8Encoding(false);
			Assert.IsFalse(utf8Encoding.GetPreamble().Any());
			Assert.IsTrue(ReferenceEquals(utf8Encoding, utf8Encoding.WithoutByteOrderMark()));
			Assert.IsFalse(utf8Encoding.WithoutByteOrderMark().GetPreamble().Any());

			utf8Encoding = new UTF8Encoding(true);
			Assert.AreEqual(3, utf8Encoding.GetPreamble().Length);
			Assert.IsFalse(ReferenceEquals(utf8Encoding, utf8Encoding.WithoutByteOrderMark()));
			Assert.IsFalse(utf8Encoding.WithoutByteOrderMark().GetPreamble().Any());
		}

		#endregion
	}
}