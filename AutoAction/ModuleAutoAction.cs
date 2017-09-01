using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoAction
{
	class ModuleAutoAction : PartModule
	{
		// Has this module been activated in flight?
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public bool hasActivated = false;

		// String type for compatibility
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string activateAbort = "";
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string activateBrakes = "";
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string activateGear = "";
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string activateLights = "";
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string activateRCS = "";
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string activateSAS = "";

		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string setThrottle = "";
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string setPrecCtrl = "";

		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string activateGroupA = "";
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string activateGroupB = "";
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string activateGroupC = "";
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string activateGroupD = "";
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string activateGroupE = "";

		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string setPitchTrim = "";
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string setYawTrim = "";
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string setRollTrim = "";
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string setWheelMotorTrim = "";
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public string setWheelSteerTrim = "";


		public bool? ActivateAbort
		{
			get => activateAbort.ParseNullableBool();
			set => activateAbort = value.ToStringValue();
		}

		public bool? ActivateBrakes
		{
			get => activateBrakes.ParseNullableBool();
			set => activateBrakes = value.ToStringValue();
		}

		public bool? ActivateGear
		{
			get => activateGear.ParseNullableBool(invertedCompatibilityValue: true);
			set => activateGear = value.ToStringValue();
		}

		public bool? ActivateLights
		{
			get => activateLights.ParseNullableBool();
			set => activateLights = value.ToStringValue();
		}

		public bool? ActivateRcs
		{
			get => activateRCS.ParseNullableBool();
			set => activateRCS = value.ToStringValue();
		}

		public bool? ActivateSas
		{
			get => activateSAS.ParseNullableBool();
			set => activateSAS = value.ToStringValue();
		}

		public int? SetThrottle
		{
			get => setThrottle.ParseNullableInt(minValue: 0, maxValue: 100);
			set => setThrottle = value.ToStringValue(nullValue: "Default");
		}

		public bool? SetPrecCtrl
		{
			get => setPrecCtrl.ParseNullableBool();
			set => setPrecCtrl = value.ToStringValue();
		}

		public int? ActivateGroupA
		{
			get => activateGroupA.ParseNullableInt(minValue: 1);
			set => activateGroupA = value.ToStringValue();
		}

		public int? ActivateGroupB
		{
			get => activateGroupB.ParseNullableInt(minValue: 1);
			set => activateGroupB = value.ToStringValue();
		}

		public int? ActivateGroupC
		{
			get => activateGroupC.ParseNullableInt(minValue: 1);
			set => activateGroupC = value.ToStringValue();
		}

		public int? ActivateGroupD
		{
			get => activateGroupD.ParseNullableInt(minValue: 1);
			set => activateGroupD = value.ToStringValue();
		}

		public int? ActivateGroupE
		{
			get => activateGroupE.ParseNullableInt(minValue: 1);
			set => activateGroupE = value.ToStringValue();
		}

		public int SetPitchTrim
		{
			get => setPitchTrim.ParseNullableInt(minValue: -500, maxValue: 500) ?? 0;
			set => setPitchTrim = value.ToStringValue();
		}

		public int SetYawTrim
		{
			get => setYawTrim.ParseNullableInt(minValue: -500, maxValue: 500) ?? 0;
			set => setYawTrim = value.ToStringValue();
		}

		public int SetRollTrim
		{
			get => setRollTrim.ParseNullableInt(minValue: -500, maxValue: 500) ?? 0;
			set => setRollTrim = value.ToStringValue();
		}

		public int SetWheelMotorTrim
		{
			get => setWheelMotorTrim.ParseNullableInt(minValue: -500, maxValue: 500) ?? 0;
			set => setWheelMotorTrim = value.ToStringValue();
		}

		public int SetWheelSteerTrim
		{
			get => setWheelSteerTrim.ParseNullableInt(minValue: -500, maxValue: 500) ?? 0;
			set => setWheelSteerTrim = value.ToStringValue();
		}
	}
}
