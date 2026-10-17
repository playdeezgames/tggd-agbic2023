#+build !js
package tests

// The rules: stats, experience, combat, peril, recipes, foraging, inventory, equipment, messages.

import "core:testing"
import "bq:game"

give :: proc(w: ^game.World, type: game.Item_Type, n := 1) -> game.Item_Id {
	last: game.Item_Id
	for _ in 0 ..< n { last = game.item_create(w, type); game.give_item(w, w.avatar, last) }
	return last
}

// The avatar in the wilderness on a given terrain, with a clear message queue.
stand_on :: proc(w: ^game.World, t: game.Terrain_Type) {
	place(w, .Wilderness, 30, 30)
	w.cells[w.chars[w.avatar].cell].terrain = t
	w.cells[w.chars[w.avatar].cell].forage_remaining = i16(game.TERRAINS[t].forage_count)
	game.msg_clear(w)
}

ground_count :: proc(w: ^game.World, cell: game.Cell_Id, type: game.Item_Type) -> int {
	n := 0
	for id := w.cells[cell].first_item; id != 0; id = w.items[id].next { if w.items[id].type == type { n += 1 } }
	return n
}

@(test)
dice_follow_the_data_grammar :: proc(t: ^testing.T) {
	r: game.Rng
	game.rng_seed(&r, 42)
	ones, total := 0, 6000
	for _ in 0 ..< total { if game.roll_dice(&r, 1, 6, 6) == 1 { ones += 1 } } // "1d6/6" is 1 only on a 6
	testing.expectf(t, ones > 800 && ones < 1200, "1d6/6 gave %d ones in %d", ones, total)
	for _ in 0 ..< 200 { v := game.roll_dice(&r, 6, 6, 6); testing.expect(t, v >= 1 && v <= 6, "6d6/6 is 1 to 6") }
	for _ in 0 ..< 200 { v := game.rng_range(&r, -3, 3); testing.expect(t, v >= -3 && v <= 3, "range") }
}

@(test)
statistics_clamp :: proc(t: ^testing.T) {
	w := new_world(1)
	defer free(w)
	a := w.avatar
	game.set_health(w, a, 99)
	testing.expect_value(t, w.chars[a].stats.health, 2)
	game.set_health(w, a, -5)
	testing.expect_value(t, w.chars[a].stats.health, 0)
	testing.expect(t, game.is_dead(w, a), "zero health is dead")
	game.set_max_health(w, a, 0)
	testing.expect_value(t, w.chars[a].stats.max_health, 1)
	game.add_energy(w, a, 100)
	testing.expect_value(t, w.chars[a].stats.energy, 10)
	game.add_energy(w, a, -100)
	testing.expect_value(t, w.chars[a].stats.energy, 0)
	game.add_jools(w, a, -100)
	testing.expect_value(t, w.chars[a].stats.jools, 0)
	testing.expect_value(t, game.health_display(w, a), "HP 0/1")
	testing.expect_value(t, game.energy_display(w, a), "EN 0/10")
	testing.expect_value(t, game.xp_display(w, a), "XP 0/10")
	testing.expect_value(t, game.level_display(w, a), "LV 1")
	testing.expect_value(t, game.jools_display(w, a), "$ 0")
	testing.expect_value(t, game.ap_display(w, a), "AP 1")
}

@(test)
displays_of_attack_and_defence :: proc(t: ^testing.T) {
	w := new_world(1)
	defer free(w)
	testing.expect_value(t, game.attack_display(w, w.avatar), "ATK: Max 1 avg 0.33")
	testing.expect_value(t, game.defend_display(w, w.avatar), "DEF: Max 2 avg 0.67")
	game.equip(w, w.avatar, give(w, .Hatchet))
	testing.expect_value(t, game.attack_display(w, w.avatar), "ATK: Max 2 avg 0.83") // 2 + 3 dice, max 1 + 1
	game.equip(w, w.avatar, give(w, .Straw_Hat))
	testing.expect_value(t, game.defend_display(w, w.avatar), "DEF: Max 2 avg 0.83") // 4 + 1 dice, max 2 + 0
}

@(test)
experience_doubles_the_goal :: proc(t: ^testing.T) {
	w := new_world(1)
	defer free(w)
	a := w.avatar
	testing.expect(t, !game.add_xp(w, a, 9), "9 xp is not a level")
	testing.expect(t, game.add_xp(w, a, 1), "10 xp is a level")
	s := w.chars[a].stats
	testing.expect_value(t, s.xp_level, 2)
	testing.expect_value(t, s.xp_goal, 20)
	testing.expect_value(t, s.xp, 0)
	testing.expect_value(t, s.ap, 11) // started with 1, +10 per level
	// a big award can level twice: 30 xp at goal 20: first 30 >= 20 -> level 3, goal 40, xp 10
	game.add_xp(w, a, 30)
	s = w.chars[a].stats
	testing.expect_value(t, s.xp_level, 3)
	testing.expect_value(t, s.xp_goal, 40)
	testing.expect_value(t, s.xp, 10)
}

// A creature with exact dice: attack/defend are fixed by making the dice many and the maxima small.
strong_avatar :: proc(w: ^game.World) {
	s := &w.chars[w.avatar].stats
	s.attack_dice, s.max_attack = 200, 5 // ~33 sixes: always capped at 5
	s.max_health, s.health = 50, 50
	s.energy, s.max_energy = 10, 10
}

@(test)
killing_a_creature_rewards_and_drops :: proc(t: ^testing.T) {
	w := new_world(2)
	defer free(w)
	stand_on(w, .Grass)
	strong_avatar(w)
	cell := w.chars[w.avatar].cell
	rat := game.spawn_character(w, .Rat, cell)
	testing.expect_value(t, game.item_count_of(w, rat, .Rat_Corpse), 1)
	w.chars[rat].stats.xp = 3
	w.chars[rat].stats.jools = 4
	w.chars[rat].stats.defend_dice = 0
	jools := w.chars[w.avatar].stats.jools
	game.combat_attack(w, 0)
	testing.expect(t, !w.chars[rat].in_use, "the rat is gone")
	testing.expect_value(t, ground_count(w, cell, .Rat_Corpse), 1)
	testing.expect_value(t, w.chars[w.avatar].stats.jools, jools + 4)
	testing.expect_value(t, w.chars[w.avatar].stats.xp, 3)
	testing.expect_value(t, w.chars[w.avatar].stats.energy, 9) // the attack costs 1 energy
	testing.expect(t, !game.combat_exists(w), "no one left to fight")
	validated(t, w)
}

@(test)
no_energy_no_fight :: proc(t: ^testing.T) {
	w := new_world(2)
	defer free(w)
	stand_on(w, .Grass)
	game.spawn_character(w, .Rat, w.chars[w.avatar].cell)
	w.chars[w.avatar].stats.energy = 0
	game.combat_attack(w, 0)
	cur := game.msg_current(w)
	testing.expect(t, cur != nil, "a message explains")
	testing.expect_value(t, game.line_text(&cur.lines[0]), "Loxy doesn't have the energy to fight!")
	testing.expect(t, game.combat_exists(w), "the rat is still there")
}

@(test)
a_fight_always_ends :: proc(t: ^testing.T) {
	for seed in u64(1) ..= 30 {
		w := new_world(seed)
		defer free(w)
		stand_on(w, .Grass)
		game.spawn_character(w, .Scarecrow, w.chars[w.avatar].cell)
		game.spawn_character(w, .Cherry_Glop, w.chars[w.avatar].cell)
		w.chars[w.avatar].stats.energy = 10
		for _ in 0 ..< 20 {
			if !game.combat_exists(w) || game.is_dead(w, w.avatar) { break }
			game.combat_attack(w, 0)
			game.msg_clear(w)
			w.chars[w.avatar].stats.energy = 10
		}
		validated(t, w)
	}
}

@(test)
weapons_wear_out_and_break :: proc(t: ^testing.T) {
	w := new_world(2)
	defer free(w)
	stand_on(w, .Grass)
	stick := give(w, .Stick)
	game.equip(w, w.avatar, stick)
	w.items[stick].durability = 1
	m := game.msg_begin(w)
	broke := game.scuff(w, w.avatar, 5, .Is_Weapon, m)
	testing.expect(t, broke, "the stick broke")
	testing.expect(t, w.items[stick].place == .Free, "a broken item is recycled")
	testing.expect_value(t, w.chars[w.avatar].equipped[.Weapon], game.Item_Id(0))
	testing.expect_value(t, game.line_text(&m.lines[0]), "Loxy's Stick breaks!")
	validated(t, w)
}

@(test)
dangerous_terrain_brings_creatures :: proc(t: ^testing.T) {
	w := new_world(4)
	defer free(w)
	stand_on(w, .Tree)
	for i in 0 ..< 100 {
		if game.combat_exists(w) { break }
		game.enter_cell(w, w.avatar)
		testing.expect(t, i < 99, "a creature should appear within 100 steps on a tree")
	}
	testing.expect(t, game.combat_exists(w), "creature appeared")
	enemy := game.combat_enemy(w, 0)
	testing.expect(t, w.chars[enemy].type == .Olive_Glop || w.chars[enemy].type == .Cherry_Glop, "trees breed glops")
	// the creature used up peril: the avatar's is less than the glop's cost would leave
	validated(t, w)
	// safe terrain never does
	stand_on(w, .Grass)
	for _ in 0 ..< 200 { game.enter_cell(w, w.avatar) }
	testing.expect_value(t, w.chars[w.avatar].stats.peril, i32(0))
}

@(test)
running_away :: proc(t: ^testing.T) {
	w := new_world(4)
	defer free(w)
	stand_on(w, .Grass)
	strong_avatar(w)
	w.chars[w.avatar].stats.defend_dice = 500 // no damage gets through
	w.chars[w.avatar].stats.max_defend = 99
	game.spawn_character(w, .Rat, w.chars[w.avatar].cell)
	moved := false
	for _ in 0 ..< 20 {
		game.msg_clear(w)
		start := w.chars[w.avatar].cell
		game.combat_run(w)
		if w.chars[w.avatar].cell != start { moved = true; break }
	}
	testing.expect(t, moved, "running should move the avatar within 20 tries (open grass all around)")
	testing.expect(t, !game.combat_exists(w), "the rat stays behind")
	validated(t, w)
}

@(test)
sleeping_is_only_for_the_wilderness :: proc(t: ^testing.T) {
	w := new_world(4)
	defer free(w)
	// in town
	w.chars[w.avatar].stats.energy = 2
	game.sleep(w, w.avatar)
	testing.expect_value(t, w.chars[w.avatar].stats.energy, 2)
	testing.expect_value(t, game.line_text(&game.msg_current(w).lines[0]), "Loxy cannot sleep here!")
	// outdoors on grass
	stand_on(w, .Grass)
	game.sleep(w, w.avatar)
	testing.expect_value(t, w.chars[w.avatar].stats.energy, 7) // + max/2
	testing.expect_value(t, game.line_text(&game.msg_current(w).lines[0]), "Loxy sleeps.")
	testing.expect(t, game.can_sleep_now(w), "still not full")
}

@(test)
recipes_keep_what_they_also_make :: proc(t: ^testing.T) {
	w := new_world(6)
	defer free(w)
	a := w.avatar
	// the furnace needs a torch but gives one back: the torch stays
	testing.expect(t, !game.craft(w, a, .Furnace), "cannot yet")
	give(w, .Rock, 10); give(w, .Clay, 10); give(w, .Torch); give(w, .Stick, 10)
	testing.expect(t, game.can_craft(w, a, .Furnace), "can now")
	testing.expect(t, game.craft(w, a, .Furnace), "crafted")
	testing.expect_value(t, game.item_count_of(w, a, .Rock), 0)
	testing.expect_value(t, game.item_count_of(w, a, .Clay), 0)
	testing.expect_value(t, game.item_count_of(w, a, .Stick), 0)
	testing.expect_value(t, game.item_count_of(w, a, .Torch), 1)
	// the bagel uses the torch up
	give(w, .Dough); give(w, .Stick, 5)
	testing.expect(t, game.craft(w, a, .Bagel), "bagel")
	testing.expect_value(t, game.item_count_of(w, a, .Torch), 0)
	testing.expect_value(t, game.item_count_of(w, a, .Bagel), 1)
	testing.expect(t, game.has_won(w), "a bagel wins")
	validated(t, w)
}

@(test)
recipes_take_the_oldest_items_first :: proc(t: ^testing.T) {
	w := new_world(6)
	defer free(w)
	a := w.avatar
	give(w, .Pepper)
	s1, s2, s3 := give(w, .Stick), give(w, .Stick), give(w, .Stick)
	testing.expect(t, game.craft(w, a, .Smoked_Pepper), "smoke a pepper") // pepper 1 + stick 2 -> smoked pepper + stick 1: net -1 stick
	testing.expect(t, w.items[s1].place == .Free, "the oldest stick was used")
	testing.expect(t, w.items[s2].place == .Carried && w.items[s3].place == .Carried, "the others remain")
	testing.expect_value(t, game.item_count_of(w, a, .Smoked_Pepper), 1)
	testing.expect_value(t, game.item_count_of(w, a, .Stick), 2)
}

@(test)
a_recipe_that_cannot_be_made_changes_nothing :: proc(t: ^testing.T) {
	w := new_world(6)
	defer free(w)
	give(w, .Rock, 3)
	before := w.item_count
	testing.expect(t, !game.craft(w, w.avatar, .Cooking_Fire), "needs 5 rock and 5 stick")
	testing.expect_value(t, w.item_count, before)
}

@(test)
foraging_uses_energy_and_the_cells_supply :: proc(t: ^testing.T) {
	w := new_world(8)
	defer free(w)
	stand_on(w, .Grass)
	testing.expect(t, !game.forage_can(w), "needs the skill first")
	game.set_flag(w, w.avatar, .Knows_Foraging, true)
	testing.expect(t, game.forage_can(w), "now it can")
	testing.expect_value(t, game.forage_remaining(w), 20)
	found := 0
	for i in 0 ..< 10 {
		if game.forage_one(w) != 0 { found += 1 }
		testing.expect_value(t, game.forage_remaining(w), 19 - i)
	}
	testing.expect_value(t, w.chars[w.avatar].stats.energy, 0)
	testing.expect(t, !game.forage_can(w), "no energy left")
	testing.expect(t, found > 0 && found < 10, "grass gives fibre about half the time")
	testing.expect_value(t, game.item_count_of(w, w.avatar, .Plant_Fiber), found + 1) // plus the one Loxy started with
	validated(t, w)
}

@(test)
grid_sizes_are_roughly_square :: proc(t: ^testing.T) {
	check :: proc(t: ^testing.T, n, cols, rows: int, loc := #caller_location) {
		c, r := game.grid_size(n)
		testing.expect_value(t, c, cols, loc = loc)
		testing.expect_value(t, r, rows, loc = loc)
	}
	check(t, 0, 0, 0)
	check(t, 1, 1, 1)
	check(t, 2, 2, 1)
	check(t, 3, 2, 2)
	check(t, 20, 5, 4)
	check(t, 30, 6, 5)
	check(t, 50, 8, 7)
}

@(test)
inventory_groups_and_moves_items :: proc(t: ^testing.T) {
	w := new_world(9)
	defer free(w)
	stand_on(w, .Grass)
	give(w, .Rock, 3); give(w, .Stick, 2)
	stacks: [32]game.Stack
	n := game.inventory_stacks(w, stacks[:])
	testing.expect_value(t, n, 3) // fibre (from the start), rock, stick: in arrival order
	testing.expect_value(t, stacks[0], game.Stack{.Plant_Fiber, 1})
	testing.expect_value(t, stacks[1], game.Stack{.Rock, 3})
	testing.expect_value(t, stacks[2], game.Stack{.Stick, 2})
	testing.expect_value(t, game.drop_items(w, .Rock, 2), 2)
	cell := w.chars[w.avatar].cell
	testing.expect_value(t, ground_count(w, cell, .Rock), 2)
	testing.expect_value(t, game.item_count_of(w, w.avatar, .Rock), 1)
	testing.expect_value(t, game.take_items(w, .Rock, 5), 2) // only two there
	testing.expect_value(t, game.item_count_of(w, w.avatar, .Rock), 3)
	validated(t, w)
}

@(test)
equipping_swaps_and_unequipping_returns :: proc(t: ^testing.T) {
	w := new_world(9)
	defer free(w)
	stick, hatchet := give(w, .Stick), give(w, .Hatchet)
	game.equip(w, w.avatar, stick)
	testing.expect_value(t, w.chars[w.avatar].equipped[.Weapon], stick)
	testing.expect(t, w.items[stick].place == .Equipped, "equipped")
	game.equip(w, w.avatar, hatchet)
	testing.expect_value(t, w.chars[w.avatar].equipped[.Weapon], hatchet)
	testing.expect(t, w.items[stick].place == .Carried, "the stick went back to the inventory")
	testing.expect(t, game.has_equipment(w, w.avatar), "has equipment")
	game.unequip(w, w.avatar, .Weapon)
	testing.expect(t, !game.has_equipment(w, w.avatar), "nothing equipped")
	testing.expect_value(t, game.item_full_name(w, hatchet), "Hatchet(20/20)")
	game.equip(w, w.avatar, give(w, .Rock)) // a rock has no slot: nothing happens
	testing.expect(t, !game.has_equipment(w, w.avatar), "rocks cannot be equipped")
	validated(t, w)
}

@(test)
crafting_menu_conditions :: proc(t: ^testing.T) {
	w := new_world(9)
	defer free(w)
	a := w.avatar
	stand_on(w, .Empty)
	testing.expect(t, !game.can_craft_anything(w, a), "knows nothing")
	game.set_flag(w, a, .Knows_Fire_Making, true)
	testing.expect(t, game.can_build_fire(w, a), "fire on clear ground outdoors")
	place(w, .Town, 1, 1) // grass in town: no fire building there
	testing.expect(t, !game.can_build_fire(w, a), "not in town")
	stand_on(w, .Cooking_Fire)
	testing.expect(t, game.can_put_out_fire(w, a), "on a fire")
	game.set_flag(w, a, .Knows_Torch_Making, true)
	testing.expect(t, game.can_make_torch(w, a), "torches at a fire")
	stand_on(w, .Empty)
	testing.expect(t, !game.can_make_torch(w, a), "but not on bare ground")
	testing.expect(t, !game.can_build_furnace(w, a), "no materials")
	give(w, .Rock, 10); give(w, .Clay, 10); give(w, .Torch); give(w, .Stick, 10)
	testing.expect(t, game.can_build_furnace(w, a), "materials on empty ground")
	stand_on(w, .Grass)
	testing.expect(t, !game.can_build_furnace(w, a), "not on grass")
}

@(test)
messages_queue_in_order_and_drop_when_full :: proc(t: ^testing.T) {
	w := new_world(9)
	defer free(w)
	game.msg_clear(w)
	for i in 0 ..< game.MAX_MESSAGES + 5 { game.msg_line(game.msg_begin(w), .White, "message %d", i) }
	testing.expect_value(t, w.msg_count, game.MAX_MESSAGES)
	testing.expect_value(t, game.line_text(&game.msg_current(w).lines[0]), "message 0")
	game.msg_dismiss(w)
	testing.expect_value(t, game.line_text(&game.msg_current(w).lines[0]), "message 1")
	game.msg_clear(w)
	testing.expect(t, !game.msg_has(w), "empty")
	m := game.msg_begin(w)
	game.msg_line(m, .White, "%s", "a line that is much longer than the screen is wide, so it must be cut off somewhere here")
	testing.expect_value(t, len(game.line_text(&m.lines[0])), game.LINE_CAP)
	for _ in 0 ..< 20 { game.msg_choice(m, .Exit_Dialog, "choice") }
	testing.expect_value(t, int(m.choice_count), game.MAX_CHOICES)
	validated(t, w)
}

@(test)
choosing_runs_the_effect_then_dismisses :: proc(t: ^testing.T) {
	w := new_world(9)
	defer free(w)
	game.msg_clear(w)
	m := game.msg_begin(w)
	game.msg_line(m, .White, "go")
	game.msg_choice(m, .Exit_Dialog, "Cool story, bro!")
	game.msg_canned(w, .Nothing_Happens)
	game.message_choose(w, 0)
	testing.expect_value(t, w.msg_count, 1) // the first is gone, the canned one is next
	testing.expect_value(t, game.line_text(&game.msg_current(w).lines[0]), "Nothing happens.")
	game.message_choose(w, 5) // no such choice: nothing
	testing.expect_value(t, w.msg_count, 1)
}
