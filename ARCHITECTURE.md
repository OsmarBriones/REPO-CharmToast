# CharmToast Architecture

A Charm mod (layer 3 of the PocketCharms family): one valuable, one power. It depends on **Pockets** (storage) and **CharmCore** (registry, wear, cooldown display, host settings, debug mode).

## The power: Butter Side Up

falls no longer throw you into a tumble.

- `ToastCharm : Charm` registers the prefab name `Valuable Museum Toast`, the defaults (wear 12% per saved landing, no cooldown) and the host-synced settings below.
- `Patches/PlayerAvatar_Update_Patch.cs`: Harmony prefix on the private `PlayerAvatar.Update`, forwarding to `ButterSideUp.Process`.
- `ButterSideUp` (plain class, reached through `CharmToastPlugin.Effect`) runs the per-frame logic on the owner's machine.
- `Domain/FallTracker.cs`: game-free state machine (unit tested in `FallTrackerTests`).

### How the game handles a fall (decompiled `PlayerAvatar.cs`)
The enclosing method is `PlayerAvatar.Update`. For the local avatar, when `PlayerController.instance.CollisionController.fallDistance >= 8`, it calls `tumble.TumbleRequest(true, false)` and, at 15 Hz, `tumble.ImpactHurtSet(0.5, damage)` (damage tiers by distance: >5 m 5, >9 m 15, >13 m 25, >17 m 50, >21 m 100), then `FallingSet(true)`. While `falling` is true the host runs `tumble.TumbleOverrideTime(2f)` (the roughly 2 s tumble). The tumble therefore starts mid-air as soon as 8 m are fallen, not at the landing. `fallDistance` grows in `PlayerCollisionController.Update` while not grounded and resets when grounded.

### The hook
The prefix runs before that code. While the charm is active, when `fallDistance` reaches 8 the effect sets it back to 0, so none of the above is requested (no tumble, no impact damage armed, `falling` stays false so the host never extends it). `FallTracker` keeps the real fall length itself (metres cleared plus the current stretch), because the game's value restarts at 0 each time. When the player is grounded it reports one landing: if the length is at least `MinFallDistance` it calls `Charm.TryUse()` (one wear per landing, never per frame). A `fallDamageResetState` (respawn teleport) ends the fall with no wear and no damage. If the charm turns off mid-fall the tracker resets.

Authority: `fallDistance`, `Grounded`, the tumble request and `FallingSet` all belong to the owner's machine (`isLocal`, request sent to the host by RPC), so the effect is purely local; no network message is needed.

### Fall damage decision
The game arms fall damage only together with the tumble (`ImpactHurtSet`; the timer is cleared whenever a tumble is not running), so stopping the tumble removes the damage as a side effect. `BlockFallDamage` (default 1) makes that explicit and documented. With 0, the tumble is still prevented but on landing the effect calls `PlayerHealth.Hurt(damage, true)` with the same tier amount the game would have used (through CharmCore's hurt hook, so other charms still apply). The `featherTimer` mechanism is not used (it also changes gravity).

### Settings
| Key | Type | Range | Default | Meaning |
|---|---|---|---|---|
| `MinFallDistance` | float, metres | 3 to 50 | 8 | A saved landing costs wear only when the whole fall was at least this long. |
| `BlockFallDamage` | float used as a switch (0 or 1; 0.5 and above is on) | 0 to 1 | 1 | 1: no fall damage on a saved fall; 0: damage dealt manually. |
| `WearPercent` (CharmCore) | float | 0 to 100 | 12 | Wear per saved landing. |

## Game members used

Checked by `CharmToast.Tests/GameContractTests.cs`:
- `PlayerAvatar.Update` (patched), `PlayerAvatar.isLocal`, `fallDamageResetState`, `playerHealth` (read), `PlayerAvatar.falling` and `FallingSet` (design dependency only).
- `PlayerController.instance`, `PlayerController.CollisionController`.
- `PlayerCollisionController.fallDistance` (read and written), `Grounded` (read).
- `PlayerTumble.TumbleRequest`, `ImpactHurtSet` (called by the game, prevented by the zeroing; not touched).
- `PlayerHealth.Hurt` (called when `BlockFallDamage` is 0).
- `ValuableObject.dollarValueCurrent` (wear, by CharmCore).

## Debug mode

Provided by CharmCore: the `Debug.DebugMode` setting (off by default) places the valuable on the floor at the start of each level and enables `Charm.LogDebug` output.

## Known limits

- Looked up by exact prefab name; a game update that renames the prefab needs a mod update.
- Not verified in game: the build and unit tests prove the logic and that the game members exist, not that the hook behaves as described in a running game.
- Unverified: that `Grounded` is reliable at every landing (a slope or a moving platform), that `PlayerAvatar.Update` is patched correctly by Harmony in the shipped game, and the 0 setting of `BlockFallDamage` (the damage amount is copied from the decompiled tiers). Whether the game's impact damage can happen without a tumble was not found in the code (the caller of `PlayerTumble.BreakImpact` is not in the decompiled source).
- Other tumbles (enemy hits, grabs, explosions) are untouched; only the fall tumble is removed.
- Falls long enough to be fatal are also saved by default; use `BlockFallDamage = 0` for a Toast that only removes the tumble.

## Decisions

- **Hook:** prefix on `PlayerAvatar.Update` that zeroes `fallDistance`, instead of patching `TumbleRequest` (it cannot tell the fall request from other requests) or using the feather mechanism.
- **Landing, not frame:** wear is charged once when the fall ends on the ground, using the tracked length.
- **`MinFallDistance`:** the game never tumbles under 8 m, so the charm only acts from 8 m. The setting only decides which saved falls cost wear; values over 8 let falls between 8 m and the setting be saved for free (the charm cannot wait, the game would already have tumbled the player). Values under 8 behave like 8.
- **`BlockFallDamage` as a 0/1 `AddSetting`:** CharmCore offers only numeric settings; 0.5 and above counts as on.
- **No cooldown:** wear per landing is cost enough.

## Runtime hygiene

- **References and level state:** fixed. `FallTracker` (inside `ButterSideUp`) remembers a fall in progress. `ToastCharm` now overrides `OnLevelEnded` and calls `ButterSideUp.Reset()`, so a fall that was being saved cannot carry into the next level. It still resets whenever the charm is inactive. `FallTracker` holds only numbers, no game objects.
- **Per-frame work:** the `PlayerAvatar.Update` prefix runs for every avatar but returns on `isLocal` first; inactive, it only resets the tracker (a few assignments). Active, `Step` is allocation-free (a struct result, no collections).
- **Debug logs:** fixed. The two `LogDebug` lines in `Process` (threshold reached, landing) are now guarded with `if (Charm.DebugMode)`.
- **Changes to the game:** ok. The only write is `fallDistance = 0` on the frame the fall reaches the tumble threshold, which is the effect itself; nothing persists once the charm stops, and the game recalculates the distance every frame.
- **Static game objects and Find calls:** none. The plugin's static `Effect` holds the effect object, which holds the charm and the tracker, not game objects.
- Known limit outside this mod: while the charm is active, CharmCore's `IsActive` and `CharmSetting.Value` build their host-setting key strings on each call; that is CharmCore's code and was left alone.
