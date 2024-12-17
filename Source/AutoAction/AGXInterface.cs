/*
	This file is part of Auto Actions /L Unleashed
		© 2018-2024 LisiasT : http://lisias.net <support@lisias.net>
		© 2016-2018 Teilnehmer(Formicant)
		© 2014-2016 Diazo

	Auto Actions /L Unleashed is licensed as follows:
		* GPL 3.0 : https://www.gnu.org/licenses/gpl-3.0.txt

	Auto Actions /L Unleashed is distributed in the hope that
	it will be useful, but WITHOUT ANY WARRANTY; without even the implied
	warranty of	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0 along
	with Auto Actions /L Unleashed. If not, see <https://www.gnu.org/licenses/>.

*/
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
