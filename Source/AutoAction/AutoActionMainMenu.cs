/*
	This file is part of Auto Actions /L Unleashed
		© 2021 Lisias T : http://lisias.net <support@lisias.net>
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
using UnityEngine;

namespace AutoAction
{
	/// <summary>
	/// Showing an empty window for a tiny moment to Initialize the legacy GUI.
	/// </summary>
	[KSPAddon(KSPAddon.Startup.MainMenu, once: true)]
	public class AutoActionMainMenu : MonoBehaviour
	{
		public void OnGUI()
		{
			if(_isFirstTime)
			{
				Debug.Log($"[{nameof(AutoAction)}] mainMenu: OnGUI");
				GUILayout.Window(WindowId, new Rect(), id => { }, " ");
				_isFirstTime = false;
			}
		}

		bool _isFirstTime = true;

		static readonly int WindowId = nameof(AutoAction).GetHashCode();
	}
}
