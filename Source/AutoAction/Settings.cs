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
using UnityEngine;
using USERDATA = KSPe.IO.Data<AutoAction.Startup>;
using SAVE = KSPe.IO.Save<AutoAction.Startup>;

namespace AutoAction
{
	class Settings
	{
		public Vector2 WindowPosition { get; set; } = DefaultWindowPosition;
		public FacilitySettings VabSettings { get; } = new FacilitySettings();
		public FacilitySettings SphSettings { get; } = new FacilitySettings();
		public bool UseSaveGameSettings { get; set; } = false;

		private void Save(ConfigNode node)
		{
			node.SetValue(nameof(WindowPosition), WindowPosition, true);

			ConfigNode vabNode = new ConfigNode();
			VabSettings.Save(vabNode);
			node.SetNode("VAB", vabNode, true);

			ConfigNode sphNode = new ConfigNode();
			SphSettings.Save(sphNode);
			node.SetNode("SPH", sphNode, true);

			node.SetValue("UseSaveGameSettings", UseSaveGameSettings, true);
		}

		private void Load(ConfigNode node)
		{
			VabSettings.Load(node.GetNode("VAB"));
			SphSettings.Load(node.GetNode("SPH"));
			WindowPosition = node.GetValue(nameof(WindowPosition))?.ParseNullableVector2() ?? DefaultWindowPosition;
			{
				bool b = false;
				node.TryGetValue("UseSaveGameSettings", ref b);
				UseSaveGameSettings = b;
			}
		}

		public void Save()
		{
			if(this.UseSaveGameSettings)
			{
				this.Save(SAVEGAME.Node);
				SAVEGAME.Save();
			}
			else
			{
				this.Save(SETTINGS.Node);
				SETTINGS.Save();
			}
		}

		public void Load()
		{
			if(SAVEGAME.IsLoadable)
			{
				SAVEGAME.Load();
				Load(SAVEGAME.Node);
				if(this.UseSaveGameSettings) return;
			}
			// If we get here, the SAVEGAME settings told us it's commiting suicide.
			else if (SETTINGS.IsLoadable) SETTINGS.Load();
			else { SETTINGS.Clear(); this.Save(); }

			Load(SETTINGS.Node);
			if(SAVEGAME.IsLoadable) SAVEGAME.Destroy();
		}

		static readonly Vector2 DefaultWindowPosition = new Vector2(431, 25);
		private static readonly USERDATA.ConfigNode SETTINGS = USERDATA.ConfigNode.For("Settings");

		// Since it's impossible to enter any Editor without having the SaveGame fully loaded,
		// there's no risk on doing things here on the static land.
		//
		// But don't try this on anything that could be loaded before KSP fully populate the HighLogic.fetch.GameSaveFolder!
		private static readonly SAVE.ConfigNode SAVEGAME = SAVE.ConfigNode.For("Settings");

		internal FacilitySettings For(EditorFacility shipType)
		{
			switch (shipType)
			{
				case EditorFacility.SPH: return this.SphSettings;
				case EditorFacility.VAB: return this.VabSettings;
				default: return new FacilitySettings();
			}
		}
	}
}
