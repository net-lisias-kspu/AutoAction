using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoAction
{
	static class AgxInterface
	{
		public static bool IsAgxInstalled()
		{
			try
			{
				Type agxType = Type.GetType(AgxTypeName);
				return
					agxType != null &&
					(bool)agxType.InvokeMember("AGXInstalled", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, null);
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
				Type agxType = Type.GetType(AgxTypeName);
				agxType?.InvokeMember("AGXToggleGroup", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new object[] { group });
			}
			catch { }
		}

		const string AgxTypeName = "ActionGroupsExtended.AGExtExternal, AGExt";
	}
}
