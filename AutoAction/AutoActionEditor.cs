using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using KSP.UI.Screens;
using static KSP.UI.Screens.CraftBrowserDialog;

namespace AutoAction
{

	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class AutoActionEditor : MonoBehaviour
	{
		static readonly string ModFolderPath = "Diazo/AutoAction/";
		static readonly string FullModFolderPath = KSPUtil.ApplicationRootPath + "GameData/" + ModFolderPath;

		private bool showBasicGroups = false;
		private bool showCustomGroups = false;
		private IButton AABtn; //toolbar button
		private bool AAWinShow = true; //show window?
		private static GUISkin AASkin;
		private static GUIStyle AAWinStyle = null; //window style
		private static GUIStyle AALblStyle = null; //window style
		private static GUIStyle AABtnStyle = null; //window style
		private static GUIStyle AAFldStyle = null; //window style
		private const int CollapsedWindowHeight = 155;
		private const int ExpandedWindowHeight = 215;
		private bool isWindowExpanded = false;
		public Rect AAWin = new Rect(100, 100, 160, CollapsedWindowHeight); //GUI window rectangle
		Texture2D ButtonTextureRed = new Texture2D(64, 64); //button textures
		Texture2D ButtonTextureGreen = new Texture2D(64, 64);
		Texture2D ButtonTextureGray = new Texture2D(64, 64);
		string facilityPrefix;

		public bool defaultActivateAbort = false;
		public bool defaultActivateGear = true;
		public bool defaultActivateLights = false;
		public bool defaultActivateBrakes = false;
		public bool defaultActivateRCS = false;
		public bool defaultActivateSAS = false;
		public int defaultSetThrottle = 50;
		public bool defaultSetPrecCtrl = false;

		public bool? masterActivateAbort = null; //variables we work with. this is our master "partModule" while in editor
		public bool? masterActivateGear = null;
		public bool? masterActivateLights = null;
		public bool? masterActivateBrakes = null;
		public bool? masterActivateRCS = null;
		public bool? masterActivateSAS = null;
		public int masterActivateGroupA = 0;
		public int masterActivateGroupB = 0;
		public int masterActivateGroupC = 0;
		public int masterActivateGroupD = 0;
		public int masterActivateGroupE = 0;
		public int masterActivateGroupF = 0;
		public bool masterSetThrottleYes = false;
		public int masterSetThrottle = -50; //end variables
		public bool? masterSetPrecCtrl = null; //precise control?

		ApplicationLauncherButton AAEditorButton = null; //stock toolbar button instance
		ConfigNode AANode;

		public void OnGUI()
		{
			AAOnDraw();
		}

		public void Start()
		{
			print("AutoActions Version 1.6.2f loaded.");
			GameEvents.onEditorLoad.Add(OnShipLoad);
			AAWinStyle = new GUIStyle(HighLogic.Skin.window); //make our style
			AAFldStyle = new GUIStyle(HighLogic.Skin.textField) { fontStyle = FontStyle.Normal };
			//load our button textures
			ButtonTextureGray.LoadImage(File.ReadAllBytes(FullModFolderPath + "ButtonTexture.png"));
			ButtonTextureGray.Apply();
			ButtonTextureGreen.LoadImage(File.ReadAllBytes(FullModFolderPath + "ButtonTextureGreen.png"));
			ButtonTextureGreen.Apply();
			ButtonTextureRed.LoadImage(File.ReadAllBytes(FullModFolderPath + "ButtonTextureRed.png"));
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
			if(ToolbarManager.ToolbarAvailable) //check if toolbar available, load if it is
			{
				AABtn = ToolbarManager.Instance.add("AutoAction", "AABtn");
				AABtn.TexturePath = ModFolderPath + "AABtn";
				AABtn.ToolTip = "Auto Actions";
				AABtn.OnClick += (e) =>
				{
					if(e.MouseButton == 0) //simply show/hide window on click
					{
						onStockToolbarClick();
					}
				};
			}
			else
			{
				//AGXShow = true; //toolbar not installed, show AGX regardless
				//now using stock toolbar as fallback
				AAEditorButton = ApplicationLauncher.Instance.AddModApplication(onStockToolbarClick, onStockToolbarClick, DummyVoid, DummyVoid, DummyVoid, DummyVoid, ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH, (Texture)GameDatabase.Instance.GetTexture(ModFolderPath + "AABtn", false));
			}

			facilityPrefix = EditorDriver.editorFacility == EditorFacility.SPH ? "SPH" : "VAB";

			AANode = ConfigNode.Load(FullModFolderPath + "AutoAction.cfg"); //load .cfg file
			AAWin.x = Convert.ToInt32(AANode.GetValue("WinX"));
			AAWin.y = Convert.ToInt32(AANode.GetValue("WinY"));
			defaultActivateAbort = AANode.GetValue(facilityPrefix + "activateAbort") == "On";
			defaultActivateGear = AANode.GetValue(facilityPrefix + "activateGear") != "Off";
			defaultActivateLights = AANode.GetValue(facilityPrefix + "activateLights") == "On";
			defaultActivateBrakes = AANode.GetValue(facilityPrefix + "activateBrakes") == "On";
			defaultActivateRCS = AANode.GetValue(facilityPrefix + "activateRCS") == "On";
			defaultActivateSAS = AANode.GetValue(facilityPrefix + "activateSAS") == "On";
			int.TryParse(AANode.GetValue(facilityPrefix + "setThrottle"), out defaultSetThrottle);
			defaultSetPrecCtrl = AANode.GetValue(facilityPrefix + "setPrecCtrl") == "On";

			LoadAAPartModule();
			//ScenarioUpgradeableFacilities upgradeScen = HighLogic.CurrentGame.scenarios.OfType<ScenarioUpgradeableFacilities>().First();
			//float edLvl = 0;
			if(AANode.HasValue("OverrideCareer")) //are action groups unlocked?
			{
				//print("b");
				if((string)AANode.GetValue("OverrideCareer") == "1")
				{
					//print("c");
					showCustomGroups = true;
					showBasicGroups = true;
				}
				else
				{

					if(EditorDriver.editorFacility == EditorFacility.SPH) //we are in SPH, what action groups are unlocked?
					{
						if(GameVariables.Instance.UnlockedActionGroupsCustom(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar), false))
						{
							showCustomGroups = true;
							showBasicGroups = true;
						}
						else if(GameVariables.Instance.UnlockedActionGroupsStock(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar), false))
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
						if(GameVariables.Instance.UnlockedActionGroupsCustom(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding), true))
						{
							showCustomGroups = true;
							showBasicGroups = true;
						}
						else if(GameVariables.Instance.UnlockedActionGroupsStock(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding), true))
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

				if(EditorDriver.editorFacility == EditorFacility.SPH) //we are in SPH, what action groups are unlocked?
				{
					if(GameVariables.Instance.UnlockedActionGroupsCustom(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar), false))
					{
						showCustomGroups = true;
						showBasicGroups = true;
					}
					else if(GameVariables.Instance.UnlockedActionGroupsStock(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar), false))
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
					if(GameVariables.Instance.UnlockedActionGroupsCustom(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding), true))
					{
						showCustomGroups = true;
						showBasicGroups = true;
					}
					else if(GameVariables.Instance.UnlockedActionGroupsStock(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding), true))
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
				foreach(Part p in EditorLogic.SortedShipList)
				{
					foreach(ModuleAutoAction pmAA in p.Modules.OfType<ModuleAutoAction>())
					{
						masterActivateAbort = pmAA.ActivateAbort;
						masterActivateGear = pmAA.ActivateGear;
						masterActivateLights = pmAA.ActivateLights;
						masterActivateBrakes = pmAA.ActivateBrakes;
						masterActivateRCS = pmAA.ActivateRCS;
						masterActivateSAS = pmAA.ActivateSAS;
						masterActivateGroupA = pmAA.activateGroupA;
						masterActivateGroupB = pmAA.activateGroupB;
						masterActivateGroupC = pmAA.activateGroupC;
						masterActivateGroupD = pmAA.activateGroupD;
						masterActivateGroupE = pmAA.activateGroupE;
						masterActivateGroupF = pmAA.activateGroupF;
						if(pmAA.setThrottle != -50)
						{
							masterSetThrottle = pmAA.setThrottle;
							masterSetThrottleYes = true;
						}
						else
						{
							masterSetThrottle = -50;
							masterSetThrottleYes = false;
						}
						masterSetPrecCtrl = pmAA.SetPrecCtrl;
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
			if(ToolbarManager.ToolbarAvailable) //if toolbar loaded, destroy button on leaving scene
			{
				AABtn.Destroy();
			}
			else
			{
				ApplicationLauncher.Instance.RemoveModApplication(AAEditorButton);
			}
			GameEvents.onEditorLoad.Remove(OnShipLoad);
		}

		public void OnShipLoad(ShipConstruct ship, LoadType loadType)
		{
			if(loadType == LoadType.Normal)
			{
				LoadAAPartModule();
			}
			else
			{
				Debug.Log("AutoAction Ship Load of type MERGE");
			}
		}

		public void RefreshPartModules() //set values on all ModuleAutoActions on vessel
		{
			foreach(Part p in EditorLogic.SortedShipList)
			{
				foreach(ModuleAutoAction pmAA in p.Modules.OfType<ModuleAutoAction>())
				{
					pmAA.ActivateAbort = masterActivateAbort;
					pmAA.ActivateGear = masterActivateGear;
					pmAA.ActivateLights = masterActivateLights;
					pmAA.ActivateBrakes = masterActivateBrakes;
					pmAA.ActivateRCS = masterActivateRCS;
					pmAA.ActivateSAS = masterActivateSAS;
					if(masterActivateGroupA == -200)
					{
						pmAA.activateGroupA = 0;
					}
					else
					{
						pmAA.activateGroupA = masterActivateGroupA;
					}
					if(masterActivateGroupB == -200)
					{
						pmAA.activateGroupB = 0;
					}
					else
					{
						pmAA.activateGroupB = masterActivateGroupB;
					}
					if(masterActivateGroupC == -200)
					{
						pmAA.activateGroupC = 0;
					}
					else
					{
						pmAA.activateGroupC = masterActivateGroupC;
					}
					if(masterActivateGroupD == -200)
					{
						pmAA.activateGroupD = 0;
					}
					else
					{
						pmAA.activateGroupD = masterActivateGroupD;
					}
					if(masterActivateGroupE == -200)
					{
						pmAA.activateGroupE = 0;
					}
					else
					{
						pmAA.activateGroupE = masterActivateGroupE;
					}
					if(masterActivateGroupF == -200)
					{
						pmAA.activateGroupF = 0;
					}
					else
					{
						pmAA.activateGroupF = masterActivateGroupF;
					}

					if(masterSetThrottleYes)
					{
						pmAA.setThrottle = masterSetThrottle;
					}
					else
					{
						pmAA.setThrottle = -50;
					}
					pmAA.SetPrecCtrl = masterSetPrecCtrl;
				}
			}

			AANode.SetValue(facilityPrefix + "activateAbort", defaultActivateAbort ? "On" : "Off", true);
			AANode.SetValue(facilityPrefix + "activateGear", defaultActivateGear ? "On" : "Off", true);
			AANode.SetValue(facilityPrefix + "activateLights", defaultActivateLights ? "On" : "Off", true);
			AANode.SetValue(facilityPrefix + "activateBrakes", defaultActivateBrakes ? "On" : "Off", true);
			AANode.SetValue(facilityPrefix + "activateRCS", defaultActivateRCS ? "On" : "Off", true);
			AANode.SetValue(facilityPrefix + "activateSAS", defaultActivateSAS ? "On" : "Off", true);
			AANode.SetValue(facilityPrefix + "setThrottle", defaultSetThrottle.ToString(), true);
			AANode.SetValue(facilityPrefix + "setPrecCtrl", defaultSetPrecCtrl ? "On" : "Off", true);

			AANode.SetValue("WinX", AAWin.x.ToString(), true);
			AANode.SetValue("WinY", AAWin.y.ToString(), true);

			AANode.Save(FullModFolderPath + "AutoAction.cfg");//same^
		}//end RefreshPartModules()

		public void AAOnDraw() //our rendering manager
		{
			if(EditorLogic.fetch.editorScreen == EditorScreen.Actions && showBasicGroups) //only show on actions screen and if at least basic actions are unlocked
			{
				if(AAWinShow)
				{
					AAWin.height = isWindowExpanded ? ExpandedWindowHeight : CollapsedWindowHeight;
					AAWin = GUI.Window(67347792, AAWin, AAWindow, "Auto Actions", AAWinStyle);
				}
			}
		}//close AAOnDraw()

		public void AAWindow(int WindowID)
		{
			GUI.Label(new Rect(5, 23, 155, 20), "Per-vessel settings", HighLogic.Skin.label);

			AABtnStyle.normal.background = AABtnStyle.hover.background = GetTextureByValue(masterActivateAbort);
			if(GUI.Button(new Rect(5, 45, 50, 18), "Abort", AABtnStyle))
			{
				masterActivateAbort = GetNextValue(masterActivateAbort);
				RefreshPartModules();
			}

			AABtnStyle.normal.background = AABtnStyle.hover.background = GetTextureByValue(masterActivateBrakes);
			if(GUI.Button(new Rect(55, 45, 50, 18), "Brakes", AABtnStyle))
			{
				masterActivateBrakes = GetNextValue(masterActivateBrakes);
				RefreshPartModules();
			}

			AABtnStyle.normal.background = AABtnStyle.hover.background = GetTextureByValue(masterActivateGear);
			if(GUI.Button(new Rect(105, 45, 50, 18), "Gear", AABtnStyle))
			{
				masterActivateGear = GetNextValue(masterActivateGear);
				RefreshPartModules();
			}

			AABtnStyle.normal.background = AABtnStyle.hover.background = GetTextureByValue(masterActivateLights);
			if(GUI.Button(new Rect(5, 63, 50, 18), "Lights", AABtnStyle))
			{
				masterActivateLights = GetNextValue(masterActivateLights);
				RefreshPartModules();
			}

			AABtnStyle.normal.background = AABtnStyle.hover.background = GetTextureByValue(masterActivateRCS);
			if(GUI.Button(new Rect(55, 63, 50, 18), "RCS", AABtnStyle))
			{
				masterActivateRCS = GetNextValue(masterActivateRCS);
				RefreshPartModules();
			}

			AABtnStyle.normal.background = AABtnStyle.hover.background = GetTextureByValue(masterActivateSAS);
			if(GUI.Button(new Rect(105, 63, 50, 18), "SAS", AABtnStyle))
			{
				masterActivateSAS = GetNextValue(masterActivateSAS);
				RefreshPartModules();
			}

			if(showCustomGroups) //only show custom groups if unlocked in editor
			{
				string masterActivateGroupAString = cvertToString(masterActivateGroupA);
				masterActivateGroupAString = GUI.TextField(new Rect(5, 83, 30, 20), masterActivateGroupAString, 4, AAFldStyle);
				try
				{
					int tempA = cvertToNum(masterActivateGroupAString); //convert string to number
					if(tempA != masterActivateGroupA)
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
				masterActivateGroupBString = GUI.TextField(new Rect(35, 83, 30, 20), masterActivateGroupBString, 4, AAFldStyle);
				try
				{
					int tempB = cvertToNum(masterActivateGroupBString); //convert string to number
					if(tempB != masterActivateGroupB)
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
				masterActivateGroupCString = GUI.TextField(new Rect(65, 83, 30, 20), masterActivateGroupCString, 4, AAFldStyle);
				try
				{
					int tempC = cvertToNum(masterActivateGroupCString); //convert string to number
					if(tempC != masterActivateGroupC)
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
				masterActivateGroupDString = GUI.TextField(new Rect(95, 83, 30, 20), masterActivateGroupDString, 4, AAFldStyle);
				try
				{
					int tempD = cvertToNum(masterActivateGroupDString); //convert string to number
					if(tempD != masterActivateGroupD)
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
				masterActivateGroupEString = GUI.TextField(new Rect(125, 83, 30, 20), masterActivateGroupEString, 4, AAFldStyle);
				try
				{
					int tempE = cvertToNum(masterActivateGroupEString); //convert string to number
					if(tempE != masterActivateGroupE)
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
				GUI.Label(new Rect(10, 83, 155, 20), "Custom actions not available", AALblStyle);
			}

			if(masterSetThrottleYes)
				AABtnStyle.normal.background = AABtnStyle.hover.background = ButtonTextureGreen;
			else
				AABtnStyle.normal.background = AABtnStyle.hover.background = ButtonTextureGray;
			if(GUI.Button(new Rect(5, 105, 50, 20), "Throttle:", AABtnStyle))
			{
				masterSetThrottleYes = !masterSetThrottleYes;
				if(masterSetThrottleYes)
				{
					masterSetThrottle = 0;
				}
				else
				{
					masterSetThrottle = -50;
				}
				RefreshPartModules();
			}

			if(!masterSetThrottleYes)
			{
				GUI.Label(new Rect(60, 105, 50, 20), "default", HighLogic.Skin.label);
			}
			else
			{
				string masterSetThrottleString = masterSetThrottle.ToString();
				masterSetThrottleString = GUI.TextField(new Rect(55, 105, 40, 20), masterSetThrottleString, 4, AAFldStyle);
				try
				{
					masterSetThrottle = Convert.ToInt32(masterSetThrottleString); //convert string to number
					RefreshPartModules();
				}
				catch
				{
					masterSetThrottleString = masterSetThrottle.ToString(); //conversion failed, reset change
				}
				GUI.Label(new Rect(97, 90, 10, 20), "%", HighLogic.Skin.label);
			}

			AABtnStyle.normal.background = AABtnStyle.hover.background = GetTextureByValue(masterSetPrecCtrl);
			if(GUI.Button(new Rect(115, 105, 40, 20), "PCtrl", AABtnStyle))
			{
				masterSetPrecCtrl = GetNextValue(masterSetPrecCtrl);
				RefreshPartModules();
			}

			// Default settings

			GUI.Label(new Rect(5, 128, 155, 20), facilityPrefix + " defaults", HighLogic.Skin.label);

			AABtnStyle.normal.background = AABtnStyle.hover.background = ButtonTextureGray;
			if(GUI.Button(new Rect(110, 130, 30, 18), isWindowExpanded ? "▲" : "▼", AABtnStyle))
			{
				isWindowExpanded = !isWindowExpanded;
				RefreshPartModules();
			}

			if(isWindowExpanded)
			{
				AABtnStyle.normal.background = AABtnStyle.hover.background = GetTextureByValue(defaultActivateAbort);
				if(GUI.Button(new Rect(5, 150, 50, 18), "Abort", AABtnStyle))
				{
					defaultActivateAbort = !defaultActivateAbort;
					RefreshPartModules();
				}

				AABtnStyle.normal.background = AABtnStyle.hover.background = GetTextureByValue(defaultActivateBrakes);
				if(GUI.Button(new Rect(55, 150, 50, 18), "Brakes", AABtnStyle))
				{
					defaultActivateBrakes = !defaultActivateBrakes;
					RefreshPartModules();
				}

				AABtnStyle.normal.background = AABtnStyle.hover.background = GetTextureByValue(defaultActivateGear);
				if(GUI.Button(new Rect(105, 150, 50, 18), "Gear", AABtnStyle))
				{
					defaultActivateGear = !defaultActivateGear;
					RefreshPartModules();
				}

				AABtnStyle.normal.background = AABtnStyle.hover.background = GetTextureByValue(defaultActivateLights);
				if(GUI.Button(new Rect(5, 168, 50, 18), "Lights", AABtnStyle))
				{
					defaultActivateLights = !defaultActivateLights;
					RefreshPartModules();
				}

				AABtnStyle.normal.background = AABtnStyle.hover.background = GetTextureByValue(defaultActivateRCS);
				if(GUI.Button(new Rect(55, 168, 50, 18), "RCS", AABtnStyle))
				{
					defaultActivateRCS = !defaultActivateRCS;
					RefreshPartModules();
				}

				AABtnStyle.normal.background = AABtnStyle.hover.background = GetTextureByValue(defaultActivateSAS);
				if(GUI.Button(new Rect(105, 168, 50, 18), "SAS", AABtnStyle))
				{
					defaultActivateSAS = !defaultActivateSAS;
					RefreshPartModules();
				}

				GUI.Label(new Rect(5, 188, 50, 20), "Throttle:", HighLogic.Skin.label);

				string defaultSetThrottleString = defaultSetThrottle.ToString();
				defaultSetThrottleString = GUI.TextField(new Rect(55, 188, 40, 20), defaultSetThrottleString, 4, AAFldStyle);
				try
				{
					defaultSetThrottle = Convert.ToInt32(defaultSetThrottleString); //convert string to number
					RefreshPartModules();
				}
				catch
				{
					defaultSetThrottleString = defaultSetThrottle.ToString(); //conversion failed, reset change
				}
				GUI.Label(new Rect(97, 173, 10, 20), "%", HighLogic.Skin.label);

				AABtnStyle.normal.background = AABtnStyle.hover.background = GetTextureByValue(defaultSetPrecCtrl);
				if(GUI.Button(new Rect(115, 188, 40, 20), "PCtrl", AABtnStyle))
				{
					defaultSetPrecCtrl = !defaultSetPrecCtrl;
					RefreshPartModules();
				}
			}

			AABtnStyle.normal.background = ButtonTextureGray;
			AABtnStyle.hover.background = ButtonTextureGray;

			GUI.DragWindow(); //window is draggable
		}//close AAWindow()

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

		private Texture2D GetTextureByValue(bool? value)
		{
			return value.HasValue ? value.Value ? ButtonTextureGreen : ButtonTextureRed : ButtonTextureGray;
		}

		private bool? GetNextValue(bool? value)
		{
			return value.HasValue ? value.Value ? false : (bool?)null : true;
		}
	}
}

