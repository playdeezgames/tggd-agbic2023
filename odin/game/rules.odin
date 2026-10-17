package game

// The rules: a port of CharacterExtensions, CellExtensions, CombatModel, ForagingModel, the inventory/ground/equipment models
// and RecipeDescriptor. Everything takes the world and a character id; nothing here knows about screens.
// The effect scripts (talk, shops, skills, crafting verbs) are phase 4 and live in effects.odin.

import "core:fmt"
import "core:math"

// ---- statistics ---------------------------------------------------------------------------------------------------
char_name :: proc(w: ^World, c: Char_Id) -> string { return CHARACTERS[w.chars[c].type].name }
is_avatar :: proc(w: ^World, c: Char_Id) -> bool { return c == w.avatar }
has_flag :: proc(w: ^World, c: Char_Id, f: Char_Flag) -> bool { return f in w.chars[c].flags }
set_flag :: proc(w: ^World, c: Char_Id, f: Char_Flag, on: bool) {
	if on { w.chars[c].flags += {f} } else { w.chars[c].flags -= {f} }
}

set_health :: proc(w: ^World, c: Char_Id, health: i32) {
	s := &w.chars[c].stats
	s.health = clamp(health, 0, s.max_health)
}
set_max_health :: proc(w: ^World, c: Char_Id, max_health: i32) {
	w.chars[c].stats.max_health = max(1, max_health)
	set_health(w, c, w.chars[c].stats.health)
}
add_energy :: proc(w: ^World, c: Char_Id, delta: i32) {
	s := &w.chars[c].stats
	s.energy = clamp(s.energy + delta, 0, s.max_energy)
}
add_peril :: proc(w: ^World, c: Char_Id, delta: i32) { w.chars[c].stats.peril = max(0, w.chars[c].stats.peril + delta) }
add_jools :: proc(w: ^World, c: Char_Id, delta: i32) { w.chars[c].stats.jools = max(0, w.chars[c].stats.jools + delta) }
add_ap :: proc(w: ^World, c: Char_Id, delta: i32) { w.chars[c].stats.ap = max(0, w.chars[c].stats.ap + delta) }
is_dead :: proc(w: ^World, c: Char_Id) -> bool { return w.chars[c].stats.health <= 0 }

// Dice and maxima include everything equipped (the original: the character's value plus the worn items').
equipped_sum :: proc(w: ^World, c: Char_Id, field: enum { Attack_Dice, Max_Attack, Defend_Dice, Max_Defend }) -> i32 {
	total: i32
	for id in w.chars[c].equipped {
		if id == 0 { continue }
		s := ITEMS[w.items[id].type].stats
		switch field {
		case .Attack_Dice: total += s.attack_dice
		case .Max_Attack: total += s.max_attack
		case .Defend_Dice: total += s.defend_dice
		case .Max_Defend: total += s.max_defend
		}
	}
	return total
}
attack_dice :: proc(w: ^World, c: Char_Id) -> i32 { return w.chars[c].stats.attack_dice + equipped_sum(w, c, .Attack_Dice) }
max_attack :: proc(w: ^World, c: Char_Id) -> i32 { return w.chars[c].stats.max_attack + equipped_sum(w, c, .Max_Attack) }
defend_dice :: proc(w: ^World, c: Char_Id) -> i32 { return w.chars[c].stats.defend_dice + equipped_sum(w, c, .Defend_Dice) }
max_defend :: proc(w: ^World, c: Char_Id) -> i32 { return w.chars[c].stats.max_defend + equipped_sum(w, c, .Max_Defend) }

// Each die is "1d6/6": a 1 only on a 6. The total is capped by the maximum.
roll_attack :: proc(w: ^World, c: Char_Id) -> i32 {
	total: i32
	for _ in 0 ..< attack_dice(w, c) { total += i32(roll_dice(&w.rng, 1, 6, 6)) }
	return min(max_attack(w, c), total)
}
roll_defend :: proc(w: ^World, c: Char_Id) -> i32 {
	total: i32
	for _ in 0 ..< defend_dice(w, c) { total += i32(roll_dice(&w.rng, 1, 6, 6)) }
	return min(max_defend(w, c), total)
}

// Statistics as the screens show them (strings are in the temp allocator).
health_display :: proc(w: ^World, c: Char_Id) -> string { return fmt.tprintf("HP %d/%d", w.chars[c].stats.health, w.chars[c].stats.max_health) }
energy_display :: proc(w: ^World, c: Char_Id) -> string { return fmt.tprintf("EN %d/%d", w.chars[c].stats.energy, w.chars[c].stats.max_energy) }
level_display :: proc(w: ^World, c: Char_Id) -> string { return fmt.tprintf("LV %d", w.chars[c].stats.xp_level) }
xp_display :: proc(w: ^World, c: Char_Id) -> string { return fmt.tprintf("XP %d/%d", w.chars[c].stats.xp, w.chars[c].stats.xp_goal) }
jools_display :: proc(w: ^World, c: Char_Id) -> string { return fmt.tprintf("$ %d", w.chars[c].stats.jools) }
ap_display :: proc(w: ^World, c: Char_Id) -> string { return fmt.tprintf("AP %d", w.chars[c].stats.ap) }
attack_display :: proc(w: ^World, c: Char_Id) -> string { return fmt.tprintf("ATK: Max %d avg %.2f", max_attack(w, c), f64(attack_dice(w, c)) / 6) }
defend_display :: proc(w: ^World, c: Char_Id) -> string { return fmt.tprintf("DEF: Max %d avg %.2f", max_defend(w, c), f64(defend_dice(w, c)) / 6) }

// ---- experience and rewards ---------------------------------------------------------------------------------------------
// Returns true when the character gained a level. The goal doubles each level; leftover XP carries over (and may level again).
add_xp :: proc(w: ^World, c: Char_Id, xp: i32) -> bool {
	s := &w.chars[c].stats
	s.xp += xp
	if s.xp_goal > 0 && s.xp >= s.xp_goal {
		add_ap(w, c, s.ap_per_level)
		s.xp_level += 1
		current_goal := s.xp_goal
		s.xp_goal += s.xp_goal
		add_xp(w, c, -current_goal)
		return true
	}
	return false
}

award_xp :: proc(w: ^World, c: Char_Id, m: ^Message, xp: i32) {
	if !is_avatar(w, c) || xp == 0 { return }
	msg_line(m, .Light_Gray, "%s gains %d XP!", char_name(w, c), xp)
	if add_xp(w, c, xp) {
		msg_line(m, .Light_Green, "%s is now level %d!", char_name(w, c), w.chars[c].stats.xp_level)
		msg_line(m, .Light_Gray, "%s now has %d AP!", char_name(w, c), w.chars[c].stats.ap)
		sfx_play(w, .Woo_Hoo)
	} else {
		s := w.chars[c].stats
		msg_line(m, .Light_Gray, "%s needs %d for the next level.", char_name(w, c), s.xp_goal - s.xp)
	}
}

award_jools :: proc(w: ^World, c: Char_Id, m: ^Message, jools: i32) {
	if !is_avatar(w, c) || jools <= 0 { return }
	msg_line(m, .Light_Gray, "%s gets %d jools!", char_name(w, c), jools)
	add_jools(w, c, jools)
}

// ---- items on a character ----------------------------------------------------------------------------------------------------
item_name :: proc(w: ^World, id: Item_Id) -> string { return ITEMS[w.items[id].type].name }

// The name in lists (FullName.lua): Stick(10/10) for items that wear out.
item_full_name :: proc(w: ^World, id: Item_Id) -> string {
	it := w.items[id]
	info := ITEMS[it.type]
	if info.full_name == .With_Durability { return fmt.tprintf("%s(%d/%d)", info.name, it.durability, info.stats.max_durability) }
	return info.name
}

item_count_of :: proc(w: ^World, c: Char_Id, type: Item_Type) -> int {
	n := 0
	for id := w.chars[c].first_item; id != 0; id = w.items[id].next { if w.items[id].type == type { n += 1 } }
	return n
}

// ---- breaking, dying, fighting ------------------------------------------------------------------------------------------------
// Wear out equipped items that have `flag` and durability left: one point at a time on a random one. A broken item vanishes.
scuff :: proc(w: ^World, c: Char_Id, amount: i32, flag: Item_Flag, m: ^Message) -> bool {
	result := false
	amount := amount
	for amount > 0 {
		candidates: [len(Equip_Slot)]Item_Id
		n := 0
		for id in w.chars[c].equipped {
			if id != 0 && flag in ITEMS[w.items[id].type].flags && w.items[id].durability > 0 { candidates[n] = id; n += 1 }
		}
		if n == 0 { break }
		id := candidates[rng_range(&w.rng, 0, n - 1)]
		w.items[id].durability = max(0, w.items[id].durability - 1)
		if w.items[id].durability <= 0 {
			msg_line(m, .Red, "%s's %s breaks!", char_name(w, c), item_name(w, id))
			item_recycle(w, id)
			result = true
		}
		amount -= 1
	}
	return result
}

// A dead creature drops everything where it died and is removed; the avatar stays (the screen shows its death).
die :: proc(w: ^World, c: Char_Id) {
	if is_avatar(w, c) { return }
	cell := w.chars[c].cell
	for slot in Equip_Slot { if id := w.chars[c].equipped[slot]; id != 0 { put_item_on_ground(w, cell, id) } }
	for w.chars[c].first_item != 0 { put_item_on_ground(w, cell, w.chars[c].first_item) }
	char_leave_cell_list(w, c)
	w.chars[c] = {}
}

// One attack. Returns true if anything was damaged or broke (the original's "something happened" flag).
attack :: proc(w: ^World, attacker, defender: Char_Id, prefix := "") -> bool {
	if is_dead(w, defender) { return false }
	result := false
	m := msg_begin(w)
	if len(prefix) > 0 { msg_line(m, .Light_Gray, "%s", prefix) }
	msg_line(m, .Light_Gray, "%s attacks %s", char_name(w, attacker), char_name(w, defender))
	attack_roll := roll_attack(w, attacker)
	msg_line(m, .Light_Gray, "%s rolls an attack of %d", char_name(w, attacker), attack_roll)
	if scuff(w, attacker, attack_roll, .Is_Weapon, m) { result = true }
	defend_roll := roll_defend(w, defender)
	msg_line(m, .Light_Gray, "%s rolls a defend of %d", char_name(w, defender), defend_roll)
	damage := max(i32(0), attack_roll - defend_roll)
	if scuff(w, defender, max(defend_roll, damage), .Is_Armor, m) { result = true }
	if damage <= 0 {
		msg_line(m, .Light_Gray, "%s misses.", char_name(w, attacker))
		m.sfx = Sfx_Id.Player_Miss if is_avatar(w, attacker) else Sfx_Id.Enemy_Miss
		return result
	}
	result = true
	msg_line(m, .Light_Gray, "%s takes %d damage", char_name(w, defender), damage)
	set_health(w, defender, w.chars[defender].stats.health - damage)
	if is_dead(w, defender) {
		m.sfx = Sfx_Id.Player_Death if is_avatar(w, defender) else Sfx_Id.Enemy_Death
		msg_line(m, .Light_Gray, "%s kills %s", char_name(w, attacker), char_name(w, defender))
		award_jools(w, attacker, m, w.chars[defender].stats.jools)
		award_xp(w, attacker, m, w.chars[defender].stats.xp)
		die(w, defender)
		return result
	}
	m.sfx = SFX_PLAYER_HIT if is_avatar(w, defender) else Sfx_Id.Enemy_Hit
	msg_line(m, .Light_Gray, "%s has %d/%d health.", char_name(w, defender), w.chars[defender].stats.health, w.chars[defender].stats.max_health)
	return result
}

// ---- movement ------------------------------------------------------------------------------------------------------------------------
MAX_COMBATANTS :: 32

can_enter :: proc(w: ^World, cell: Cell_Id) -> bool { return cell != NO_CELL && terrain_flag(w, cell, .Tenable) }

do_map_effect :: proc(w: ^World, c: Char_Id, cell: Cell_Id) {
	if cell == NO_CELL || w.cells[cell].effect == 0 { return }
	e := w.effects[w.cells[cell].effect]
	run_effect(w, c, {kind = e.kind, dest = e.dest, message = e.message, number = e.number, cell = cell})
}

// Move by (dx, dy) within the map. A cell that cannot be entered runs its effect instead (signs, doors, people).
// Returns whether the character moved.
move :: proc(w: ^World, c: Char_Id, dx, dy: int) -> bool {
	pos := cell_pos(w.chars[c].cell)
	next := cell_at(pos.map_id, pos.col + dx, pos.row + dy)
	if !can_enter(w, next) {
		do_map_effect(w, c, next)
		return false
	}
	if dx != 0 || dy != 0 {
		char_leave_cell_list(w, c)
		char_enter_cell_list(w, c, next)
	}
	do_map_effect(w, c, next)
	enter_cell(w, c)
	return true
}

pick_creature :: proc(w: ^World, terrain: Terrain_Type) -> Character_Type {
	r := TERRAINS[terrain].creatures
	pool := CREATURE_WEIGHTS[r.start:r.start + r.count]
	total: i32
	for e in pool { total += e.weight }
	roll := i32(rng_range(&w.rng, 0, int(total) - 1))
	for e in pool {
		roll -= e.weight
		if roll < 0 { return e.type }
	}
	return pool[0].type
}

// Walking on dangerous terrain builds the character's peril; a roll of 1d20 at or under it brings a creature, which uses
// up some of that peril.
enter_cell :: proc(w: ^World, c: Char_Id) {
	cell := w.chars[c].cell
	cell_peril := TERRAINS[w.cells[cell].terrain].peril
	if cell_peril <= 0 { return }
	add_peril(w, c, cell_peril)
	peril := w.chars[c].stats.peril
	if peril <= 0 { return }
	if i32(roll_dice(&w.rng, 1, 20)) <= peril {
		if TERRAINS[w.cells[cell].terrain].creatures.count == 0 { return }
		enemy := spawn_character(w, pick_creature(w, w.cells[cell].terrain), cell)
		if enemy != 0 { add_peril(w, c, -w.chars[enemy].stats.peril) }
	}
}

// ---- sleeping -------------------------------------------------------------------------------------------------------------------------
can_sleep_now :: proc(w: ^World) -> bool { s := w.chars[w.avatar].stats; return s.energy < s.max_energy }

sleep :: proc(w: ^World, c: Char_Id) {
	if !is_avatar(w, c) { return }
	cell := w.chars[c].cell
	if !map_allows_camping(cell_pos(cell).map_id) || !terrain_flag(w, cell, .Can_Sleep) {
		msg_line(msg_begin(w), .Light_Gray, "%s cannot sleep here!", char_name(w, c))
		return
	}
	add_energy(w, c, w.chars[c].stats.max_energy / 2)
	m := msg_begin(w)
	msg_line(m, .Light_Gray, "%s sleeps.", char_name(w, c))
	msg_line(m, .Light_Gray, "%s now has %d/%d energy.", char_name(w, c), w.chars[c].stats.energy, w.chars[c].stats.max_energy)
	add_peril(w, c, w.chars[c].stats.max_energy / 2)
	move(w, c, 0, 0)
	others: [MAX_COMBATANTS]Char_Id
	if others_in_cell(w, c, others[:]) > 0 { msg_line(m, .Red, "%s awakens to a surprise attack!", char_name(w, c)) }
}

// ---- combat (the avatar's side) -----------------------------------------------------------------------------------------------------------
combat_exists :: proc(w: ^World) -> bool {
	others: [1]Char_Id
	return others_in_cell(w, w.avatar, others[:]) > 0
}

combat_count :: proc(w: ^World) -> int {
	others: [MAX_COMBATANTS]Char_Id
	return others_in_cell(w, w.avatar, others[:])
}

combat_enemy :: proc(w: ^World, index: int) -> Char_Id {
	others: [MAX_COMBATANTS]Char_Id
	n := others_in_cell(w, w.avatar, others[:])
	if index < 0 || index >= n { return 0 }
	return others[index]
}

// Every creature in the cell attacks the avatar once. `damage_done` is the running flag.
retaliation :: proc(w: ^World, text: string, damage_done: bool) -> bool {
	damage_done := damage_done
	others: [MAX_COMBATANTS]Char_Id
	n := others_in_cell(w, w.avatar, others[:])
	for i in 0 ..< n {
		label := fmt.tprintf("%s %d/%d", text, i + 1, n)
		hit := attack(w, others[i], w.avatar, label) // always evaluated: the attack happens even if damage was already done
		damage_done = damage_done || hit
	}
	return damage_done
}

MAX_FIGHT_ROUNDS :: 1000 // the original loops until someone is hurt, with no limit

// "Attack!": costs 1 energy, then rounds repeat (the avatar strikes, everyone strikes back) until something is damaged.
// Messages from earlier rounds are dropped so only the last round is shown.
combat_attack :: proc(w: ^World, enemy_index: int) {
	a := w.avatar
	if w.chars[a].stats.energy < 1 {
		msg_line(msg_begin(w), .Red, "%s doesn't have the energy to fight!", char_name(w, a))
		return
	}
	add_energy(w, a, -1)
	target := combat_enemy(w, enemy_index)
	if target == 0 { return }
	for _ in 0 ..< MAX_FIGHT_ROUNDS {
		msg_clear(w)
		damage_done := attack(w, a, target)
		damage_done = retaliation(w, "Counter Attack", damage_done) || damage_done
		if damage_done { return }
	}
	msg_line(msg_begin(w), .Light_Gray, "The fight drags on; everyone stops for breath.")
}

RUN_DELTAS := [4][2]int{{0, -1}, {0, 1}, {-1, 0}, {1, 0}}

// "Run!": everyone gets an opportunity attack, then a random direction is tried.
combat_run :: proc(w: ^World) {
	retaliation(w, "Opportunity Attack", false)
	a := w.avatar
	if is_dead(w, a) { return }
	d := RUN_DELTAS[rng_range(&w.rng, 0, 3)]
	if move(w, a, d.x, d.y) {
		m := msg_begin(w)
		msg_line(m, .Light_Gray, "%s runs away!", char_name(w, a))
		if combat_exists(w) { msg_line(m, .Red, "An ambush awaits!") }
		return
	}
	msg_line(msg_begin(w), .Light_Gray, "%s cannot run away!", char_name(w, a))
}

// ---- recipes ---------------------------------------------------------------------------------------------------------------------------------
recipe_inputs :: proc(r: Recipe_Type) -> []Ingredient { i := RECIPES[r].inputs; return RECIPE_INGREDIENTS[i.start:i.start + i.count] }
recipe_outputs :: proc(r: Recipe_Type) -> []Ingredient { i := RECIPES[r].outputs; return RECIPE_INGREDIENTS[i.start:i.start + i.count] }

can_craft :: proc(w: ^World, c: Char_Id, r: Recipe_Type) -> bool {
	for in_ in recipe_inputs(r) { if item_count_of(w, c, in_.item) < int(in_.count) { return false } }
	return true
}

// Craft: remove the inputs, add the outputs, but only the net change (an item that is both input and output is untouched, so
// the Furnace keeps its Torch). Inputs are taken oldest first. Returns false when the character cannot craft it.
craft :: proc(w: ^World, c: Char_Id, r: Recipe_Type) -> bool {
	if !can_craft(w, c, r) { return false }
	Delta :: struct { type: Item_Type, n: int }
	deltas: [16]Delta
	nd := 0
	bump :: proc(deltas: ^[16]Delta, nd: ^int, type: Item_Type, by: int) {
		for i in 0 ..< nd^ { if deltas[i].type == type { deltas[i].n += by; return } }
		deltas[nd^] = {type, by}
		nd^ += 1
	}
	for o in recipe_outputs(r) { bump(&deltas, &nd, o.item, int(o.count)) }
	for in_ in recipe_inputs(r) { bump(&deltas, &nd, in_.item, -int(in_.count)) }
	created := 0
	for i in 0 ..< nd { if deltas[i].n > 0 { created += deltas[i].n } }
	// the pool must have room for what is made (items consumed free their slots first, so count them)
	freed := 0
	for i in 0 ..< nd { if deltas[i].n < 0 { freed += -deltas[i].n } }
	if created > freed + (MAX_ITEMS - 1 - w.item_count) { return false }
	for i in 0 ..< nd {
		d := deltas[i]
		if d.n < 0 {
			for _ in 0 ..< -d.n {
				for id := w.chars[c].first_item; id != 0; id = w.items[id].next {
					if w.items[id].type == d.type { item_recycle(w, id); break }
				}
			}
		}
	}
	for i in 0 ..< nd {
		d := deltas[i]
		for _ in 0 ..< max(d.n, 0) { give_item(w, c, item_create(w, d.type)) }
	}
	return true
}

// What the Craft menu offers (AvatarCraftingModel).
can_make_twine :: proc(w: ^World, c: Char_Id) -> bool { return has_flag(w, c, .Knows_Twine_Making) }
can_make_hatchet :: proc(w: ^World, c: Char_Id) -> bool { return has_flag(w, c, .Knows_Hatchet_Making) }
can_knap :: proc(w: ^World, c: Char_Id) -> bool { return has_flag(w, c, .Knows_Rock_Sharpening) }
cell_has_verb :: proc(w: ^World, cell: Cell_Id, v: Terrain_Verb) -> bool { return v in TERRAINS[w.cells[cell].terrain].verbs }
can_build_fire :: proc(w: ^World, c: Char_Id) -> bool {
	cell := w.chars[c].cell
	return has_flag(w, c, .Knows_Fire_Making) && map_allows_fire(cell_pos(cell).map_id) && cell_has_verb(w, cell, .Build_Fire)
}
can_put_out_fire :: proc(w: ^World, c: Char_Id) -> bool { return terrain_flag(w, w.chars[c].cell, .Has_Fire) }
can_make_torch :: proc(w: ^World, c: Char_Id) -> bool { return has_flag(w, c, .Knows_Torch_Making) && cell_has_verb(w, w.chars[c].cell, .Make_Torch) }
can_build_furnace :: proc(w: ^World, c: Char_Id) -> bool { return can_craft(w, c, .Furnace) && terrain_flag(w, w.chars[c].cell, .Can_Build_Furnace) }
can_cook_bagel :: proc(w: ^World, c: Char_Id) -> bool { return can_craft(w, c, .Bagel) && cell_has_verb(w, w.chars[c].cell, .Cook_Bagel) }
can_craft_anything :: proc(w: ^World, c: Char_Id) -> bool {
	return can_build_fire(w, c) || can_build_furnace(w, c) || can_cook_bagel(w, c) || can_knap(w, c) || can_make_hatchet(w, c) ||
		can_make_torch(w, c) || can_make_twine(w, c)
}

// The game is won by holding a bagel.
has_won :: proc(w: ^World) -> bool { return item_count_of(w, w.avatar, .Bagel) > 0 }

// ---- foraging ---------------------------------------------------------------------------------------------------------------------------------
forage_remaining :: proc(w: ^World) -> int { return int(w.cells[w.chars[w.avatar].cell].forage_remaining) }

forage_can :: proc(w: ^World) -> bool {
	return has_flag(w, w.avatar, .Knows_Foraging) && forage_remaining(w) > 0 && w.chars[w.avatar].stats.energy > 0
}

// Roughly square: rows = round(sqrt(n)), enough columns to hold n. (0, 0) when n is 0 (the original divided by zero there).
grid_size :: proc(n: int) -> (cols, rows: int) {
	if n <= 0 { return 0, 0 }
	rows = int(math.round(math.sqrt(f64(n))))
	return (n + rows - 1) / rows, rows
}

pick_forage :: proc(w: ^World, terrain: Terrain_Type) -> Item_Type {
	r := TERRAINS[terrain].forage
	pool := FORAGE_WEIGHTS[r.start:r.start + r.count]
	total: i32
	for e in pool { total += e.weight }
	roll := i32(rng_range(&w.rng, 0, int(total) - 1))
	for e in pool {
		roll -= e.weight
		if roll < 0 { return e.item }
	}
	return .None
}

// Search one more square: uses 1 energy and one of the cell's remaining finds. Returns the item found, or 0.
forage_one :: proc(w: ^World) -> Item_Id {
	if !forage_can(w) { return 0 }
	cell := w.chars[w.avatar].cell
	type := pick_forage(w, w.cells[cell].terrain)
	w.cells[cell].forage_remaining -= 1
	add_energy(w, w.avatar, -1)
	if type == .None { return 0 }
	id := item_create(w, type)
	if id != 0 { give_item(w, w.avatar, id); sfx_play(w, .Take) }
	return id
}

// ---- inventory, ground, equipment ----------------------------------------------------------------------------------------------------------------
Stack :: struct { type: Item_Type, count: int }

// Items grouped by type, in the order the first of each arrived.
stacks_of :: proc(w: ^World, first: Item_Id, out: []Stack) -> int {
	n := 0
	for id := first; id != 0; id = w.items[id].next {
		found := false
		for i in 0 ..< n { if out[i].type == w.items[id].type { out[i].count += 1; found = true; break } }
		if !found && n < len(out) { out[n] = {w.items[id].type, 1}; n += 1 }
	}
	return n
}
inventory_stacks :: proc(w: ^World, out: []Stack) -> int { return stacks_of(w, w.chars[w.avatar].first_item, out) }
ground_stacks :: proc(w: ^World, out: []Stack) -> int { return stacks_of(w, w.cells[w.chars[w.avatar].cell].first_item, out) }

// Take up to `count` items of a type from the ground (oldest first); returns how many.
take_items :: proc(w: ^World, type: Item_Type, count: int) -> int {
	cell := w.chars[w.avatar].cell
	taken := 0
	for id := w.cells[cell].first_item; id != 0 && taken < count; {
		next := w.items[id].next // give_item re-links the item
		if w.items[id].type == type { give_item(w, w.avatar, id); taken += 1 }
		id = next
	}
	if taken > 0 { sfx_play(w, .Take) }
	return taken
}

drop_items :: proc(w: ^World, type: Item_Type, count: int) -> int {
	cell := w.chars[w.avatar].cell
	dropped := 0
	for id := w.chars[w.avatar].first_item; id != 0 && dropped < count; {
		next := w.items[id].next
		if w.items[id].type == type { put_item_on_ground(w, cell, id); dropped += 1 }
		id = next
	}
	return dropped
}

// Equip an item from the inventory into its slot; whatever was there goes back to the inventory.
equip :: proc(w: ^World, c: Char_Id, id: Item_Id) {
	slot := ITEMS[w.items[id].type].slot
	if slot == .None { return }
	unequip(w, c, slot)
	item_detach(w, id)
	w.items[id].place, w.items[id].owner = .Equipped, u16(c)
	w.chars[c].equipped[slot] = id
}

unequip :: proc(w: ^World, c: Char_Id, slot: Equip_Slot) {
	if id := w.chars[c].equipped[slot]; id != 0 { give_item(w, c, id) }
}

has_equipment :: proc(w: ^World, c: Char_Id) -> bool { for id in w.chars[c].equipped { if id != 0 { return true } }; return false }
