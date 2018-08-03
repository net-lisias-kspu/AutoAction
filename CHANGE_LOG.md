# AutoAction :: Change Log

* 2014-1226: 1.3 (Diazo) for KSP 0.90
	+ Add Precise Control option
	+ Fix Override Career flag added in previous version so it actually works.
* 2014-1224: 1.2.1 (Diazo) for KSP 0.90
	+ Add ability to override career mode lockout on action groups so this mod will show even if action groups are not yet unlocked.
	+ Note that only shows this mod, another mod is required in order actually assign actions in early career mode. (Action Groups Extended is one that does.)
	+ To enable: KSP-Install\GameData\Diazo\AutoAction\AutoAction.cfg
		- Change OverrideCareer = 0 to 1
		- Value of 0: Respect KSP's action group lockout
		- Value of 1: Ignore the lockout and always show this mod.
* 2014-1222: 1.2 (Diazo) for KSP 0.90
	+ Stock toolbar support added, will be used if Blizzy's toolbar is not installed.
	+ Now respects the action groups available in career mode and will only show actions you have access too. Note this means the mod will not show at the start of a career game, you must upgrade the VAB/SPH once to show it.
* 2014-1216: 1.1 (Diazo) for KSP 0.90
	+ KSP 0.90 Compatibility fix.
		- There are code changes under the hood, you must update to 1.1 for this mod to work in KSP 0.90 
* 2014-1204: 1.0 (Diazo) for KSP 0.x
	+ Automatically activate any action groups on vessel launch.
	+ Including RCS and SAS
	+ Set Main Throttle to a specific value
	+ All settings save on a per vessel basis	
