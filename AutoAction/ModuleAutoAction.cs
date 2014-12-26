using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

namespace AutoAction
{
    class ModuleAutoAction : PartModule
    {
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)] //has this module been activated in flight?
        public bool hasActivated = false;
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)] //activate these groups?
        public bool activateAbort = false;
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public bool activateBrakes = false;
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public bool activateGear = false;
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public bool activateLights = false;
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)] //activate RCS? same as R key
        public bool activateRCS = false;
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)] //activate SAS? same as T key
        public bool activateSAS = false;
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)] //hard code to 6 groups for screen size reasons
        public int activateGroupA = 0;
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public int activateGroupB = 0;
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public int activateGroupC = 0;
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public int activateGroupD = 0;
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public int activateGroupE = 0;
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public int activateGroupF = 0;
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)]
        public int setThrottle = -50;
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false)] //activate SAS? same as T key
        public bool setPrecCtrl = false;

        //public void Start()
        //{
        //    print("Auto activate run!");
        //    if (HighLogic.LoadedSceneIsFlight)
        //    {
        //        if (activateAbort)
        //        {
        //            this.part.vessel.ActionGroups.ToggleGroup(KSPActionGroup.Abort);
        //            activateAbort = false;
        //        }
        //        if (activateBrakes)
        //        {
        //            FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Brakes);
        //            activateBrakes = false;
        //        }
        //        if (activateGear)
        //        {
        //            FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Gear);
        //            activateGear = false;
        //        }
        //        if (activateLights)
        //        {
        //            FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.Light);
        //            activateLights = false;
        //        }
        //        if (activateRCS)
        //        {
        //            FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.RCS);
        //            activateRCS = false;
        //        }
        //        if (activateSAS)
        //        {
        //            FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActionGroup.SAS);
        //            activateSAS = false;
        //        }
                
        //            if(activateGroupA != 0)
        //            {
        //                callActionGroup(activateGroupA);
        //                activateGroupA = 0;
        //            }
        //            if (activateGroupB != 0)
        //            {
        //                callActionGroup(activateGroupB);
        //                activateGroupB = 0;
        //            }
        //            if (activateGroupC != 0)
        //            {
        //                callActionGroup(activateGroupC);
        //                activateGroupC = 0;
        //            }
        //            if (activateGroupD != 0)
        //            {
        //                callActionGroup(activateGroupD);
        //                activateGroupD = 0;
        //            }
        //            if (activateGroupE != 0)
        //            {
        //                callActionGroup(activateGroupE);
        //                activateGroupE = 0;
        //            }
        //            if (activateGroupF != 0)
        //            {
        //                callActionGroup(activateGroupF);
        //                activateGroupF = 0;
        //            }
        //        if(setThrottle != -50)
        //        {
        //            FlightInputHandler.state.mainThrottle = Mathf.Max(0, Mathf.Min((float)setThrottle / 100, 1)); //set throttle, from 0 to 1
        //            setThrottle = -50;
        //        }
                   
        //    }//close IF statement checking flight scene
        //    print("Auto activate end run");
        //}//close START()

        //public void callActionGroup(int group)
        //{
        //    if(AGXInterface.AGExtInstalled()) //if agx installed, can activate any group
        //    {
        //        AGXInterface.AGExtToggleGroup(group);
        //    }
        //    else if(group <= 10) //base KSP can only activate groups 1 through 10
        //    {
        //        Dictionary<int, KSPActionGroup> KSPActs = new Dictionary<int, KSPActionGroup>();
        //    KSPActs[1] = KSPActionGroup.Custom01; //setup list to activate groups
        //    KSPActs[2] = KSPActionGroup.Custom02;
        //    KSPActs[3] = KSPActionGroup.Custom03;
        //    KSPActs[4] = KSPActionGroup.Custom04;
        //    KSPActs[5] = KSPActionGroup.Custom05;
        //    KSPActs[6] = KSPActionGroup.Custom06;
        //    KSPActs[7] = KSPActionGroup.Custom07;
        //    KSPActs[8] = KSPActionGroup.Custom08;
        //    KSPActs[9] = KSPActionGroup.Custom09;
        //    KSPActs[10] = KSPActionGroup.Custom10;
        //    FlightGlobals.ActiveVessel.ActionGroups.ToggleGroup(KSPActs[group]);
        //    }
        //    else //error catch
        //    {
        //        ScreenMessages.PostScreenMessage("Can not Auto Activate group " + group, 10F, ScreenMessageStyle.UPPER_CENTER);
        //    }
        //}

        
    }
}
