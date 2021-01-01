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
