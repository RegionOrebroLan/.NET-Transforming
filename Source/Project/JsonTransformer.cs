using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Jdt;
using RegionOrebroLan.Transforming.IO;
using RegionOrebroLan.Transforming.IO.Extensions;
using RegionOrebroLan.Transforming.Runtime;
using RegionOrebroLan.Transforming.Text.Extensions;

namespace RegionOrebroLan.Transforming
{
	public class JsonTransformer(IFileSystem fileSystem, ILoggerFactory loggerFactory, IPlatform platform) : BasicFileTransformer(fileSystem, loggerFactory, platform)
	{
		#region Methods

		protected internal override void TransformInternal(string destination, string source, string transformation, bool? avoidByteOrderMark = null)
		{
			var useByteOrderMark = this.UseByteOrderMark(avoidByteOrderMark, source);

			using(var stream = new JsonTransformation(transformation).Apply(source))
			{
				using(var streamReader = new StreamReader(stream, true))
				{
					var encoding = useByteOrderMark ? streamReader.CurrentEncoding : streamReader.CurrentEncoding.WithoutByteOrderMark();

					this.FileSystem.WriteFile(streamReader.ReadToEnd(), encoding, destination);
				}
			}
		}

		#endregion
	}
}