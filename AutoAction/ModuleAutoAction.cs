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
