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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoAction
{
	static class ModuleExtensions
	{
		public static VesselSettings GetVesselSettings(this IEnumerable<Part> parts)
		{
			Debug.Log($"[{nameof(AutoAction)}] GetVesselSettings");

			return parts
				.SelectMany(part => part.Modules.OfType<ModuleAutoAction>())
				.FirstOrDefault(module => module.VesselSettings?.HasNonDefaultValues == true)
				?.VesselSettings ?? new VesselSettings();
		}

		public static void UpdateVesselSettings(this IEnumerable<Part> parts, VesselSettings vesselSettings)
		{
			Debug.Log($"[{nameof(AutoAction)}] UpdateVesselSettings");

			// removing all AutoAction modules
			foreach(var part in parts)
			{
				var autoActionModules = part.Modules.OfType<ModuleAutoAction>().ToList();
				foreach(var module in autoActionModules)
					part.RemoveModule(module);
			}

			// adding an AutoAction module
			if(vesselSettings.HasNonDefaultValues)
			{
				var part = parts
					.Where(p => p.Modules.OfType<ModuleCommand>().Any() || p.Modules.OfType<KerbalSeat>().Any())
					.FirstOrDefault();
				var module = part.AddModule(nameof(ModuleAutoAction)) as ModuleAutoAction;
				module.VesselSettings = vesselSettings;
			}
		}
	}
}
