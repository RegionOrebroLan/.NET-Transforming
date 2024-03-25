namespace RegionOrebroLan.Transforming.IO
{
	public interface IDirectory
	{
		#region Methods

		void CreateDirectory(string path);
		void Delete(string path);
		void Delete(string path, bool recursive);
		bool Exists(string path);
		IEnumerable<string> GetDirectories(string path);
		IEnumerable<string> GetFiles(string path);
		IEnumerable<string> GetFilesRecursive(string path);

		#endregion
	}
}