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
using System.Linq;
using UnityEngine;

namespace AutoAction
{
	class VesselSettings
	{
		public bool? ActivateAbort;
		public bool? ActivateBrakes;
		public bool? ActivateGear;
		public bool? ActivateLights;
		public bool? ActivateRCS;
		public bool? ActivateSAS;
		public bool? SetPrecCtrl;
		public bool? Stage;
		public int? ActionSet;

		public string SetThrottleString;
		public int? SetThrottle
		{
			get => SetThrottleString?.ParseNullableInt(0, 100);
			set => SetThrottleString = value?.ToStringValue();
		}

		public string[] CustomGroupStrings = new string[CustomGroupCount];
		public int?[] CustomGroups
		{
			get => CustomGroupStrings.Select(s => s.ParseNullableInt(1, 999)).ToArray();
			set => CustomGroupStrings = value.Select(v => v.ToStringValue()).ToArray();
		}


		public string SetPitchTrimString = "0";
		public int SetPitchTrim
		{
			get => SetPitchTrimString?.ParseNullableInt(-500, 500) ?? 0;
			set => SetPitchTrimString = value.ToStringSigned();
		}

		public string SetYawTrimString = "0";
		public int SetYawTrim
		{
			get => SetYawTrimString?.ParseNullableInt(-500, 500) ?? 0;
			set => SetYawTrimString = value.ToStringSigned();
		}

		public string SetRollTrimString = "0";
		public int SetRollTrim
		{
			get => SetRollTrimString?.ParseNullableInt(-500, 500) ?? 0;
			set => SetRollTrimString = value.ToStringSigned();
		}

		public string SetWheelMotorTrimString = "0";
		public int SetWheelMotorTrim
		{
			get => SetWheelMotorTrimString?.ParseNullableInt(-500, 500) ?? 0;
			set => SetWheelMotorTrimString = value.ToStringSigned();
		}

		public string SetWheelSteerTrimString = "0";
		public int SetWheelSteerTrim
		{
			get => SetWheelSteerTrimString?.ParseNullableInt(-500, 500) ?? 0;
			set => SetWheelSteerTrimString = value.ToStringSigned();
		}

		public void Save(ConfigNode node)
		{
			if(ActivateAbort .HasValue) node.SetValue(nameof(ActivateAbort    ), ActivateAbort    .ToStringValue(), true);
			if(ActivateBrakes.HasValue) node.SetValue(nameof(ActivateBrakes   ), ActivateBrakes   .ToStringValue(), true);
			if(ActivateGear  .HasValue) node.SetValue(nameof(ActivateGear     ), ActivateGear     .ToStringValue(), true);
			if(ActivateLights.HasValue) node.SetValue(nameof(ActivateLights   ), ActivateLights   .ToStringValue(), true);
			if(ActivateRCS   .HasValue) node.SetValue(nameof(ActivateRCS      ), ActivateRCS      .ToStringValue(), true);
			if(ActivateSAS   .HasValue) node.SetValue(nameof(ActivateSAS      ), ActivateSAS      .ToStringValue(), true);
			if(SetPrecCtrl   .HasValue) node.SetValue(nameof(SetPrecCtrl      ), SetPrecCtrl      .ToStringValue(), true);
			if(Stage         .HasValue) node.SetValue(nameof(Stage            ), Stage            .ToStringValue(), true);
			if(SetThrottle   .HasValue) node.SetValue(nameof(SetThrottle      ), SetThrottle      .ToStringValue(), true);
			if(ActionSet     .HasValue) node.SetValue(nameof(ActionSet        ), ActionSet        .ToStringValue(), true);
			if(HasCustomGroups        ) node.SetValue(nameof(CustomGroups     ), CustomGroups     .ToStringValue(), true);
			if(SetPitchTrim       != 0) node.SetValue(nameof(SetPitchTrim     ), SetPitchTrim     .ToStringValue(), true);
			if(SetYawTrim         != 0) node.SetValue(nameof(SetYawTrim       ), SetYawTrim       .ToStringValue(), true);
			if(SetRollTrim        != 0) node.SetValue(nameof(SetRollTrim      ), SetRollTrim      .ToStringValue(), true);
			if(SetWheelMotorTrim  != 0) node.SetValue(nameof(SetWheelMotorTrim), SetWheelMotorTrim.ToStringValue(), true);
			if(SetWheelSteerTrim  != 0) node.SetValue(nameof(SetWheelSteerTrim), SetWheelSteerTrim.ToStringValue(), true);
		}

		public void Load(ConfigNode node)
		{
			ActivateAbort  = node.GetValue(nameof(ActivateAbort )).ParseNullableBool();
			ActivateBrakes = node.GetValue(nameof(ActivateBrakes)).ParseNullableBool();
			ActivateGear   = node.GetValue(nameof(ActivateGear  )).ParseNullableBool(invertedCompatibilityValue: true);
			ActivateLights = node.GetValue(nameof(ActivateLights)).ParseNullableBool();
			ActivateRCS    = node.GetValue(nameof(ActivateRCS   )).ParseNullableBool();
			ActivateSAS    = node.GetValue(nameof(ActivateSAS   )).ParseNullableBool();
			SetPrecCtrl    = node.GetValue(nameof(SetPrecCtrl   )).ParseNullableBool();
			Stage          = node.GetValue(nameof(Stage         )).ParseNullableBool();
			SetThrottle    = node.GetValue(nameof(SetThrottle   )).ParseNullableInt(minValue: 0, maxValue: 100);
			ActionSet      = node.GetValue(nameof(ActionSet     )).ParseNullableInt(minValue: 1, maxValue: 4);

			int?[] customGroups = node.GetValue(nameof(CustomGroups)).ParseNullableIntArray(CustomGroupCount);
			for (int i = 0; i < CustomGroupCount; i++) // Older version compatibility
				if(customGroups[i] is null)
					customGroups[i] = node.GetValue("ActivateGroup" + (char) ('A' + i)).ParseNullableInt(minValue: 1, maxValue: 999);
			CustomGroups = customGroups;

			SetPitchTrim      = node.GetValue(nameof(SetPitchTrim     )).ParseNullableInt(minValue: -500, maxValue: 500) ?? 0;
			SetYawTrim        = node.GetValue(nameof(SetYawTrim       )).ParseNullableInt(minValue: -500, maxValue: 500) ?? 0;
			SetRollTrim       = node.GetValue(nameof(SetRollTrim      )).ParseNullableInt(minValue: -500, maxValue: 500) ?? 0;
			SetWheelMotorTrim = node.GetValue(nameof(SetWheelMotorTrim)).ParseNullableInt(minValue: -500, maxValue: 500) ?? 0;
			SetWheelSteerTrim = node.GetValue(nameof(SetWheelSteerTrim)).ParseNullableInt(minValue: -500, maxValue: 500) ?? 0;
		}

		public bool HasNonDefaultTrim =>
			SetPitchTrim      != 0 ||
			SetYawTrim        != 0 ||
			SetRollTrim       != 0 ||
			SetWheelMotorTrim != 0 ||
			SetWheelSteerTrim != 0;

		public bool HasCustomGroups =>
			CustomGroups.Any(v => v.HasValue);

		public bool HasNonDefaultValues =>
			ActivateAbort .HasValue ||
			ActivateBrakes.HasValue ||
			ActivateGear  .HasValue ||
			ActivateLights.HasValue ||
			ActivateRCS   .HasValue ||
			ActivateSAS   .HasValue ||
			SetPrecCtrl   .HasValue ||
			Stage         .HasValue ||
			SetThrottle   .HasValue ||
			ActionSet     .HasValue ||
			HasCustomGroups || HasNonDefaultTrim;

		public const int CustomGroupCount = 5;
	}
}
