# Changelog

## [Unreleased]

### Added
- Butter Side Up: while the Toast is pocketed, falls no longer throw you into a tumble; each saved landing wears the Toast.
- `MinFallDistance` (3 to 50 m, default 8) and `BlockFallDamage` (0 or 1, default 1) settings.
- `Debug.DebugMode` (off by default): puts the Toast on the floor at the start of each level and writes detailed logs.

### Changed
- Runtime hygiene pass: fall tracker reset at level end, debug logs in the per-frame prefix guarded.
