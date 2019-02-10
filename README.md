# Auto Actions /L Unofficial

KSP plugin for automatic action groups activation on launch. Unofficial fork by Lisias.


## In a Hurry

* [Latest Release](https://github.com/net-lisias-kspu/AutoAction/releases)
    + [Binaries](https://github.com/net-lisias-kspu/AutoAction/tree/Archive)
* [Source](https://github.com/net-lisias-kspu/AutoAction)
* [Change Log](./CHANGE_LOG.md)


## Description

Automatically activates action groups on launch.

You can control the initial state of:

* Built-in action groups (SAS, RCS, Brakes, Abort, Lights, Gear);
* Custom action groups (1—10, and more with @Diazo’s Action Groups Extended mod);
* Precise Control mode;
* Throttle level;
* Trim (Pitch, Yaw, Roll, Wheel motor, Wheel steer).﻿

### How to use:

* In editor, switch to the Action editing mode (XBdOgJf.png).
* Press the “VAB defaults ▼” / “SPH defaults ▼” button to edit the defaults for all vessels (separate defaults for VAB and SPH):
		+ green — action on; red — action off.
* Edit per-vessel values saved with the craft file:
		+ gray — keep default value; green — action on; red — action off;
		+ the 5 text boxes allow to set up to 5 custom actions you want to turn on by default. Just type action numbers into them.
* Press the “Trim ▼” button to edit vessel’s initial trim values.
* If you want to override the career mode lockout on action groups so this mod will show even if action groups are not yet unlocked, change the `OverrideCareer` parameter in the `[KSP-Install]/GameData/AutoAction/AutoAction.settings` file:
	+ `OverrideCareer = False`: Respect KSP’s action group lockout;
	+ `OverrideCareer = True`: Ignore the lockout and always show this mod.

### Known issues:

* A﻿ few second hang-up can occur at the first appearance of the GUI since the mod still uses the old GUI system.


## Installation

To install, place the GameData folder inside your Kerbal Space Program folder.

**REMOVE ANY OLD VERSIONS OF THE PRODUCT BEFORE INSTALLING**.

### Dependencies
<!--
* Hard Dependencies:
	+ [KSP API Extensions/L](https://github.com/net-lisias-ksp/KSPAPIExtensions)
-->
None at the moment. :)

### Licensing
This work is licensed under [GPL 3.0](https://www.gnu.org/licenses/gpl-3.0.txt). See [here](./LICENSE)

+ You are free to:
	- Use : unpack and use the material in any computer or device
	- Redistribute : redistribute the original package in any medium
	- Adapt : Reuse, modify or incorporate source code into your works (and redistribute it!) 
+ Under the following terms:
	- You retain any copyright notices
	- You recognize and respect any trademarks
	- You don't impersonate the authors, neither redistribute a derivative that could be misrepresented as theirs.
	- You credit the author and republish the copyright notices on your works where the code is used.
	- You relicense (and fully comply) your works using GPL 3.0 (or later)
	- You don't mix your work with GPL incompatible works.

Please note the copyrights and trademarks in [NOTICE](./NOTICE).

## UPSTREAM

* [Teilnehmer](https://forum.kerbalspaceprogram.com/index.php?/profile/143330-teilnehmer/) CURRENT MAINTAINER
	* [Forum](https://forum.kerbalspaceprogram.com/index.php?/topic/150666-14x-auto-actions-continued-v1103-—-2018-07-28/)
	* [GitHub](https://github.com/formicant/AutoAction)
* [Diazo](https://forum.kerbalspaceprogram.com/index.php?/profile/81549-diazo/) ROOT
	* [Forum](https://forum.kerbalspaceprogram.com/index.php?/topic/91980-105nov1815auto-actions-automatically-activate-action-groupsrcssas-on-launch/)
	* [GitHub](https://github.com/SirDiazo/AutoAction)
