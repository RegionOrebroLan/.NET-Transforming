namespace RegionOrebroLan.Transforming.IO
{
	public interface IDirectory
	{
		#region Methods

		void CreateDirectory(string path);
		void Delete(string path);
		void Delete(string path, bool recursive);
		IEnumerable<string> EnumerateFileSystemEntries(string path);
		bool Exists(string path);
		IEnumerable<string> GetDirectories(string path);
		IEnumerable<string> GetFiles(string path);

		#endregion
	}
}