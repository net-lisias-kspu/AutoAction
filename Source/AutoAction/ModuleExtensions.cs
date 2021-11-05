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

			VesselSettings vesselSettings = parts
				?.SelectMany(part => part.Modules.OfType<ModuleAutoAction>())
				.Select(module => module.VesselSettings)
				.OfType<VesselSettings>()
				.OrderByDescending(s => s.HasNonDefaultValues)
				.FirstOrDefault();
			return vesselSettings ?? new VesselSettings();
		}

		public static void UpdateVesselSettings(this IList<Part> parts, VesselSettings vesselSettings)
		{
			Debug.Log($"[{nameof(AutoAction)}] UpdateVesselSettings");

			foreach (ModuleAutoAction module in parts.GetOrAddAutoActionModules())
			{
				module.hasActivated = false;
				// Storing settings in the first module only
				module.VesselSettings = vesselSettings;
				vesselSettings = null;
			}
		}

		public static bool GetHasActivated(this IList<Part> parts)
		{
			Debug.Log($"[{nameof(AutoAction)}] GetHasActivated");

			List<ModuleAutoAction> modules = parts
				.SelectMany(part => part.Modules.OfType<ModuleAutoAction>())
				.ToList();

			return modules.Count == 0 || modules.All(m => m.hasActivated);
		}

		public static void SetHasActivated(this IList<Part> parts)
		{
			Debug.Log($"[{nameof(AutoAction)}] SetHasActivated");

			foreach (ModuleAutoAction module in parts.GetOrAddAutoActionModules())
				module.hasActivated = true;
		}

		static IEnumerable<ModuleAutoAction> GetOrAddAutoActionModules(this IEnumerable<Part> parts)
		{
			IEnumerable<Part> commandParts = parts.Where(p =>
				p.Modules.OfType<ModuleCommand>().Any() ||
				p.Modules.OfType<KerbalSeat>().Any());

			foreach (Part part in commandParts)
				yield return
					part.Modules.OfType<ModuleAutoAction>().FirstOrDefault() ??
					part.AddModule(nameof(ModuleAutoAction)) as ModuleAutoAction;
		}
	}
}
