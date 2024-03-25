using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Transforming;
using RegionOrebroLan.Transforming.IO;

namespace IntegrationTests
{
	[TestClass]
	public class JsonTransformerTest : BasicTransformingTest
	{
		#region Fields

		private static JsonTransformer _jsonTransformer;

		#endregion

		#region Properties

		protected internal virtual JsonTransformer JsonTransformer => _jsonTransformer ??= new JsonTransformer(new FileSystem(), new NullLoggerFactory(), this.OptionsMonitor);

		#endregion

		#region Methods

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheDestinationParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformDestinationParameterException<ArgumentException>(string.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Transform_IfTheDestinationParameterIsNull_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformDestinationParameterException<ArgumentNullException>(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheDestinationParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformDestinationParameterException<ArgumentException>(" ");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheSourceParameterDoesNotExistAsFile_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformSourceParameterException<ArgumentException>(this.GetTestResourcePath("Non-existing-file.txt"));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheSourceParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformSourceParameterException<ArgumentException>(string.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Transform_IfTheSourceParameterIsNull_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformSourceParameterException<ArgumentNullException>(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheSourceParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformSourceParameterException<ArgumentException>(" ");
		}

		[TestMethod]
		public void Transform_IfTheTransformationContentIsEmpty_ShouldTransformCorrectly()
		{
			const string fileName = "AppSettings.json";
			var destination = this.GetOutputPath(fileName);
			var source = this.GetTestResourcePath(fileName);
			var transformation = this.GetTestResourcePath("AppSettings.No-Transformation.json");

			this.JsonTransformer.Transform(destination, source, transformation);

			var expectedContent = File.ReadAllText(this.GetTestResourcePath("AppSettings.No-Transformation.Expected.json"));
			var actualContent = File.ReadAllText(destination);

			Assert.AreEqual(expectedContent, actualContent);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheTransformationParameterDoesNotExistAsFile_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformTransformationParameterException<ArgumentException>(this.GetTestResourcePath("Non-existing-file.txt"));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheTransformationParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformTransformationParameterException<ArgumentException>(string.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Transform_IfTheTransformationParameterIsNull_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformTransformationParameterException<ArgumentNullException>(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheTransformationParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformTransformationParameterException<ArgumentException>(" ");
		}

		[TestMethod]
		public void Transform_ShouldTransformCorrectly()
		{
			const string fileName = "AppSettings.json";
			var destination = this.GetOutputPath(fileName);
			var source = this.GetTestResourcePath(fileName);
			var transformation = this.GetTestResourcePath("AppSettings.Transformation.json");

			this.JsonTransformer.Transform(destination, source, transformation);

			var expectedContent = File.ReadAllText(this.GetTestResourcePath("AppSettings.Expected.json"));
			var actualContent = File.ReadAllText(destination);

			Assert.AreEqual(expectedContent, actualContent);
		}

		protected internal virtual void ValidateTransformDestinationParameterException<T>(string destination) where T : ArgumentException
		{
			this.ValidateDestinationParameterException<T>(() => { this.JsonTransformer.Transform(destination, this.GetTestResourcePath("AppSettings.json"), this.GetTestResourcePath("AppSettings.No-Transformation.json")); });
		}

		protected internal virtual void ValidateTransformSourceParameterException<T>(string source) where T : ArgumentException
		{
			this.ValidateSourceParameterException<T>(() => { this.JsonTransformer.Transform(this.GetOutputPath("AppSettings.json"), source, this.GetTestResourcePath("AppSettings.No-Transformation.json")); });
		}

		protected internal virtual void ValidateTransformTransformationParameterException<T>(string transformation) where T : ArgumentException
		{
			this.ValidateTransformationParameterException<T>(() => { this.JsonTransformer.Transform(this.GetOutputPath("AppSettings.json"), this.GetTestResourcePath("AppSettings.json"), transformation); });
		}

		#endregion
	}
}