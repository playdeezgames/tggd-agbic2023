# Defect and content audit

Found by reading the VB and Lua source in phase 0 (nothing was run). Per decision 1, **clear bugs are fixed in the port** and logged here; **content, balance and text questions are listed and left as in the original until the user decides**. Update the Status column as the port proceeds.

## Clear defects (fix in the port)

| # | Where | What | Fix | Status |
|---|---|---|---|---|
| D1 | `Scripts/Characters/Loxy/Heal.lua` | Message says `loses {jools} jools!` literally (Lua has no interpolation) | print the number | open |
| D2 | `PotterMakePot.lua` | Not-enough-jools message ends with a stray `)`: `has 20 jools)` | remove it | open |
| D3 | `CharacterExtensions.ScuffWeapons/ScuffArmors` | "Loxy' Stick breaks!" (stray apostrophe) | "Loxy's Stick breaks!" | open |
| D4 | `CharacterExtensions.AlreadyKnows` | "Loxy already know how to ..." | "already knows how" | open |
| D5 | `messages.json` / `HealerTalk.lua` | "If you go to the basin And wash," (capital A) | "and" | open |
| D6 | `Items/RatCorpse/FullName.lua` | returns `"test"` | delete the script (name only) | open (never displayed today) |
| D7 | `CombatModel.Attack` | `Do ... Loop Until damageDone` has no bound; it terminates only because every pairing in the data can eventually deal damage | cap the number of rounds in Odin (for example 1000) and end the fight with a message | open |
| D8 | `WildernessInitializer.PlaceTown`, `ForageState.PopulateGrid` | unbounded retry loops (fine in practice) | cap every retry loop (vault Gotchas) | open |
| D9 | `Sfx`: Craft, Eat, Take, Tasty, WooHoo, Yoink, (Shucks as `Failure` only on load failure) | files and constants exist but nothing plays them, while the vault's other games use them for feedback | **content question**, see C6 | see C6 |
| D10 | `ForagingModel.GridSize` | `Remaining = 0` gives `rows = 0` and a division by zero | unreachable today (the menu hides Forage) but guard it | open |
| D11 | `GameState.GroundDetail = "GroudDetail"` | typo in an internal id | harmless; not carried over | n/a |
| D12 | `TerrainTypes` `DepletedTerrainType` metadata, `DepletedGrass`, `DepletedTree` | never read, so cells never change when used up; terrain type stays, only `ForageRemaining` hits 0 | **behaviour question**, see C1 | see C1 |

## Dead code (not ported)

`SPLORR.Game` maze generator is used, but `Scripts/maze.lua` (a full Lua copy that prints to the console) is not; `CommonTerrainVerbs.DoForage`, `Scripts/.../Forage.lua` and the `Forage` terrain effect (superseded by the forage grid; `CellExtensions.CanForage`); `CharacterExtensions.HasCuttingTool`, `Weapons`, `Armors`; `RNG.ValidateDice`, `MaximumRoll`, `CombineGenerator`, `FromRange`-style helpers not used by the game; stale `messages.json` entries `HealerTalk`, `HealthTrainerTalk`, `DruidTalk` (different text from the Lua), `TownSign4`; `CrackedPot` recipe and item (no way to make one); effect types listed in `effectTypes.json` but unused; `BQ.Business.Tests` is named in `InternalsVisibleTo` but does not exist; `DruidAllergies` is flavour only.

## Content and balance questions (leave as is until the user decides)

| # | Question |
|---|---|
| C1 | **DECIDED: keep as is** (no depletion; worked-out tiles look unchanged). **Depleted terrain:** was it meant to switch a used-up Tree/Grass/ClayPit/RockQuarry/Farm to its depleted type (`DepletedGrass`/`DepletedTree`/`Empty`)? Today a worked-out tile looks unchanged. |
| C2 | **DECIDED: real text, done** (generator `MESSAGE_TEXT_FIXES`; edit there). **Placeholder signs:** four town signs read "This is sign #2 / #5 / #6 / #8". Real text? |
| C3 | **Opening economy:** Loxy starts with 20 jools and 1 AP. The pot costs exactly 20, the inn 1, so buying the pot leaves nothing for a bed; jools otherwise come from rat tails (1 each), Olive Glops (about 0.5 on average) and Cherry Glops (about 1). Is that intended? |
| C4 | **Very low combat stats:** Loxy has 2 HP, and one attack die is a hit only on a 6. A Stick adds 2 dice but no maximum, Hatchet 3 dice and +1 maximum. Balance is untested here. |
| C5 | **Key repeat:** a held key waits 1.0 s before repeating, then repeats every 0.3 s (Host). Keep for fidelity, or make movement snappier? |
| C6 | **DECIDED: yes, done in phase 4** (mapping below). **Silent sounds:** use Craft, Eat, Take, Tasty, WooHoo and Yoink where they obviously fit (crafting, eating, taking items, level-up, win)? |
| C7 | **Repeatable quest income:** rat tails can be handed in any number of times for 1 jool each, forever. |
| C8 | **Inn:** you pay once for "a bed" (either of two) and the flag stays until you sleep. Paying twice is blocked ("already paid"). |
| C9 | **Furnace placement:** `CanBuildFurnace` checks only the terrain (`Empty`), not the map, so a furnace can be built inside the inn or a shop, replacing its floor. Fire is correctly limited to the wilderness. |
| C10 | **Scum save** quick-saves to slot 0 and "Scum Load" restores it, a deliberate save-scumming feature; the web port can keep it as one extra slot. |
| C11 | **Unspecified win path:** the About text, README ideas (tea, paprika, molasses, yeast, salt, Thindal the cook) are design notes; the shipped game has none of them. |
| C12 | **Window size options** (multipliers 3, 4, 5, 9, 10, 14, 15, 19, 20 of 384 by 216) have no meaning on the web; the web build replaces them with the page's own scaling, and Quit goes to a "closed" state. |

## Phase 5 findings (the screens, compared pixel for pixel with the original)

| # | Where | What | Status |
|---|---|---|---|
| D15 | `InventoryState` | dropping your **last stack** returns to an empty Inventory screen, which then throws `ArgumentOutOfRangeException` drawing its header: **the original crashes** | fixed: the Actions menu opens instead |
| D16 | `EquipState` | with two or more equippable stacks the header reads `<placeholder>` | fixed: the item's name |
| D17 | `MakeHatchet.lua` | the 1-energy "make a hatchet" effect is never reached; the Craft menu's Make Hatchet is `DoMakeItem` (no energy). Same for Knap and Make Twine | kept: the Craft menu's version is the one ported (`do_make_item`); the effect stays as written |
| D18 | `ItemTypeDescriptor.FullName` | the Equipment list and the Equip list show **`System.Object[]`** for every item with a name script (Hatchet, Stick, Straw Hat): `DoString` returns an array and the code calls `ToString` on it | fixed: `Hatchet(20/20)` |
| D19 | main menu `About` | the About text ends "See 'aboot.txt'", a file the web build does not have | **removed** (user decision, phase 8); the `about` frame is a known difference of 242 pixels |

## Phase 4 decisions and findings

**C6 sound map** (each plays when the message that carries it is shown, or at once for the two with no message): Craft = every crafting verb, Eat = eating (corpse, body, pepper, smoked pepper, energy herb), Tasty = eating a cooked or seasoned rat, Take = picking up from the ground and a forage find (no message), Yoink = a purchase and a pot from the potter, WooHoo = level up (no message), a skill learned, training, a healing at the basin, the rat reward, Failure (the original "Shucks") = a refusal (not enough jools, energy, materials or AP).

| # | Where | What | Status |
|---|---|---|---|
| D13 | `EatSeasonedRat.lua` | "That was a spicy one!" is added to a new message **every time**, and `DetermineSpiciness` then adds it again (with the 1 XP) half the time, so it can show twice | fixed: it shows only when spicy, once |
| D14 | `LearnByDoing`, `PotterMakePot.lua`, `FillClayPot.lua` | `FillClayPot` says "fills a Clay Pot" even if the pot is gone | fixed: only when it worked |
| C13 | Druid menu labels | "Fire Making(-1AP, -5 Rock, -5 Sticks)" and "Torch Making(...)" look like they consume materials; they only require them to be held (the others do consume) | kept as is |
| C14 | `BuildFire`, `BuildFurnace`, `MakeTorch` | the energy is spent **before** checking for materials, so a failed attempt still costs 1 energy (the other recipes check first) | kept as is |
| C15 | `Heal.lua` | the basin heals only up to **5** health (`MaximumHealth` 5 on the cell), whatever your maximum is, and always costs half your jools | kept as is |
