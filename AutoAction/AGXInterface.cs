using System;
using System.Reflection;

namespace AutoAction
{
	static class AgxInterface
	{
		public static bool IsAgxInstalled()
		{
			try
			{
				var agxType = Type.GetType(AgxTypeName);
				return
					agxType is object &&
					(bool) agxType.InvokeMember("AGXInstalled", InvokePublicStaticMethod, null, null, null);
			}
			catch
			{
				return false;
			}
		}

		public static void AgxToggleGroup(int group)
		{
			try
			{
				var agxType = Type.GetType(AgxTypeName);
				agxType?.InvokeMember("AGXToggleGroup", InvokePublicStaticMethod, null, null, new object[] { group });
			}
			catch { }
		}

		static readonly BindingFlags InvokePublicStaticMethod = BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static;
		const string AgxTypeName = "ActionGroupsExtended.AGExtExternal, AGExt";
	}
}
