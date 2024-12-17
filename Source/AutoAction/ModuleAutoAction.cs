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
	class ModuleAutoAction : PartModule
	{
		// Has this module been activated in flight?
		[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
		public bool hasActivated = false;

		public VesselSettings VesselSettings { get; set; }

		public override void OnLoad(ConfigNode node)
		{
			if(node.CountNodes > 0)  // not in prefab
			{
				VesselSettings = new VesselSettings();
				VesselSettings.Load(node);
			}
		}

		public override void OnSave(ConfigNode node)
		{
			VesselSettings?.Save(node);
		}
	}
}
