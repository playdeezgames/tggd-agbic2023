#+build !js
package tests

// Soak: random play at the model level (the calls the screens will make), checking world_validate as it goes.

import "core:testing"
import "bq:game"

SOAK_STEPS :: 30000

Soak_Stats :: struct { moves, fights, runs, forages, crafts, takes, drops, equips, sleeps, messages, deaths, levels, item_effects, verbs: int }

soak :: proc(t: ^testing.T, seed: u64, steps: int) -> Soak_Stats {
	w := new_world(seed)
	defer free(w)
	r: game.Rng
	game.rng_seed(&r, seed * 7919 + 13)
	s: Soak_Stats
	// let the avatar try everything: skills known, a start in the open
	for f in game.Char_Flag { if f != .Rat_Quest && f != .Paid_Innkeeper { game.set_flag(w, w.avatar, f, true) } }
	for step in 0 ..< steps {
		a := w.avatar
		// housekeeping a player's luck would provide: come back from the dead, keep some energy
		if game.is_dead(w, a) { s.deaths += 1; game.msg_clear(w); game.set_health(w, a, w.chars[a].stats.max_health) }
		if w.chars[a].stats.energy < 2 { game.add_energy(w, a, 10) }
		if w.chars[a].stats.max_health < 20 && game.rng_range(&r, 0, 50) == 0 { game.set_max_health(w, a, 20); game.set_health(w, a, 20) }
		if game.msg_has(w) {
			s.messages += 1
			m := game.msg_current(w)
			if m.choice_count > 0 { game.message_choose(w, game.rng_range(&r, 0, int(m.choice_count) - 1)) } else { game.msg_dismiss(w) }
		} else if game.combat_exists(w) {
			if game.rng_range(&r, 0, 3) == 0 { game.combat_run(w); s.runs += 1 } else { game.combat_attack(w, game.rng_range(&r, 0, game.combat_count(w) - 1)); s.fights += 1 }
		} else {
			switch game.rng_range(&r, 0, 13) {
			case 0 ..= 5:
				d := game.RUN_DELTAS[game.rng_range(&r, 0, 3)]
				game.move(w, a, d.x, d.y)
				s.moves += 1
			case 6:
				if game.forage_can(w) { game.forage_one(w); s.forages += 1 }
			case 7:
				stacks: [32]game.Stack
				n := game.ground_stacks(w, stacks[:])
				if n > 0 { game.take_items(w, stacks[game.rng_range(&r, 0, n - 1)].type, game.rng_range(&r, 1, 5)); s.takes += 1 }
			case 8:
				stacks: [32]game.Stack
				n := game.inventory_stacks(w, stacks[:])
				if n > 0 { game.drop_items(w, stacks[game.rng_range(&r, 0, n - 1)].type, game.rng_range(&r, 1, 5)); s.drops += 1 }
			case 9:
				// equip something equippable, or take it off
				stacks: [32]game.Stack
				n := game.inventory_stacks(w, stacks[:])
				equipped := false
				for i in 0 ..< n {
					if game.ITEMS[stacks[i].type].slot != .None {
						for id := w.chars[a].first_item; id != 0; id = w.items[id].next { if w.items[id].type == stacks[i].type { game.equip(w, a, id); equipped = true; break } }
						break
					}
				}
				if !equipped { game.unequip(w, a, .Weapon); game.unequip(w, a, .Head) }
				s.equips += 1
			case 10:
				game.sleep(w, a)
				s.sleeps += 1
			case 11:
				// craft a random recipe if the avatar has the means, after stocking up now and then
				if game.rng_range(&r, 0, 8) == 0 {
					for _ in 0 ..< 12 {
						type := game.Item_Type(game.rng_range(&r, 1, len(game.Item_Type) - 1))
						it := game.item_create(w, type)
						if it != 0 { game.give_item(w, a, it) }
					}
				}
				rec := game.Recipe_Type(game.rng_range(&r, 0, len(game.Recipe_Type) - 1))
				if game.craft(w, a, rec) { s.crafts += 1 }
			case 12:
				// use a random verb of a random carried item (eat, cook, mill, ...)
				stacks: [32]game.Stack
				n := game.inventory_stacks(w, stacks[:])
				if n > 0 {
					type := stacks[game.rng_range(&r, 0, n - 1)].type
					kind := game.ITEMS[type].effects[game.rng_range(&r, 0, game.MAX_ITEM_EFFECTS - 1)]
					if kind != .None { game.do_item_effect(w, type, kind); s.item_effects += 1 }
				}
			case 13:
				// stand on ground that offers a crafting verb (the wilderness only: rooms keep their floors), then try one
				if game.cell_pos(w.chars[a].cell).map_id == .Wilderness && game.terrain_flag(w, w.chars[a].cell, .Tenable) {
					ground := [?]game.Terrain_Type{.Empty, .Cooking_Fire, .Furnace, .Grass}
					w.cells[w.chars[a].cell].terrain = ground[game.rng_range(&r, 0, len(ground) - 1)]
					game.do_terrain_verb(w, game.Terrain_Verb(game.rng_range(&r, 0, len(game.Terrain_Verb) - 1)))
					s.verbs += 1
				}
			}
		}
		if step % 50 == 0 {
			ok, why := game.world_validate(w)
			if !ok { testing.expectf(t, false, "seed %d step %d: %s", seed, step, why); return s }
		}
	}
	ok, why := game.world_validate(w)
	testing.expectf(t, ok, "seed %d at the end: %s", seed, why)
	s.levels = int(w.chars[w.avatar].stats.xp_level)
	return s
}

@(test)
soak_random_play_keeps_the_world_valid :: proc(t: ^testing.T) {
	total: Soak_Stats
	for seed in u64(1) ..= 6 {
		s := soak(t, seed, SOAK_STEPS)
		total.moves += s.moves; total.fights += s.fights; total.runs += s.runs; total.forages += s.forages; total.crafts += s.crafts
		total.takes += s.takes; total.drops += s.drops; total.equips += s.equips; total.sleeps += s.sleeps
		total.messages += s.messages; total.deaths += s.deaths; total.item_effects += s.item_effects; total.verbs += s.verbs
	}
	// the run must actually have exercised the rules, or a pass means nothing
	testing.expectf(t, total.moves > 20000, "moves %d", total.moves)
	testing.expectf(t, total.fights > 100, "fights %d", total.fights)
	testing.expectf(t, total.runs > 10, "runs %d", total.runs)
	testing.expectf(t, total.forages > 100, "forages %d", total.forages)
	testing.expectf(t, total.crafts > 20, "crafts %d", total.crafts)
	testing.expectf(t, total.takes > 50 && total.drops > 50, "takes %d drops %d", total.takes, total.drops)
	testing.expectf(t, total.equips > 100, "equips %d", total.equips)
	testing.expectf(t, total.sleeps > 100, "sleeps %d", total.sleeps)
	testing.expectf(t, total.messages > 100, "messages %d", total.messages)
	testing.expectf(t, total.item_effects > 200, "item effects %d", total.item_effects)
	testing.expectf(t, total.verbs > 100, "terrain verbs %d", total.verbs)
}

@(test)
validate_catches_corruption :: proc(t: ^testing.T) {
	w := new_world(3)
	defer free(w)
	validated(t, w)
	ok: bool
	// an item that claims a cell it is not listed in
	it := give(w, .Rock)
	w.items[it].place = .Ground
	ok, _ = game.world_validate(w)
	testing.expect(t, !ok, "a misplaced item must fail")
	w.items[it].place = .Carried
	validated(t, w)
	// a wrong item count
	w.item_count += 1
	ok, _ = game.world_validate(w)
	testing.expect(t, !ok, "a wrong item count must fail")
	w.item_count -= 1
	// health above the maximum
	w.chars[w.avatar].stats.health = 99
	ok, _ = game.world_validate(w)
	testing.expect(t, !ok, "health above max must fail")
	w.chars[w.avatar].stats.health = 2
	// a character in the wrong cell
	w.chars[w.avatar].cell = 0
	ok, _ = game.world_validate(w)
	testing.expect(t, !ok, "a character in the wrong cell must fail")
}
