package game

import "core:fmt"

// world_validate: every invariant the pools and lists must keep. Tests call it after generation, during soak runs and on
// every loaded save (phase 6). It returns the first problem found, or ok.

world_validate :: proc(w: ^World) -> (ok: bool, why: string) {
	fail :: proc(format: string, args: ..any) -> (bool, string) { return false, fmt.tprintf(format, ..args) }

	if w.avatar == 0 || int(w.avatar) >= MAX_CHARS || !w.chars[w.avatar].in_use { return fail("no avatar") }
	if w.chars[w.avatar].type != .Loxy { return fail("the avatar is not Loxy") }

	seen_item: [MAX_ITEMS]bool
	seen_char: [MAX_CHARS]bool
	items_found, chars_found := 0, 0

	// cells: their characters and their ground items
	for ci in 0 ..< TOTAL_CELLS {
		cell := &w.cells[ci]
		if cell.forage_remaining < 0 { return fail("cell %d has negative forage", ci) }
		if int(cell.effect) >= max(w.effect_count, 1) { return fail("cell %d has effect %d of %d", ci, cell.effect, w.effect_count) }
		n := 0
		for c := cell.first_char; c != 0; c = w.chars[c].next_in_cell {
			n += 1
			if n > MAX_CHARS || int(c) >= MAX_CHARS { return fail("cell %d character list is broken", ci) }
			ch := &w.chars[c]
			if !ch.in_use || int(ch.cell) != ci || seen_char[c] { return fail("character %d is wrongly listed in cell %d", c, ci) }
			seen_char[c] = true
			chars_found += 1
		}
		last := Item_Id(0)
		n = 0
		for id := cell.first_item; id != 0; id = w.items[id].next {
			n += 1
			if n > MAX_ITEMS || int(id) >= MAX_ITEMS { return fail("cell %d item list is broken", ci) }
			it := &w.items[id]
			if it.place != .Ground || int(it.owner) != ci || seen_item[id] { return fail("item %d is wrongly on the ground of cell %d", id, ci) }
			seen_item[id] = true
			items_found += 1
			last = id
		}
		if cell.last_item != last { return fail("cell %d last_item is wrong", ci) }
	}

	// characters: stats, carried and equipped items
	in_use := 0
	for ci in 1 ..< MAX_CHARS {
		ch := &w.chars[ci]
		if !ch.in_use { continue }
		in_use += 1
		if !seen_char[ci] { return fail("character %d is in no cell list", ci) }
		if int(ch.cell) >= TOTAL_CELLS { return fail("character %d has no cell", ci) }
		s := ch.stats
		if s.max_health < 1 || s.health < 0 || s.health > s.max_health { return fail("character %d health %d/%d", ci, s.health, s.max_health) }
		if s.max_energy < 0 || s.energy < 0 || s.energy > s.max_energy { return fail("character %d energy %d/%d", ci, s.energy, s.max_energy) }
		if s.jools < 0 || s.peril < 0 || s.ap < 0 || s.xp < 0 { return fail("character %d has a negative statistic", ci) }
		last := Item_Id(0)
		n := 0
		for id := ch.first_item; id != 0; id = w.items[id].next {
			n += 1
			if n > MAX_ITEMS || int(id) >= MAX_ITEMS { return fail("character %d item list is broken", ci) }
			it := &w.items[id]
			if it.place != .Carried || int(it.owner) != ci || seen_item[id] { return fail("item %d is wrongly carried by %d", id, ci) }
			seen_item[id] = true
			items_found += 1
			last = id
		}
		if ch.last_item != last { return fail("character %d last_item is wrong", ci) }
		for slot in Equip_Slot {
			id := ch.equipped[slot]
			if id == 0 { continue }
			if slot == .None { return fail("character %d has something in the None slot", ci) }
			it := &w.items[id]
			if it.place != .Equipped || int(it.owner) != ci || seen_item[id] || ITEMS[it.type].slot != slot {
				return fail("item %d is wrongly equipped by %d", id, ci)
			}
			seen_item[id] = true
			items_found += 1
		}
	}
	if in_use != chars_found { return fail("%d characters in use but %d in cell lists", in_use, chars_found) }

	// items: counts, and the free list
	if items_found != w.item_count { return fail("%d items found but item_count is %d", items_found, w.item_count) }
	free := 0
	for id := w.free_items; id != 0; id = w.items[id].next {
		free += 1
		if free > MAX_ITEMS || int(id) >= MAX_ITEMS || w.items[id].place != .Free || seen_item[id] { return fail("the item free list is broken") }
	}
	if free + w.item_count != MAX_ITEMS - 1 { return fail("%d free + %d used is not %d", free, w.item_count, MAX_ITEMS - 1) }

	// effects
	for i in 1 ..< w.effect_count {
		e := w.effects[i]
		if (e.kind == .Teleport || e.kind == .Enter_Cellar) && int(e.dest) >= TOTAL_CELLS { return fail("effect %d has no destination", i) }
	}

	if w.sfx_count < 0 || w.sfx_count > MAX_SFX { return fail("the sound queue is corrupt") }
	// messages
	if w.msg_count < 0 || w.msg_count > MAX_MESSAGES || w.msg_head < 0 || w.msg_head >= MAX_MESSAGES { return fail("the message queue is corrupt") }
	for i in 0 ..< w.msg_count {
		m := &w.messages[(w.msg_head + i) % MAX_MESSAGES]
		if int(m.line_count) > MAX_LINES || int(m.choice_count) > MAX_CHOICES { return fail("message %d has too many lines or choices", i) }
		for l in 0 ..< m.line_count { if int(m.lines[l].len) > LINE_CAP { return fail("message %d line %d is too long", i, l) } }
	}
	return true, ""
}
