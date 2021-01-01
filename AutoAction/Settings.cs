using System;
using UnityEngine;

namespace AutoAction
{
	class Settings
	{
		public Vector2 WindowPosition { get; set; } = DefaultWindowPosition;
		public FacilitySettings VabSettings { get; } = new FacilitySettings();
		public FacilitySettings SphSettings { get; } = new FacilitySettings();

		public void Save(ConfigNode node)
		{
			node.SetValue(nameof(WindowPosition), WindowPosition, true);

			var vabNode = new ConfigNode();
			VabSettings.Save(vabNode);
			node.SetNode("VAB", vabNode, true);

			var sphNode = new ConfigNode();
			SphSettings.Save(sphNode);
			node.SetNode("SPH", sphNode, true);
		}

		public void Load(ConfigNode node)
		{
			VabSettings.Load(node.GetNode("VAB"));
			SphSettings.Load(node.GetNode("SPH"));
			WindowPosition = node.GetValue(nameof(WindowPosition))?.ParseNullableVector2() ?? DefaultWindowPosition;
		}

		public void Save()
		{
			var node = new ConfigNode();
			Save(node);
			node.Save(SettingsFilePath);
		}

		public void Load()
		{
			var node = ConfigNode.Load(SettingsFilePath);
			if(node is object)
				Load(node);
		}

		static readonly Vector2 DefaultWindowPosition = new Vector2(431, 25);
		static readonly string SettingsFilePath = $"GameData/{nameof(AutoAction)}/Plugins/PluginData/{nameof(AutoAction)}.settings";
	}
}
