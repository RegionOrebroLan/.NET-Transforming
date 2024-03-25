using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Web.XmlTransform;
using RegionOrebroLan.Transforming.Configuration;
using RegionOrebroLan.Transforming.IO;
using RegionOrebroLan.Transforming.IO.Extensions;
using RegionOrebroLan.Transforming.Text.Extensions;

namespace RegionOrebroLan.Transforming
{
	public class XmlTransformer(IFileSystem fileSystem, ILoggerFactory loggerFactory, IOptionsMonitor<TransformingOptions> optionsMonitor) : BasicFileTransformer(fileSystem, loggerFactory, optionsMonitor)
	{
		#region Methods

		protected internal override void TransformInternal(string destination, string source, string transformation, FileTransformingOptions options)
		{
			var useByteOrderMark = this.UseByteOrderMark(options, source);

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
						using(var stream = new MemoryStream())
						{
							xmlTransformableDocument.Save(stream);

							stream.Position = 0;

							using(var streamReader = new StreamReader(stream, true))
							{
								var content = this.GetContent(options, streamReader);

								var encoding = useByteOrderMark ? streamReader.CurrentEncoding : streamReader.CurrentEncoding.WithoutByteOrderMark();

								this.FileSystem.WriteFile(content, encoding, destination);
							}
						}
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