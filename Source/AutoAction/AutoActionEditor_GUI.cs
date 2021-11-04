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
using KSP.Localization;
using UnityEngine;
using Asset = KSPe.IO.Asset<AutoAction.Startup>;

namespace AutoAction
{
	public partial class AutoActionEditor
	{
		static readonly int WindowId = nameof(AutoAction).GetHashCode();
		static readonly string WindowTitle = Localizer.GetStringByTag("#ModAutoAction_Title");
		static readonly float Width = Localizer.GetStringByTag("#ModAutoAction_WindowWidth").ParseNullableInt() ?? 160;

		#region Styles

		static readonly GUISkin Skin = HighLogic.Skin;
		static readonly GUIStyle WindowStyle = new GUIStyle(Skin.window) { padding = new RectOffset(6, 8, 26, 8) };

		static GUIStyle GetButtonStyle(string texname)
		{
			Texture2D texture = Asset.Texture2D.LoadFromFile("Textures", texname);
			return new GUIStyle(Skin.button)
			{
				alignment = TextAnchor.MiddleCenter,
				fontStyle = FontStyle.Normal,
				margin = new RectOffset(),
				padding = new RectOffset(4, 0, 3, 2),
				normal = { background = texture },
				hover = { background = texture },
			};
		}

		static readonly GUIStyle DefaultButtonStyle = GetButtonStyle("ButtonTexture");
		static readonly GUIStyle OffButtonStyle = GetButtonStyle("ButtonTextureRed");
		static readonly GUIStyle OnButtonStyle = GetButtonStyle("ButtonTextureGreen");

		GUIStyle GetButtonStyleByValue(bool? value)
		{
			switch(value)
			{
				case false	: return OffButtonStyle;
				case true	: return OnButtonStyle;
				default		: return DefaultButtonStyle;
			};
		}

		static readonly GUIStyle LabelStyle = new GUIStyle(Skin.label)
		{
			wordWrap = false,
			alignment = TextAnchor.MiddleLeft,
			stretchWidth = true,
			margin = new RectOffset(),
			padding = new RectOffset(1, 1, 3, 2),
		};
		static readonly GUIStyle SectionLabelStyle = new GUIStyle(LabelStyle) { padding = new RectOffset(1, 1, 0, 2) };
		static readonly GUIStyle DefaultThrottleLabelStyle = new GUIStyle(LabelStyle) { alignment = TextAnchor.MiddleCenter };
		static readonly GUIStyle PercentLabelStyle = new GUIStyle(LabelStyle) { alignment = TextAnchor.MiddleRight };
		static readonly GUIStyle CustomActionsLabelStyle = new GUIStyle(LabelStyle) { padding = new RectOffset(1, 1, 3, 0) };
		static readonly GUIStyle TrimLabelStyle = new GUIStyle(LabelStyle)
		{
			alignment = TextAnchor.MiddleRight,
			padding = new RectOffset(1, 6, 3, 2),
		};

		static readonly GUIStyle NumericFieldStyle = new GUIStyle(Skin.textField)
		{
			fontStyle = FontStyle.Normal,
			normal = { textColor = Color.white },
			hover = { textColor = Color.white },
			alignment = TextAnchor.MiddleCenter,
			margin = new RectOffset(1, 0, 3, 1),
			padding = new RectOffset(3, 1, 0, 1),
		};
		static readonly GUIStyle InvalidFieldStyle = new GUIStyle(NumericFieldStyle)
		{
			normal = { textColor = Color.red },
			hover = { textColor = Color.red },
		};

		static readonly GUILayoutOption LabelSize = GUILayout.ExpandWidth(true);
		static readonly GUILayoutOption ButtonSize = GUILayout.MinWidth(Width / 2);
		static readonly GUILayoutOption ExpandButtonSize = GUILayout.Width(32);
		static readonly GUILayoutOption ActionSetButtonSize = GUILayout.Width(20);
		static readonly GUILayoutOption NumberUpDownButtonSize = GUILayout.Width(16);
		static readonly GUILayoutOption CustomActionFieldSize = GUILayout.Width(Width / VesselSettings.CustomGroupCount - 1);
		static readonly GUILayoutOption ThrottleFieldSize = GUILayout.Width(Width / 2 - 21);
		static readonly GUILayoutOption TrimFieldSize = GUILayout.Width(40);
		static readonly GUILayoutOption PercentLabelSize = GUILayout.Width(20);
		static readonly GUILayoutOption DefaultThrottleLabelSize = GUILayout.Width(Width / 2);

		#endregion

		void WindowGUI(int windowId)
		{
			if(_vesselSettings is object)
			{
				DrawSectionTitle("#ModAutoAction_PerVesselSettings");
				GUILayout.Space(2);
				GUILayout.BeginHorizontal();
				GUILayout.Space(10);
				GUILayout.BeginVertical();
				DrawVesselSettings();
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
				GUILayout.Space(6);
			}

			if(!_facilitySettings.AlreadyShown)
			{
				_isDefaultsSectionExpanded = true;
				_facilitySettings.AlreadyShown = true;
			}
			DrawExpandableSectionTitle(ref _isDefaultsSectionExpanded, _facilityDefaultsTitle);
			if(_isDefaultsSectionExpanded)
			{
				GUILayout.Space(2);
				GUILayout.BeginHorizontal();
				GUILayout.Space(10);
				GUILayout.BeginVertical();
				DrawFacilitySettings();
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
			}

			GUI.DragWindow();
		}

		void DrawVesselSettings()
		{
			// Basic groups
			GUILayout.BeginHorizontal();
			DrawThreeStateButton(ref _vesselSettings.ActivateAbort, "#ModAutoAction_Abort");
			DrawThreeStateButton(ref _vesselSettings.ActivateBrakes, "#ModAutoAction_Brakes");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			DrawThreeStateButton(ref _vesselSettings.ActivateRCS, "#ModAutoAction_Rcs");
			DrawThreeStateButton(ref _vesselSettings.ActivateSAS, "#ModAutoAction_Sas");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			DrawThreeStateButton(ref _vesselSettings.ActivateGear, "#ModAutoAction_Gear");
			DrawThreeStateButton(ref _vesselSettings.ActivateLights, "#ModAutoAction_Lights");
			GUILayout.EndHorizontal();

			// Custom groups
			GUILayout.Label(Localizer.Format("#ModAutoAction_CustomActions"), CustomActionsLabelStyle);
			GUILayout.BeginHorizontal();
			for(int i = 0; i < VesselSettings.CustomGroupCount; i++)
				DrawCustomAction(ref _vesselSettings.CustomGroupStrings[i]);
			GUILayout.EndHorizontal();

			// Action sets
			DrawActionSet(ref _vesselSettings.ActionSet);
			GUILayout.Space(3);

			// Additional settings
			DrawThreeStateButton(ref _vesselSettings.SetPrecCtrl, "#ModAutoAction_PCtrl");
			DrawThreeStateButton(ref _vesselSettings.Stage, "#ModAutoAction_Stage");

			// Throttle
			GUILayout.BeginHorizontal();
			bool hasThrottle = _vesselSettings.SetThrottleString is object;
			if(GUILayout.Button(Localizer.Format("#ModAutoAction_Throttle"), hasThrottle ? OnButtonStyle : DefaultButtonStyle, ButtonSize))
			{
				hasThrottle = !hasThrottle;
				_vesselSettings.SetThrottle = hasThrottle ? _facilitySettings.SetThrottle : (int?) null;
				UpdateVesselSettings();
			}
			if(hasThrottle)
				DrawThrottleSetting(ref _vesselSettings.SetThrottleString);
			else
				GUILayout.Label(Localizer.Format("#ModAutoAction_Default"), DefaultThrottleLabelStyle, DefaultThrottleLabelSize);
			GUILayout.EndHorizontal();

			// Trim
			GUILayout.Space(3);
			DrawExpandableSectionTitle(ref _isTrimSectionExpanded, "#ModAutoAction_Trim");
			if(_isTrimSectionExpanded)
			{
				DrawTrimSettings(ref _vesselSettings.SetPitchTrimString, "#ModAutoAction_Pitch");
				DrawTrimSettings(ref _vesselSettings.SetYawTrimString, "#ModAutoAction_Yaw");
				DrawTrimSettings(ref _vesselSettings.SetRollTrimString, "#ModAutoAction_Roll");
				DrawTrimSettings(ref _vesselSettings.SetWheelMotorTrimString, "#ModAutoAction_WheelThrottle");
				DrawTrimSettings(ref _vesselSettings.SetWheelSteerTrimString, "#ModAutoAction_WheelSteer");
			}
		}

		void DrawFacilitySettings()
		{
			GUILayout.BeginHorizontal();
			DrawOnOffButton(ref _facilitySettings.ActivateAbort, "#ModAutoAction_Abort");
			DrawOnOffButton(ref _facilitySettings.ActivateBrakes, "#ModAutoAction_Brakes");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			DrawOnOffButton(ref _facilitySettings.ActivateRCS, "#ModAutoAction_Rcs");
			DrawOnOffButton(ref _facilitySettings.ActivateSAS, "#ModAutoAction_Sas");
			GUILayout.EndHorizontal();

			DrawOnOffButton(ref _facilitySettings.SetPrecCtrl, "#ModAutoAction_PCtrl");
			DrawOnOffButton(ref _facilitySettings.Stage, "#ModAutoAction_Stage");

			GUILayout.BeginHorizontal();
			GUILayout.Label(Localizer.Format("#ModAutoAction_Throttle"), LabelStyle, LabelSize);
			DrawThrottleSetting(ref _facilitySettings.SetThrottleString);
			GUILayout.EndHorizontal();
		}

		void DrawSectionTitle(string text) =>
			GUILayout.Label(Localizer.Format(text), SectionLabelStyle, LabelSize);

		void DrawExpandableSectionTitle(ref bool isExpanded, string text)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(Localizer.Format(text), LabelStyle, LabelSize);
			if(GUILayout.Button(isExpanded ? "▲" : "▼", DefaultButtonStyle, ExpandButtonSize))
			{
				isExpanded = !isExpanded;
				_windowRectangle.height = 0;
			}
			GUILayout.EndHorizontal();
		}

		void DrawOnOffButton(ref bool value, string text)
		{
			if(GUILayout.Button(Localizer.Format(text), GetButtonStyleByValue(value), ButtonSize))
				value = !value;
		}

		void DrawThreeStateButton(ref bool? value, string text)
		{
			if(GUILayout.Button(Localizer.Format(text), GetButtonStyleByValue(value), ButtonSize))
			{
				switch(value)
				{
					case true	: value = false; break;
					case false	: value = null; break;
					default		: value = true; break;
				};
				UpdateVesselSettings();
			}
		}

		void DrawActionSet(ref int? value)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(Localizer.Format("#ModAutoAction_ActionSet"), LabelStyle, LabelSize);
			for(int i = 1; i <= 4; i++)
				if(GUILayout.Button(i.ToStringValue(), value == i ? OnButtonStyle : DefaultButtonStyle, ActionSetButtonSize))
				{
					value = value == i ? (int?) null : i;
					UpdateVesselSettings();
				}
			GUILayout.EndHorizontal();
		}

		void DrawThrottleSetting(ref string value)
		{
			DrawNumericField(ref value, ThrottleFieldSize, maxLength: 3, minValue: 0, maxValue: 100);
			GUILayout.Label("%", PercentLabelStyle, PercentLabelSize);
		}

		void DrawCustomAction(ref string value) =>
			DrawNumericField(ref value, CustomActionFieldSize, maxLength: 3, minValue: 1, maxValue: 999);

		void DrawTrimSettings(ref string value, string text)
		{
			GUILayout.BeginHorizontal();

			GUILayout.Label(Localizer.Format(text), TrimLabelStyle, LabelSize);

			DrawNumericField(ref value, TrimFieldSize, maxLength: 4, minValue: -500, maxValue: 500);

			if(GUILayout.Button("−", DefaultButtonStyle, NumberUpDownButtonSize))
			{
				value = Math.Max(-500, (value.ParseNullableInt() ?? 0) - 1).ToStringSigned();
				UpdateVesselSettings();
			}
			if(GUILayout.Button("+", DefaultButtonStyle, NumberUpDownButtonSize))
			{
				value = Math.Min(500, (value.ParseNullableInt() ?? 0) + 1).ToStringSigned();
				UpdateVesselSettings();
			}

			GUILayout.EndHorizontal();
		}

		void DrawNumericField(ref string value, GUILayoutOption size, int maxLength, int minValue = int.MinValue, int maxValue = int.MaxValue)
		{
			var style = value.ParseNullableInt(minValue, maxValue).HasValue ? NumericFieldStyle : InvalidFieldStyle;
			var newValue = GUILayout.TextField(value ?? "", maxLength, style, size);
			if(newValue != value)
			{
				value = newValue;
				UpdateVesselSettings();
			}
		}
	}
}
