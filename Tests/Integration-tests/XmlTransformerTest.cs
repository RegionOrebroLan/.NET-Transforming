using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.Transforming;
using RegionOrebroLan.Transforming.IO;
using RegionOrebroLan.Transforming.Runtime;

namespace IntegrationTests
{
	[TestClass]
	public class XmlTransformerTest : BasicTransformingTest
	{
		#region Fields

		private static XmlTransformer _xmlTransformer;

		#endregion

		#region Properties

		protected internal virtual XmlTransformer XmlTransformer => _xmlTransformer ??= new XmlTransformer(new FileSystem(), new Platform());

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
			const string fileName = "Web.config";
			var destination = this.GetOutputPath(fileName);
			var source = this.GetTestResourcePath(fileName);
			var transformation = this.GetTestResourcePath("Web.No-Transformation.config");

			this.XmlTransformer.Transform(destination, source, transformation);

			var expectedContent = File.ReadAllText(this.GetTestResourcePath("Web.No-Transformation.Expected.config"));
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
			const string fileName = "Web.config";
			var destination = this.GetOutputPath(fileName);
			var source = this.GetTestResourcePath(fileName);
			var transformation = this.GetTestResourcePath("Web.Transformation.config");

			this.XmlTransformer.Transform(destination, source, transformation);

			var expectedContent = File.ReadAllText(this.GetTestResourcePath("Web.Expected.config"));
			var actualContent = File.ReadAllText(destination);

			Assert.AreEqual(expectedContent, actualContent);
		}

		protected internal virtual void ValidateTransformDestinationParameterException<T>(string destination) where T : ArgumentException
		{
			this.ValidateDestinationParameterException<T>(() => { this.XmlTransformer.Transform(destination, this.GetTestResourcePath("Web.config"), this.GetTestResourcePath("Web.No-Transformation.config")); });
		}

		protected internal virtual void ValidateTransformSourceParameterException<T>(string source) where T : ArgumentException
		{
			this.ValidateSourceParameterException<T>(() => { this.XmlTransformer.Transform(this.GetOutputPath("Web.config"), source, this.GetTestResourcePath("Web.No-Transformation.config")); });
		}

		protected internal virtual void ValidateTransformTransformationParameterException<T>(string transformation) where T : ArgumentException
		{
			this.ValidateTransformationParameterException<T>(() => { this.XmlTransformer.Transform(this.GetOutputPath("Web.config"), this.GetTestResourcePath("Web.config"), transformation); });
		}

		#endregion
	}
}