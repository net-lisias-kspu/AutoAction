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
using System.Linq;

using UnityEngine;

namespace AutoAction
{
	static class ParsingExtensions
	{
		public static string ToStringValue(this int value) =>
			value.ToString(CultureInfo.InvariantCulture);

		public static string ToStringValue(this int? nullableInt, string nullValue = "") =>
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

		public static string ToStringValue(this int?[] array) =>
			string.Join(",", array.Select(v => v.ToStringValue()).ToArray<string>()).TrimEnd(',');

		public static int? ParseNullableInt(this string text, int minValue = int.MinValue, int maxValue = int.MaxValue) =>
			text is object
				? int.TryParse(text.Replace("−", "-"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
					? minValue <= value && value <= maxValue
						? value
						: (int?)null
					: (int?)null
				: (int?)null;

		public static bool? ParseNullableBool(this string text, bool invertedCompatibilityValue = false, string falseValue = "Off", string trueValue = "On") =>
			text is object
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

		public static Vector2? ParseNullableVector2(this string text)
		{
			string[] parts = text.Split(',');
			return parts.Length == 2 && float.TryParse(parts[0].Trim(), out float x) && float.TryParse(parts[1].Trim(), out float y)
				? new Vector2(x, y)
				: (Vector2?)null;
		}

		public static int?[] ParseNullableIntArray(this string text, int count)
		{
			int?[] values = text?.Split(',').Select(s => s.Trim().ParseNullableInt()).ToArray() ?? new int?[count];
			return values.Length == count
				? values
				: values.Take(count).Concat(Enumerable.Repeat<int?>(null, Math.Max(0, count - values.Length))).ToArray();
		}
	}
}
