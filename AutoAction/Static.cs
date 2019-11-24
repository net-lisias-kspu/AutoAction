using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AutoAction
{
	static class Static
	{
		static readonly string FullModFolderPath = KSPUtil.ApplicationRootPath + $"GameData/{nameof(AutoAction)}/";

		public static readonly string TextureFolderPath = nameof(AutoAction) + "/Textures/";

		public static readonly string SettingsFilePath = FullModFolderPath + $"Plugins/PluginData/{nameof(AutoAction)}.settings";


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
