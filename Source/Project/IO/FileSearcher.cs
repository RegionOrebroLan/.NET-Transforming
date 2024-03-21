using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace RegionOrebroLan.Transforming.IO
{
	public class FileSearcher : IFileSearcher
	{
		#region Methods

		public virtual IEnumerable<string> Find(string directoryPath, IEnumerable<string> excludePatterns, IEnumerable<string> includePatterns)
		{
			var matcher = new Matcher();

			matcher.AddExcludePatterns(excludePatterns ?? []);
			matcher.AddIncludePatterns(includePatterns ?? []);

			return matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(directoryPath))).Files.Select(file => file.Path);
		}

		#endregion
	}
}