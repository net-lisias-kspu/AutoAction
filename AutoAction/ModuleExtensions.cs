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
					.Where(part => part.Modules.OfType<ModuleCommand>().Any() || part.Modules.OfType<KerbalSeat>().Any())
					.FirstOrDefault();
				var module = part.AddModule(nameof(ModuleAutoAction)) as ModuleAutoAction;
				module.VesselSettings = vesselSettings;
			}
		}
	}
}
