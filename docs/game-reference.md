# Bagel Quest: game reference (from the VB source)

Written in phase 0 of the Odin port (`PORT.md`) by reading the VB.NET and Lua source. Source paths are under `src/`. Behaviour here is what the **code** does; nothing was run (no VB build, no play-through). Anything marked **(unverified)** needs the oracle or a play test. Defects and content questions are in `docs/defect-audit.md`.

## 1. The game in one paragraph

You are Loxy, a tiny character (2 HP, 10 energy, 20 jools, 1 AP) on a 75 by 75 wilderness of grass, trees, farms, clay pits, rock quarries and rivers, with one town (a 17 by 17 map with seven buildings). **You win the moment a Bagel is in your inventory.** A bagel needs dough, a torch and five sticks, cooked at a furnace you build yourself, which needs a torch, rocks, clay and sticks, and a torch needs charcoal from a campfire you built and put out. Dough needs flour (milled from wheat) and a pot of water (a pot bought from the potter, filled at a river). You die at 0 HP. Both end screens return to the main menu.

## 2. Screens and flow

State machine from `AOS.UI` (`BaseGameController`: a stack of named states; `SetState` replaces the top, `SetStates(pushed, next)` pushes). Each state has `HandleCommand(cmd)`, `Render(buffer)`, `OnStart()`. Commands are `Up Down Left Right A B`.

```
Splash (A) -> MainMenu: Embark! | Scum Load | Load... | Options... | About... | Quit
  Embark -> new world -> Neutral
Neutral (no UI; picks the next state, first match wins):
  message pending -> Message ; avatar dead -> Dead ; enemy in cell -> Combat ; avatar has Bagel -> Winner ; else Navigation
Navigation: A = ActionMenu, B = GameMenu, arrows = Move*
ActionMenu (entries only when valid): Ground... | Inventory | Equipment | Sleep | Forage... | Craft... | Put Out Fire | Statistics
GameMenu: Continue Game | Scum Save | Save... | Scum Load | Options... | Abandon Game
Options: Toggle Full Screen | Window Size... | Sfx Volume... | Mux Volume...
```

- Every action state (`BaseActionState`: Move*, Sleep, Take, Drop, BuildFire, ...) runs one model call in `OnStart` and then goes to `Neutral` (Take returns to `Ground`, Drop to `Inventory`).
- **Messages are a FIFO queue in the world.** Rules push messages; `Neutral` shows them one at a time before anything else. A message has colour-hued lines, optional choices (each with an effect type that is run on choose) and an optional sound. `A` on a choice runs it; `B` first moves the highlight to the cancel choice (index 0, which is always the first choice) and a second `B` selects it. After a choice runs, the message is dismissed, so any new messages the choice made are shown next.
- **Scum save** = slot 0 (`scum.json`), a one-key quick save/load. Slots 1 to 5 are `slotN.json` with an overwrite note. Save is the world JSON; settings are `config.json`.
- Dead and Winner screens block input for 1 second, then `A`/`B` abandons the world and goes to the main menu. Dead plays DeathTheme, Winner VictoryTheme.
- Key repeat (Host): first press fires at once, repeat starts after **1.0 s** and then every **0.3 s**. Commands are polled each Update (about 60 per second).

### Screen list (what the Odin port must draw)

| State | Layout (frame is 384 by 216, hue-indexed) |
|---|---|
| Splash | Title "Bagel Quest" centred in the 8x8 UI font with a 1-pixel tan (13) outline and brown (6) fill; status bar "Space/(A) - Continue" |
| Picker menus (`BasePickerState`) | Black screen; a blue (1) highlight bar across the vertical centre; items centred, selected item black on blue, others blue on black; orange (11) header on black at the top; status bar black on light gray (7) at the bottom. Used by main/game/options/save/load menus, ActionMenu, Craft, Equipment, Equip, Ground, GroundDetail, InventoryDetail |
| Navigation | Fill dark gray (8); the map is drawn around the avatar from `LeftColumn..` with 16 by 16 cells; each cell is black + terrain glyph, item glyphs (one per distinct type), then the first character (mask glyph then glyph). Stats top-left, one per line with a 1-pixel black shadow: HP (12 pink), EN (1 blue), LV (5 purple), XP (3 cyan), `$ n` (10 light green). Status bar "Space/(A) - Actions \| Esc/(B) - Game Menu" |
| Message | Lines centred, in their own hues, one third down (or at half height with no choices); choices over a blue bar at two thirds height, selected black on blue |
| Combat | Avatar glyph at 1/3 width, enemies as a row at 2/3 width with a pointer glyph (0x1C, white) under the selected one; avatar name/HP/EN at left, enemy name and `HP n/m` at right; menu `Attack!` / `Run!` centred on a blue bar. No status bar. `B` does nothing |
| Inventory | Grid of item glyphs with `xN` under each, selected cell marked with glyph 255 (white); header is the item name |
| Forage | Grid of hidden cells (glyph 254, yellow 14) sized from the cell's remaining forage count, a cursor (glyph 255, green if revealable else red), loot tally on the left, header = energy |
| Statistics | Seven centred lines: HP, EN, ATK, DEF, XP, LV, AP, `$`; header "Statistics" |
| Dead / Winner | Black; "Yer dead!" (red 4) or "You Win!" (light green 10) centred; "Continue"/"Main Menu" appears after 1 s |

Map window: `CenterCell` pixel (184, 100); columns -12..+12 (25), rows -7..+7 (15) drawn, partly clipped at the edges. Cells outside the map are not drawn (background stays gray).

### Rendering model (what makes the port easy)

- Everything is drawn into a **hue-indexed pixel buffer**; `DisplayBuffer` maps hue to colour at commit (`hue.json`, 16 colours, index 0 to 15, all alpha 255). Out-of-range `SetPixel` is ignored.
- **Fonts are JSON bitmaps** (`Content/Fonts/*.json`): `{Height, Glyphs: {char: {Width, Lines: {row: [x,...]}}}}`. `CyFont8x8` is the UI font; `BagelQuest.json` is a 16 by 16 glyph set (indexed by character code) holding **all the game art**: terrain, items, characters, masks, cursors. (The about text credits Kenney's 1-bit pack; the sheet appears to be hand-converted to this format.)
- Text drawing = for each char, copy the glyph's set pixels in the hue (unset pixels transparent). Character drawing = mask glyph in `MaskHue` (0 = black, to cut out the cell) then the glyph.
- Named hues: Black 0, Blue 1, Green 2, Cyan 3, Red 4, Purple 5, Brown 6, LightGray 7, DarkGray 8, LightBlue 9, LightGreen 10, Orange 11, Pink 12, Tan 13, Yellow 14, White 15.

## 3. The world

### Maps (`MapTypes`, `Initializers/`)

| Map | Size | Floor | Notes |
|---|---|---|---|
| Wilderness | 75 x 75 (5 by 5 maze cells of 15 by 15) | Grass | flags `CampingAllowed`, `AllowFireBuilding` |
| Town | 17 x 17 | Empty | from an ASCII minimap; spawns Loxy |
| Healer, HealthTrainer, Potter, DruidHouse, EnergyTrainer | 7 x 7 | Empty | walled rooms |
| Inn | 7 x 7 | Empty | minimap with beds, doors, Gorachan, stairs down |
| Cellar | 7 x 7 | CellarFloor | stairs up at (1, 5) |

**Wilderness generation:** every cell gets `RNG.FromGenerator` over `Empty 100, Grass 50, Tree 150, Farm 10, ClayPit 1, RockQuarry 5` (total 316). Then a 5 by 5 maze (random Prim-style spanning tree) is drawn as rivers: for each maze cell the centre tile (offset 7,7) gets a river piece chosen by which of its four doors are open (`riverTable`: N=1, E=2, S=4, W=8 added), and straight river lines run from the centre to each open side. Every river tile gets one shared `BumpRiver` effect. Finally a random tenable tile becomes `Town` (retries until tenable). Cells then run their terrain's Initialize script (sets `ForageRemaining`).

**Town minimap** (`!` Fence, `.` Grass, `*` Gravel, `^` House, `+` Sign):

```
!!!!!!!!*!!!!!!!!
!.......*.......!
!.......*.......!
!.+^*********^+.!
!..**..+^...**..!
!..*.........*..!
!..*.+^***^+.*..!
!..*..**.**..*..!
*******...*******
!..*..**.**..*..!
!..*.+^***^+.*..!
!..*.........*..!
!..**...^+..**..!
!.+^*********^+.!
!.......*.......!
!.......*.......!
!!!!!!!!*!!!!!!!!
```

Signs (x, y -> message): (2,13) HealerSign, (2,3) InnSign, (14,3) TownSign2, (7,4) HealthTrainerSign, (5,6) EnergyTrainerSign, (11,6) TownSign5, (5,10) TownSign6, (11,10) DruidSign, (9,12) TownSign8, (14,13) PotterSign.

**Stitching** (`WorldInitializer`), all via `Teleport` effects (a cell effect that moves you to a destination cell; walking onto or bumping a non-tenable cell with an effect triggers it):

| From | To |
|---|---|
| Town (13,13) house door | Potter bottom row, one up from the door |
| Town (6,6) | EnergyTrainer top, one down |
| Town (3,3) | Inn bottom, one up |
| Town (10,10) | DruidHouse left, one in |
| Town (8,4) | HealthTrainer bottom, one up |
| Town (3,13) | Healer bottom, one up |
| each room's door | back to the cell just outside the town door ((13,12), (6,5), (3,2), (9,10), (8,3), (4,13)) |
| Inn stairs down (1,5) | Cellar stairs up; stairs up back (Inn effect is `EnterCellar`: only with the rat-quest flag) |
| Town edges N/S/E/W middle ((8,0), (8,16), (0,8), (16,8)) | the `Town` tile in the wilderness |
| Wilderness `Town` tile | Town (8, 15) (`Columns\2, Rows-2`) |

Building interiors (`*Initializer`): walls on the border; a Door; the NPC cell with its talk effect: Healer = `Basin` (effect `Heal`, `MaximumHealth` 5) + `OldMan` (`HealerTalk`); HealthTrainer = `StrongMan` (`HealthTrainerTalk`); Potter = `Potter` (`PotterTalk`); Druid = `Druid` (`DruidTalk`); EnergyTrainer = `EnergyTrainer` (`EnergyTrainerTalk`); Inn = Gorachan (`GorachanTalk`), four 2 Beds (they share one `SleepAtInn` effect), StairsDown; Cellar = ring of Wall, StairsUp.

Spawns: none on the maps except Loxy in the town (random tenable cell without characters? `spawnCharacters` picks `RNG.FromEnumerable(tenable cells with no characters)`). **Enemies are not placed; they appear from peril** (below).

### Cells, terrain, items, characters

A cell has a terrain type, a list of items, a list of characters, an optional effect (teleport/message/etc.), statistics (`ForageRemaining`). Terrain data is in `terrainTypes.json` (43 types): glyph, hue, flags (`Tenable` = can be walked on, `CanSleep`, `HasFire`, `IsFurnace`, `CanBuildFurnace`, `IsWaterSource`), `Peril` statistic, `CreatureTypeGenerator` (weighted enemy table), `Foragables` (weighted item table; empty string = nothing), terrain effects (verbs). Full tables are in section 6.

**Movement** (`CharacterExtensions.Move`): target cell must exist and be `Tenable`; otherwise the target cell's effect (if any) runs and you stay put. If you move, you enter the cell, its effect runs, then `EnterCell`:

- **Peril:** if the cell's terrain has `Peril > 0`, add it to your `Peril` statistic; if your Peril is then > 0, roll 1d20; on `roll <= Peril` an enemy from the terrain's generator appears in your cell (so combat starts) and your Peril drops by the enemy's own Peril. Peril never decays by itself. Sleeping outdoors adds `MaxEnergy \ 2` Peril.
- Terrain peril: Tree 1, DepletedTree 1, Farm 1, CellarFloor 1, ClayPit 2, RockQuarry 2. Grass, Empty, Gravel: 0.
- Enemy tables: Tree/DepletedTree = Olive Glop 75, Cherry Glop 25; ClayPit/RockQuarry = Cherry Glop; Farm = Scarecrow; CellarFloor = Rat.

### Statistics and rules (`CharacterExtensions`)

| Rule | Behaviour |
|---|---|
| Attack roll | `min(MaximumAttack, sum of AttackDice rolls of "1d6/6")`, so each die is 1 only on a 6 (1 in 6), else 0. Both dice count and max include equipped items |
| Defend roll | same with DefendDice / MaximumDefend |
| One attack | damage = max(0, attack - defend). Log lines: "X attacks Y", "X rolls an attack of n", "Y rolls a defend of n", then "misses" or "takes n damage" and HP. Weapons lose `attackRoll` durability points (spread randomly over durable equipped weapons; a broken item vanishes with a red message). Armour loses `max(defendRoll, damage)`. Kill: avatar gets the victim's Jools and XP; victim drops all items on the cell and is removed |
| Combat command `Attack!` | costs 1 energy (refused with a message if 0). Then **repeats rounds** (avatar attacks, then every other character in the cell counter-attacks) **until some damage is done**, clearing queued messages at the start of each round, so only the last round's messages show |
| `Run!` | every enemy gets an "Opportunity Attack"; if you survive, a random one of four directions is tried; if the move works you flee ("An ambush awaits!" if another enemy is there), else "cannot run away" |
| XP | `AddXP`: if XP >= XPGoal then +`AdvancementPointsPerLevel` AP, level +1, **XPGoal doubles**, XP reduced by the old goal (recursive). Loxy: goal 10, 10 AP per level |
| Energy | max 10; forage 1, fight 1, build fire/furnace/torch 1, make hatchet 1, mill wheat 1, make dough 2, cut tail 2. Sleeping outdoors (needs energy < max and a `CanSleep` terrain in a `CampingAllowed` map) restores `Max\2` and adds Peril; the inn restores to full |
| Healing | Eating: Cooked Rat +2 HP, Seasoned Rat +2 (and maybe spicy), Rat Corpse/Rat Body 50% +1 HP or 50% "tainted" -1 HP (can kill). Healer basin sets HP to `min(MaxHP, 5)` and takes **half your jools** |
| Spicy | after eating pepper/smoked pepper/seasoned rat: 50% "That was a spicy one!" and +1 XP |
| Energy herb | +10 energy (clamped) |
| Training | Energy trainer: needs 1 AP and `2 x MaxEnergy` jools; +1 max energy. Health trainer: costs `5 x MaxHealth` AP; +1 max HP and +1 HP |
| Skills | Druid teaches (1 AP each): Foraging, Twine Making (learn-by-doing: consumes 2 Plant Fiber, makes twine), Knapping (consumes 2 Rock), Fire Making, Torch Making, Hatchet Making (learn-by-doing). Flags: `KnowsForaging`, `KnowsTwineMaking`, `KnowsRockSharpening`, `KnowsFireMaking`, `KnowsTorchMaking`, `KnowsHatchetMaking` |
| Recipes | Check all inputs by item type; craft removes inputs, adds outputs (net of overlap, so items that are both input and output are untouched); takes items from the character's **unordered set** |

### Foraging (the grid mini-game)

Only with `KnowsForaging`, `ForageRemaining > 0` on the cell and energy > 0. The grid is `rows = floor(sqrt(Remaining))`, `columns = ceil(Remaining / rows)`; `Remaining` cells are hidden at random. `A` on a hidden cell reveals it: costs 1 energy, decrements the cell's remaining count, and gives a draw from the terrain's `Foragables` (nothing if the empty key is drawn). Exit with `B` shows a summary message. Starting counts: Grass 20, Farm/ClayPit 30, Tree/RockQuarry 50. **Cells never become "depleted terrain"** (see audit): they just stop being forageable.

Foragables: Grass {nothing 1, Plant Fiber 1}; Tree {nothing 75, Stick 100, Rock 25}; ClayPit {nothing 50, Clay 50}; RockQuarry {nothing 50, Rock 50}; Farm {nothing 1, Plant Fiber 1, Pepper 1, Wheat 1}.

### Crafting verbs

Shown in `Craft...` only when known/valid: Make Twine (knows twine), Build Fire (knows fire, terrain `Empty` in a map with `AllowFireBuilding`), Make Torch (knows torch, standing on a fire), Build Furnace (can afford recipe and terrain `CanBuildFurnace`, i.e. `Empty`, in any map), Cook Bagel (can afford and on a Furnace), Knap (knows), Make Hatchet (knows). `Put Out Fire` is its own action-menu entry when standing on a fire (energy 1, drops Charcoal, terrain back to Empty).

### Messages that need an effect (dialogue engine)

A message choice carries an effect type string; selecting it runs the **avatar's** effect script of that name (`CharacterType.EffectScripts`). Choices may carry stats/metadata (for example `Buy` reads `ItemType` and `Price` from its choice and re-opens the previous menu by effect name). This is the entire dialogue/quest system.

## 4. Quest and economy facts

- **Rat quest:** Gorachan offers "I need a job!" until you accept (flag `RatQuest`). With the flag, the Inn stairs lead to the Cellar (rats spawn from peril on CellarFloor). Cut tails from Rat Corpses (needs a Sharp Rock; costs 2 energy; gives a Rat Body and a Rat Tail and keeps the rock). Hand in tails for 1 jool each, any number of times.
- **Inn:** pay 1 jool once (flag `PaidInnkeeper`), then either of the 2 beds restores full energy and clears the flag.
- **Potter:** 20 jools plus an Unfired Pot recipe (5 Plant Fiber + 5 Clay) gives a Clay Pot. Note starting jools are exactly 20.
- **Rivers:** bumping a river tile with a Clay Pot offers "Fill Clay Pot with Water".
- **Win chain:** Wheat x5 + Rock x2 -> Flour (rocks kept, at the mill action from the item) ; Flour + Water Pot -> Dough (pot returns as Clay Pot); Charcoal (from a put-out fire) + Stick -> Torch (at a fire); Furnace: 10 Rock, 10 Clay, Torch (kept), 10 Stick on an Empty cell; Bagel: Dough, Torch (consumed), 5 Sticks, at the furnace. A Bagel in the inventory = Winner.
- Finite supply: every forage cell has a fixed count and never refills; the wilderness is random so scarcity varies by seed.

## 5. Save format (for the record; not imported)

`WorldData` serialized with System.Text.Json: maps (cells with terrain, item ids, character ids, statistics, effect), characters, items, messages, flags, statistics, ids. `Load` replaces the world. No version field. The port uses its own validated JSON (decision 6).

## 6. Content tables

### Characters

| Type | Glyph/Hue | Stats | Notes |
|---|---|---|---|
| Loxy (avatar) | 0x02 / 12, mask 0x01 | HP 2/2, Atk dice 2 max 1, Def dice 4 max 2, XP 0/10, level 1, AP 1 (10 per level), Energy 10/10, Jools 20 | starts with 1 Plant Fiber |
| Olive Glop | 0x1B / 10, mask 0x1A | HP 1, Atk 2/1, Def 1/1, Peril 5, XP 1 | jools = `3d6/6` |
| Cherry Glop | 0x1B / 4, mask 0x1A | HP 2, Atk 4/2, Def 1/1, Peril 10, XP 2 | jools = `6d6/6` |
| Rat | `*` / 6, mask `2` | HP 1, Atk 1/1, Def 1/1, Peril 3, XP 0 | carries a Rat Corpse |
| Scarecrow | `:` / 14, mask `<` | HP 3, Atk 4/2, Def 3/3, Peril 7, XP 3 | Straw Hat with probability 1/5 (`1d5/5 > 0`) |

`RollDice` grammar used in data: `NdS`, `+` sums, `/D` and `*M` integer scaling (`N d S / D` is integer-divided per set).

### Items (all stackable by display name; effects appear in the inventory detail menu)

| Item | Glyph | Hue | Slot / stats | Effects |
|---|---|---|---|---|
| Plant Fiber | `!` | 10 | | |
| Wheat | `B` | 14 | | Mill (1 energy) |
| Flour | `C` | 15 | | Make Dough (2 energy; needs a Water Pot) |
| Dough | `D` | 13 | | |
| Bagel | `D` | 6 | | |
| Paprika | `C` | 11 | | |
| Pepper | `9` | 4 | | Smoke (at a fire), Eat |
| Smoked Pepper | `9` | 11 | | Make Paprika (1 energy), Eat |
| Rat Corpse | `-` | 8 | | Cut Off Tail, Eat, Cook (at a fire) |
| Rat Body | `/` | 8 | | Eat, Cook |
| Rat Tail | `.` | 8 | | |
| Cooked Rat | `/` | 6 | | Eat, Season |
| Seasoned Rat | `/` | 11 | | Eat |
| Rock | `0` | 8 | | |
| Clay | `0` | 13 | | |
| Sharp Rock | `1` | 7 | cutting tool | |
| Twine | `!` | 13 | | |
| Torch | `4` | 4 | | |
| Charcoal | `6` | 8 | | |
| Straw Hat | `;` | 14 | Head: DefendDice 1, MaxDefend 0, durability 5/5, armour | full name shows durability |
| Unfired / Clay / Cracked / Water-Filled Pot | `?` `?` `@` `@` | 13/6/6/6 | | |
| Hatchet | `=` | 8 | Weapon: AttackDice 3, MaxAttack 1, durability 20/20 | full name shows durability |
| Stick | `%` | 6 | Weapon: AttackDice 2, durability 10/10 (no MaxAttack) | full name shows durability |
| Energy Herb | `(` | 3 | | Use (+10 energy) |

(Glyph is the character code in the `BagelQuest` font; the actual art lives there.)

### Recipes (inputs -> outputs)

Bagel: Dough 1, Torch 1, Stick 5 -> Bagel. Furnace: Rock 10, Clay 10, Torch 1, Stick 10 -> Torch (net: torch kept). SeasonedRat: CookedRat, Paprika -> SeasonedRat. Paprika: SmokedPepper 1, Rock 2 -> Paprika, Rock 2 (rocks kept). SmokedPepper: Pepper 1, Stick 2 -> SmokedPepper, Stick 1 (net -1 stick). Dough: Flour, WaterPot -> Dough, ClayPot. Flour: Wheat 5, Rock 2 -> Flour, Rock 2. WaterPot: ClayPot -> WaterPot. ClayPot: UnfiredPot -> ClayPot. CrackedPot: UnfiredPot -> CrackedPot (**no way to reach it**). UnfiredPot: PlantFiber 5, Clay 5. Hatchet: Twine, SharpRock, Stick. Twine: PlantFiber 2. RatTail: RatCorpse, SharpRock -> RatBody, RatTail, SharpRock. CookedRatBody / CookedRatCorpse: RatBody / RatCorpse + Stick 2 -> CookedRat + Stick 1. Torch: Charcoal, Stick. SharpRock: Rock 2 -> Rock 1, SharpRock. CookingFire: Rock 5, Stick 5 (no output; the effect changes terrain). Foraging: none.

### Terrain (flags: T tenable, S can sleep)

| Terrain | Glyph | Hue | Flags / notes |
|---|---|---|---|
| Grass | 4 | 2 | T S; forage 20 |
| ClayPit / RockQuarry | `>` | 13 / 8 | T S; peril 2; forage 30 / 50 |
| Tree | 10 | 2 | T S; peril 1; forage 50 |
| Farm | `8` | 6 | T S; peril 1; forage 30 |
| Depleted Grass / Tree | 4 / 10 | 6 | unreachable (see audit) |
| Empty | 0 | 0 | T S; can build furnace; BuildFire/BuildFurnace verbs |
| CellarFloor | 0 | 0 | T S; peril 1 |
| Wall, Fence, House, Sign | 3, 5, 7, 8 | 7, 6, 4, 6 | S only (solid) |
| Gravel | 6 | 8 | T S |
| Town | 7 | 4 | T (the wilderness tile) |
| Door | 29 | 11 | T S |
| Basin, OldMan, StrongMan, Potter, Druid, Bed, Gorachan, EnergyTrainer | 30, 31, 35, 65, 36, 38, 39, 41 | 1, 5, 6, 13, 10, 13, 4, 1 | S only (bump to talk/use) |
| StairsUp / StairsDown | 43 / 44 | 11 | S only |
| Rivers (16 pieces N E S W NE SE SW NW NES ESW SWN WNE NESW NS EW) | 11 to 25 | 1 | water source, not tenable; `BumpRiver` effect |
| CookingFire | `3` | 4 | T S; fire; PutOutFire (-> Empty, drops Charcoal), MakeTorch |
| Furnace | `E` | 7 | T S; furnace; CookBagel |

### Effect (verb/dialogue) catalogue: the 56 Loxy scripts

Grouped by what the Odin port needs to implement. All of them read the avatar `character` and an `effect` (item, terrain cell, map effect or message choice).

- **Plumbing:** `Teleport` (move to MapId/CellColumn/CellRow stored on the effect), `EnterCellar` (teleport if `RatQuest`, else message), `Message` (show the named canned message), `ExitDialog` (no-op).
- **Dialogue-only:** `GorachanTalk`, `PervertInnkeeper`, `DruidTalk`, `DruidAllergies`, `DruidPrices`, `HealerTalk`, `NihilistPrices`, `EnergyTrainerTalk`, `HealthTrainerTalk`, `PotterTalk`, `PotterFlavorText`, `StartRatQuest`.
- **Quest/economy:** `AcceptRatQuest`, `CompleteRatQuest`, `PayInnkeeper`, `SleepAtInn`, `Buy`, `Heal`, `TrainEnergy`, `TrainHealth`, `PotterMakePot`, `BumpRiver`, `FillClayPot`.
- **Skills:** `DruidTeachMenu` (builds the choices from unknown skills), `LearnForaging/Twine/Knapping/FireMaking/TorchMaking/HatchedMaking` (all call `DoLearnSkill`).
- **Crafting verbs:** `BuildFire`, `BuildFurnace`, `PutOutFire`, `MakeTorch`, `CookBagel`, `CookRatBody`, `CookRatCorpse`, `SmokePepper`, `SeasonRat`, `MakeHatchet`, `MakePaprika`, `MillWheat`, `MakeDough`, `CutOffTail`.
- **Consumables:** `EatCookedRat`, `EatSeasonedRat`, `EatRatCorpse`, `EatPepper`, `EatSmokedPepper`, `UseEnergyHerb`.
- **Dead:** `Forage` (the grid mini-game replaced it).

Character `Initialize` scripts: Loxy (give 1 Plant Fiber), Rat (give Rat Corpse), Scarecrow (20% Straw Hat), OliveGlop/CherryGlop (set Jools). Terrain Initialize scripts only set `ForageRemaining` (Grass 20, Tree 50, Farm 30, ClayPit 30, RockQuarry 50); the other 36 are empty. Item `FullName` scripts: Hatchet, Stick, Straw Hat show `Name(durability/max)`; Rat Corpse returns "test" (never displayed); the rest are empty (name only).

**Conclusion for the port (done in phase 4, `odin/game/effects.odin`):** none of the Lua needs an interpreter. Every effect is either a recipe call plus messages, a flag/stat check, or a teleport. All of it fits an enum-keyed `switch` (`Effect_Kind`), with choice stats/metadata carried in a small fixed struct.

### Messages (canned, `messages.json`)

Used: `NothingHappens` and the ten sign texts above. The `HealerTalk`, `HealthTrainerTalk`, `DruidTalk` entries are stale (the Lua versions replace them, with different text).

### Audio

Sfx ids to files: PlayerHit (id `SfxVolumeTest`), PlayerDeath, EnemyHit, EnemyDeath, EnemyMiss, PlayerMiss, Failure (Shucks.wav), Craft, Eat, Take, Tasty, WooHoo, Yoink. Only the five combat sounds are triggered by rules (`msg.SetSfx` in `Attack`, played when that message is shown), plus `SfxVolumeTest` (the volume menu) and `Failure` (a failed load). **Craft, Eat, Take, Tasty, WooHoo and Yoink have files and constants but nothing plays them** (grep of `SetSfx`/`PlaySfx`). Music: MainTheme (menus and navigation), CombatTheme (combat and the victory theme), DeathTheme = MainTheme. `MediaPlayer` repeats and only switches when the id changes.

## 7. What the oracle should record (decision 4)

Because rendering is hue-indexed and the UI never touches MonoGame, a VB harness can build `BagelQuestContext` + `GameController` with a stub `ISettings`, feed commands, and dump `OffscreenBuffer`-style hue arrays. Suggested screens: splash, main menu, about, options, game menu, the town at each building door, each interior, a wilderness view with river, action menu, inventory (several items, 1/2/3+ stacks), ground menus, equipment, statistics, forage grid (fresh and half-revealed), every NPC dialogue (first message of each talk), combat (1 and 2 enemies), a fight log, dead, winner. Dice cannot be set (decision 5: seed search). Runtime check: this machine has only the .NET 10 runtime (no 8.0), while the projects target net8.0; building the harness needs either `DOTNET_ROLL_FORWARD=Major` or retargeting the harness only.

## 8. Port implications (inputs for phases 1 to 5)

1. The rasterizer is a straight port: hue buffer, `Fill`, `Colorize` (glyph copy), plus the font JSON converted to Odin bit rows at generation time.
2. Flat world: 9 maps with fixed sizes (75*75 + 8*49 + 17*17 cells); cells hold up to a handful of items and characters, so use a pooled item array with `Cell.first_item` links rather than per-cell dynamic arrays.
3. The state stack is shallow (`SetStates` only pushes Options): model it as a small fixed stack in the core.
4. The message queue needs an upper bound (a fight round makes about 6 messages; clear on each round as the VB does).
5. Recipe consumption order is **undefined in VB** (unordered set). Choose a deterministic rule in Odin (consume the lowest-id first) and note any difference in the oracle step. Inventory display order has the same property (VB groups by first-seen order of a `HashSet`), so inventory screens with mixed stacks may need ordering normalised in the comparison.
6. Cap every retry loop (town placement, forage grid population, fight rounds).
