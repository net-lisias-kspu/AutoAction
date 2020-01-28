using System;
using System.Linq;
using UnityEngine;
using System.Reflection;
using KSP.Localization;

using static KSP.UI.Screens.CraftBrowserDialog;

using GUI = KSPe.UI.GUI;

namespace AutoAction
{
	using Asset = KSPe.IO.Asset<AutoActionEditor>;

	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class AutoActionEditor : MonoBehaviour
	{
		bool _showBasicGroups;
		bool _showCustomGroups;
		bool _isTrimSectionExpanded;
		bool _isDefaultsSectionExpanded;

		Vector2 _windowPosition = DefaultWindowPosition;
		Rect _windowRectangle = new Rect(DefaultWindowPosition, InitialWindowSize);

		string _facilityName;

		bool _overrideCareer;

		bool _defaultActivateAbort;
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
		string _setPitchTrimString;
		string _setYawTrimString;
		string _setRollTrimString;
		string _setWheelMotorTrimString;
		string _setWheelSteerTrimString;

		ConfigNode _settings;

		public void Start()
		{
			Log.Info("AutoActions Version {0} loaded.", Assembly.GetCallingAssembly().GetName().Version);

			GameEvents.onEditorLoad.Add(OnShipLoad);

			bool isVab = EditorDriver.editorFacility == EditorFacility.VAB;
			_facilityName = isVab ? "VAB" : "SPH";

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
				float editorNormLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(
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
			if(EditorLogic.fetch?.editorScreen == EditorScreen.Actions && _showBasicGroups)
			{
				//_windowRectangle.height = _isWindowExpanded ? ExpandedWindowHeight : CollapsedWindowHeight;
				_windowRectangle = GUI.Window(WindowId, _windowRectangle, DrawWindow, Localizer.Format("#ModAutoAction_Title"), WindowStyle);
				if(_windowPosition != _windowRectangle.position)
				{
					_windowPosition = _windowRectangle.position;
					SaveDefaultSettings();
				}
			}
		}

		void DrawWindow(int windowId)
		{
			int windowHeight = 23;

			// Get window width from localization
			float windowWidth =
				int.TryParse(Localizer.GetStringByTag("#ModAutoAction_WindowWidth"), out int localizationWindowWidth)
					? localizationWindowWidth
					: InitialWindowSize.x;
			// Relative width unit
			float unit = (windowWidth - 10F) / 20F;

			Label(0, 20, 0, "#ModAutoAction_PerVesselSettings");
			windowHeight += 22;

			ThreeStateButton(0, 10, "#ModAutoAction_Abort", RefreshPartModules, ref _activateAbort);
			ThreeStateButton(10, 10, "#ModAutoAction_Brakes", RefreshPartModules, ref _activateBrakes);
			windowHeight += 20;

			ThreeStateButton(0, 10, "#ModAutoAction_Rcs", RefreshPartModules, ref _activateRcs);
			ThreeStateButton(10, 10, "#ModAutoAction_Sas", RefreshPartModules, ref _activateSas);
			windowHeight += 20;

			ThreeStateButton(0, 10, "#ModAutoAction_Gear", RefreshPartModules, ref _activateGear);
			ThreeStateButton(10, 10, "#ModAutoAction_Lights", RefreshPartModules, ref _activateLights);
			windowHeight += 20;

			// Only show custom groups if unlocked in editor
			if(_showCustomGroups)
			{
				Label(0, 20, 3, "#ModAutoAction_CustomActions");
				windowHeight += 22;

				NullableNumberField(0, 4, 4, RefreshPartModules, ref _activateGroupA, minValue: 1);
				NullableNumberField(4, 4, 4, RefreshPartModules, ref _activateGroupB, minValue: 1);
				NullableNumberField(8, 4, 4, RefreshPartModules, ref _activateGroupC, minValue: 1);
				NullableNumberField(12, 4, 4, RefreshPartModules, ref _activateGroupD, minValue: 1);
				NullableNumberField(16, 4, 4, RefreshPartModules, ref _activateGroupE, minValue: 1);
				windowHeight += 20;
			}

			windowHeight += 4;

			ThreeStateButton(0, 20, "#ModAutoAction_PCtrl", RefreshPartModules, ref _setPrecCtrl);
			windowHeight += 24;

			OnDefaultButton(0, 9, "#ModAutoAction_Throttle", ref _setThrottle, RefreshPartModules, _defaultSetThrottle);
			if(_setThrottle.HasValue)
			{
				NullableNumberField(10, 8, 3, RefreshPartModules, ref _setThrottle, minValue: 0, maxValue: 100);
				Label(18, 2, 5, "%");
			}
			else
			{
				Label(10, 10, 5, "#ModAutoAction_Default");
			}
			windowHeight += 23;

			// Trim

			Label(0, 15, 0, "#ModAutoAction_Trim", RightAlingedLabelStyle);
			ExpandButton(ref _isTrimSectionExpanded);
			windowHeight += 22;

			if(_isTrimSectionExpanded)
			{
				windowHeight += 2;

				Label(0, 10, 0, "#ModAutoAction_Pitch", RightAlingedLabelStyle);
				NumberUpDown(11, 9, 4, RefreshPartModules, ref _setPitchTrimString, defaultValue: 0, minValue: -500, maxValue: 500);
				windowHeight += 24;

				Label(0, 10, 0, "#ModAutoAction_Yaw", RightAlingedLabelStyle);
				NumberUpDown(11, 9, 4, RefreshPartModules, ref _setYawTrimString, defaultValue: 0, minValue: -500, maxValue: 500);
				windowHeight += 24;

				Label(0, 10, 0, "#ModAutoAction_Roll", RightAlingedLabelStyle);
				NumberUpDown(11, 9, 4, RefreshPartModules, ref _setRollTrimString, defaultValue: 0, minValue: -500, maxValue: 500);
				windowHeight += 24;

				Label(0, 10, 0, "#ModAutoAction_WheelThrottle", RightAlingedLabelStyle);
				NumberUpDown(11, 9, 4, RefreshPartModules, ref _setWheelMotorTrimString, defaultValue: 0, minValue: -500, maxValue: 500);
				windowHeight += 24;

				Label(0, 10, 0, "#ModAutoAction_WheelSteer", RightAlingedLabelStyle);
				NumberUpDown(11, 9, 4, RefreshPartModules, ref _setWheelSteerTrimString, defaultValue: 0, minValue: -500, maxValue: 500);
				windowHeight += 26;
			}

			// Default settings

			Label(0, 20, 0, _facilityName == "VAB" ? "#ModAutoAction_VabDefaults" : "#ModAutoAction_SphDefaults");
			ExpandButton(ref _isDefaultsSectionExpanded);
			windowHeight += 22;

			if(_isDefaultsSectionExpanded)
			{
				OnOffButton(0, 10, "#ModAutoAction_Abort", SaveDefaultSettings, ref _defaultActivateAbort);
				OnOffButton(10, 10, "#ModAutoAction_Brakes", SaveDefaultSettings, ref _defaultActivateBrakes);
				windowHeight += 20;

				OnOffButton(0, 10, "#ModAutoAction_Rcs", SaveDefaultSettings, ref _defaultActivateRcs);
				OnOffButton(10, 10, "#ModAutoAction_Sas", SaveDefaultSettings, ref _defaultActivateSas);
				windowHeight += 20;

				OnOffButton(0, 20, "#ModAutoAction_PCtrl", SaveDefaultSettings, ref _defaultSetPrecCtrl);
				windowHeight += 24;

				Label(0, 9, 0, "#ModAutoAction_Throttle", CenterAlingedLabelStyle);
				NumberField(10, 8, 3, SaveDefaultSettings, ref _defaultSetThrottle, defaultValue: 0, minValue: 0, maxValue: 100);
				Label(18, 2, 5, "%");
				windowHeight += 22;
			}

			windowHeight += 5;

			_windowRectangle.width = windowWidth;
			_windowRectangle.height = windowHeight;

			// Window is draggable
			GUI.DragWindow();


			#region UI elements

			const float Margin = 5;

			void Label(float left, float width, float indent, string text, GUIStyle style = null)
			{
				GUI.Label(new Rect(Margin + left * unit + indent, windowHeight, width * unit - indent, 20), Localizer.Format(text), style ?? LabelStyle);
			}

			void OnOffButton(float left, float width, string text, Action changeAction, ref bool value)
			{
				if(GUI.Button(new Rect(Margin + left * unit, windowHeight, width * unit, 20), Localizer.Format(text), GetButtonStyleByValue(value)))
				{
					value = !value;
					changeAction();
				}
			}

			void OnDefaultButton(float left, float width, string text, ref int? value, Action changeAction, int defaultValue)
			{
				if(GUI.Button(new Rect(Margin + left * unit, windowHeight, width * unit, 20), Localizer.Format(text), value.HasValue ? OnButtonStyle : DefaultButtonStyle))
				{
					value = value.HasValue
						? (int?)null
						: defaultValue;
					changeAction();
				}
			}

			void ThreeStateButton(float left, float width, string text, Action changeAction, ref bool? value)
			{
				if(GUI.Button(new Rect(Margin + left * unit, windowHeight, width * unit, 20), Localizer.Format(text), GetButtonStyleByValue(value)))
				{
					value = GetNextValue(value);
					changeAction();
				}
			}

			void ExpandButton(ref bool value)
			{
				if(GUI.Button(new Rect(windowWidth - 35, windowHeight + 2, 30, 18), value ? "▲" : "▼", DefaultButtonStyle))
					value = !value;
			}

			void NumberField(float left, float width, int maxLength, Action changeAction, ref int value, int defaultValue, int minValue = int.MinValue, int maxValue = int.MaxValue)
			{
				int fieldValue =
					GUI.TextField(new Rect(Margin + left * unit, windowHeight, width * unit, 20), value.ToStringValue(), maxLength, TextFieldStyle)
						.ParseNullableInt(minValue, maxValue) ?? defaultValue;
				if(fieldValue != value)
				{
					value = fieldValue;
					changeAction();
				}
			}

			void NullableNumberField(float left, float width, int maxLength, Action changeAction, ref int? value, int minValue = int.MinValue, int maxValue = int.MaxValue)
			{
				int? fieldValue =
					GUI.TextField(new Rect(Margin + left * unit, windowHeight, width * unit, 20), value.ToStringValue(nullValue: ""), maxLength, TextFieldStyle)
						.ParseNullableInt(minValue, maxValue);
				if(fieldValue != value)
				{
					value = fieldValue;
					changeAction();
				}
			}

			void NumberUpDown(float left, float width, int maxLength, Action changeAction, ref string value, int defaultValue, int minValue = int.MinValue, int maxValue = int.MaxValue)
			{
				if(string.IsNullOrEmpty(value))
					value = defaultValue.ToStringSigned();
				string fieldValue = GUI.TextField(new Rect(Margin + left * unit, windowHeight, width * unit - 16, 20), value, maxLength, NumberUpDownTextFieldStyle);
				if(fieldValue != value)
				{
					value = fieldValue;
					changeAction();
				}
				if(GUI.Button(new Rect(Margin + (left + width) * unit - 16, windowHeight - 1, 16, 11), "▴", DefaultButtonStyle))
				{
					int numberValue = fieldValue.ParseNullableInt(minValue, maxValue) ?? defaultValue;
					value = Math.Min(maxValue, numberValue + 1).ToStringSigned();
					changeAction();
				}
				if(GUI.Button(new Rect(Margin + (left + width) * unit - 16, windowHeight + 10, 16, 11), "▾", DefaultButtonStyle))
				{
					int numberValue = fieldValue.ParseNullableInt(minValue, maxValue) ?? defaultValue;
					value = Math.Max(minValue, numberValue - 1).ToStringSigned();
					changeAction();
				}
			}

			#endregion
		}

		void RefreshPartModules()
		{
			System.Collections.Generic.IEnumerable<ModuleAutoAction> autoActionModules =
				EditorLogic.SortedShipList
					?.SelectMany(part => part.Modules.OfType<ModuleAutoAction>());

			if(autoActionModules != null)
				foreach(ModuleAutoAction module in autoActionModules)
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

			module.SetPitchTrim = _setPitchTrimString.ParseNullableInt(minValue: -500, maxValue: 500) ?? 0;
			module.SetYawTrim = _setYawTrimString.ParseNullableInt(minValue: -500, maxValue: 500) ?? 0;
			module.SetRollTrim = _setRollTrimString.ParseNullableInt(minValue: -500, maxValue: 500) ?? 0;
			module.SetWheelMotorTrim = _setWheelMotorTrimString.ParseNullableInt(minValue: -500, maxValue: 500) ?? 0;
			module.SetWheelSteerTrim = _setWheelSteerTrimString.ParseNullableInt(minValue: -500, maxValue: 500) ?? 0;
		}

		void LoadPartModule()
		{
			ModuleAutoAction module = EditorLogic.SortedShipList
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
				_setPitchTrimString = module.SetPitchTrim.ToStringSigned();
				_setYawTrimString = module.SetYawTrim.ToStringSigned();
				_setRollTrimString = module.SetRollTrim.ToStringSigned();
				_setWheelMotorTrimString = module.SetWheelMotorTrim.ToStringSigned();
				_setWheelSteerTrimString = module.SetWheelSteerTrim.ToStringSigned();

				_isTrimSectionExpanded =
					module.SetPitchTrim != 0 ||
					module.SetYawTrim != 0 ||
					module.SetRollTrim != 0 ||
					module.SetWheelMotorTrim != 0 ||
					module.SetWheelSteerTrim != 0;
			}
		}

		void LoadDefaultSettings()
		{
            if (Static.SETTINGS_FILE.IsLoadable)
                Static.SETTINGS_FILE.Load();

            _settings = Static.SETTINGS_FILE.Node;

			_overrideCareer = _settings.GetValue("OverrideCareer").ParseNullableBool() ?? false;

			ConfigNode windowPosition = _settings.GetNode("WindowPosition") ?? new ConfigNode();
			_windowPosition.x = windowPosition.GetValue("X").ParseNullableInt() ?? DefaultWindowPosition.x;
			_windowPosition.y = windowPosition.GetValue("Y").ParseNullableInt() ?? DefaultWindowPosition.y;
			_windowRectangle.position = _windowPosition;

			ConfigNode facilityDefaults = _settings.GetNode(_facilityName) ?? new ConfigNode();
			_defaultActivateAbort = facilityDefaults.GetValue("ActivateAbort").ParseNullableBool() ?? false;
			_defaultActivateBrakes = facilityDefaults.GetValue("ActivateBrakes").ParseNullableBool() ?? false;
			_defaultActivateRcs = facilityDefaults.GetValue("ActivateRCS").ParseNullableBool() ?? false;
			_defaultActivateSas = facilityDefaults.GetValue("ActivateSAS").ParseNullableBool() ?? false;
			_defaultSetThrottle = facilityDefaults.GetValue("SetThrottle").ParseNullableInt(minValue: 0, maxValue: 100) ?? 0;
			_defaultSetPrecCtrl = facilityDefaults.GetValue("SetPrecCtrl").ParseNullableBool() ?? false;
		}

		void SaveDefaultSettings()
		{
			_settings.SetValue("OverrideCareer", _overrideCareer.ToStringValue(), true);

			ConfigNode windowPosition = new ConfigNode("WindowPosition");
			windowPosition.SetValue("X", _windowPosition.x.ToStringValue(), true);
			windowPosition.SetValue("Y", _windowPosition.y.ToStringValue(), true);
			_settings.SetNode(windowPosition.name, windowPosition, true);

			ConfigNode facilityDefaults = new ConfigNode(_facilityName);
			facilityDefaults.SetValue("ActivateAbort", _defaultActivateAbort.ToStringValue(), true);
			facilityDefaults.SetValue("ActivateBrakes", _defaultActivateBrakes.ToStringValue(), true);
			facilityDefaults.SetValue("ActivateRCS", _defaultActivateRcs.ToStringValue(), true);
			facilityDefaults.SetValue("ActivateSAS", _defaultActivateSas.ToStringValue(), true);
			facilityDefaults.SetValue("SetThrottle", _defaultSetThrottle.ToStringValue(), true);
			facilityDefaults.SetValue("SetPrecCtrl", _defaultSetPrecCtrl.ToStringValue(), true);
			_settings.SetNode(facilityDefaults.name, facilityDefaults, true);

			Static.SETTINGS_FILE.Save();
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

		static Texture2D LoadTexture(string textureName)
		{
			return Asset.Texture2D.LoadFromFile("Textures", textureName);
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
		static readonly GUIStyle RightAlingedLabelStyle = new GUIStyle(Skin.label)
		{
			wordWrap = false,
			alignment = TextAnchor.MiddleRight,
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
		static readonly GUIStyle NumberUpDownTextFieldStyle = new GUIStyle(Skin.textField)
		{
			fontStyle = FontStyle.Normal,
			alignment = TextAnchor.MiddleRight,
			normal = { textColor = new Color(0.9f, 0.9f, 0.9f, 1f) },
		};

		static readonly GUIStyle DefaultButtonStyle = GetButtonStyle(LoadTexture("ButtonTexture"));
		static readonly GUIStyle OffButtonStyle = GetButtonStyle(LoadTexture("ButtonTextureRed"));
		static readonly GUIStyle OnButtonStyle = GetButtonStyle(LoadTexture("ButtonTextureGreen"));

		static readonly Vector2 DefaultWindowPosition = new Vector2(431, 25);
		static readonly Vector2 InitialWindowSize = new Vector2(160, 175);

		static readonly int WindowId = nameof(AutoAction).GetHashCode();
	}
}

