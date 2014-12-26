using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace AutoAction
{

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class AutoActionEditor : MonoBehaviour
    {

        private bool showBasicGroups = false;
        private bool showCustomGroups = false;
        private IButton AABtn; //toolbar button
        private bool AAWinShow = true; //show window?
        private static GUISkin AASkin;
        private static GUIStyle AAWinStyle = null; //window style
        private static GUIStyle AALblStyle = null; //window style
        private static GUIStyle AABtnStyle = null; //window style
        private static GUIStyle AAFldStyle = null; //window style
        public Rect AAWin = new Rect(100, 100, 160, 110); //GUI window rectangle
        Texture2D ButtonTextureRed = new Texture2D(64, 64); //button textures
        Texture2D ButtonTextureGreen = new Texture2D(64, 64);
        Texture2D ButtonTextureGray = new Texture2D(64, 64);

        public bool masterActivateAbort = false; //variables we work with. this is our master "partModule" while in editor
        public bool masterActivateGear = false;
        public bool masterActivateLights = false;
        public bool masterActivateBrakes = false;
        public bool masterActivateRCS = false;
        public bool masterActivateSAS = false;
        public int masterActivateGroupA = 0;
        public int masterActivateGroupB = 0;
        public int masterActivateGroupC = 0;
        public int masterActivateGroupD = 0;
        public int masterActivateGroupE = 0;
        public int masterActivateGroupF = 0;
        public bool masterSetThrottleYes = false;
        public int masterSetThrottle = -50; //end variables
        public bool masterSetPrecCtrl = false; //precise control?
        ApplicationLauncherButton AAEditorButton = null; //stock toolbar button instance
        ConfigNode AANode;

        public void Start()
        {
            print("AutoActions Version 1.3 loaded.");
            AAWinStyle = new GUIStyle(HighLogic.Skin.window); //make our style
            AAFldStyle = new GUIStyle(HighLogic.Skin.textField);
            AAFldStyle.fontStyle = FontStyle.Normal;
            RenderingManager.AddToPostDrawQueue(0, AAOnDraw); //GUI window hook
            byte[] importTxtRed = File.ReadAllBytes(KSPUtil.ApplicationRootPath + "GameData/Diazo/AutoAction/ButtonTextureRed.png"); //load our button textures
            byte[] importTxtGreen = File.ReadAllBytes(KSPUtil.ApplicationRootPath + "GameData/Diazo/AutoAction/ButtonTextureGreen.png");
            byte[] importTxt = File.ReadAllBytes(KSPUtil.ApplicationRootPath + "GameData/Diazo/AutoAction/ButtonTexture.png");
            ButtonTextureGray.LoadImage(importTxt);
            ButtonTextureGray.Apply();
            ButtonTextureGreen.LoadImage(importTxtGreen);
            ButtonTextureGreen.Apply();
            ButtonTextureRed.LoadImage(importTxtRed);
            ButtonTextureRed.Apply();
            AASkin = (GUISkin)MonoBehaviour.Instantiate(HighLogic.Skin);
            AAWinStyle = new GUIStyle(AASkin.window); //GUI skin style
            AALblStyle = new GUIStyle(AASkin.label);
            AAFldStyle = new GUIStyle(AASkin.textField);
            AAFldStyle.fontStyle = FontStyle.Normal;
            AAFldStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            //Font fontTest = Font("calibri");
            //AALblStyle.font = UnityEngine.Font("calibri");
            AALblStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            AALblStyle.wordWrap = false;
            AABtnStyle = new GUIStyle(AASkin.button);
            AABtnStyle.fontStyle = FontStyle.Normal;
            AABtnStyle.alignment = TextAnchor.MiddleCenter;
            if (ToolbarManager.ToolbarAvailable) //check if toolbar available, load if it is
            {
                AABtn = ToolbarManager.Instance.add("AutoAction", "AABtn");
                AABtn.TexturePath = "Diazo/AutoAction/AABtn";
                AABtn.ToolTip = "Auto Actions";
                AABtn.OnClick += (e) =>
                {
                    if (e.MouseButton == 0) //simply show/hide window on click
                    {
                        onStockToolbarClick();

                    }
                };
            }
            else
            {
                //AGXShow = true; //toolbar not installed, show AGX regardless
                //now using stock toolbar as fallback
                AAEditorButton = ApplicationLauncher.Instance.AddModApplication(onStockToolbarClick, onStockToolbarClick, DummyVoid, DummyVoid, DummyVoid, DummyVoid, ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH, (Texture)GameDatabase.Instance.GetTexture("Diazo/AutoAction/AABtn", false));
            }
            AANode = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/Diazo/AutoAction/AutoAction.cfg"); //load .cfg file
            AAWin.x = Convert.ToInt32(AANode.GetValue("WinX"));
            AAWin.y = Convert.ToInt32(AANode.GetValue("WinY"));



            LoadAAPartModule();
            //ScenarioUpgradeableFacilities upgradeScen = HighLogic.CurrentGame.scenarios.OfType<ScenarioUpgradeableFacilities>().First();
            //float edLvl = 0;
            if (AANode.HasValue("OverrideCareer")) //are action groups unlocked?
            {
                //print("b");
                if ((string)AANode.GetValue("OverrideCareer") == "1")
                {
                    //print("c");
                    showCustomGroups = true;
                    showBasicGroups = true;
                }
                else
                {

                    if (EditorDriver.editorFacility == EditorFacility.SPH) //we are in SPH, what action groups are unlocked?
                    {
                        if (GameVariables.Instance.UnlockedActionGroupsCustom(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar)))
                        {
                            showCustomGroups = true;
                            showBasicGroups = true;
                        }
                        else if (GameVariables.Instance.UnlockedActionGroupsStock(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar)))
                        {
                            showCustomGroups = false;
                            showBasicGroups = true;
                        }
                        else
                        {
                            showCustomGroups = false;
                            showBasicGroups = false;
                        }

                    }
                    else //we are in VAB, what action groups are unlocked?
                    {
                        if (GameVariables.Instance.UnlockedActionGroupsCustom(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding)))
                        {
                            showCustomGroups = true;
                            showBasicGroups = true;
                        }
                        else if (GameVariables.Instance.UnlockedActionGroupsStock(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding)))
                        {
                            showCustomGroups = false;
                            showBasicGroups = true;
                        }
                        else
                        {
                            showCustomGroups = false;
                            showBasicGroups = false;
                        }

                    }
                }
            }
            else
            {

                if (EditorDriver.editorFacility == EditorFacility.SPH) //we are in SPH, what action groups are unlocked?
                {
                    if (GameVariables.Instance.UnlockedActionGroupsCustom(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar)))
                    {
                        showCustomGroups = true;
                        showBasicGroups = true;
                    }
                    else if (GameVariables.Instance.UnlockedActionGroupsStock(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar)))
                    {
                        showCustomGroups = false;
                        showBasicGroups = true;
                    }
                    else
                    {
                        showCustomGroups = false;
                        showBasicGroups = false;
                    }

                }
                else //we are in VAB, what action groups are unlocked?
                {
                    if (GameVariables.Instance.UnlockedActionGroupsCustom(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding)))
                    {
                        showCustomGroups = true;
                        showBasicGroups = true;
                    }
                    else if (GameVariables.Instance.UnlockedActionGroupsStock(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding)))
                    {
                        showCustomGroups = false;
                        showBasicGroups = true;
                    }
                    else
                    {
                        showCustomGroups = false;
                        showBasicGroups = false;
                    }

                }
            }
        }//close Start()

        //public void Update()
        //{
        //    print("throttle " + masterSetThrottle + masterSetThrottleYes);
        //}

        public void onStockToolbarClick()
        {
            AAWinShow = !AAWinShow;
        }

        public void DummyVoid()
        {

        }

        public void LoadAAPartModule() //load from partmodule
        {
            try
            {
                foreach (Part p in EditorLogic.SortedShipList)
                {
                    foreach (ModuleAutoAction pmAA in p.Modules.OfType<ModuleAutoAction>())
                    {
                        masterActivateAbort = pmAA.activateAbort;
                        masterActivateGear = pmAA.activateGear;
                        masterActivateLights = pmAA.activateLights;
                        masterActivateBrakes = pmAA.activateBrakes;
                        masterActivateRCS = pmAA.activateRCS;
                        masterActivateSAS = pmAA.activateSAS;
                        masterActivateGroupA = pmAA.activateGroupA;
                        masterActivateGroupB = pmAA.activateGroupB;
                        masterActivateGroupC = pmAA.activateGroupC;
                        masterActivateGroupD = pmAA.activateGroupD;
                        masterActivateGroupE = pmAA.activateGroupE;
                        masterActivateGroupF = pmAA.activateGroupF;
                        if (pmAA.setThrottle != -50)
                        {
                            masterSetThrottle = pmAA.setThrottle;
                            masterSetThrottleYes = true;
                        }
                        else
                        {
                            masterSetThrottle = -50;
                            masterSetThrottleYes = false;
                        }
                        masterSetPrecCtrl = pmAA.setPrecCtrl;
                    }
                }
            }
            catch
            {
                //leave blank, need this to catch the error on entering editor the first time with no vessel loaded because it throws a null ref otherwise
            }
        }

        public void OnDisable()
        {
            if (ToolbarManager.ToolbarAvailable) //if toolbar loaded, destroy button on leaving scene
            {
                AABtn.Destroy();
            }
            else
            {
                ApplicationLauncher.Instance.RemoveModApplication(AAEditorButton);
            }
        }

        public void RefreshPartModules() //set values on all ModuleAutoActions on vessel
        {
            foreach (Part p in EditorLogic.SortedShipList)
            {
                foreach (ModuleAutoAction pmAA in p.Modules.OfType<ModuleAutoAction>())
                {
                    pmAA.activateAbort = masterActivateAbort;
                    pmAA.activateGear = masterActivateGear;
                    pmAA.activateLights = masterActivateLights;
                    pmAA.activateBrakes = masterActivateBrakes;
                    pmAA.activateRCS = masterActivateRCS;
                    pmAA.activateSAS = masterActivateSAS;
                    if (masterActivateGroupA == -200)
                    {
                        pmAA.activateGroupA = 0;
                    }
                    else
                    {
                        pmAA.activateGroupA = masterActivateGroupA;
                    }
                    if (masterActivateGroupB == -200)
                    {
                        pmAA.activateGroupB = 0;
                    }
                    else
                    {
                        pmAA.activateGroupB = masterActivateGroupB;
                    }
                    if (masterActivateGroupC == -200)
                    {
                        pmAA.activateGroupC = 0;
                    }
                    else
                    {
                        pmAA.activateGroupC = masterActivateGroupC;
                    }
                    if (masterActivateGroupD == -200)
                    {
                        pmAA.activateGroupD = 0;
                    }
                    else
                    {
                        pmAA.activateGroupD = masterActivateGroupD;
                    }
                    if (masterActivateGroupE == -200)
                    {
                        pmAA.activateGroupE = 0;
                    }
                    else
                    {
                        pmAA.activateGroupE = masterActivateGroupE;
                    }
                    if (masterActivateGroupF == -200)
                    {
                        pmAA.activateGroupF = 0;
                    }
                    else
                    {
                        pmAA.activateGroupF = masterActivateGroupF;
                    }
                    
                    if (masterSetThrottleYes)
                    {
                        pmAA.setThrottle = masterSetThrottle;
                    }
                    else
                    {
                        pmAA.setThrottle = -50;
                    }
                    pmAA.setPrecCtrl = masterSetPrecCtrl;
                }
            }
            if (AANode.HasValue("WinX"))
            {
                AANode.RemoveValue("WinX");
            }
            AANode.AddValue("WinX", AAWin.x.ToString());
            if (AANode.HasValue("WinY"))
            {
                AANode.RemoveValue("WinY");
            }
            AANode.AddValue("WinY", AAWin.y.ToString());
            AANode.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AutoAction/AutoAction.cfg");//same^
        }//end RefreshPartModules()

        public void AAOnDraw() //our rendering manager
        {
            if (EditorLogic.fetch.editorScreen == EditorScreen.Actions && showBasicGroups) //only show on actions screen and if at least basic actions are unlocked
            {
                if (AAWinShow)
                {
                    AAWin = GUI.Window(67347792, AAWin, AAWindow, "Auto Actions", AAWinStyle);
                }
            }
        }//close AAOnDraw()

        public void AAWindow(int WindowID)
        {


            if (masterActivateAbort)
            {
                AABtnStyle.normal.background = ButtonTextureGreen;
                AABtnStyle.hover.background = ButtonTextureGreen;
            }
            else
            {
                AABtnStyle.normal.background = ButtonTextureRed;
                AABtnStyle.hover.background = ButtonTextureRed;
            }
            if (GUI.Button(new Rect(5, 25, 50, 18), "Abort", AABtnStyle))
            {
                masterActivateAbort = !masterActivateAbort;
                RefreshPartModules();
            }

            if (masterActivateBrakes)
            {
                AABtnStyle.normal.background = ButtonTextureGreen;
                AABtnStyle.hover.background = ButtonTextureGreen;
            }
            else
            {
                AABtnStyle.normal.background = ButtonTextureRed;
                AABtnStyle.hover.background = ButtonTextureRed;
            }
            if (GUI.Button(new Rect(55, 25, 50, 18), "Brakes", AABtnStyle))
            {
                masterActivateBrakes = !masterActivateBrakes;
                RefreshPartModules();
            }

            if (masterActivateGear)
            {
                AABtnStyle.normal.background = ButtonTextureGreen;
                AABtnStyle.hover.background = ButtonTextureGreen;
            }
            else
            {
                AABtnStyle.normal.background = ButtonTextureRed;
                AABtnStyle.hover.background = ButtonTextureRed;
            }
            if (GUI.Button(new Rect(105, 25, 50, 18), "Gear", AABtnStyle))
            {
                masterActivateGear = !masterActivateGear;
                RefreshPartModules();
            }

            if (masterActivateLights)
            {
                AABtnStyle.normal.background = ButtonTextureGreen;
                AABtnStyle.hover.background = ButtonTextureGreen;
            }
            else
            {
                AABtnStyle.normal.background = ButtonTextureRed;
                AABtnStyle.hover.background = ButtonTextureRed;
            }
            if (GUI.Button(new Rect(5, 43, 50, 18), "Lights", AABtnStyle))
            {
                masterActivateLights = !masterActivateLights;
                RefreshPartModules();
            }

            if (masterActivateRCS)
            {
                AABtnStyle.normal.background = ButtonTextureGreen;
                AABtnStyle.hover.background = ButtonTextureGreen;
            }
            else
            {
                AABtnStyle.normal.background = ButtonTextureRed;
                AABtnStyle.hover.background = ButtonTextureRed;
            }
            if (GUI.Button(new Rect(55, 43, 50, 18), "RCS", AABtnStyle))
            {
                masterActivateRCS = !masterActivateRCS;
                RefreshPartModules();
            }

            if (masterActivateSAS)
            {
                AABtnStyle.normal.background = ButtonTextureGreen;
                AABtnStyle.hover.background = ButtonTextureGreen;
            }
            else
            {
                AABtnStyle.normal.background = ButtonTextureRed;
                AABtnStyle.hover.background = ButtonTextureRed;
            }
            if (GUI.Button(new Rect(105, 43, 50, 18), "SAS", AABtnStyle))
            {
                masterActivateSAS = !masterActivateSAS;
                RefreshPartModules();
            }
            if (showCustomGroups) //only show custom groups if unlocked in editor
            {
                string masterActivateGroupAString = cvertToString(masterActivateGroupA);
                masterActivateGroupAString = GUI.TextField(new Rect(5, 63, 30, 20), masterActivateGroupAString, 4, AAFldStyle);
                try
                {
                    int tempA = cvertToNum(masterActivateGroupAString); //convert string to number
                    if (tempA != masterActivateGroupA)
                    {
                        masterActivateGroupA = tempA;
                        RefreshPartModules();
                    }
                     
                }
                catch
                {
                    masterActivateGroupAString = masterActivateGroupA.ToString(); //conversion failed, reset change
                }
                string masterActivateGroupBString = cvertToString(masterActivateGroupB);
                masterActivateGroupBString = GUI.TextField(new Rect(35, 63, 30, 20), masterActivateGroupBString, 4, AAFldStyle);
                try
                {
                    int tempB = cvertToNum(masterActivateGroupBString); //convert string to number
                    if (tempB != masterActivateGroupB)
                    {
                        masterActivateGroupB = tempB; 
                        RefreshPartModules();
                    }
                    
                }
                catch
                {
                    masterActivateGroupBString = masterActivateGroupB.ToString(); //conversion failed, reset change
                }
                string masterActivateGroupCString = cvertToString(masterActivateGroupC);
                masterActivateGroupCString = GUI.TextField(new Rect(65, 63, 30, 20), masterActivateGroupCString, 4, AAFldStyle);
                try
                {
                    int tempC = cvertToNum(masterActivateGroupCString); //convert string to number
                    if (tempC != masterActivateGroupC)
                    {
                        masterActivateGroupC = tempC;
                        RefreshPartModules();
                    }
                     
                }
                catch
                {
                    masterActivateGroupCString = masterActivateGroupC.ToString(); //conversion failed, reset change
                }
                string masterActivateGroupDString = cvertToString(masterActivateGroupD);
                masterActivateGroupDString = GUI.TextField(new Rect(95, 63, 30, 20), masterActivateGroupDString, 4, AAFldStyle);
                try
                {
                    int tempD = cvertToNum(masterActivateGroupDString); //convert string to number
                    if (tempD != masterActivateGroupD)
                    {
                        masterActivateGroupD = tempD; 
                        RefreshPartModules();
                    }
                   
                }
                catch
                {
                    masterActivateGroupDString = masterActivateGroupD.ToString(); //conversion failed, reset change
                }
                string masterActivateGroupEString = cvertToString(masterActivateGroupE);
                masterActivateGroupEString = GUI.TextField(new Rect(125, 63, 30, 20), masterActivateGroupEString, 4, AAFldStyle);
                try
                {
                    int tempE = cvertToNum(masterActivateGroupEString); //convert string to number
                    if (tempE != masterActivateGroupE)
                    {
                        masterActivateGroupE = tempE;
                        RefreshPartModules();
                    }
                     
                }
                catch
                {
                    masterActivateGroupEString = masterActivateGroupE.ToString(); //conversion failed, reset change
                }
            }
            else
            {
                GUI.Label(new Rect(10, 63, 155, 20), "Custom actions not available", AALblStyle);
            }

            if (masterSetThrottleYes)
            {
                AABtnStyle.normal.background = ButtonTextureGreen;
                AABtnStyle.hover.background = ButtonTextureGreen;
            }
            else
            {
                AABtnStyle.normal.background = ButtonTextureRed;
                AABtnStyle.hover.background = ButtonTextureRed;
            }
            if (GUI.Button(new Rect(5, 85, 50, 20), "Throttle:", AABtnStyle))
            {
                masterSetThrottleYes = !masterSetThrottleYes;
                if (masterSetThrottleYes)
                {
                    masterSetThrottle = 50;
                }
                else
                {
                    masterSetThrottle = -50;
                }
                RefreshPartModules();
            }

            if (!masterSetThrottleYes)
            {
                GUI.Label(new Rect(68, 85, 40, 20), "No", HighLogic.Skin.label);
            }
            else
            {
                string masterSetThrottleString = masterSetThrottle.ToString();
                masterSetThrottleString = GUI.TextField(new Rect(60, 85, 30, 20), masterSetThrottleString, 4, AAFldStyle);
                try
                {
                    masterSetThrottle = Convert.ToInt32(masterSetThrottleString); //convert string to number
                    RefreshPartModules();
                }
                catch
                {
                    masterSetThrottleString = masterSetThrottle.ToString(); //conversion failed, reset change
                }
                GUI.Label(new Rect(92, 70, 10, 20), "%", HighLogic.Skin.label);
            }

            if (masterSetPrecCtrl)
            {
                AABtnStyle.normal.background = ButtonTextureGreen;
                AABtnStyle.hover.background = ButtonTextureGreen;
            }
            else
            {
                AABtnStyle.normal.background = ButtonTextureRed;
                AABtnStyle.hover.background = ButtonTextureRed;
            }
            if (GUI.Button(new Rect(110, 85, 40, 20), "PCtrl", AABtnStyle))
            {
                masterSetPrecCtrl = !masterSetPrecCtrl;
                
                RefreshPartModules();
            }


            AABtnStyle.normal.background = ButtonTextureGray;
            AABtnStyle.hover.background = ButtonTextureGray;

            GUI.DragWindow(); //window is draggable
        }//close AAWindow()
        //public void Update()
        //{
        //    print("Upde" + masterActivateGroupA);
        //}
        public string cvertToString(int num)
        {
            if(num == -200)
            {
                return "";
            }
            else
            {
                return num.ToString();
            }
        }
        public int cvertToNum(string str)
        {
            if(str == "")
            {
                return -200;
            }
            else 
            {
                return Convert.ToInt32(str);
            }
        }
    }
    }

