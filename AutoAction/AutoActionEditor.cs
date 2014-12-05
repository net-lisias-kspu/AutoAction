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

        public void Start()
        {
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
                        AAWinShow = !AAWinShow;
                    }
                };
            }
            ConfigNode AANode = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/Diazo/AutoAction/AutoAction.cfg"); //load .cfg file
            AAWin.x = Convert.ToInt32(AANode.GetValue("WinX"));
            AAWin.y = Convert.ToInt32(AANode.GetValue("WinY"));
            LoadAAPartModule();
        }//close Start()

        //public void Update()
        //{
        //    print("throttle " + masterSetThrottle + masterSetThrottleYes);
        //}

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
                    }
                }
            }
            catch
            {
                //leave blank, need this to catch the error on entering editor the first time with no vessel loaded because it throws a null ref otherwise
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
                    pmAA.activateGroupA = masterActivateGroupA;
                    pmAA.activateGroupB = masterActivateGroupB;
                    pmAA.activateGroupC = masterActivateGroupC;
                    pmAA.activateGroupD = masterActivateGroupD;
                    pmAA.activateGroupE = masterActivateGroupE;
                    pmAA.activateGroupF = masterActivateGroupF;
                    if (masterSetThrottleYes)
                    {
                        pmAA.setThrottle = masterSetThrottle;
                    }
                    else
                    {
                        pmAA.setThrottle = -50;
                    }
                }
            }
            ConfigNode AANode = new ConfigNode();
            AANode.AddValue("WinX", AAWin.x.ToString());
            AANode.AddValue("WinY", AAWin.y.ToString());
            AANode.Save(KSPUtil.ApplicationRootPath + "GameData/Diazo/AutoAction/AutoAction.cfg");//same^
        }//end RefreshPartModules()

        public void AAOnDraw() //our rendering manager
        {
            if (EditorLogic.fetch.editorScreen == EditorLogic.EditorScreen.Actions) //only show on actions screen
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

            string masterActivateGroupAString = masterActivateGroupA.ToString();
            masterActivateGroupAString = GUI.TextField(new Rect(5, 63, 30, 20), masterActivateGroupAString, 4, AAFldStyle);
            try
            {
                masterActivateGroupA = Convert.ToInt32(masterActivateGroupAString); //convert string to number
            }
            catch
            {
                masterActivateGroupAString = masterActivateGroupA.ToString(); //conversion failed, reset change
            }
            string masterActivateGroupBString = masterActivateGroupB.ToString();
            masterActivateGroupBString = GUI.TextField(new Rect(35, 63, 30, 20), masterActivateGroupBString, 4, AAFldStyle);
            try
            {
                masterActivateGroupB = Convert.ToInt32(masterActivateGroupBString); //convert string to number
            }
            catch
            {
                masterActivateGroupBString = masterActivateGroupB.ToString(); //conversion failed, reset change
            }
            string masterActivateGroupCString = masterActivateGroupC.ToString();
            masterActivateGroupCString = GUI.TextField(new Rect(65, 63, 30, 20), masterActivateGroupCString, 4, AAFldStyle);
            try
            {
                masterActivateGroupC = Convert.ToInt32(masterActivateGroupCString); //convert string to number
            }
            catch
            {
                masterActivateGroupCString = masterActivateGroupC.ToString(); //conversion failed, reset change
            }
            string masterActivateGroupDString = masterActivateGroupD.ToString();
            masterActivateGroupDString = GUI.TextField(new Rect(95, 63, 30, 20), masterActivateGroupDString, 4, AAFldStyle);
            try
            {
                masterActivateGroupD = Convert.ToInt32(masterActivateGroupDString); //convert string to number
            }
            catch
            {
                masterActivateGroupDString = masterActivateGroupD.ToString(); //conversion failed, reset change
            }
            string masterActivateGroupEString = masterActivateGroupE.ToString();
            masterActivateGroupEString = GUI.TextField(new Rect(125, 63, 30, 20), masterActivateGroupEString, 4, AAFldStyle);
            try
            {
                masterActivateGroupE = Convert.ToInt32(masterActivateGroupEString); //convert string to number
            }
            catch
            {
                masterActivateGroupEString = masterActivateGroupE.ToString(); //conversion failed, reset change
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
            if (GUI.Button(new Rect(5, 85, 80, 20), "Set throttle:", AABtnStyle))
            {
                masterSetThrottleYes = !masterSetThrottleYes;
                if(masterSetThrottleYes)
                {
                    masterSetThrottle = 50;
                }
                else
                {
                    masterSetThrottle = -50;
                }
                RefreshPartModules();
            }

            if(!masterSetThrottleYes)
            {
                GUI.Label(new Rect(90, 85, 40, 20), "No", HighLogic.Skin.label);
            }
            else
            {
                string masterSetThrottleString = masterSetThrottle.ToString();
                masterSetThrottleString = GUI.TextField(new Rect(90, 85, 30, 20), masterSetThrottleString, 4, AAFldStyle);
                try
                {
                    masterSetThrottle = Convert.ToInt32(masterSetThrottleString); //convert string to number
                    RefreshPartModules(); 
                }
                catch
                {
                    masterSetThrottleString = masterSetThrottle.ToString(); //conversion failed, reset change
                }
                GUI.Label(new Rect(122, 70, 10, 20), "%", HighLogic.Skin.label);
            }


            AABtnStyle.normal.background = ButtonTextureGray;
            AABtnStyle.hover.background = ButtonTextureGray;
            
                GUI.DragWindow(); //window is draggable
            }//close AAWindow()

        }
    }

