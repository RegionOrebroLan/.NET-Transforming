using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Jdt;
using RegionOrebroLan.Transforming.Configuration;
using RegionOrebroLan.Transforming.IO;
using RegionOrebroLan.Transforming.IO.Extensions;
using RegionOrebroLan.Transforming.Text.Extensions;

namespace RegionOrebroLan.Transforming
{
	public class JsonTransformer(IFileSystem fileSystem, ILoggerFactory loggerFactory, IOptionsMonitor<TransformingOptions> optionsMonitor) : BasicFileTransformer(fileSystem, loggerFactory, optionsMonitor)
	{
		#region Methods

		protected internal override void TransformInternal(string destination, string source, string transformation, FileTransformingOptions options)
		{
			var useByteOrderMark = this.UseByteOrderMark(options, source);

			using(var stream = new JsonTransformation(transformation).Apply(source))
			{
				using(var streamReader = new StreamReader(stream, true))
				{
					var content = this.GetFileContent(options, streamReader);

					var encoding = useByteOrderMark ? streamReader.CurrentEncoding : streamReader.CurrentEncoding.WithoutByteOrderMark();

					this.FileSystem.WriteFile(content, encoding, destination);
				}
			}
		}

		#endregion
	}
}