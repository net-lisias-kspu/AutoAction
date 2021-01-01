using System;

namespace AutoAction
{
	class FacilitySettings
	{
		public bool ActivateAbort;
		public bool ActivateBrakes;
		public bool ActivateRCS;
		public bool ActivateSAS;
		public bool SetPrecCtrl;
		public bool Stage;

		public string SetThrottleString = "0";
		public int SetThrottle
		{
			get => SetThrottleString.ParseNullableInt(0, 100) ?? 0;
			set => SetThrottleString = value.ToStringValue();
		}

		public bool AlreadyShown;

		public void Save(ConfigNode node)
		{
			node.SetValue(nameof(ActivateAbort ), ActivateAbort .ToStringValue(), true);
			node.SetValue(nameof(ActivateBrakes), ActivateBrakes.ToStringValue(), true);
			node.SetValue(nameof(ActivateRCS   ), ActivateRCS   .ToStringValue(), true);
			node.SetValue(nameof(ActivateSAS   ), ActivateSAS   .ToStringValue(), true);
			node.SetValue(nameof(SetPrecCtrl   ), SetPrecCtrl   .ToStringValue(), true);
			node.SetValue(nameof(Stage         ), Stage         .ToStringValue(), true);
			node.SetValue(nameof(SetThrottle   ), SetThrottle   .ToStringValue(), true);
			node.SetValue(nameof(AlreadyShown  ), AlreadyShown  .ToStringValue(), true);
		}

		public void Load(ConfigNode node)
		{
			ActivateAbort  = node.GetValue(nameof(ActivateAbort )).ParseNullableBool() ?? false;
			ActivateBrakes = node.GetValue(nameof(ActivateBrakes)).ParseNullableBool() ?? false;
			ActivateRCS    = node.GetValue(nameof(ActivateRCS   )).ParseNullableBool() ?? false;
			ActivateSAS    = node.GetValue(nameof(ActivateSAS   )).ParseNullableBool() ?? false;
			SetPrecCtrl    = node.GetValue(nameof(SetPrecCtrl   )).ParseNullableBool() ?? false;
			Stage          = node.GetValue(nameof(Stage         )).ParseNullableBool() ?? false;
			SetThrottle    = node.GetValue(nameof(SetThrottle   )).ParseNullableInt(minValue: 0, maxValue: 100) ?? 0;
			AlreadyShown   = node.GetValue(nameof(AlreadyShown  )).ParseNullableBool() ?? false;
		}
	}
}
