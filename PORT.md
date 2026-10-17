# Bagel Quest: Odin port plan

Status: **plan, not started. Nothing below is decided until the user settles it** (see "Decisions"). Read `CLAUDE.md` first. Method follows the vault's `Tech/Kordanor's Cabal Odin port.md`: decide first, build in small steps, end each step with tests green and a findings entry at the bottom of this file (done, verified, **not** verified, what it means for later steps).

## What is being ported (measured, not guessed)

| Piece | Size | Notes |
|---|---|---|
| `BQ.Business` | ~2,800 VB lines | the rules; the real work |
| `BQ.Presentation` | ~1,350 lines | one state class per screen |
| `AOS.UI` | ~1,150 lines | generic menus, fonts, pixel buffer, state base classes |
| `BQ.Persistence.Implementation` + `Data` + interfaces | ~1,350 lines | object graph and save records; collapses into flat Odin structs |
| Lua scripts | 132 files, ~400 lines in `Loxy/` (56 files), the rest tiny | e.g. `PotterMakePot`, `Buy`, `BuildFire`; `maze.lua` is a full maze generator |
| JSON tables | ~46 KB: 43 terrain types, 45 effect types, 5 characters, ~27 items, recipes, 15 messages | generated into Odin tables |
| Screen | 384 by 216 pixels (`ViewWidth`/`ViewHeight`), glyph + hue drawing (`font.json`, `hue.json`) | same frame size as Murder Hobo |
| Tests | `BQ.Persistence.Tests` only (9 files, persistence layer) | no rule or screen tests exist |

Not yet read in depth: `WorldInitializer` (208 lines), `WildernessInitializer`, `CombatModel`, `ForagingModel`, the `AOS.UI` boilerplate states, and most Lua. Step 0 reads them before anything is promised.

## Target shape

Copy Murder Hobo's split (vault: `Tech/Odin wasm recipe.md`, "one core, a shared framebuffer"), minus the native client unless the user wants it:

```
odin/game/           pure core, no platform imports: rules, world, content tables, UI states, rasterizer
odin/platform/web/   #+build js, package main: exported proc "c" functions, no game logic
odin/platform/native/ (optional, #+build !js) SDL2 dev client; also what makes QA scripts possible
odin/tests/          odin test natively; same cases checked as wasm under node
web/                 index.html, platform.js, storage.js
tools/               build.sh, test.sh, serve.sh, ship.sh, gen/ (content generator), run_wasm_node.js
docs/reference/vb/   frames recorded from the VB game
src/                 the VB original, kept until the port ships
```

- `core_step(core, {dt, now_ms, events}, &out)` once per frame; `Services` (storage get/set/remove, entropy, log) passed once. Output: 384 by 216 `u32` RGBA frame, sound ids, volumes, quit flag. The wall clock is an argument.
- **The VB UI already draws into an in-memory pixel buffer**, with only `AOS.Presentation/Host` touching MonoGame. So the rasterizer is a port of `AOS.UI/Buffer`, `Font`, `Drawer`, `Sprite`, not a redesign.
- **Persistence layer collapses.** The `IWorld`/`ICell`/`ICharacter` interfaces plus `*DataClient` wrappers exist to serialize an object graph. In Odin this becomes one flat `World` (maps as fixed arrays of cells, characters and items in creation-order arrays with ids), with JSON save/load that validates every enum, id and reference (Shark Attackers / Kordanor's pattern; `json.parse_string(text, .JSON, true, allocator)`).
- **Lua is replaced, not embedded.** Scripts become Odin procedures selected by enum `switch` (Kordanor's `Check`/`Action` pattern) and the JSON tables become generated enums plus read-only tables (HamQuest's strict generator: fail on unknown properties and dangling cross references). The string-path script fields (`"FullNameScript": "./Content/Scripts/..."`) become enum-keyed hooks.
- RNG: the original uses `SPLORR.Game.RNG` (not seedable from outside). Use the Kordanor's xoshiro256** with state in the save so a seed reproduces a world; the maze generator in `SPLORR.Game/Maze` and `maze.lua` is ported as plain Odin.
- Audio and ship follow Fortune Hunter / Turn Left at the Cow: `SOUND_FILES` table, WAVs decoded in the page, unlock on first input, `tools/ship.sh` from the Fortune Hunter template. Music is two OGGs (`MainTheme`, `CombatTheme`) streamed by the page.

## Decisions settled (user, this session)

1. **Fidelity:** faithful port; clear bugs (crashes, unreachable states, unwinnable quest paths) are fixed as found and every one is logged in `docs/defect-audit.md`. Content and balance questions are listed, not changed, without the user.
2. **Native client:** web plus SDL2 (Murder Hobo split: `odin/platform/native`, `--script`, `--dump`, `--seed`, `--data`).
3. **Channels:** **web only** release. Add `thegrumpygamedev/bagel-quest:html`; the live `windows`/`linux`/`mac` channels are left untouched and not renamed. The SDL2 client is therefore a dev and QA tool, not shipped.
4. **Oracle:** VB oracle harness records reference screens into `docs/reference/vb/`; tests require pixel matches; harness removed after shipping.
5. **Dice:** seed search, as in Kordanor's, to reproduce recorded fights.
6. **Saves:** no import of VB saves.
7. **Controls:** keys + gamepad + touch (touch in phase 7).
8. **Keyboard:** read `e.key` before `e.code` (vault Gotchas; not a real choice).

The list below is the original question set, kept for reference.

## Decisions (original list)

1. **Fidelity.** Faithful port (same rules, same map, same text) or fix defects as found? Kordanor's fixed VB bugs and logged them in a `dead-code-audit.md`; content questions were *not* changed without the user. Recommended: faithful, with a defect list.
2. **Native client.** Web only (Fortune Hunter) or web plus SDL2 (Murder Hobo)? The native client buys scripted QA (`--script`, `--dump`) and a dev build; it costs a second platform package. Recommended: web only, with `tools/frame_dump.js` under node for screenshots, unless parity bugs appear.
3. **Release channel.** The live itch page has `windows`, `linux`, `mac` channels. A web build is an extra `html` channel. Keep native builds up, retire them, or stay web-only? **No existing channel is renamed regardless.**
4. **Reference oracle.** Record screens from the real VB UI headlessly (as Murder Hobo and Kordanor's did) and compare pixel for pixel, or accept Odin-owned golden frames? Recommended: oracle, since `AOS.UI` draws to a buffer without MonoGame. Cost: a small VB/C# harness referencing the old projects; it is deleted after shipping.
5. **Dice and RNG.** The VB RNG cannot be seeded, so recorded fights cannot be reproduced exactly. Kordanor's solved this by searching world seeds until the first rolls matched. Decide whether combat screens need that or only layout checks.
6. **Saves.** No VB save import (as in Kordanor's) or a converter for the old JSON? Recommended: no import; slots in `localStorage`, plus export/import files.
7. **Controls.** `keys.json` defaults (arrows/WASD/ZQSD/numpad, Space/Enter = A, Esc = B). Keep, and add touch (tap cells with the `precise` flag) and gamepad? Recommended: keys and gamepad first, touch in the last phase.
8. **Input read order.** Read `e.key` before `e.code` (vault Gotchas).

## Phases

Each phase ends at an approval gate: `tools/test.sh` green, a findings entry, the user plays the build.

0. **Read and inventory.** Read the unread code listed above; write `docs/game-reference.md` (map list and sizes, every effect/terrain/item/character with its Lua behaviour, recipes, quest flow, win condition). Output is the list of every Lua behaviour that needs a hand-written Odin procedure versus pure data. *Gate:* the user confirms the inventory and answers decisions 1 to 4.
1. **Skeleton and rasterizer.** `odin/game` with `core_step`, framebuffer, font/glyph/hue drawing, `web` platform + page, `build.sh`, `serve.sh`, `test.sh` (native `odin test -o:speed`, `odin check -vet` on both targets, `-vet-shadowing -vet-unused`). Draw the splash. *Gate:* splash matches the VB frame.
2. **Content generator.** `tools/gen/gen_content.py` (stdlib only) turns the JSON tables into enums and tables; `CONTENT_HASH` goes into saves; `test.sh` fails on stale output. Lua stays out of this step.
3. **World, maps, rules.** Flat `World`, cell/terrain/item/character model, map initializers (town, buildings, cellar, wilderness, rivers, maze), movement, foraging, crafting, equipment, inventory, statistics, energy, combat, messages. Soak test plays thousands of random commands with `world_validate`; a tracking allocator makes leaks fail.
4. **Scripted behaviour.** Port the 132 Lua scripts to enum-dispatched procedures, grouped by character/terrain/item, with a unit test per quest step (rat quest, fire, furnace, pottery, trainers, druid, innkeeper, cellar, teleport, shops).
5. **Screens.** Port the `AOS.UI` boilerplate states and the `BQ.Presentation` states (move, forage, craft, equip, inventory, ground, combat, message, dead, winner, navigation, statistics). Compare against oracle frames as decided.
6. **Saves and options.** JSON save with strict validation and fuzz test; slots; export/import; volume and window-size options (window size becomes the page's job).
7. **Audio, input polish, wasm parity.** Sounds and music in the page; gamepad; touch if chosen. `tools/run_wasm_node.js` replays seed and inputs through the wasm and compares a frame hash with the native test (catches 32-bit `int` and allocator differences).
8. **Release.** Bring the repo to the release standard: `ITCH_DESCRIPTION.md`, `assets/cover.png` (630 by 500, from `ss/Cover.png`), `assets/screenshots/` (from `ss/`), `devlog/YYYYMMDD/devlog.md`, `tools/ship.sh` (Fortune Hunter template; tests, build `-o:size`, zip; pushes only with `--push`; never commits) and a one-line `shippit.sh` wrapper. Prove it with a no-push run. Then decide whether to delete `src/` and the old publish scripts. *Nothing is pushed without the user saying so.*

## Risks

- **Hidden behaviour in Lua and `Initializers`** is the main unknown; phase 0 exists to size it before committing.
- **Unseeded VB RNG** limits exact comparison of combat and world generation to layout-level checks.
- **No existing rule tests** means the old game gives no regression net; the oracle frames and a written reference doc are the substitute.
- Odin constant arrays cannot be indexed at runtime (use `:=`), and unbounded retry loops freeze the tab: cap every "find a free tile" loop (vault Gotchas).
- `int` is 32 bits on wasm: use explicit sized types in saves and test parity under node.

## Findings log

### Phase 0: read and inventory (done, awaiting your approval)

**Done.** Read all of `BQ.Business`, `BQ.Presentation`, the generic `AOS.UI` controller/picker/font/buffer code, `SPLORR.Game` RNG and maze, all JSON tables and all 132 Lua scripts. Wrote `docs/game-reference.md` and `docs/defect-audit.md`.

**Findings that change the plan:**
- **No interpreter is needed.** The 56 Loxy effect scripts are recipe calls, flag/stat checks, messages and teleports; they fit an enum `switch`. Estimated Odin size is similar to the VB (about 4,000 lines plus generated tables).
- **Rendering is hue-indexed.** All art (terrain, items, creatures) is in `BagelQuest.json`, a 16 by 16 bitmap font; the whole UI is `Fill` plus glyph copies. This makes phase 1 smaller and the oracle comparison exact.
- **Foraging is a mini-game** (a grid of hidden cells); the `Forage` terrain effect and `Forage.lua` are dead.
- **The VB RNG is unseeded** (`System.Random`) and recipes consume items from an unordered set, so exact replays need the seed-search method and a defined consume order in Odin.
- **Combat repeats rounds until damage is dealt**, one energy per command.
- 12 clear defects (D1 to D12) and 12 content questions (C1 to C12) are in the audit; the clear ones are fixed per decision 1.

**Verified:** by reading the source only. **Not verified:** nothing was built or run (this machine has .NET 10 only; the projects target net8.0, so the oracle harness will need roll-forward). Sound usage was confirmed by grep (six sound files are never played). The `Persistence.Tests` project was not run.

**For later phases:** C1 to C12 want your answers before phase 4 (they change scripts and content): at least C1 (depleted terrain), C2 (placeholder signs), C6 (silent sounds).


### Phase 1: skeleton and rasterizer (done, awaiting your approval)

**Done.**
- `odin/game`: `api.odin` (platform interface, same shape as Murder Hobo), `render.odin` (hue-indexed rasterizer: fill, UI-font text, 16 by 16 tile draw, status bar, header, `controls_text`), `core.odin` (`core_init`/`core_step`, splash), generated `assets_data.odin`.
- `tools/gen/gen_assets.py` turns `hue.json`, `CyFont8x8.json` and `BagelQuest.json` into Odin tables; `tools/test.sh` fails if the output is stale.
- `odin/platform/web` + `page/` (canvas, whole-number scaling, key and tap input), `tools/build.sh`, `tools/serve.sh`, `tools/test.sh`.
- **The VB oracle works** (`tools/vb-oracle`, `tools/oracle.sh`): a small C# harness drives the real `GameController` and `BagelQuestContext` headlessly and records hue frames into `docs/reference/vb/` (splash, main menu, main menu down 1, about, options). The VB libraries build with the .NET 10 SDK as netstandard2.1, so no roll-forward was needed after all.
- Tests (5): palette, no-redraw when idle, off-frame drawing is ignored, `controls_text`, and **the splash matches the VB frame pixel for pixel**. I broke the outline colour on purpose and the test failed with a pixel count, then restored it.
- Checked in the Claude browser pane: the splash draws, no console errors.

**Not done / not verified:** the SDL2 native client (decision 2) is not started; it fits after phase 5 when there is something to drive, unless you want it now. Only the splash is compared with the VB; the main menu, about and options frames are recorded but not yet drawn. No sound, saves or gamepad yet.

**For later phases:** the oracle harness is the place to add every later scenario. Its commands run one Render per command, as the Host does per Update.


### Phase 2: content generator (done, awaiting your approval)

**Done.**
- `tools/gen/gen_content.py` (stdlib only) turns the original JSON tables and the data-carrying Lua into `odin/game/content_data.odin` (440 lines) and `odin/platform/web/page/keys.js`. Shared helpers are in `tools/gen/common.py` (`gen_assets.py` now uses them).
- **Strict:** every property of every object goes through a schema (`Strict.take`/`finish`), every cross reference is checked, and Lua that carries data is parsed with a fixed pattern (terrain `Initialize.lua` = `ForageRemaining`, item `FullName.lua` = plain or with durability) and anything else fails. I added a fake property to `itemTypes.json` and the run failed with `unknown properties ['Weight']`, then restored the file.
- **Output:** enums (`Item_Type` 27, `Terrain_Type` 43, `Character_Type` 5, `Recipe_Type` 20, `Effect_Kind` 55, `Message_Type` 11, `Equip_Slot`, `Sfx_Id`, `Mux_Id`) and read-only tables (`ITEMS`, `TERRAINS`, `CHARACTERS`, `RECIPES`, `MESSAGES`, plus pools for ingredients, creature and forage weights, message lines). Weighted lists are `Range{start,count}` into one pool each. `CONTENT_HASH` (FNV-1a of the generated text) is ready for saves.
- **`Effect_Kind` is the exact set of 55 effect scripts phase 4 must write** (taken from the avatar's script table; every message choice, item verb and terrain verb is checked to be in it).
- `keys.js` comes from `Content/keys.json`, so the page now accepts the original's keys including Z and Q (AZERTY) and the numpad; the Murder Hobo extras (Backspace) are gone. I checked in the browser pane that `z` and numpad 8 map to up and `x` is ignored.
- `tools/test.sh` now also fails when any of the three generated files is stale. 12 tests pass: counts, Loxy's start stats, item stats and verb order, the bagel chain, terrain flags/peril/forage tables, pool bounds, messages and sound ids.

**The generator's complete list of departures** (in `IGNORED` and `DATA_FIXES`): the dead terrain `Forage` effect, four stale messages (`HealerTalk`, `HealthTrainerTalk`, `DruidTalk`, `TownSign4`), the `"test"` full name of Rat Corpse (D6), `Config/keys.json` (an older copy that lacks Q and Z; `Content/keys.json` is what the game loads), and character `Initialize.lua` (hand-written in phase 4).

**Not generated, by design:** map sizes and layouts (phase 3 code), the effect behaviour, and the dialogue/skill data inside `DruidTeachMenu` and friends (phase 4). Not verified: nothing here has been compared with a running VB game, only with the source data; `depleted` is generated but unused (audit C1).

**Open items for you:** C1 (depleted terrain), C2 (placeholder signs; the text is carried over as is) and C6 (silent sounds) before phase 4.


### Phase 3: world, maps and rules (done, awaiting your approval)

**Done.** All in `odin/game`, none of it touches a platform:
- `world.odin`: one flat `World` (**190 KB**, no heap, no globals). Pools addressed by small ids: 6,257 cells (town 17 by 17, wilderness 75 by 75, seven 7 by 7 rooms), 8,191 items, 127 characters, 63 cell effects, a ring of 32 messages. Items on the ground, in an inventory and characters in a cell are linked lists that keep arrival order.
- `maps.odin`: town from its minimap, the six buildings, inn, cellar, the wilderness (terrain by the original's weights, a 5 by 5 spanning-tree maze drawn as rivers, one town tile), spawn of Loxy, and all the teleport stitching. Character initializers (the Lua `Initialize` scripts, a line each) are plain code here.
- `rules.odin`: statistics and displays, XP with the doubling goal, attack and counter-attack, scuffing and breaking, death and drops, movement with bumping and peril, sleeping, combat commands, recipes (net change, oldest first), foraging and the grid sizes, inventory/ground stacks, take/drop, equip/unequip, and the Craft menu conditions.
- `messages.odin`, `effects.odin` (teleport, signs, the cellar door; the other 48 effect kinds show a stub line until phase 4), `rng.odin` (xoshiro256**), `validate.odin` (`world_validate`: lists, counts, free list, stats, effects, messages).
- **Tests: 49 pass** (13 new world tests, 16 rules tests, soak, validate-catches-corruption, size). The soak plays 6 seeds of 30,000 random model-level steps (moves, fights, runs, forages, crafts, take, drop, equip, sleep, choices), validating every 50 steps, and requires thresholds on how much of each it did, so it cannot pass by doing nothing. A tracking allocator shows world creation and play make no leaks and no bad frees.

**Decisions made while building (all reversible):**
- Recipes take items **oldest first** and inventories group in **arrival order**; the VB used an unordered hash set (inventory screens with mixed stacks may differ from the VB in order; the oracle comparison will normalise that).
- Defects D3 (apostrophe), D7 (fight loop capped at 1,000 rounds) and D8/D10 (retry loops bounded, empty grid guarded) are fixed in code. The others (D1, D2, D4, D5) are in text that belongs to phase 4 scripts.
- A full item pool makes crafting fail instead of losing items; forage simply finds nothing.

**Corrections to earlier docs:** the inn has **two** beds, not four (fixed in `game-reference.md` and the audit); the town minimap has 10 houses (six are building doors, four decoration).

**Not verified:** (1) the world code is not referenced by the core yet, so the **wasm build does not include it** (wasm size unchanged) and 32-bit `int` behaviour is untested there; phase 7's native-versus-wasm parity run covers it, and phase 5 will wire it in. (2) Rules were checked against my reading of the VB source, not against recorded VB frames or fights (needs phase 5 screens and the seed search). (3) Eating, shops, quests and the crafting verbs are phase 4.

**For phase 4:** C1, C2 and C6 still want your answers; they change scripts and content.


### Phase 4: scripted behaviour (done, awaiting your approval)

**Done.**
- `odin/game/effects.odin`: **all 55 effect kinds** are real code now, one `switch` over `Effect_Kind` with no default, so a new effect in the data is a compile error. That covers the plumbing (teleport, signs, cellar), 12 dialogues, the rat quest, inn, shops (Buy), the basin, both trainers, the potter, the river, the Druid's skill menu and six skills, every crafting verb, and all six ways to eat. Texts are the originals'. Screen entry points: `do_terrain_verb`, `do_item_effect`, `message_choose`.
- **Your decisions applied:** C1 kept (no depletion). C2: the four placeholder signs now read "Home of Barry the Forager", "For Rent: One Hut", "Beware of Rat" and "Bagel Appreciation Society" (generator `MESSAGE_TEXT_FIXES`; edit the strings there and regenerate). C6: the six silent sounds are used; the full map is in `docs/defect-audit.md` (crafting = Craft, eating = Eat or Tasty, picking up = Take, purchases and pots = Yoink, level up, learning, training, healing and the rat reward = WooHoo, refusals = the original "Shucks"). Sounds without a message (pick-ups, level-up) go through a small queue the platform will drain (`sfx_drain`).
- **Fixed from the audit:** D1 (`{jools}` shown literally), D2 (stray bracket), D4 ("already know"), D5 ("And wash"), plus two found now: D13 (seasoned rat showed "That was a spicy one!" every time, sometimes twice) and D14 (filling a pot you no longer have claimed success).
- **Found while testing:** the message choice buffer was 40 characters and cut the Druid's longer menu labels (45 chars); it is 48 now, the screen's width.
- **New content questions, kept as the original** (C13 to C15 in the audit): the Fire and Torch menu labels say "-5 Rock" but only require the materials, building a fire, furnace or torch spends the energy before checking the materials, and the basin heals only up to 5 health at any maximum.
- **Tests: 64 pass.** 15 new test functions cover each dialogue by exact text and choice list, the whole rat quest, the inn, skills (including each refusal), shop and re-opened menus, basin and trainers, the potter, fire/torch/furnace (including the energy order), mixing and cooking, eating (coin tosses and death), spice odds, sounds, the signs, and **a walk from the Druid to a bagel** through the same calls the screens will make. The soak now also uses random item verbs and terrain verbs.

**Not verified:** the texts were compared with the Lua by reading, not against a running VB game (no recorded dialogue screens yet; phase 5 adds them to the oracle). The wasm build still does not contain the world (phase 5 wires it in).

### Phase 5: the screens (IN PROGRESS, stopped at a usage limit)

**Done.** `odin/game/ui*.odin` port the whole state machine (stack, pickers, menus, Navigation, Actions, Craft, Ground, Inventory, Forage, Combat, Message, Statistics, Dead, Winner); `core.odin` now owns the world and the UI and reports sounds/music/volumes. The oracle replays `docs/reference/scenes.txt` (a small DSL) against the real VB UI and `odin/tests/screens_test.odin` replays the same file on the port: **127 shots, all matching pixel for pixel except 5 listed known differences** (each with its exact pixel count). `tools/frame2png.py` shows a diff.

**Found in the original:** it crashes when you drop your last stack (D15), shows `System.Object[]` as the name of every worn or wielded item (D18), and has a `<placeholder>` header (D16); all fixed in the port and logged in `docs/defect-audit.md`. Two of my scripted scenes (putting out a fire and a failed knap, then emptying the pack) crashed the VB for the same reason and were cut from the scene file.

**Not done:** the web page is not updated for the new core (sound/music output, key repeat 1.0 s then 0.3 s, full screen), nothing has been run in the browser since the core changed, the wasm build with the world has not been size or parity checked, save slots are stubs (phase 6), the native SDL2 client is not started, `tools/test.sh` was last run only as `odin test` (65 pass); run it in full before approving.

#### Browser test pass (phase 5, played by hand in the Claude browser pane)

`tools/test.sh` passes in full (65 tests, web build with vet flags; wasm is now 491 KB with the world in it). Played the wasm build with synthetic and real key presses: splash, main menu, Embark, the town (walls, signs read, gate to the wilderness), the river-bank dialogue on arrival, wilderness view (river, trees, farms), a random Olive Glop fight (attack, kill, jools and XP lines), Statistics, Actions, Inventory and dropping the last stack (the original's crash, now goes back to Actions), Game Menu, Abandon, Options (no Window Size on the web, as designed), Quit to the closing screen and Play again. **No console errors and no crash.** Not exercised by hand: the Druid, forage grid, crafting, dying, winning (covered by the unit tests and the 127 compared screens).

**Gaps seen (all phase 6 and 7 work):** holding a key does nothing (`e.repeat` is ignored; the original repeats after 1.0 s then every 0.3 s); Toggle Full Screen does nothing yet; no sound or music (the core reports them, the page does not play them); no touch controls (the phone-width layout just scales the picture); no saves.

### Phase 6: saves and options (done, awaiting your approval)

**Done.**
- `odin/game/save.odin`: one JSON text `{format, version, content, summary, rng, avatar, terrain, forage, effects, cell_effects, ground, items, chars}`. Enums are stored by name; the terrain is one character per cell; a fresh game is **about 13 KB**. It records `CONTENT_HASH` and `SAVE_VERSION`, so a save only loads against the content and rules that made it. The loader validates every name, id, range and list into a **scratch world** and then `world_validate`s it; the live world is replaced only if all of it holds, so a bad file changes nothing.
- Screens: **Save...** and **Load...** use five slots, **Scum Save/Load** use slot 0 (all in `localStorage` as `bq:slotN`), the Save screen gets an **Export...** entry (a file download) and the Load screen an **Import...** entry (a small file-chooser panel, so the click is the player's own). They appear only when the platform offers them, so the frames compared with the original are unchanged. A bad slot or file plays the failure sound and shows why.
- **Settings** (sfx and mux volume, full screen, window scale) are saved as `bq:config` and read at start; out-of-range values keep the defaults.
- `Services` gained storage, download and file request; `Input_Event` gained a `File` kind with its data; `odin/platform/web` has the storage glue (`storage.js`), `platform_alloc`/`platform_file`, and the download and file panel in `platform.js`.
- **Tests: 73 pass** (8 new): round trips over six played worlds (save, load, save again gives identical text; a loaded game then plays on exactly like the original), size, nine kinds of bad file each refused with the live world untouched, **a fuzz run of 1,500 mutated saves** (each refused or a valid world), slots through the real screens including a damaged slot, scum save/load, settings (including junk values), export and import (including a bad file and an unrequested one).
- **Played in the browser:** embarked, saved to slot 1 (13,391 bytes in localStorage), reloaded the page, loaded it back, then abandoned and imported the same text through the file panel: all worked with no console errors.

**Not verified / not done:** the real file-chooser dialog and the actual file download were not clicked (I fed the input a file by script and checked only that the save text was produced); phone browsers (especially Safari) are untested; full screen is stored but the page still does not act on it, and sound, key repeat and touch remain phase 7. A save made mid-fight loads into the fight (the creature is a character in the cell); messages waiting to be shown are not saved (the menus cannot be opened while one is pending).

### Phase 7: audio, input polish, wasm parity (done, awaiting your approval)

**Done.**
- **Music decision:** you asked to drop the original music. The OGG files are **no longer shipped** (`tools/build.sh` copies only the 13 sound effects; `sounds.js` lists only those). The music is now **synthesized in the page** (`page/music.js`): a square-wave lead over a triangle-wave bass, one short original loop per theme (main, combat, victory, death; ids from the core's `Mux_Id`), driven by the same core output, with the Mux volume setting. These tunes are **placeholders I wrote**; they are one table (`THEMES`) to edit, or `makeMusic()` can be swapped for something else with the same three methods. I could not listen to them: please judge them by ear.
- **Sound effects:** the 13 original WAVs are decoded with Web Audio and played at the Sfx volume when the core asks (message sounds, pick-ups, level-up, refusals, and the C6 map). Everything unlocks on the first key, click or touch (browser rule). The first-run sound pipeline was checked in the browser pane (audio unlocked, volumes read from the core), but not by ear.
- **Input:** a held key repeats like the original's Host: at once, again after 1.0 s, then every 0.3 s (checked in the pane: a 1.7 s hold moved the cursor three times, as the throttled pane allows). A gamepad (standard mapping: d-pad or left stick, A, B), and on-screen touch buttons (shown on a touch screen or after the first touch). Toggle Full Screen now acts (a key press is the click browsers want).
- **Wasm parity:** `odin/tests/parity_test.odin` plays a scripted 4,000-step game natively; `tools/wasm_parity.sh` plays the same script through the built wasm in headless Chrome (this machine has no node); `tools/test.sh` requires the digests (every frame, sound, music request and periodic save hash) to be equal. **They are: `frames=891959909 state=4215958799`.** This proves 32-bit `int`, the wasm allocator and `core:encoding/json` behave like native, including saving.
- `tools/test.sh` passes in full: 74 tests, generated-file freshness (now four files), web build with vet flags, and parity.

**Not verified:** nothing was heard (music, effects); the gamepad and the touch buttons were not exercised on real devices; full screen was not clicked; the parity script is random play, so it covers the screens and rules broadly but not every path (the unit tests and the 127 compared screens cover the rest).


### Phase 8: release layout (done, nothing pushed, awaiting your approval)

**Done.** `Tech/release-audit.sh` now reports **OK on every column** (description, cover, screenshots, ship script, wrapper, devlog).
- `ITCH_DESCRIPTION.md` (short description, page text, notes), `devlog/20261017/devlog.md` (a draft in the Fortune Hunter style, with the honest part about the old game's bugs and the decisions), `assets/cover.png` (the 2023 cover, already 630 by 500, `git mv`'d), `assets/screenshots/` (six 1152 by 648 images from `tools/make_screenshots.py`, regenerated from the frames the port reproduces; the 2023 window screenshots moved to `assets/source/vb-screenshots/`, with the cover source and the old 640 by 512 jam cover beside them), `tools/ship.sh` (from the Fortune Hunter template, target `thegrumpygamedev/bagel-quest:html`) and `shippit.sh` reduced to the one-line wrapper. **The old `shippit.sh`, which ran `dotnet publish`, `butler push` and `git commit`, is retired.**
- **Dry run without `--push`:** `tools/ship.sh` ran the full tests (74 tests, freshness, web build, wasm parity: all pass), built with `-o:size` (wasm 480 KB) and zipped `build/bagel-quest-html5.zip` (1.2 MB, 23 files, `index.html` at the root). It printed "not pushed". No `git add`, `git commit`, `git push` or `butler` was run.

**For you to decide before posting:** (1) whether the old windows/linux/mac downloads stay on the page next to the new html channel; (2) the credit for the sound effects (the page text does not claim them) and what to do about music; (3) the About screen's last line "See 'aboot.txt'" (a file the web build does not have; it is pixel-compared, so changing it means re-recording one frame); (4) the page's generative-AI disclosure field, if it has one; (5) deleting `src/`, the old oracle harness and the VB reference once you no longer need them (they are all recoverable from git history; the Murder Hobo port removed its VB after shipping).

**Not done on purpose:** no commit, no push, nothing uploaded; the vault page for the game (the standard asks for a line "Release layout follows [[Release standard]]") does not exist yet, so I did not write one.

### Release decisions (settled with you after phase 8)

1. **Old downloads: remove.** The page becomes browser-only; you delete the windows, linux and mac uploads by hand on itch. The repo never touches those channels. (The native SDL2 client is therefore not needed for release.)
2. **Sound effects credit: yours.** The page says the sound effects are by TheGrumpyGameDev.
3. **Music: keep my synthesized tunes** (`page/music.js`), placeholders you can edit; the page text says they are simple chiptune loops written for the 2026 port.
4. **About screen: the "See 'aboot.txt'" line is removed** (the `about` frame is a known difference of 242 pixels, listed in the screens test).
