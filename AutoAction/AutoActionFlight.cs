﻿using System;
using System.Collections.Generic;
using System.Linq;
using KSP.Localization;
using KSP.UI.Screens;
using UnityEngine;

namespace AutoAction
{
	[KSPAddon(KSPAddon.Startup.Flight, once: false)]
	public class AutoActionFlight : MonoBehaviour
	{
		public void Start()
		{
			Debug.Log($"[{nameof(AutoAction)}] flight: Start");
			GameEvents.OnVesselRollout.Add(OnVesselRollout);
			StartCoroutine(ActivateWhenReady());
		}

		public void OnDestroy()
		{
			Debug.Log($"[{nameof(AutoAction)}] flight: OnDestroy");
			GameEvents.OnVesselRollout.Remove(OnVesselRollout);
		}

		void OnVesselRollout(ShipConstruct _)
		{
			Debug.Log($"[{nameof(AutoAction)}] flight: OnVesselRollout");
			_isRollout = true;
			StartCoroutine(ActivateWhenReady());
		}

		bool _isRollout;
		bool _hasTriggered;

		IEnumerator<YieldInstruction> ActivateWhenReady()
		{
			var wait = new WaitForFixedUpdate();
			while(FlightGlobals.ActiveVessel is null || FlightGlobals.ActiveVessel.HoldPhysics)
				yield return wait;

			if(!_hasTriggered)
			{
				var parts = FlightGlobals.ActiveVessel.Parts;
				var hasActivated = parts.GetHasActivated();
				var vesselSettings = parts.GetVesselSettings();
				var isLanded = FlightGlobals.ActiveVessel.Landed;

				if(_isRollout || isLanded && !hasActivated)
				{
					_hasTriggered = true;
					parts.SetHasActivated();
					Activate(vesselSettings);
				}
			}
		}

		void Activate(VesselSettings vessel)
		{
			Debug.Log($"[{nameof(AutoAction)}] flight: Activate");

			// Loading facility default settings
			var facility = GetFacilitySettings();
			
			// Selecting action set
			if(vessel.ActionSet is int set)
				FlightGlobals.ActiveVessel.SetGroupOverride(set);

			// Activating standard action groups
			var actionGroups = FlightGlobals.ActiveVessel.ActionGroups;
			actionGroups.SetGroup(KSPActionGroup.SAS,    vessel.ActivateSAS    ?? facility.ActivateSAS   );
			actionGroups.SetGroup(KSPActionGroup.RCS,    vessel.ActivateRCS    ?? facility.ActivateRCS   );
			actionGroups.SetGroup(KSPActionGroup.Brakes, vessel.ActivateBrakes ?? facility.ActivateBrakes);
			actionGroups.SetGroup(KSPActionGroup.Abort,  vessel.ActivateAbort  ?? facility.ActivateAbort );
			// Special treatment for the groups with the initial state determined by the part state
			SetAutoInitializingGroup(KSPActionGroup.Gear,  vessel.ActivateGear  );
			SetAutoInitializingGroup(KSPActionGroup.Light, vessel.ActivateLights);

			// Activating custom action groups
			foreach(var customGroup in vessel.CustomGroups.OfType<int>())
				ActivateCustomActionGroup(customGroup);

			// Setting precision control
			SetPrecisionMode(vessel.SetPrecCtrl ?? facility.SetPrecCtrl);

			// Setting throttle
			FlightInputHandler.state.mainThrottle = Mathf.Max(0, Mathf.Min(1, (vessel.SetThrottle ?? facility.SetThrottle) / 100F));

			// Setting trim
			FlightInputHandler.state.pitchTrim         =  TrimStep * vessel.SetPitchTrim;
			FlightInputHandler.state.yawTrim           =  TrimStep * vessel.SetYawTrim;
			FlightInputHandler.state.rollTrim          =  TrimStep * vessel.SetRollTrim;
			FlightInputHandler.state.wheelThrottleTrim =  TrimStep * vessel.SetWheelMotorTrim;
			FlightInputHandler.state.wheelSteerTrim    = -TrimStep * vessel.SetWheelSteerTrim;  // inverted

			// Staging
			if(vessel.Stage ?? facility.Stage)
				StageManager.ActivateNextStage();
		}

		void SetAutoInitializingGroup(KSPActionGroup group, bool? activate)
		{
			if(activate.HasValue)
			{
				FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(group);
				if(activate.Value != FlightGlobals.ActiveVessel.ActionGroups[group])
					FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(group);
			}
		}

		void SetPrecisionMode(bool precisionMode)
		{
			FlightInputHandler.fetch.precisionMode = precisionMode;

			// Changing the gauge color
			var gauges = FindObjectOfType<KSP.UI.Screens.Flight.LinearControlGauges>();
			if(gauges is object)
				foreach(var image in gauges.inputGaugeImages)
					image.color = precisionMode
						? XKCDColors.BrightCyan
						: XKCDColors.Orange;
		}

		static void ActivateCustomActionGroup(int actionGroup)
		{
			// If AGX installed, can activate any group
			if(AgxInterface.IsAgxInstalled())
				AgxInterface.AgxToggleGroup(actionGroup);
			// Base KSP can only activate groups 1 through 10
			else if(actionGroup >= 1 && actionGroup <= 10)
				FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KspActions[actionGroup]);
			// Error catch
			else
				ScreenMessages.PostScreenMessage(Localizer.Format("#ModAutoAction_CanNotActivateGroup", actionGroup), 10F, ScreenMessageStyle.UPPER_CENTER);
		}

		static FacilitySettings GetFacilitySettings()
		{
			Settings settings = new Settings();
			settings.Load();
			return settings.For(ShipConstruction.ShipType);
		}

		static readonly IDictionary<int, KSPActionGroup> KspActions = new Dictionary<int, KSPActionGroup>
		{
			[1]  = KSPActionGroup.Custom01,
			[2]  = KSPActionGroup.Custom02,
			[3]  = KSPActionGroup.Custom03,
			[4]  = KSPActionGroup.Custom04,
			[5]  = KSPActionGroup.Custom05,
			[6]  = KSPActionGroup.Custom06,
			[7]  = KSPActionGroup.Custom07,
			[8]  = KSPActionGroup.Custom08,
			[9]  = KSPActionGroup.Custom09,
			[10] = KSPActionGroup.Custom10,
		};

		const float TrimStep = 0.002F;
	}
}
