# Auto Actions /L Unleashed :: Change Log

* 2020-0128: 1.11.0.1 (lisias) for KSP >= 1.4
	+ Updated to the most recent KSPe API and facilities
	+ Moved the shebang to `net.lisias.ksp` Vendor hierarchy to prevent clashes with upstream
	+ Bumping version to catch up with upstream 
* 2019-0210: 1.10.6.1 (lisias) for KSP >= 1.4
	+ Catching up with upstream's:
		- Supporting Command Seats
		- Persisting Option per Facility (nice! :) )
	+ Guaranteed support for KSP 1.4.1 and newer.
* 2018-0803: 1.10.3.1 (lisias) for KSP 1.4.x
	+ Moved Settings file to <KSP_ROOT>/PluginData 
* 2018-0625: 1.10.3 (Formicant) for KSP 1.4.5
	+ Updated to KSP 1.4.5.
* 2018-0625: 1.10.2 (Formicant) for KSP 1.4.4
	+ Updated to KSP 1.4.4.
* 2018-0410: 1.10.1 (Formicant) for KSP 1.4.3
	+ Updated to KSP 1.4.3
* 2018-0410: 1.10.0 (Formicant) for KSP 1.3.1 & 1.4.2
	+ Gear and Light behavior fixed.
	+ UI layout changed.
* 2018-0406: 1.9.5 (Formicant) for KSP 1.4.2
	+ Updated to KSP 1.4.2.
* 2018-0317: 1.9.4 (Formicant) for KSP 1.4.1
	+ Updated to KSP 1.4.1.
	+ Fixed null exception when displaying the trim section for the first time.
* 2017-1019: 1.9.3 (Formicant) for KSP 1.3.1
	+ Exception on Editor to Flight scene change fixed.
* 2017-1011: 1.9.2 (Formicant) for KSP 1.3.1
	+ Updated to KSP 1.3.1
* 2017-0903: 1.9.1 (Formicant) for KSP 1.3
	+ Spanish, Japanese, and Chinese localization added. 
* 2017-0901: 1.9 (Formicant) for KSP 1.3
	+ Trim settings added. 
* 2017-0621: 1.8.1 (Formicant) for KSP 1.3
	+ Spanish, Japanese, and Chinese translations added.
* 2017-0606: 1.8 (Formicant) for KSP 1.3
	+ Updated to KSP 1.3.
	+ Localization support added.
	+ Russian GUI added.
	+ Code refactoring.
* 2017-0103: 1.7.3 (Formicant) for KSP 1.2.2
	+ Updated to KSP 1.2.2.
* 2016-1106: 1.7.2 (Formicant) for KSP 1.2.1
	+ Updated to KSP 1.2.1. 
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
