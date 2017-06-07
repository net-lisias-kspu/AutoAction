using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Reflection;
using KSP.Localization;
using static KSP.UI.Screens.CraftBrowserDialog;

namespace AutoAction
{
	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class AutoActionEditor : MonoBehaviour
	{
		bool _showBasicGroups;
		bool _showCustomGroups;
		bool _isWindowExpanded;

		Rect _windowRectangle = new Rect(100, 100, 160, CollapsedWindowHeight);

		string _facilityPrefix;

		bool _defaultActivateAbort;
		bool _defaultActivateGear = true;
		bool _defaultActivateLights;
		bool _defaultActivateBrakes;
		bool _defaultActivateRcs;
		bool _defaultActivateSas;
		int _defaultSetThrottle;
		bool _defaultSetPrecCtrl;

		bool? _activateAbort;
		bool? _activateGear;
		bool? _activateLights;
		bool? _activateBrakes;
		bool? _activateRcs;
		bool? _activateSas;
		int? _activateGroupA;
		int? _activateGroupB;
		int? _activateGroupC;
		int? _activateGroupD;
		int? _activateGroupE;
		int? _setThrottle;
		bool? _setPrecCtrl;

		ConfigNode _settings;

		public void Start()
		{
			print($"AutoActions Version {Assembly.GetCallingAssembly().GetName().Version} loaded.");

			GameEvents.onEditorLoad.Add(OnShipLoad);

			bool isVab = EditorDriver.editorFacility == EditorFacility.VAB;
			_facilityPrefix = isVab ? "VAB" : "SPH";

			LoadDefaultSettings();
			LoadPartModule();

			// Are action groups unlocked?
			if(_settings.GetValue("OverrideCareer").ParseNullableBool() == true)
			{
				_showCustomGroups = true;
				_showBasicGroups = true;
			}
			else
			{
				var editorNormLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(
					isVab
						? SpaceCenterFacility.VehicleAssemblyBuilding
						: SpaceCenterFacility.SpaceplaneHangar);

				_showCustomGroups = GameVariables.Instance.UnlockedActionGroupsCustom(editorNormLevel, isVab);
				_showBasicGroups = GameVariables.Instance.UnlockedActionGroupsStock(editorNormLevel, isVab);
			}
		}

		public void OnDisable()
		{
			GameEvents.onEditorLoad.Remove(OnShipLoad);
		}

		void OnShipLoad(ShipConstruct ship, LoadType loadType)
		{
			if(loadType == LoadType.Normal)
				LoadPartModule();
		}

		public void OnGUI()
		{
			// Only show on actions screen and if at least basic actions are unlocked
			if(EditorLogic.fetch.editorScreen == EditorScreen.Actions && _showBasicGroups)
			{
				_windowRectangle.height = _isWindowExpanded ? ExpandedWindowHeight : CollapsedWindowHeight;
				_windowRectangle = GUI.Window(WindowId, _windowRectangle, DrawWindow, Localizer.Format("#ModAutoAction_Title"), WindowStyle);
			}
		}

		void DrawWindow(int windowId)
		{
			// Get window width from localization
			int windowWidth;
			if(int.TryParse(Localizer.GetStringByTag("#ModAutoAction_WindowWidth"), out windowWidth))
				_windowRectangle.width = windowWidth;
			else
				windowWidth = (int)_windowRectangle.width;
			// Relative width unit
			float unit = (windowWidth - 10F) / 15F;

			GUI.Label(
				new Rect(5, 23, windowWidth - 5, 20),
				Localizer.Format("#ModAutoAction_PerVesselSettings"),
				LabelStyle);

			if(GUI.Button(
				new Rect(5, 45, 5 * unit, 18),
				Localizer.Format("#ModAutoAction_Abort"),
				GetButtonStyleByValue(_activateAbort)))
			{
				_activateAbort = GetNextValue(_activateAbort);
				RefreshPartModules();
			}

			if(GUI.Button(
				new Rect(5 + 5 * unit, 45, 5 * unit, 18),
				Localizer.Format("#ModAutoAction_Brakes"),
				GetButtonStyleByValue(_activateBrakes)))
			{
				_activateBrakes = GetNextValue(_activateBrakes);
				RefreshPartModules();
			}

			if(GUI.Button(
				new Rect(5 + 10 * unit, 45, 5 * unit, 18),
				Localizer.Format("#ModAutoAction_Gear"),
				GetButtonStyleByValue(_activateGear)))
			{
				_activateGear = GetNextValue(_activateGear);
				RefreshPartModules();
			}

			if(GUI.Button(
				new Rect(5, 63, 5 * unit, 18),
				Localizer.Format("#ModAutoAction_Lights"),
				GetButtonStyleByValue(_activateLights)))
			{
				_activateLights = GetNextValue(_activateLights);
				RefreshPartModules();
			}

			if(GUI.Button(
				new Rect(5 + 5 * unit, 63, 5 * unit, 18),
				Localizer.Format("#ModAutoAction_Rcs"),
				GetButtonStyleByValue(_activateRcs)))
			{
				_activateRcs = GetNextValue(_activateRcs);
				RefreshPartModules();
			}

			if(GUI.Button(
				new Rect(5 + 10 * unit, 63, 5 * unit, 18),
				Localizer.Format("#ModAutoAction_Sas"),
				GetButtonStyleByValue(_activateSas)))
			{
				_activateSas = GetNextValue(_activateSas);
				RefreshPartModules();
			}

			GUI.Label(
				new Rect(8, 81, windowWidth - 10, 20),
				Localizer.Format("#ModAutoAction_CustomActions"),
				LabelStyle);

			// Only show custom groups if unlocked in editor
			if(_showCustomGroups)
			{
				var activateGroupA = GUI.TextField(
					new Rect(5, 103, 3 * unit, 20),
					_activateGroupA.ToStringValue(nullValue: ""),
					4,
					TextFieldStyle).ParseNullableInt(minValue: 1);
				if(activateGroupA != _activateGroupA)
				{
					_activateGroupA = activateGroupA;
					RefreshPartModules();
				}

				var activateGroupB = GUI.TextField(
					new Rect(5 + 3 * unit, 103, 3 * unit, 20),
					_activateGroupB.ToStringValue(nullValue: ""),
					4,
					TextFieldStyle).ParseNullableInt(minValue: 1);
				if(activateGroupB != _activateGroupB)
				{
					_activateGroupB = activateGroupB;
					RefreshPartModules();
				}

				var activateGroupC = GUI.TextField(
					new Rect(5 + 6 * unit, 103, 3 * unit, 20),
					_activateGroupC.ToStringValue(nullValue: ""),
					4,
					TextFieldStyle).ParseNullableInt(minValue: 1);
				if(activateGroupC != _activateGroupC)
				{
					_activateGroupC = activateGroupC;
					RefreshPartModules();
				}

				var activateGroupD = GUI.TextField(
					new Rect(5 + 9 * unit, 103, 3 * unit, 20),
					_activateGroupD.ToStringValue(nullValue: ""),
					4,
					TextFieldStyle).ParseNullableInt(minValue: 1);
				if(activateGroupD != _activateGroupD)
				{
					_activateGroupD = activateGroupD;
					RefreshPartModules();
				}

				var activateGroupE = GUI.TextField(
					new Rect(5 + 12 * unit, 103, 3 * unit, 20),
					_activateGroupE.ToStringValue(nullValue: ""),
					4,
					TextFieldStyle).ParseNullableInt(minValue: 1);
				if(activateGroupE != _activateGroupE)
				{
					_activateGroupE = activateGroupE;
					RefreshPartModules();
				}
			}
			else
				GUI.Label(
					new Rect(8, 103, windowWidth - 10, 20),
					Localizer.Format("#ModAutoAction_NotAvailable"),
					FailLabelStyle);

			if(GUI.Button(
				new Rect(5, 125, 5 * unit, 20),
				Localizer.Format("#ModAutoAction_Throttle"),
				_setThrottle.HasValue ? OnButtonStyle : DefaultButtonStyle))
			{
				_setThrottle = _setThrottle.HasValue
					? (int?)null
					: _defaultSetThrottle;
				RefreshPartModules();
			}

			if(_setThrottle.HasValue)
			{
				var setThrottle = GUI.TextField(
					new Rect(5 + 5 * unit, 125, 4 * unit, 20),
					_setThrottle.ToStringValue(nullValue: ""),
					3,
					TextFieldStyle).ParseNullableInt(minValue: 0, maxValue: 100);
				if(setThrottle != _setThrottle)
				{
					_setThrottle = setThrottle;
					RefreshPartModules();
				}

				GUI.Label(
					new Rect(10 + 9 * unit, 125, 2 * unit - 5, 20),
					"%",
					LabelStyle);
			}
			else
				GUI.Label(
					new Rect(10 + 5 * unit, 125, 6 * unit - 5, 20),
					Localizer.Format("#ModAutoAction_Default"),
					LabelStyle);

			if(GUI.Button(
				new Rect(5 + 11 * unit, 125, 4 * unit, 20),
				Localizer.Format("#ModAutoAction_PCtrl"),
				GetButtonStyleByValue(_setPrecCtrl)))
			{
				_setPrecCtrl = GetNextValue(_setPrecCtrl);
				RefreshPartModules();
			}


			// Default settings

			GUI.Label(
				new Rect(5, 148, windowWidth - 5, 20),
				Localizer.Format(_facilityPrefix == "VAB" ? "#ModAutoAction_VabDefaults" : "#ModAutoAction_SphDefaults"),
				LabelStyle);

			if(GUI.Button(
				new Rect(windowWidth - 35, 150, 30, 18),
				_isWindowExpanded ? "▲" : "▼",
				DefaultButtonStyle))
			{
				_isWindowExpanded = !_isWindowExpanded;
			}

			if(_isWindowExpanded)
			{
				if(GUI.Button(
					new Rect(5, 170, 5 * unit, 18),
					Localizer.Format("#ModAutoAction_Abort"),
					GetButtonStyleByValue(_defaultActivateAbort)))
				{
					_defaultActivateAbort = !_defaultActivateAbort;
					SaveDefaultSettings();
				}

				if(GUI.Button(
					new Rect(5 + 5 * unit, 170, 5 * unit, 18),
					Localizer.Format("#ModAutoAction_Brakes"),
					GetButtonStyleByValue(_defaultActivateBrakes)))
				{
					_defaultActivateBrakes = !_defaultActivateBrakes;
					SaveDefaultSettings();
				}

				if(GUI.Button(
					new Rect(5 + 10 * unit, 170, 5 * unit, 18),
					Localizer.Format("#ModAutoAction_Gear"),
					GetButtonStyleByValue(_defaultActivateGear)))
				{
					_defaultActivateGear = !_defaultActivateGear;
					SaveDefaultSettings();
				}

				if(GUI.Button(
					new Rect(5, 188, 5 * unit, 18),
					Localizer.Format("#ModAutoAction_Lights"),
					GetButtonStyleByValue(_defaultActivateLights)))
				{
					_defaultActivateLights = !_defaultActivateLights;
					SaveDefaultSettings();
				}

				if(GUI.Button(
					new Rect(5 + 5 * unit, 188, 5 * unit, 18),
					Localizer.Format("#ModAutoAction_Rcs"),
					GetButtonStyleByValue(_defaultActivateRcs)))
				{
					_defaultActivateRcs = !_defaultActivateRcs;
					SaveDefaultSettings();
				}

				if(GUI.Button(
					new Rect(5 + 10 * unit, 188, 5 * unit, 18),
					Localizer.Format("#ModAutoAction_Sas"),
					GetButtonStyleByValue(_defaultActivateSas)))
				{
					_defaultActivateSas = !_defaultActivateSas;
					SaveDefaultSettings();
				}

				GUI.Label(
					new Rect(5, 208, 5 * unit, 20),
					Localizer.Format("#ModAutoAction_Throttle"),
					CenterAlingedLabelStyle);

				var defaultSetThrottle = GUI.TextField(
					new Rect(5 + 5 * unit, 208, 4 * unit, 20),
					_defaultSetThrottle.ToStringValue(),
					3,
					TextFieldStyle).ParseNullableInt(minValue: 0, maxValue: 100) ?? 0;
				if(defaultSetThrottle != _defaultSetThrottle)
				{
					_defaultSetThrottle = defaultSetThrottle;
					SaveDefaultSettings();
				}

				GUI.Label(
					new Rect(10 + 9 * unit, 208, 2 * unit - 5, 20),
					"%",
					LabelStyle);

				if(GUI.Button(
					new Rect(5 + 11 * unit, 208, 4 * unit, 20),
					Localizer.Format("#ModAutoAction_PCtrl"),
					GetButtonStyleByValue(_defaultSetPrecCtrl)))
				{
					_defaultSetPrecCtrl = !_defaultSetPrecCtrl;
					SaveDefaultSettings();
				}
			}

			// Window is draggable
			GUI.DragWindow();
		}

		void RefreshPartModules()
		{
			var autoActionModules =
				EditorLogic.SortedShipList
					?.SelectMany(part => part.Modules.OfType<ModuleAutoAction>());

			if(autoActionModules != null)
				foreach(var module in autoActionModules)
					RefreshPartModule(module);
		}

		void RefreshPartModule(ModuleAutoAction module)
		{
			module.ActivateAbort = _activateAbort;
			module.ActivateBrakes = _activateBrakes;
			module.ActivateGear = _activateGear;
			module.ActivateLights = _activateLights;
			module.ActivateRcs = _activateRcs;
			module.ActivateSas = _activateSas;

			module.SetThrottle = _setThrottle;
			module.SetPrecCtrl = _setPrecCtrl;

			module.ActivateGroupA = _activateGroupA;
			module.ActivateGroupB = _activateGroupB;
			module.ActivateGroupC = _activateGroupC;
			module.ActivateGroupD = _activateGroupD;
			module.ActivateGroupE = _activateGroupE;
		}

		void LoadPartModule()
		{
			var module = EditorLogic.SortedShipList
				?.SelectMany(part => part.Modules.OfType<ModuleAutoAction>())
				.FirstOrDefault();
			if(module != null)
			{
				_activateAbort = module.ActivateAbort;
				_activateGear = module.ActivateGear;
				_activateLights = module.ActivateLights;
				_activateBrakes = module.ActivateBrakes;
				_activateRcs = module.ActivateRcs;
				_activateSas = module.ActivateSas;
				_activateGroupA = module.ActivateGroupA;
				_activateGroupB = module.ActivateGroupB;
				_activateGroupC = module.ActivateGroupC;
				_activateGroupD = module.ActivateGroupD;
				_activateGroupE = module.ActivateGroupE;
				_setThrottle = module.SetThrottle;
				_setPrecCtrl = module.SetPrecCtrl;
			}
		}

		void LoadDefaultSettings()
		{
			_settings = ConfigNode.Load(Static.SettingsFilePath);

			_windowRectangle.x = _settings.GetValue("WinX").ParseNullableInt() ?? 0;
			_windowRectangle.y = _settings.GetValue("WinY").ParseNullableInt() ?? 0;

			_defaultActivateAbort = _settings.GetValue(_facilityPrefix + "activateAbort").ParseNullableBool() ?? false;
			_defaultActivateBrakes = _settings.GetValue(_facilityPrefix + "activateBrakes").ParseNullableBool() ?? false;
			_defaultActivateGear = _settings.GetValue(_facilityPrefix + "activateGear").ParseNullableBool(invertedCompatibilityValue: true) ?? true;
			_defaultActivateLights = _settings.GetValue(_facilityPrefix + "activateLights").ParseNullableBool() ?? false;
			_defaultActivateRcs = _settings.GetValue(_facilityPrefix + "activateRCS").ParseNullableBool() ?? false;
			_defaultActivateSas = _settings.GetValue(_facilityPrefix + "activateSAS").ParseNullableBool() ?? false;
			_defaultSetThrottle = _settings.GetValue(_facilityPrefix + "setThrottle").ParseNullableInt(minValue: 0, maxValue: 100) ?? 0;
			_defaultSetPrecCtrl = _settings.GetValue(_facilityPrefix + "setPrecCtrl").ParseNullableBool() ?? false;
		}

		void SaveDefaultSettings()
		{
			_settings.SetValue("WinX", _windowRectangle.x.ToStringValue(), true);
			_settings.SetValue("WinY", _windowRectangle.y.ToStringValue(), true);

			_settings.SetValue(_facilityPrefix + "activateAbort", _defaultActivateAbort.ToStringValue(), true);
			_settings.SetValue(_facilityPrefix + "activateBrakes", _defaultActivateBrakes.ToStringValue(), true);
			_settings.SetValue(_facilityPrefix + "activateGear", _defaultActivateGear.ToStringValue(), true);
			_settings.SetValue(_facilityPrefix + "activateLights", _defaultActivateLights.ToStringValue(), true);
			_settings.SetValue(_facilityPrefix + "activateRCS", _defaultActivateRcs.ToStringValue(), true);
			_settings.SetValue(_facilityPrefix + "activateSAS", _defaultActivateSas.ToStringValue(), true);
			_settings.SetValue(_facilityPrefix + "setThrottle", _defaultSetThrottle.ToStringValue(), true);
			_settings.SetValue(_facilityPrefix + "setPrecCtrl", _defaultSetPrecCtrl.ToStringValue(), true);

			_settings.Save(Static.SettingsFilePath);
		}

		GUIStyle GetButtonStyleByValue(bool? value) =>
			value.HasValue ? value.Value ? OnButtonStyle : OffButtonStyle : DefaultButtonStyle;

		static bool? GetNextValue(bool? value) =>
			value.HasValue ? value.Value ? false : (bool?)null : true;

		static GUIStyle GetButtonStyle(Texture2D texture) =>
			new GUIStyle(Skin.button)
			{
				alignment = TextAnchor.MiddleCenter,
				fontStyle = FontStyle.Normal,
				normal = { background = texture },
				hover = { background = texture },
			};

		static Texture2D LoadTexture(string teaxtureName)
		{
			var texture = new Texture2D(64, 64);
			texture.LoadImage(File.ReadAllBytes(Static.TextureFolderPath + teaxtureName + ".png"));
			texture.Apply();
			return texture;
		}

		// Styles
		static readonly GUISkin Skin = Instantiate(HighLogic.Skin);
		static readonly GUIStyle WindowStyle = new GUIStyle(Skin.window);
		static readonly GUIStyle LabelStyle = new GUIStyle(Skin.label)
		{
			wordWrap = false,
			alignment = TextAnchor.MiddleLeft,
		};
		static readonly GUIStyle CenterAlingedLabelStyle = new GUIStyle(Skin.label)
		{
			wordWrap = false,
			alignment = TextAnchor.MiddleCenter,
		};
		static readonly GUIStyle FailLabelStyle = new GUIStyle(Skin.label)
		{
			wordWrap = false,
			alignment = TextAnchor.MiddleLeft,
			normal = { textColor = new Color(0.9f, 0.9f, 0.9f, 1f) },
		};
		static readonly GUIStyle TextFieldStyle = new GUIStyle(Skin.textField)
		{
			fontStyle = FontStyle.Normal,
			normal = { textColor = new Color(0.9f, 0.9f, 0.9f, 1f) },
		};

		static readonly GUIStyle DefaultButtonStyle = GetButtonStyle(LoadTexture("ButtonTexture"));
		static readonly GUIStyle OffButtonStyle = GetButtonStyle(LoadTexture("ButtonTextureRed"));
		static readonly GUIStyle OnButtonStyle = GetButtonStyle(LoadTexture("ButtonTextureGreen"));

		const int CollapsedWindowHeight = 175;
		const int ExpandedWindowHeight = 235;

		const int WindowId = 67347792;
	}
}

