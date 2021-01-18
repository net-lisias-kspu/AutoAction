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

			var vesselSettings = parts
				?.SelectMany(part => part.Modules.OfType<ModuleAutoAction>())
				.Select(module => module.VesselSettings)
				.OfType<VesselSettings>()
				.OrderByDescending(s => s.HasNonDefaultValues)
				.FirstOrDefault();
			return vesselSettings ?? new VesselSettings();
		}

		public static void UpdateVesselSettings(this IReadOnlyList<Part> parts, VesselSettings vesselSettings)
		{
			Debug.Log($"[{nameof(AutoAction)}] UpdateVesselSettings");

			foreach(var module in parts.GetOrAddAutoActionModules())
			{
				module.hasActivated = false;
				// Storing settings in the first module only
				module.VesselSettings = vesselSettings;
				vesselSettings = null;
			}
		}

		public static bool GetHasActivated(this IReadOnlyList<Part> parts)
		{
			Debug.Log($"[{nameof(AutoAction)}] GetHasActivated");

			var modules = parts
				.SelectMany(part => part.Modules.OfType<ModuleAutoAction>())
				.ToList();

			return modules.Count == 0 || modules.All(m => m.hasActivated);
		}

		public static void SetHasActivated(this IReadOnlyList<Part> parts)
		{
			Debug.Log($"[{nameof(AutoAction)}] SetHasActivated");

			foreach(var module in parts.GetOrAddAutoActionModules())
				module.hasActivated = true;
		}

		static IEnumerable<ModuleAutoAction> GetOrAddAutoActionModules(this IEnumerable<Part> parts)
		{
			var commandParts = parts.Where(p =>
				p.Modules.OfType<ModuleCommand>().Any() ||
				p.Modules.OfType<KerbalSeat>().Any());

			foreach(var part in commandParts)
				yield return
					part.Modules.OfType<ModuleAutoAction>().FirstOrDefault() ??
					part.AddModule(nameof(ModuleAutoAction)) as ModuleAutoAction;
		}
	}
}
