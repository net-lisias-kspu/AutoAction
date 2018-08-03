# AutoAction :: Change Log

* 2016-1024: 1.7.1 (Formicant) for KSP 1.2
	+ Licence added.
	+ .version file added.
	+ .netkan file updated.
* 2016-1023: 1.7 (Formicant) for KSP 1.2
	+ AutoAction.cfg file renamed to AutoAction.settings to prevent ModuleManager to invalidate the cache.
	+ Bizzy’s toolbar wrapper updated.
* 2016-1015: 1.6.3 (Formicant) for KSP 1.2
	+ Updated to KSP 1.2
* 2016-0428: 1.6.2 (Formicant) for KSP 1.1.2
	+ Fix stock toolbar.
	+ Fix gauge color.
	+ Separate VAB and SPH defaults. 
* 2016-0422: 1.6.1 (Formicant) for KSP 1.1.2
	+ KSP 1.1.2 recompile
* 2015-1119: 1.6 (Diazo) for KSP 1.0.5
	+ KSP 1.0.5 Update
	+ No code changes, Squad just moved some stuff in the API I have to update for.
* 2015-0625: 1.5 (Diazo) for KSP 1.0.4
	+ KSP 1.0.4
	+ Fix a rare NullRef
* 2015-0428: 1.4 (Diazo) for KSP 1.0
	+ KSP 1.0 Update
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
