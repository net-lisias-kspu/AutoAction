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
        public Part aaRootPart = null;

        public void Update()
        {
            try
            {//print("AA " + FlightGlobals.ActiveVessel.loaded + " " + FlightGlobals.ActiveVessel.situation + FlightGlobals.ActiveVessel.HoldPhysics);
                if (!FlightGlobals.ActiveVessel.HoldPhysics) //wait until physics is running to tell us everything is loaded, the first Update() acutally runs while on the black loading screen, action states won't match onscreen otherwise (a deployed solar panel drawn as stowed, etc.)
                {
                    if (aaRootPart != FlightGlobals.ActiveVessel.rootPart)
                    {
                        //print("Auto act part change");
                        bool aaPMfound = false; //only process first aaPM we find
                        foreach (Part p in FlightGlobals.ActiveVessel.parts)
                        {
                            foreach (ModuleAutoAction aaPM in p.Modules.OfType<ModuleAutoAction>())
                            {
                                if (!aaPMfound) //if false, first aaPM we've found
                                {
                                    if (!aaPM.hasActivated) //make sure this aaPM has not activated before
                                    {
                                        if (aaPM.activateAbort)
                                        {
                                            FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Abort);
                                            //activateAbort = false;
                                        }
                                        if (aaPM.activateBrakes)
                                        {
                                            FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Brakes);
                                            //activateBrakes = false;
                                        }
                                        if (aaPM.activateGear)
                                        {
                                            FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Gear);
                                            //activateGear = false;
                                        }
                                        if (aaPM.activateLights)
                                        {
                                            //print("Auto act lights");
                                            FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Light);
                                            //activateLights = false;
                                        }
                                        if (aaPM.activateRCS)
                                        {
                                            FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.RCS);
                                            //activateRCS = false;
                                        }
                                        if (aaPM.activateSAS)
                                        {
                                            FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.SAS);
                                            //activateSAS = false;
                                        }

                                        if (aaPM.activateGroupA != 0)
                                        {
                                            callActionGroup(aaPM.activateGroupA);
                                            //activateGroupA = 0;
                                        }
                                        if (aaPM.activateGroupB != 0)
                                        {
                                            callActionGroup(aaPM.activateGroupB);
                                            //activateGroupB = 0;
                                        }
                                        if (aaPM.activateGroupC != 0)
                                        {
                                            callActionGroup(aaPM.activateGroupC);
                                            //activateGroupC = 0;
                                        }
                                        if (aaPM.activateGroupD != 0)
                                        {
                                            callActionGroup(aaPM.activateGroupD);
                                            //activateGroupD = 0;
                                        }
                                        if (aaPM.activateGroupE != 0)
                                        {
                                            callActionGroup(aaPM.activateGroupE);
                                            //activateGroupE = 0;
                                        }
                                        if (aaPM.activateGroupF != 0)
                                        {
                                            callActionGroup(aaPM.activateGroupF);
                                            //activateGroupF = 0;
                                        }
                                        if (aaPM.setThrottle != -50)
                                        {
                                            //print("throttle set " + aaPM.setThrottle);
                                            FlightInputHandler.state.mainThrottle = Mathf.Max(0, Mathf.Min((float)aaPM.setThrottle / 100, 1)); //set throttle, from 0 to 1
                                            //setThrottle = -50;
                                        }
                                        if (aaPM.setPrecCtrl)
                                        {

                                            FlightInputHandler.fetch.precisionMode = true;
                                            foreach (Renderer rend in FlightInputHandler.fetch.inputGaugeRenderers)
                                            {
                                                //rend.material.color = new Color(0.976f, 0.451f, 0.024f);
                                                rend.material.color = new Color(0.255f, 0.992f, 0.996f);
                                            }

                                        }
                                        else
                                        {


                                            FlightInputHandler.fetch.precisionMode = false;
                                            foreach (Renderer rend in FlightInputHandler.fetch.inputGaugeRenderers)
                                            {
                                                //rend.material.color = new Color(0.255f, 0.992f, 0.996f);
                                                rend.material.color = new Color(0.976f, 0.451f, 0.024f);
                                            }
                                        }
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
            if (AGXInterface.AGExtInstalled()) //if agx installed, can activate any group
            {
                AGXInterface.AGExtToggleGroup(group);
            }
            else if (group <= 10) //base KSP can only activate groups 1 through 10
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
