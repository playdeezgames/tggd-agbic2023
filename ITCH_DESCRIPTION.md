# Itch.io page text

Page: https://thegrumpygamedev.itch.io/bagel-quest (the page exists; it becomes browser-only: you remove the 2023 windows, linux and mac downloads by hand on itch).
Pasted in by hand; nothing here is published by the repo.

## Short description

Forage, craft and cook yer way to a bagel. A tiny survival quest from A Game By Its Cover 2023, now in yer browser.

## Paste this as the page description

Loxy wants a bagel. Loxy has two hit points, ten energy and twenty jools.

Walk the wilderness, learn to forage from the druid, and gather sticks, rocks, clay, wheat and peppers. Build a fire, put it out for charcoal, make a torch, throw up a furnace, grind flour, get a pot from the potter and fill it at a river for dough. Cook the bagel and you win. Meet anything on the way (glops, scarecrows, rats) and it is a fight: attack, or run.

The town has an inn, a healer, a health trainer, an endurance trainer, a druid and a potter. The innkeeper pays a jool for every rat tail from the cellar, if you can face the rats.

**Controls:** arrow keys, WASD (or ZQSD), or the numpad to move; Space or Enter to select; Escape to go back and open the game menu. A gamepad works too (d-pad and A and B), and on a phone or tablet there are on-screen buttons.

**Saving:** pick Save in the game menu: five slots, plus a quick save, are kept in yer browser. You can also export a save to a file and load it back from the Load screen.

Art: Kenney's 1-bit pack (kenney.nl). Sound effects and the original game by TheGrumpyGameDev, made for A Game By Its Cover 2023. The music is a handful of simple chiptune loops the game makes for itself in the browser, written for the 2026 port. The game was written in VB.NET in 2023 and rebuilt in Odin for the browser in 2026. The 2026 Odin port was written with Claude Code (generative AI).

## Notes (not for the page)

- Push target: `thegrumpygamedev/bagel-quest:html`, a new channel. The old `windows`, `linux` and `mac` channels are left as they are by the repo: **you remove those three uploads by hand on itch** (decided; the repo never renames or touches them). `tools/ship.sh` builds and zips; it only pushes with `--push`.
- On the page's Edit game screen: set "Kind of project" to HTML, tick "This file will be played in the browser" on the html upload and make the embed 1152 by 648 (or larger, it scales to fit).
- The cover is `assets/cover.png` (630 by 500, the 2023 cover); screenshots come from `tools/make_screenshots.py` (regenerable, 1152 by 648).
- Credits (decided): the user made the sound effects, so the page says so. Music: the original is not in this build; the page's own tunes (`page/music.js`) are kept; edit `THEMES` to change them.
- The page text discloses that the Odin port was written with Claude Code. If the edit screen has a generative-AI disclosure field, fill it in to match.
