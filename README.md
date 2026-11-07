# Charm Toast

Butter Side Up: falls no longer throw you into a tumble.

> **Everyone in the lobby must install this mod, together with Pockets and CharmCore.** The power changes what your own character can do and its state shows on your inventory slot, so friends without the mods would see an empty slot and be unable to use the pocket.

## How to Use

1. Find the **Toast** valuable and store it in a free inventory slot with [Pockets](https://thunderstore.io/c/repo/p/OsmarBriones/Pockets/) (grab it, press the slot key).
2. Jump from somewhere high. Where a long fall would normally knock you into a tumble, you simply land on your feet and keep walking.
3. Take the valuable out of the pocket and the power stops.

## Features

- **Butter Side Up:** while the Toast is in your pocket, falling from a great height no longer throws you into a tumble. By default you also take no fall damage.
- **It costs the valuable:** each saved landing wears away a share of the Toast's value, and a worn-out valuable breaks.
- **Everything is configurable:** wear, how long a fall must be to cost wear, and whether fall damage is blocked too. The host's choices apply to everyone.

## Requirements
- [BepInEx Pack for R.E.P.O.](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/)
- [Pockets](https://thunderstore.io/c/repo/p/OsmarBriones/Pockets/)
- [CharmCore](https://thunderstore.io/c/repo/p/OsmarBriones/CharmCore/)

## Installation
1. Install through r2modman / Thunderstore Mod Manager (Pockets and CharmCore come with it), or place `CharmToast.dll` next to `Pockets.dll` and `CharmCore.dll` in `BepInEx/plugins`. **All players need it.**
2. Launch the game once; the configuration file is created inside `BepInEx/config`.

## Configuration
Settings are in `com.osmar.CharmToast.cfg` inside `BepInEx/config`, section `Charm.Toast`. The host's choices apply to everyone.

- `Enabled` (default `true`): turn this charm on or off.
- `WearPercent` (default `12`): share of the valuable's original value lost on each saved landing.
- `MinFallDistance` (default `8`, from 3 to 50 metres): a saved landing costs wear only when the fall was at least this long. Falls the game would tumble you for are saved for free when shorter; values below 8 behave like 8.
- `BlockFallDamage` (default `1`, 0 or 1): 1 means a saved fall also deals no damage; 0 means you do not tumble but still take the game's usual fall damage.

Section `Debug`: `DebugMode` (default `false`) places a Toast on the floor in front of you when a level starts (host or singleplayer) and writes extra log lines. For testing. `SpawnNow` (default `false`, needs `DebugMode`): set it to `true` while a level is running to place one right away; it switches itself back off.

## Issues & Bug Reports
The official way to report issues, suggest improvements, or submit feedback is by opening an issue on the official GitHub repository:
👉 [GitHub Issues](https://github.com/OsmarBriones/REPO-CharmToast/issues)

## Credits
Developed by **Osmar Briones**
