/*
	This file is part of Auto Actions /L Unleashed
		© 2018-2024 LisiasT : http://lisias.net <support@lisias.net>
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
