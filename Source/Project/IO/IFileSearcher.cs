namespace RegionOrebroLan.Transforming.IO
{
	public interface IFileSearcher
	{
		#region Methods

		IEnumerable<string> Find(string directoryPath, IEnumerable<string> excludePatterns, IEnumerable<string> includePatterns);

		#endregion
	}
}