using System;
using System.Collections.Generic;
using System.Linq;
using KSP.Localization;
using UnityEngine;

namespace AutoAction
{
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class AutoActionFlight : MonoBehaviour
	{
		bool _defaultActivateAbort;
		bool _defaultActivateBrakes;
		//bool _defaultActivateGear = true;
		//bool _defaultActivateLights;
		bool _defaultActivateRcs;
		bool _defaultActivateSas;
		int _defaultSetThrottle;
		bool _defaultSetPrecCtrl;

		Part _rootPart;

		FlightInputHandler _flightHandler;

		public void Start()
		{
			var facilityPrefix = ShipConstruction.ShipType == EditorFacility.SPH ? "SPH" : "VAB";

			// Load defaults from .settings file
			var settings = ConfigNode.Load(Static.SettingsFilePath);
			_defaultActivateAbort = settings.GetValue(facilityPrefix + "activateAbort").ParseNullableBool() ?? false;
			_defaultActivateBrakes = settings.GetValue(facilityPrefix + "activateBrakes").ParseNullableBool() ?? false;
			//_defaultActivateGear = settings.GetValue(facilityPrefix + "activateGear").ParseNullableBool(invertedCompatibilityValue: true) ?? true;
			//_defaultActivateLights = settings.GetValue(facilityPrefix + "activateLights").ParseNullableBool() ?? false;
			_defaultActivateRcs = settings.GetValue(facilityPrefix + "activateRCS").ParseNullableBool() ?? false;
			_defaultActivateSas = settings.GetValue(facilityPrefix + "activateSAS").ParseNullableBool() ?? false;
			_defaultSetThrottle = settings.GetValue(facilityPrefix + "setThrottle").ParseNullableInt(minValue: 0, maxValue: 100) ?? 0;
			_defaultSetPrecCtrl = settings.GetValue(facilityPrefix + "setPrecCtrl").ParseNullableBool() ?? false;

			_flightHandler = FlightInputHandler.fetch;
		}

		public void Update()
		{
			try
			{
				if(!FlightGlobals.ActiveVessel.HoldPhysics)
				{
					if(_rootPart != FlightGlobals.ActiveVessel.rootPart)
					{
						var autoActionPartModules =
							FlightGlobals.ActiveVessel.parts
								.SelectMany(part => part.Modules.OfType<ModuleAutoAction>())
								.Where(module => !module.hasActivated);

						// Only process the first AutoAction part module we find
						bool moduleFound = false;
						foreach(var module in autoActionPartModules)
						{
							if(!moduleFound)
							{
								moduleFound = true;
								ProcessModule(module);
							}
							module.hasActivated = true;
						}

						_rootPart = FlightGlobals.ActiveVessel.rootPart;
					}
				}
			}
			catch
			{
				print("AutoAction Error: Safe to ignore if you did not just launch a new vessel.");
			}
		}

		void ProcessModule(ModuleAutoAction module)
		{
			var actionGroups = FlightGlobals.ActiveVessel.ActionGroups;
			actionGroups.SetGroup(KSPActionGroup.Abort, module.ActivateAbort ?? _defaultActivateAbort);
			actionGroups.SetGroup(KSPActionGroup.Brakes, module.ActivateBrakes ?? _defaultActivateBrakes);
			actionGroups.SetGroup(KSPActionGroup.RCS, module.ActivateRcs ?? _defaultActivateRcs);
			actionGroups.SetGroup(KSPActionGroup.SAS, module.ActivateSas ?? _defaultActivateSas);

			if(module.ActivateGear.HasValue)
			{
				FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Gear);
				if(module.ActivateGear.Value != FlightGlobals.ActiveVessel.ActionGroups[KSPActionGroup.Gear])
					FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Gear);
			}
			if(module.ActivateLights.HasValue)
			{
				FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Light);
				if(module.ActivateLights.Value != FlightGlobals.ActiveVessel.ActionGroups[KSPActionGroup.Light])
					FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Light);
			}

			FlightInputHandler.state.mainThrottle = Mathf.Max(0, Mathf.Min(1, (module.SetThrottle ?? _defaultSetThrottle) / 100F));
			SetPrecisionMode(module.SetPrecCtrl ?? _defaultSetPrecCtrl);

			FlightInputHandler.state.pitchTrim = TrimStep * module.SetPitchTrim;
			FlightInputHandler.state.yawTrim = TrimStep * module.SetYawTrim;
			FlightInputHandler.state.rollTrim = TrimStep * module.SetRollTrim;
			FlightInputHandler.state.wheelThrottleTrim = TrimStep * module.SetWheelMotorTrim;
			FlightInputHandler.state.wheelSteerTrim = -TrimStep * module.SetWheelSteerTrim; // Inverted

			CallActionGroup(module.ActivateGroupA);
			CallActionGroup(module.ActivateGroupB);
			CallActionGroup(module.ActivateGroupC);
			CallActionGroup(module.ActivateGroupD);
			CallActionGroup(module.ActivateGroupE);
		}

		void SetPrecisionMode(bool precisionMode)
		{
			_flightHandler.precisionMode = precisionMode;
			// Change the gauge color
			var gauges = FindObjectOfType<KSP.UI.Screens.Flight.LinearControlGauges>();
			if(gauges != null)
				foreach(var image in gauges.inputGaugeImages)
					image.color = precisionMode
						? XKCDColors.BrightCyan
						: XKCDColors.Orange;
		}

		static void CallActionGroup(int? actionGroup)
		{
			if(actionGroup.HasValue)
			{
				// If AGX installed, can activate any group
				if(AgxInterface.IsAgxInstalled())
					AgxInterface.AgxToggleGroup(actionGroup.Value);
				// Base KSP can only activate groups 1 through 10
				else if(actionGroup <= 10)
					FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KspActions[actionGroup.Value]);
				// Error catch
				else
					ScreenMessages.PostScreenMessage(Localizer.Format("#ModAutoAction_CanNotActivateGroup", actionGroup), 10F, ScreenMessageStyle.UPPER_CENTER);
			}
		}

		static readonly IDictionary<int, KSPActionGroup> KspActions = new Dictionary<int, KSPActionGroup>
		{
			[1] = KSPActionGroup.Custom01,
			[2] = KSPActionGroup.Custom02,
			[3] = KSPActionGroup.Custom03,
			[4] = KSPActionGroup.Custom04,
			[5] = KSPActionGroup.Custom05,
			[6] = KSPActionGroup.Custom06,
			[7] = KSPActionGroup.Custom07,
			[8] = KSPActionGroup.Custom08,
			[9] = KSPActionGroup.Custom09,
			[10] = KSPActionGroup.Custom10
		};

		const float TrimStep = 0.002F;
	}
}
