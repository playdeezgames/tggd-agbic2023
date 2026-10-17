package game

// Saving: one JSON text {format, version, content, summary, ...world}. Enums are stored by name, so a save stays readable and a
// renamed or removed name is rejected. The loader validates everything (names, ids, ranges, lists, then world_validate) into a
// scratch world and copies it over the live one only if all of it holds, so a bad file leaves nothing behind.
// The terrain is one character per cell (62 symbols), the rest are lists of what differs from nothing. Saves record CONTENT_HASH
// and SAVE_VERSION: a save only loads against the content and rules that made it.

import "base:intrinsics"
import "core:encoding/json"
import "core:fmt"
import "core:strings"

SAVE_FORMAT :: "bagel-quest"
SAVE_VERSION :: 1 // bump when the meaning of anything below changes

Save_Effect :: struct { kind: Effect_Kind, dest: int, message: Message_Type, number: i32 }
Save_Ground :: struct { cell: int, items: []int }
Save_Item :: struct { id: int, type: Item_Type, durability: i32 }
Save_Char :: struct {
	id:       int,
	type:     Character_Type,
	cell:     int,
	stats:    Character_Stats,
	flags:    []Char_Flag,
	items:    []int,
	equipped: [2]int, // Weapon, Head (the slots after None)
}

Save_File :: struct {
	format:       string,
	version:      int,
	content:      string,
	summary:      string,
	rng:          [4]string, // hex: a JSON number cannot hold 64 bits exactly
	avatar:       int,
	terrain:      string,
	forage:       [][2]int,   // [cell, finds left] where it differs from the terrain's start
	effects:      []Save_Effect, // effect 1, 2, ... (0 is "none")
	cell_effects: [][2]int,   // [cell, effect]
	ground:       []Save_Ground,
	items:        []Save_Item,
	chars:        []Save_Char, // in the order they stand in their cells
}

TERRAIN_SYMBOLS :: "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
terrain_symbols := TERRAIN_SYMBOLS // a constant cannot be indexed at run time

// ---- writing ---------------------------------------------------------------------------------------------------------------------------------------------------
save_summary :: proc(w: ^World) -> string {
	s := w.chars[w.avatar].stats
	return fmt.tprintf("Level %d, HP %d/%d, %d jools, %v", s.xp_level, s.health, s.max_health, s.jools, cell_pos(w.chars[w.avatar].cell).map_id)
}

// The world as save text (in `allocator`). Messages waiting to be shown are not saved (the menus are only reachable when none are).
world_to_save_text :: proc(w: ^World, allocator := context.allocator) -> (text: string, ok: bool) {
	f: Save_File
	f.format, f.version, f.content = SAVE_FORMAT, SAVE_VERSION, fmt.tprintf("%016x", CONTENT_HASH)
	f.summary = save_summary(w)
	for i in 0 ..< 4 { f.rng[i] = fmt.tprintf("%016x", w.rng.s[i]) }
	f.avatar = int(w.avatar)
	terr := make([]u8, TOTAL_CELLS, context.temp_allocator)
	forage := make([dynamic][2]int, context.temp_allocator)
	cell_fx := make([dynamic][2]int, context.temp_allocator)
	ground := make([dynamic]Save_Ground, context.temp_allocator)
	chars := make([dynamic]Save_Char, context.temp_allocator)
	for ci in 0 ..< TOTAL_CELLS {
		c := &w.cells[ci]
		terr[ci] = terrain_symbols[int(c.terrain)]
		if int(c.forage_remaining) != int(TERRAINS[c.terrain].forage_count) { append(&forage, [2]int{ci, int(c.forage_remaining)}) }
		if c.effect != 0 { append(&cell_fx, [2]int{ci, int(c.effect)}) }
		if c.first_item != 0 {
			ids := make([dynamic]int, context.temp_allocator)
			for it := c.first_item; it != 0; it = w.items[it].next { append(&ids, int(it)) }
			append(&ground, Save_Ground{ci, ids[:]})
		}
		for cid := c.first_char; cid != 0; cid = w.chars[cid].next_in_cell {
			ch := &w.chars[cid]
			sc := Save_Char{id = int(cid), type = ch.type, cell = ci, stats = ch.stats}
			flags := make([dynamic]Char_Flag, context.temp_allocator)
			for fl in Char_Flag { if fl in ch.flags { append(&flags, fl) } }
			sc.flags = flags[:]
			ids := make([dynamic]int, context.temp_allocator)
			for it := ch.first_item; it != 0; it = w.items[it].next { append(&ids, int(it)) }
			sc.items = ids[:]
			sc.equipped = {int(ch.equipped[.Weapon]), int(ch.equipped[.Head])}
			append(&chars, sc)
		}
	}
	f.terrain = string(terr)
	f.forage, f.cell_effects, f.ground, f.chars = forage[:], cell_fx[:], ground[:], chars[:]
	effects := make([dynamic]Save_Effect, context.temp_allocator)
	for i in 1 ..< w.effect_count {
		e := w.effects[i]
		append(&effects, Save_Effect{e.kind, int(e.dest), e.message, e.number})
	}
	f.effects = effects[:]
	items := make([dynamic]Save_Item, context.temp_allocator)
	for id in 1 ..< MAX_ITEMS { if w.items[id].place != .Free { append(&items, Save_Item{id, w.items[id].type, i32(w.items[id].durability)}) } }
	f.items = items[:]
	data, err := json.marshal(f, {use_enum_names = true, spec = .JSON}, allocator)
	if err != nil { return "", false }
	return string(data), true
}

// ---- reading ------------------------------------------------------------------------------------------------------------------------------------------------------
parse_hex :: proc(s: string) -> (v: u64, ok: bool) {
	if len(s) == 0 || len(s) > 16 { return 0, false }
	for ch in s {
		d: u64
		switch ch {
		case '0' ..= '9': d = u64(ch - '0')
		case 'a' ..= 'f': d = u64(ch - 'a') + 10
		case: return 0, false
		}
		v = v << 4 | d
	}
	return v, true
}

// Build a world from save text into `w` (which is overwritten). Returns why not, if not.
world_from_save_text :: proc(w: ^World, text: string) -> (ok: bool, why: string) {
	f: Save_File
	if err := json.unmarshal(transmute([]byte)text, &f, .JSON, context.temp_allocator); err != nil { return false, "not a valid save file" }
	if f.format != SAVE_FORMAT { return false, "not a Bagel Quest save" }
	if f.version != SAVE_VERSION { return false, "saved by a different version of the game" }
	if f.content != fmt.tprintf("%016x", CONTENT_HASH) { return false, "saved with different game content" }
	if len(f.terrain) != TOTAL_CELLS { return false, "the map is the wrong size" }

	world_blank(w)
	for i in 0 ..< 4 {
		v, good := parse_hex(f.rng[i])
		if !good { return false, "bad random state" }
		w.rng.s[i] = v
	}
	if w.rng.s == {} { return false, "bad random state" }
	cell_ok :: proc(c: int) -> bool { return c >= 0 && c < TOTAL_CELLS }

	for ci in 0 ..< TOTAL_CELLS {
		idx := strings.index_byte(TERRAIN_SYMBOLS, f.terrain[ci])
		if idx < 0 || idx >= len(Terrain_Type) { return false, "unknown terrain" }
		t := Terrain_Type(idx)
		w.cells[ci].terrain = t
		w.cells[ci].forage_remaining = i16(TERRAINS[t].forage_count)
	}
	for p in f.forage {
		if !cell_ok(p[0]) || p[1] < 0 || p[1] > 1000 { return false, "bad forage entry" }
		w.cells[p[0]].forage_remaining = i16(p[1])
	}
	if len(f.effects) >= MAX_EFFECTS { return false, "too many effects" }
	w.effect_count = 1
	for e in f.effects {
		if e.dest != 0xFFFF && !cell_ok(e.dest) { return false, "bad effect destination" }
		w.effects[w.effect_count] = {kind = e.kind, dest = Cell_Id(e.dest), message = e.message, number = e.number}
		w.effect_count += 1
	}
	for p in f.cell_effects {
		if !cell_ok(p[0]) || p[1] < 1 || p[1] >= w.effect_count { return false, "bad cell effect" }
		w.cells[p[0]].effect = Effect_Ref(p[1])
	}
	// items: take their slots out of the free list by rebuilding it from what is left
	used: [MAX_ITEMS]bool
	for it in f.items {
		if it.id < 1 || it.id >= MAX_ITEMS || used[it.id] || it.type == .None || it.durability < 0 || it.durability > 1000 { return false, "bad item" }
		used[it.id] = true
		w.items[it.id] = {type = it.type, place = .Ground, durability = i16(it.durability)}
		w.item_count += 1
	}
	w.free_items = 0
	for id := MAX_ITEMS - 1; id >= 1; id -= 1 {
		if !used[id] { w.items[id] = {place = .Free, next = w.free_items}; w.free_items = Item_Id(id) }
	}
	// where each item is: exactly one list
	placed: [MAX_ITEMS]bool
	claim :: proc(placed: ^[MAX_ITEMS]bool, used: ^[MAX_ITEMS]bool, id: int) -> bool {
		if id < 1 || id >= MAX_ITEMS || !used[id] || placed[id] { return false }
		placed[id] = true
		return true
	}
	for g in f.ground {
		if !cell_ok(g.cell) || w.cells[g.cell].first_item != 0 { return false, "bad ground list" }
		for id in g.items {
			if !claim(&placed, &used, id) { return false, "an item is listed twice or not at all" }
			put_item_on_ground(w, Cell_Id(g.cell), Item_Id(id))
		}
	}
	for sc in f.chars {
		if sc.id < 1 || sc.id >= MAX_CHARS || w.chars[sc.id].in_use || !cell_ok(sc.cell) { return false, "bad character" }
		w.chars[sc.id] = {in_use = true, type = sc.type, stats = sc.stats}
		char_enter_cell_list(w, Char_Id(sc.id), Cell_Id(sc.cell))
		for fl in sc.flags { w.chars[sc.id].flags += {fl} }
		for id in sc.items {
			if !claim(&placed, &used, id) { return false, "an item is listed twice or not at all" }
			give_item(w, Char_Id(sc.id), Item_Id(id))
		}
		slots := [2]Equip_Slot{.Weapon, .Head}
		for slot, i in slots {
			id := sc.equipped[i]
			if id == 0 { continue }
			if !claim(&placed, &used, id) || ITEMS[w.items[id].type].slot != slot { return false, "bad equipment" }
			w.items[id].place, w.items[id].owner = .Equipped, u16(sc.id)
			w.chars[sc.id].equipped[slot] = Item_Id(id)
		}
	}
	for id in 1 ..< MAX_ITEMS { if used[id] && !placed[id] { return false, "an item is in no list" } }
	if f.avatar < 1 || f.avatar >= MAX_CHARS { return false, "no avatar" }
	w.avatar = Char_Id(f.avatar)
	if good, reason := world_validate(w); !good { return false, reason }
	return true, ""
}
