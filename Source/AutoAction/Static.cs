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
using System.Globalization;

namespace AutoAction
{
	using Data = KSPe.IO.Data<AutoActionEditor>;

	static class Static
	{
        public static readonly Data.ConfigNode SETTINGS_FILE = Data.ConfigNode.For("AutoAction", "AutoAction.settings");

		// Conversion extension methods

		public static string ToStringValue(this float value) =>
			value.ToString(CultureInfo.InvariantCulture);

		public static string ToStringValue(this int value) =>
			value.ToString(CultureInfo.InvariantCulture);

		public static string ToStringValue(this int? nullableInt, string nullValue = "None") =>
			nullableInt?.ToStringValue() ?? nullValue;

		public static string ToStringSigned(this int value) =>
			value != 0 ? (value > 0 ? "+" : "−") + Math.Abs(value).ToString(CultureInfo.InvariantCulture) : "0";

		public static string ToStringValue(this bool value, string falseValue = "Off", string trueValue = "On") =>
			value
				? trueValue
				: falseValue;

		public static string ToStringValue(this bool? nullableBool, string nullValue = "Default", string falseValue = "Off", string trueValue = "On") =>
			nullableBool.HasValue
				? nullableBool.Value.ToStringValue(falseValue, trueValue)
				: nullValue;

		public static int? ParseNullableInt(this string text, int minValue = int.MinValue, int maxValue = int.MaxValue) =>
			text != null
				? int.TryParse(text.Replace("−", "-"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
					? minValue <= value && value <= maxValue
						? value
						: (int?)null
					: (int?)null
				: (int?)null;

		public static bool? ParseNullableBool(this string text, bool invertedCompatibilityValue = false, string falseValue = "Off", string trueValue = "On") =>
			text != null
				? bool.TryParse(text, out bool compatibilityValue)
					// Older version compatibility
					? compatibilityValue
						? !invertedCompatibilityValue
						: (bool?)null
					// Current version
					: text.Equals(trueValue, StringComparison.OrdinalIgnoreCase)
						? true
						: text.Equals(falseValue, StringComparison.OrdinalIgnoreCase)
							? false
							: (bool?)null
				: (bool?)null;
	}
}
