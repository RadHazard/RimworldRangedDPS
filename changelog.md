# V1.6.0 - Rimworld 1.4 support
**Features**
- Added a Korean translation, supplied by inbae on steam.


# V1.5.0 - Korean Translation
**Features**
- Added a Korean translation, supplied by inbae on steam.


# v1.4.0 - Damage Per Resource
**Features**
- Added two new stats to turrets: Damage Per Resource and Max Damage Per Resource.  Thanks to /u/Hanif_Shakiba on Reddit for the inspiration.
- These stats measure how resource-efficient turrets are -- specifically, this is how much damage you can expect out of a turret for each unit of resource spent on rearming it.
- Like the DPS stats, Max Damage Per Resource measures the theoretical maximum amount of damage you can get out of a single unit of resource.  Damage Per Resource is generally a more realistic number for measuring DPR


# v1.3.0 - Rimworld 1.3 support
**Features**
- Updated to 1.3 (Also still compatible with 1.2)


# v1.2.2 - Hotfix
**Bugfixes**
- Fix broken debug menu


# v1.2.1 - Misc Fixes
**Bugfixes**
- Cap hit chance at 100% when calculating DPS to avoid showing impossibly high DPS in cases where hit chance is greater than 100%.
  - Note: >100% accuracy is not possible in vanilla but is possible with mods, e.g. Overcapped Accuracy
  - Note 2:  Bonus accuracy is still applied to the intermediate hit chance calculations.  Only if the final hit chance accounting for all factors exceeds 100% will it be capped, since at that point all shots are guaranteed to hit and the extra accuracy is wasted.
- Factor in pawn's aiming time stat when calculating a pawn's ranged DPS
  - This should fix Trigger-Happy and Careful Shooter traits showing incorrect DPS values
- Use the correct warmup and cooldown values when calculating turret DPS.
  - This should fix certain modded turrets showing incorrect DPS values (particularly Simple Turrets, but probably other mods as well).

**Changes**
- Updated the About.xml description and the README to match what's already on the Steam Workshop page

**Internal Changes**
- Major refactor and cleanup of the DPS calculation code.  This shouldn't have any visible effect but makes my life easier :)
  - As with all major rewrites, there's a chance new bugs have been introduced.  Report any found bugs on the workshop page or via Github
- Added several unit tests using RimTest to validate the basic DPS calculations (only in the debug build)

**Known Issues**
- Modded turrets with extremely low full-cycle times (warmup + burst time + cooldown is less than 10 ticks/0.1333 seconds) will show higher DPS values than what they actually achieve in gameplay.  This is because in vanilla, turrets will only ever attempt to fire a burst every 10th tick.


# v1.2.0 - Rimworld 1.2 support
**Features**
- Updated to Rimworld 1.2
- Added a new Ranged DPS stat to pawns and turrets that accounts for their shooting ability in addition to everything else.
- The Ranged DPS and Average DPS stats now also shows the range at which their respective DPS is achieved.  As before, this is the range that maximizes DPS.

**Changes**
- Turrets' old "Ranged damage per second" stat has been renamed to "Weapon ranged DPS", to make it clear it's only based on the turret's weapon and does not include the shooting accuracy of the mount.
- "Ranged damage per second" for guns has been renamed "Average ranged DPS" to differentiate it from the "Ranged DPS" stat on pawns.
- "Max ranged damage per second" has been renamed "Max ranged DPS" to be consistent with the other stat names


# v1.1.2 - Hotfix
**Bugfixes**
- Fixed an issue that caused a null pointer exception to be thrown when looking at an abstract ThingDef (e.g. in the Prepare Carefully screen)
- Fixed the turret support to work with mortars and similar ammunition-using turrets.
- Added several sanity checks with verbose error messages to hopefully catch the most common null pointer exceptions

**Known Issues**
- Turrets with swappable ammunition (e.g. mortars) will only show their DPS when loaded with ammunition.  This is a technical limitation due to there being no easy way to get a sane default ammunition.


# v1.1.1 - Hotfix
**Bugfixes**
- Replaced the bad assembly that was uploaded in v1.1.0 by accident.  Should fix the null pointer exceptions


# v1.1.0 - Turret Support
**Features**
- Added support for turrets as well
- Added back the original DPS ignoring accuracy as a separate stat, "Max Ranged Damage Per Second"

**Bugfixes**
- Added support for minimum ranges to the turrets


# v1.0.1 - Best Accuracy
**Minor tweaks**
- Updated the DPS stat to use the best-achieveable accuracy of the gun instead of assuming an accuracy of 100%.  Should make it easier to compare weapons with low base accuracy, like miniguns


# v1.0.0 - Initial Release
- Adds a "Ranged Damage Per Second" stat to ranged weapons
- The base stat is the expected DPS if all shots hit.  Clicking it gives a detailed breakdown of expected DPS at various ranges based on the the weapon's accuracy stats
