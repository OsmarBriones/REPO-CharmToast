# QA battery: Butter Side Up (CharmToast)

Deploy: Pockets, CharmCore and CharmToast are in the r2modman `Debug` profile (compare hashes with the builds). Restart the game first.

## Quick start with debug mode
Set `DebugMode = true` in the `Debug` section of `BepInEx/config/com.osmar.CharmToast.cfg` (off by default). When a level starts, a Toast is placed on the floor about 2.5 m in front of you; the log then also shows the detailed lines. Any level works.

## Single player
- [ ] Log shows `Charm registered: Toast - Butter Side Up` and `CharmToast 1.0.0 loaded!`.
- [ ] Store the valuable in a slot: the slot shows its picture and value.
- [ ] With the Toast pocketed, jump from a height of at least 15 m (the truck roof or a tall shelf area): you land without tumbling and the log (DebugMode) shows `Fall of 8 m reached` once or twice and one `Landed after a fall`.
- [ ] The Toast loses about 12% of its value once per landing (not repeatedly while falling).
- [ ] A 5 to 7 m drop: no tumble (vanilla also has none), no wear.
- [ ] Same jump with the Toast out of the pocket: you tumble as usual (control).
- [ ] An enemy hit or a grab still tumbles you with the Toast pocketed.
- [ ] With `BlockFallDamage = 1` you take no damage from a big fall; with `0` you take the usual damage (5/15/25/50/100 by fall length) but still do not tumble.
- [ ] `MinFallDistance = 20`: an 8 to 15 m fall costs no wear, a 25 m fall does.
- [ ] Take the valuable out: the power stops.
- [ ] Use it until the valuable breaks: the slot empties, the power stops, no errors in the log.

## Settings
- [ ] `WearPercent = 0`: no wear. (This charm has no cooldown.)
- [ ] Charm `Enabled = false`: no effect and no wear. CharmCore `Enabled = false`: nothing works.

## Multiplayer (host and client)
- [ ] On the client: no tumble after a big fall, the value drops on both machines, and the host sees no tumble either.
- [ ] Host settings override the client's.
- [ ] Client leaves and rejoins: no stuck veil.

Log patterns: `Charm registered`, `threw`, `Exception`.
