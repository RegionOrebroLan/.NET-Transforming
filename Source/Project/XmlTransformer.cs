using System.Diagnostics.CodeAnalysis;
using Microsoft.Web.XmlTransform;
using RegionOrebroLan.Transforming.IO;
using RegionOrebroLan.Transforming.IO.Extensions;

namespace RegionOrebroLan.Transforming
{
	public class XmlTransformer : BasicFileTransformer
	{
		#region Constructors

		public XmlTransformer(IFileSystem fileSystem) : base(fileSystem) { }

		#endregion

		#region Methods

		[SuppressMessage("Security", "CA3075:Insecure DTD processing in XML")]
		protected internal override void TransformInternal(string destination, string source, string transformation)
		{
			using(var xmlTransformableDocument = new XmlTransformableDocument())
			{
				using(var xmlTransformation = new XmlTransformation(transformation))
				{
					xmlTransformableDocument.PreserveWhitespace = true;
					xmlTransformableDocument.Load(source);

					if(!xmlTransformation.Apply(xmlTransformableDocument))
						throw new InvalidOperationException("The xml-transformation did not apply.");

					var directoryToDeleteOnError = this.FileSystem.GetRequiredTopAncestorDirectoryForFile(destination);
					var file = new FileInfo(destination);

					if(!this.FileSystem.Directory.Exists(file.DirectoryName))
						this.FileSystem.Directory.CreateDirectory(file.DirectoryName);

					try
					{
						xmlTransformableDocument.Save(destination);
					}
					catch
					{
						if(directoryToDeleteOnError != null && this.FileSystem.Directory.Exists(directoryToDeleteOnError))
							this.FileSystem.Directory.Delete(directoryToDeleteOnError);

						throw;
					}
				}
			}
		}

		#endregion
	}
}