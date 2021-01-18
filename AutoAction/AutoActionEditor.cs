using System;
using System.Collections.Generic;
using UnityEngine;
using static KSP.UI.Screens.CraftBrowserDialog;

namespace AutoAction
{
	[KSPAddon(KSPAddon.Startup.EditorAny, once: false)]
	public partial class AutoActionEditor : MonoBehaviour
	{
		Settings _settings;
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

			_settings?.Save();
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

