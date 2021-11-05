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
﻿using System;
using System.Collections.Generic;
using UnityEngine;
using static KSP.UI.Screens.CraftBrowserDialog;

namespace AutoAction
{
	[KSPAddon(KSPAddon.Startup.EditorAny, once: false)]
	public partial class AutoActionEditor : MonoBehaviour
	{
		Settings _settings;
		bool _settings_UseSaveGameSettings;
		FacilitySettings _facilitySettings;
		VesselSettings _vesselSettings;

		EditorFacility _currentFacility;
		string _facilityDefaultsTitle;

		bool _isTrimSectionExpanded;
		bool _isDefaultsSectionExpanded;
		Rect _windowRectangle;

		public void Start()
		{
			Debug.Log($"[{nameof(AutoAction)}] editor: Start ({EditorDriver.editorFacility})");

			_settings = new Settings();
			_settings.Load();
			_settings_UseSaveGameSettings = _settings.UseSaveGameSettings;

			InitializeFacility();
			SetVesselSettingsFrom(EditorLogic.SortedShipList);
			
			_windowRectangle = new Rect(_settings.WindowPosition, Vector2.zero);

			GameEvents.onEditorRestart.Add(OnEditorRestart);
			GameEvents.onEditorPodPicked.Add(OnEditorPodPicked);
			GameEvents.onEditorLoad.Add(OnEditorLoad);
		}

		public void OnDestroy()
		{
			Debug.Log($"[{nameof(AutoAction)}] editor: OnDestroy");

			GameEvents.onEditorRestart.Remove(OnEditorRestart);
			GameEvents.onEditorPodPicked.Remove(OnEditorPodPicked);
			GameEvents.onEditorLoad.Remove(OnEditorLoad);

			if(null != _settings)
			{
				_settings.UseSaveGameSettings = _settings_UseSaveGameSettings;
				_settings.Save();
			}
		}

		void OnEditorRestart()
		{
			Debug.Log($"[{nameof(AutoAction)}] editor: OnEditorRestart ({EditorDriver.editorFacility})");
			if(EditorDriver.editorFacility != _currentFacility)
			{
				_settings.Save();
				InitializeFacility();
			}
		}

		void InitializeFacility()
		{
			_currentFacility = EditorDriver.editorFacility;
			if(_currentFacility == EditorFacility.VAB)
			{
				_facilityDefaultsTitle = "#ModAutoAction_VabDefaults";
				_facilitySettings = _settings.VabSettings;
			}
			else
			{
				_facilityDefaultsTitle = "#ModAutoAction_SphDefaults";
				_facilitySettings = _settings.SphSettings;
			}
		}

		void OnEditorPodPicked(Part part)
		{
			Debug.Log($"[{nameof(AutoAction)}] editor: OnEditorPodPicked");
			SetVesselSettingsFrom(null);
		}

		void OnEditorLoad(ShipConstruct ship, LoadType loadType)
		{
			if(loadType == LoadType.Normal)
			{
				Debug.Log($"[{nameof(AutoAction)}] editor: OnEditorLoad '{ship.shipName}'");
				SetVesselSettingsFrom(ship.Parts);
			}
		}

		void SetVesselSettingsFrom(IEnumerable<Part> parts)
		{
			_vesselSettings = parts.GetVesselSettings();
			_isTrimSectionExpanded = _vesselSettings.HasNonDefaultTrim;
			_windowRectangle.height = 0;
		}

		void UpdateVesselSettings() =>
			EditorLogic.SortedShipList?.UpdateVesselSettings(_vesselSettings);

		public void OnGUI()
		{
			// Only show on actions screen
			if(EditorLogic.fetch?.editorScreen == EditorScreen.Actions && _settings is object)
			{
				_windowRectangle = GUILayout.Window(WindowId, _windowRectangle, WindowGUI, WindowTitle, WindowStyle);
				_settings.WindowPosition = _windowRectangle.position;
			}
		}
	}
}

