using System.Runtime.InteropServices;

namespace RegionOrebroLan.Transforming.IO.Extensions
{
	public static class PathExtension
	{
		#region Methods

		public static string EnsureTrailingDirectorySeparator(string path)
		{
			if(path == null)
				throw new ArgumentNullException(nameof(path));

			if(path.EndsWith(Path.AltDirectorySeparatorChar.ToString()) || path.EndsWith(Path.DirectorySeparatorChar.ToString()))
				return path;

			return $"{path}{Path.DirectorySeparatorChar}";
		}

		private static bool IsDirectorySeparator(char character)
		{
			return character == Path.DirectorySeparatorChar || character == Path.AltDirectorySeparatorChar;
		}

		/// <summary>
		/// - https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/IO/PathInternal.cs
		/// - https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/IO/PathInternal.Unix.cs
		/// - https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/IO/PathInternal.Windows.cs
		/// </summary>
		private static bool IsPartiallyQualified(string path)
		{
			// https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/IO/PathInternal.Unix.cs#L77
			if(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return !Path.IsPathRooted(path);

			// https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/IO/PathInternal.Windows.cs#L250
			if(path.Length < 2)
				return true;

			if(IsDirectorySeparator(path[0]))
				return !(path[1] == '?' || IsDirectorySeparator(path[1]));

			return !(path.Length >= 3 && path[1] == Path.VolumeSeparatorChar && IsDirectorySeparator(path[2]) && IsValidDriveCharacter(path[0]));
		}

		/// <summary>
		/// - https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/IO/Path.cs#L304
		/// </summary>
		public static bool IsPathFullyQualified(string path)
		{
			return path == null ? throw new ArgumentNullException(nameof(path)) : !IsPartiallyQualified(path);
		}

		private static bool IsValidDriveCharacter(char character)
		{
			return (uint)((character | 0x20) - 'a') <= 'z' - 'a';
		}

		#endregion
	}
}