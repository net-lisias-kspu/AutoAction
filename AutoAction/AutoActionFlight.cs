using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;


namespace AutoAction
{
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class AutoActionFlight : MonoBehaviour
	{
		static readonly string ModFolderPath = "Diazo/AutoAction/";
		static readonly string FullModFolderPath = KSPUtil.ApplicationRootPath + "GameData/" + ModFolderPath;

		public bool defaultActivateAbort = false;
		public bool defaultActivateGear = true;
		public bool defaultActivateLights = false;
		public bool defaultActivateBrakes = false;
		public bool defaultActivateRCS = false;
		public bool defaultActivateSAS = false;
		public int defaultSetThrottle = 50;
		public bool defaultSetPrecCtrl = false;

		public Part aaRootPart = null;

		public void Start()
		{
			var aaNode = ConfigNode.Load(FullModFolderPath + "AutoAction.cfg"); //load .cfg file
			defaultActivateAbort = aaNode.GetValue("activateAbort") == "On";
			defaultActivateGear = aaNode.GetValue("activateGear") != "Off";
			defaultActivateLights = aaNode.GetValue("activateLights") == "On";
			defaultActivateBrakes = aaNode.GetValue("activateBrakes") == "On";
			defaultActivateRCS = aaNode.GetValue("activateRCS") == "On";
			defaultActivateSAS = aaNode.GetValue("activateSAS") == "On";
			int.TryParse(aaNode.GetValue("setThrottle"), out defaultSetThrottle);
			defaultSetPrecCtrl = aaNode.GetValue("setPrecCtrl") == "On";
		}

		public void Update()
		{
			try
			{//print("AA " + FlightGlobals.ActiveVessel.loaded + " " + FlightGlobals.ActiveVessel.situation + FlightGlobals.ActiveVessel.HoldPhysics);
				if(!FlightGlobals.ActiveVessel.HoldPhysics) //wait until physics is running to tell us everything is loaded, the first Update() acutally runs while on the black loading screen, action states won't match onscreen otherwise (a deployed solar panel drawn as stowed, etc.)
				{
					if(aaRootPart != FlightGlobals.ActiveVessel.rootPart)
					{
						//print("Auto act part change");
						bool aaPMfound = false; //only process first aaPM we find
						foreach(Part p in FlightGlobals.ActiveVessel.parts)
						{
							foreach(ModuleAutoAction aaPM in p.Modules.OfType<ModuleAutoAction>())
							{
								if(!aaPMfound) //if false, first aaPM we've found
								{
									if(!aaPM.hasActivated) //make sure this aaPM has not activated before
									{
										FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.Abort, aaPM.ActivateAbort ?? defaultActivateAbort);
										FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, aaPM.ActivateBrakes ?? defaultActivateBrakes);
										FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.Gear, aaPM.ActivateGear ?? defaultActivateGear);
										FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.Light, aaPM.ActivateLights ?? defaultActivateLights);
										FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.RCS, aaPM.ActivateRCS ?? defaultActivateRCS);
										FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.SAS, aaPM.ActivateSAS ?? defaultActivateSAS);

										if(aaPM.activateGroupA != 0)
											callActionGroup(aaPM.activateGroupA);
										if(aaPM.activateGroupB != 0)
											callActionGroup(aaPM.activateGroupB);
										if(aaPM.activateGroupC != 0)
											callActionGroup(aaPM.activateGroupC);
										if(aaPM.activateGroupD != 0)
											callActionGroup(aaPM.activateGroupD);
										if(aaPM.activateGroupE != 0)
											callActionGroup(aaPM.activateGroupE);
										if(aaPM.activateGroupF != 0)
											callActionGroup(aaPM.activateGroupF);

										var throttle = aaPM.setThrottle >= 0 ? aaPM.setThrottle : defaultSetThrottle;
										if(throttle >= 0)
											FlightInputHandler.state.mainThrottle = Mathf.Max(0, Mathf.Min((float)throttle / 100, 1));

										FlightInputHandler.fetch.precisionMode = aaPM.SetPrecCtrl ?? defaultSetPrecCtrl;
										// todo: Find a way to change the gauge color!
										//foreach(Renderer rend in FlightInputHandler.fetch.inputGaugeRenderers)
										//	rend.material.color = aaPM.SetPrecCtrl ?? defaultSetPrecCtrl
										//		? XKCDColors.BrightCyan
										//		: XKCDColors.Orange;

										aaPM.hasActivated = true; //this aaPM has been processed
										aaPMfound = true;
									}
								}
								else //aaPM has been found and processed already on this vessel, ignore any more aaPM's we find and mark them processed.
								{
									aaPM.hasActivated = true;
								}
							}
						}

						aaRootPart = FlightGlobals.ActiveVessel.rootPart;
					}
				}
				//print("test " + FlightUIController.fetch.stgPitch.Value + "|" + FlightUIController.fetch.stgPitch.pointer.renderer.material.color);
			}
			catch
			{
				Debug.Log("AutoAction Error: Safe to ignore if you did not just launch a new vessel.");
			}
		} //close Update()

		public void callActionGroup(int group)
		{
			if(AGXInterface.AGExtInstalled()) //if agx installed, can activate any group
			{
				AGXInterface.AGExtToggleGroup(group);
			}
			else if(group <= 10) //base KSP can only activate groups 1 through 10
			{
				Dictionary<int, KSPActionGroup> KSPActs = new Dictionary<int, KSPActionGroup>();
				KSPActs[1] = KSPActionGroup.Custom01; //setup list to activate groups
				KSPActs[2] = KSPActionGroup.Custom02;
				KSPActs[3] = KSPActionGroup.Custom03;
				KSPActs[4] = KSPActionGroup.Custom04;
				KSPActs[5] = KSPActionGroup.Custom05;
				KSPActs[6] = KSPActionGroup.Custom06;
				KSPActs[7] = KSPActionGroup.Custom07;
				KSPActs[8] = KSPActionGroup.Custom08;
				KSPActs[9] = KSPActionGroup.Custom09;
				KSPActs[10] = KSPActionGroup.Custom10;
				FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActs[group]);
			}
			else //error catch
			{
				ScreenMessages.PostScreenMessage("Can not Auto Activate group " + group, 10F, ScreenMessageStyle.UPPER_CENTER);
			}
		}
	}
}
