# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Status: a VB.NET game being ported to Odin/wasm

Bagel Quest! is a 2023 jam game ("A Game By Its Cover 2023", art from Kenney's 1-bit pack) written in VB.NET (net8.0) on MonoGame, with game content scripted in Lua (NLua). The task is to port it to the user's Odin/`js_wasm32` stack. The Odin port is **built and passing its tests** (phases 0 to 8 of `PORT.md`; phase 8, the release layout, is done, nothing pushed). `src/` is the original VB game and is the reference. Keep it in the tree until the port ships (Murder Hobo and Kordanor's Cabal kept their VB the same way; Murder Hobo removed it afterwards, recoverable from git history).

The user's notes vault is the source of truth for the stack. Read these before writing Odin (vault: `/home/yermom/git/bok-of-splorr/splorr`):

- `Tech/Odin wasm recipe.md`: build command, frame loop, and the per-game "variant" sections. Closest fits here: **Kordanor's Cabal** (pure core, framebuffer, JS-owned loop, save slots, VB reference frames), **Murder Hobo** (same split plus a native SDL2 client and a wasm parity test), **Turn Left at the Cow** (tinted 1-bit sheets, which suits the Kenney art), **HamQuest** (generated content tables from a data-driven original).
- `Tech/Kordanor's Cabal Odin port.md`: the port process (decisions settled one at a time with the user, a findings log per step, tests green at the end of each step).
- `Tech/Release standard.md`, `Tech/Shipping to itch.io.md`, `Gotchas.md`.

Toolchain: Odin `dev-2026-07-nightly` at `/home/yermom/ODIN/odin`. Target `js_wasm32` with Odin's own `odin.js` (no emscripten). Serve `build/web` over http, never `file://`.

## The original's architecture (what to port)

Eleven projects in `src/src.sln`, layered bottom-up. Dependencies run one way: Presentation → Business → Persistence interfaces; Persistence.Implementation → Data.

- **`SPLORR.Game`**: shared helpers (`RNG`, `Utility`, maze generator). Lua scripts call into it via `import('SPLORR.Game')`.
- **`AOS.UI`** and **`AOS.Presentation`**: generic engine, not game-specific. `AOS.UI` has the state-machine base classes (`BaseGameState`, `BasePickerState`, `BaseGameController`), the pixel buffer (`IPixelSink`/`OffscreenBuffer`), bitmap fonts (`Font`, `GlyphBuffer`), and ready-made boilerplate states (splash, main menu, options, save/load, volume, confirm quit/abandon). `AOS.Presentation` is the MonoGame `Host` (window, input, audio). **Only `Host` is MonoGame-specific**; everything above it draws into an in-memory pixel buffer, which maps directly to the Odin framebuffer approach.
- **`BQ.Persistence`**: interfaces only (`IWorld`, `ICharacter`, `IItem`, `IMap`, `ICell`, `IMessage`, ...). **`BQ.Persistence.Implementation`** has the in-memory classes and `*DataClient`s that convert to and from **`BQ.Data`** (the serializable save records). `BQ.Persistence.Tests` covers this layer.
- **`BQ.Business`** (about 2,800 lines, the real game): `Model/` (`WorldModel`, `AvatarModel` with crafting/equipment/inventory/statistics sub-models, foraging, combat, maps, messages), `Extensions/` (rules as extension methods over the persistence interfaces), `Initializers/` (world, towns, wilderness, cellar, item/avatar setup), and `*Types/` descriptor tables (character, item, terrain, effect, equip-slot, map, message, recipe types).
- **`BQ.Presentation`**: the game's `GameController` plus one `*State` per screen (`MoveState`, `CombatState`, `CraftState`, `ForageState`, inventory/ground/equipment menus, `DeadState`, `WinnerState`, ...). `BagelQuestConfig`/`Settings` hold options.
- **`BagelQuest`**: `Program.vb` wires it together and loads `Content/*.json` (keys, hues, fonts, sfx, mux) at startup. `FontMaker` is a stand-alone tool.

### Content is data plus Lua

`src/BagelQuest/Content/` holds the JSON type tables (`itemTypes`, `characterTypes`, `terrainTypes`, `effectTypes`, `equipSlots`, `recipes`, `messages`, `hue`, `keys`, `font`/`sfx`/`mux`) and `Scripts/**/*.lua`. Types point at scripts by path (for example an item's `"FullNameScript"`). Scripts run through `WorldModel.LuaState` and are used for character `Initialize`, item `FullName`, terrain behaviour, and effect handlers (`Characters/Loxy/*` is the largest set, the quest NPC: build fire/furnace, buy, accept quest, ...). The port has to decide what replaces Lua (generated Odin tables plus `switch`es over effect/terrain enums is the pattern the vault ports used; see HamQuest's strict generator and Kordanor's `Check`/`Action` enums). Settle that with the user before generating anything.

Audio: `Content/Audio/Sfx/*.wav`, `Mux/*.ogg` (and `sourcematerial/` has the MIDI/m4a sources). The vault's web ports decode audio in the page; see the Fortune Hunter and Turn Left at the Cow variants.

## Commands (Odin port)

```bash
tools/test.sh          # generated-asset freshness, odin test (native), web build with vet flags; must end ALL TESTS PASSED
tools/build.sh web     # build/web (wasm + page); tools/serve.sh serves it on :8080 (PORT= to change)
tools/oracle.sh        # re-record reference screens from the original VB UI into docs/reference/vb/
python3 tools/gen/gen_assets.py   # regenerate odin/game/assets_data.odin from src/BagelQuest/Content
```

Odin code lives in `odin/game` (portable core, imported as `bq:game` via `-collection:bq=odin`) and `odin/platform/web`. Reference-frame tests compare hue indexes with `docs/reference/vb/*.txt`. Plan, decisions and per-phase findings are in `PORT.md`; the original's behaviour is in `docs/game-reference.md` and its defects in `docs/defect-audit.md`.

## Commands (original VB game)

```bash
dotnet build src/src.sln
dotnet run --project src/BagelQuest/BagelQuest.vbproj      # run from the project dir's output; Content/ paths are relative
dotnet test src/BQ.Persistence.Tests                        # only the persistence layer has tests
```

`OptionStrict On` and `TreatWarningsAsErrors` are set for the game project. The `AOS.UI` boilerplate persists through its own settings/saves; `keys.json` maps MonoGame key names to commands (`A`, `B`, `Up`, `Down`, `Left`, `Right`).

## Release layout

The repo follows the vault's release standard (`Tech/release-audit.sh /home/yermom/git/tggd-agbic2023` shows all OK): `ITCH_DESCRIPTION.md`, `assets/cover.png` (630 by 500), `assets/screenshots/` (made by `tools/make_screenshots.py` from the compared frames), `devlog/20261017/devlog.md` (a draft for the user to post), `tools/ship.sh` and a one-line `shippit.sh` wrapper. `assets/source/` holds the old cover source and the 2023 window screenshots.

- `tools/ship.sh` runs the full tests, builds `-o:size`, zips `build/web` to `build/bagel-quest-html5.zip` (index.html at the root, about 1.2 MB) and **pushes only with `--push`**. It never commits.
- The itch page is live with channels `thegrumpygamedev/bagel-quest:windows`, `:linux`, `:mac` (2023 VB builds). **Never rename or push to those.** The web build goes to a new `:html` channel; the user decided to remove the old downloads by hand on itch.
- Never `git push`, `butler push` or `shippit.sh --push` unless the user says so.
