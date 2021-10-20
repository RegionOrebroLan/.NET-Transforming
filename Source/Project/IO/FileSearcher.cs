using System.Collections.Generic;
using System.IO;
using System.Linq;
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

			matcher.AddExcludePatterns(excludePatterns ?? Enumerable.Empty<string>());
			matcher.AddIncludePatterns(includePatterns ?? Enumerable.Empty<string>());

			return matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(directoryPath))).Files.Select(file => file.Path);
		}

		#endregion
	}
}