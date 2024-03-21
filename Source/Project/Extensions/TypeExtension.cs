using System.Reflection;
using RegionOrebroLan.Transforming.Reflection;

namespace RegionOrebroLan.Transforming.Extensions
{
	public static class TypeExtension
	{
		#region Methods

		public static FieldInfo GetPrivateInstanceField(this Type type, string name, string dotNetCoreNamePrefix = "_")
		{
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			return type.GetField(ReflectionHelper.ResolveFieldName(name, dotNetCoreNamePrefix), BindingFlags.Instance | BindingFlags.NonPublic)!;
		}

		#endregion
	}
}