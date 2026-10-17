#+build !js
package tests

// Checks on the generated content tables against facts read from the original data and code (docs/game-reference.md).
// The generator itself is strict (unknown properties and dangling references fail the run); these tests pin the results.

import "core:testing"
import "bq:game"

ingredients :: proc(r: game.Range) -> []game.Ingredient { return game.RECIPE_INGREDIENTS[r.start:r.start + r.count] }

total_weight :: proc(weights: []$T) -> (sum: i32) { for w in weights { sum += w.weight }; return }

@(test)
content_has_the_original_counts :: proc(t: ^testing.T) {
	testing.expect_value(t, len(game.Terrain_Type), 43)
	testing.expect_value(t, len(game.Item_Type), 27 + 1) // plus None
	testing.expect_value(t, len(game.Character_Type), 5)
	testing.expect_value(t, len(game.Recipe_Type), 20)
	testing.expect_value(t, len(game.Effect_Kind), 55 + 1) // the avatar's scripts, plus None
	testing.expect_value(t, len(game.Message_Type), 11)     // the 15 minus 4 stale ones (see the generator's IGNORED)
	testing.expect(t, game.CONTENT_HASH != 0, "the content hash must be set")
}

@(test)
loxy_starts_as_in_the_original :: proc(t: ^testing.T) {
	s := game.CHARACTERS[.Loxy].stats
	testing.expect_value(t, s.health, 2)
	testing.expect_value(t, s.max_health, 2)
	testing.expect_value(t, s.attack_dice, 2)
	testing.expect_value(t, s.max_attack, 1)
	testing.expect_value(t, s.defend_dice, 4)
	testing.expect_value(t, s.max_defend, 2)
	testing.expect_value(t, s.xp_goal, 10)
	testing.expect_value(t, s.ap, 1)
	testing.expect_value(t, s.ap_per_level, 10)
	testing.expect_value(t, s.energy, 10)
	testing.expect_value(t, s.jools, 20)
	testing.expect_value(t, game.CHARACTERS[.Loxy].glyph, u8(2))
	testing.expect_value(t, game.CHARACTERS[.Loxy].hue, game.Hue.Pink)
	testing.expect_value(t, game.CHARACTERS[.Scarecrow].mask_glyph, u8('<'))
}

@(test)
items_match_the_original :: proc(t: ^testing.T) {
	h := game.ITEMS[.Hatchet]
	testing.expect_value(t, h.slot, game.Equip_Slot.Weapon)
	testing.expect_value(t, h.stats, game.Item_Stats{attack_dice = 3, max_attack = 1, durability = 20, max_durability = 20})
	testing.expect(t, .Is_Weapon in h.flags, "hatchet is a weapon")
	testing.expect_value(t, h.full_name, game.Full_Name.With_Durability)
	stick := game.ITEMS[.Stick]
	testing.expect_value(t, stick.stats.attack_dice, 2)
	testing.expect_value(t, stick.stats.max_attack, 0) // a Stick adds dice but no maximum
	hat := game.ITEMS[.Straw_Hat]
	testing.expect_value(t, hat.slot, game.Equip_Slot.Head)
	testing.expect(t, .Is_Armor in hat.flags, "straw hat is armour")
	testing.expect(t, .Is_Cutting_Tool in game.ITEMS[.Sharp_Rock].flags, "sharp rock cuts")
	// verbs in the original's order
	testing.expect_value(t, game.ITEMS[.Rat_Corpse].effects, [3]game.Effect_Kind{.Cut_Off_Tail, .Eat_Rat_Corpse, .Cook_Rat_Corpse})
	testing.expect_value(t, game.ITEMS[.Wheat].effects[0], game.Effect_Kind.Mill_Wheat)
	testing.expect_value(t, game.EFFECT_NAMES[.Eat_Rat_Corpse], "Eat")
	testing.expect_value(t, game.EFFECT_NAMES[.Mill_Wheat], "Mill")
	// the stale script text "test" is not carried over (defect D6)
	testing.expect_value(t, game.ITEMS[.Rat_Corpse].full_name, game.Full_Name.Plain)
}

@(test)
bagel_chain_matches_the_original :: proc(t: ^testing.T) {
	bagel := ingredients(game.RECIPES[.Bagel].inputs)
	testing.expect_value(t, len(bagel), 3)
	testing.expect_value(t, bagel[0], game.Ingredient{.Dough, 1})
	testing.expect_value(t, bagel[1], game.Ingredient{.Torch, 1})
	testing.expect_value(t, bagel[2], game.Ingredient{.Stick, 5})
	out := ingredients(game.RECIPES[.Bagel].outputs)
	testing.expect_value(t, len(out), 1)
	testing.expect_value(t, out[0], game.Ingredient{.Bagel, 1})
	testing.expect_value(t, len(ingredients(game.RECIPES[.Furnace].inputs)), 4)
	testing.expect_value(t, len(ingredients(game.RECIPES[.Foraging].inputs)), 0)
	testing.expect_value(t, game.RECIPES[.Water_Pot].name, "Water-Filled Pot")
}

@(test)
terrain_matches_the_original :: proc(t: ^testing.T) {
	tr := game.TERRAINS
	testing.expect(t, .Tenable in tr[.Grass].flags && .Can_Sleep in tr[.Grass].flags, "grass flags")
	testing.expect(t, .Tenable not_in tr[.Wall].flags, "walls are solid")
	testing.expect(t, .Is_Water_Source in tr[.River_NESW].flags, "rivers are water")
	testing.expect_value(t, tr[.Tree].peril, 1)
	testing.expect_value(t, tr[.Clay_Pit].peril, 2)
	testing.expect_value(t, tr[.Tree].forage_count, 50)
	testing.expect_value(t, tr[.Grass].forage_count, 20)
	testing.expect_value(t, tr[.Farm].forage_count, 30)
	testing.expect_value(t, tr[.Wall].forage_count, 0)
	testing.expect_value(t, tr[.Empty].verbs, bit_set[game.Terrain_Verb]{.Build_Fire, .Build_Furnace})
	testing.expect(t, .Can_Build_Furnace in tr[.Empty].flags, "furnaces go on empty ground")
	testing.expect_value(t, tr[.Cooking_Fire].put_out_into, game.Terrain_Type.Empty)
	testing.expect_value(t, tr[.Cooking_Fire].put_out_drops, game.Item_Type.Charcoal)
	testing.expect_value(t, tr[.Furnace].verbs, bit_set[game.Terrain_Verb]{.Cook_Bagel})
	// forage and creature tables: weights as in terrainTypes.json
	tree := tr[.Tree]
	testing.expect_value(t, total_weight(game.FORAGE_WEIGHTS[tree.forage.start:tree.forage.start + tree.forage.count]), 200)
	testing.expect_value(t, tr[.Farm].forage.count, 4)
	cs := game.CREATURE_WEIGHTS[tree.creatures.start:tree.creatures.start + tree.creatures.count]
	testing.expect_value(t, len(cs), 2)
	testing.expect_value(t, cs[0], game.Creature_Weight{.Olive_Glop, 75})
	testing.expect_value(t, cs[1], game.Creature_Weight{.Cherry_Glop, 25})
	testing.expect_value(t, tr[.Cellar_Floor].creatures.count, 1)
	testing.expect_value(t, tr[.Grass].creatures.count, 0)
}

@(test)
pools_are_in_bounds :: proc(t: ^testing.T) {
	for terrain in game.Terrain_Type {
		info := game.TERRAINS[terrain]
		testing.expectf(t, int(info.forage.start + info.forage.count) <= len(game.FORAGE_WEIGHTS), "%v forage range", terrain)
		testing.expectf(t, int(info.creatures.start + info.creatures.count) <= len(game.CREATURE_WEIGHTS), "%v creature range", terrain)
		testing.expectf(t, info.forage.count >= 1, "%v must have a forage table (the empty forage at least)", terrain)
		testing.expectf(t, len(info.name) > 0, "%v has a name", terrain)
	}
	for recipe in game.Recipe_Type {
		info := game.RECIPES[recipe]
		testing.expectf(t, int(info.inputs.start + info.inputs.count) <= len(game.RECIPE_INGREDIENTS), "%v inputs range", recipe)
		testing.expectf(t, int(info.outputs.start + info.outputs.count) <= len(game.RECIPE_INGREDIENTS), "%v outputs range", recipe)
	}
	for item in game.Item_Type {
		if item == .None { continue }
		testing.expectf(t, len(game.ITEMS[item].name) > 0, "%v has a name", item)
	}
}

@(test)
messages_and_sounds :: proc(t: ^testing.T) {
	m := game.MESSAGES[.Nothing_Happens]
	testing.expect_value(t, m.lines.count, 1)
	testing.expect_value(t, game.MESSAGE_LINES[m.lines.start].text, "Nothing happens.")
	testing.expect_value(t, game.MESSAGE_LINES[m.lines.start].hue, game.Hue.Light_Gray)
	pot := game.MESSAGES[.Potter_Sign]
	testing.expect_value(t, pot.lines.count, 2)
	testing.expect_value(t, game.MESSAGE_LINES[pot.lines.start + 1].text, "(no relation)")
	// the sound ids keep the original's keys; PlayerHit is stored under "SfxVolumeTest" in sfx.json
	testing.expect_value(t, game.SFX_FILES[.Sfx_Volume_Test], "PlayerHit.wav")
	testing.expect_value(t, game.SFX_FILES[.Failure], "Shucks.wav")
	testing.expect_value(t, game.MUX_FILES[.Victory_Theme], "CombatTheme.ogg")
	testing.expect_value(t, game.MUX_FILES[.Death_Theme], "MainTheme.ogg")
}
