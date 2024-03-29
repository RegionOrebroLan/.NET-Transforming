using RegionOrebroLan.Transforming.Configuration;
using RegionOrebroLan.Transforming.IO.Extensions;

namespace RegionOrebroLan.Transforming
{
	public abstract class BasicTransformer
	{
		#region Methods

		protected internal virtual string GetFileContent(FileTransformingOptions options, StreamReader streamReader)
		{
			if(options == null)
				throw new ArgumentNullException(nameof(options));

			if(streamReader == null)
				throw new ArgumentNullException(nameof(streamReader));

			var content = streamReader.ReadToEnd();

			if(options.Replacement.Enabled)
				content = options.Replacement.Replace(content);

			return content;
		}

		protected internal virtual bool UseByteOrderMark(FileTransformingOptions options, string source)
		{
			if(options == null)
				throw new ArgumentNullException(nameof(options));

			using(var streamReader = new StreamReader(source, true))
			{
				return streamReader.HasByteOrderMark() && !options.AvoidByteOrderMark;
			}
		}

		#endregion
	}
}