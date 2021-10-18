using System.IO;
using System.Text;
using Microsoft.VisualStudio.Jdt;
using RegionOrebroLan.Transforming.IO;
using RegionOrebroLan.Transforming.IO.Extensions;

namespace RegionOrebroLan.Transforming
{
	public class JsonTransformer : BasicFileTransformer
	{
		#region Constructors

		public JsonTransformer(IFileSystem fileSystem) : base(fileSystem) { }

		#endregion

		#region Methods

		protected internal virtual void SaveToFile(string destination, Stream stream)
		{
			Encoding encoding;
			string content;

			using(var streamReader = new StreamReader(stream, true))
			{
				// ReSharper disable ReturnValueOfPureMethodIsNotUsed
				streamReader.Peek();
				// ReSharper restore ReturnValueOfPureMethodIsNotUsed
				encoding = streamReader.CurrentEncoding;
				content = streamReader.ReadToEnd();
			}

			this.FileSystem.WriteFile(content, encoding, destination);
		}

		protected internal override void TransformInternal(string destination, string source, string transformation)
		{
			using(var stream = new JsonTransformation(transformation).Apply(source))
			{
				this.SaveToFile(destination, stream);
			}
		}

		#endregion
	}
}