package game

// The world: one flat struct, no heap, no globals (so a save is a copy and tests can make as many as they like).
// Entities are values in fixed pools addressed by small ids; id 0 means "none". Lists (items on the ground, items carried,
// characters in a cell) are singly linked through the entities and keep insertion order, which stands in for the original's
// insertion-ordered hash sets: inventory stacks show in the order their first item arrived, recipes consume oldest first.

// ---- maps --------------------------------------------------------------------------------------------------------
Map_Id :: enum u8 { Town, Wilderness, Healer, Health_Trainer, Potter, Druid_House, Inn, Cellar, Energy_Trainer }

Map_Size :: struct { cols, rows: int }

TOWN_SIZE :: 17
WILDERNESS_SIZE :: 75 // 5 by 5 maze cells of 15 by 15
ROOM_SIZE :: 7

MAP_SIZES := [Map_Id]Map_Size{
	.Town = {TOWN_SIZE, TOWN_SIZE}, .Wilderness = {WILDERNESS_SIZE, WILDERNESS_SIZE}, .Healer = {ROOM_SIZE, ROOM_SIZE},
	.Health_Trainer = {ROOM_SIZE, ROOM_SIZE}, .Potter = {ROOM_SIZE, ROOM_SIZE}, .Druid_House = {ROOM_SIZE, ROOM_SIZE},
	.Inn = {ROOM_SIZE, ROOM_SIZE}, .Cellar = {ROOM_SIZE, ROOM_SIZE}, .Energy_Trainer = {ROOM_SIZE, ROOM_SIZE},
}

TOTAL_CELLS :: TOWN_SIZE * TOWN_SIZE + WILDERNESS_SIZE * WILDERNESS_SIZE + 7 * ROOM_SIZE * ROOM_SIZE

// Only the wilderness allows camping and fire building (the original's map flags).
map_allows_camping :: proc(m: Map_Id) -> bool { return m == .Wilderness }
map_allows_fire :: proc(m: Map_Id) -> bool { return m == .Wilderness }

Cell_Id :: distinct u16
NO_CELL :: Cell_Id(0xFFFF)

map_offset :: proc(m: Map_Id) -> int {
	o := 0
	for i in Map_Id { if i == m { break }; o += MAP_SIZES[i].cols * MAP_SIZES[i].rows }
	return o
}

// The cell at (col, row) of a map, or NO_CELL outside it.
cell_at :: proc(m: Map_Id, col, row: int) -> Cell_Id {
	s := MAP_SIZES[m]
	if col < 0 || row < 0 || col >= s.cols || row >= s.rows { return NO_CELL }
	return Cell_Id(map_offset(m) + row * s.cols + col)
}

Cell_Pos :: struct { map_id: Map_Id, col, row: int }

cell_pos :: proc(c: Cell_Id) -> Cell_Pos {
	rest := int(c)
	for m in Map_Id {
		n := MAP_SIZES[m].cols * MAP_SIZES[m].rows
		if rest < n { return {m, rest % MAP_SIZES[m].cols, rest / MAP_SIZES[m].cols} }
		rest -= n
	}
	return {}
}

// ---- effects ------------------------------------------------------------------------------------------------------
// A trigger on a cell (what the original calls a map effect): bumping or entering the cell runs the effect for the avatar.
Effect_Ref :: distinct u8 // 0 = none
MAX_EFFECTS :: 64

Cell_Effect :: struct {
	kind:    Effect_Kind,
	dest:    Cell_Id,      // Teleport and Enter_Cellar: where to
	message: Message_Type, // Message: which canned text
	number:  i32,          // Heal: the most health the basin restores
}

// ---- items --------------------------------------------------------------------------------------------------------
Item_Id :: distinct u16 // 0 = none
MAX_ITEMS :: 8192

Item_Place :: enum u8 { Free, Ground, Carried, Equipped }

Item :: struct {
	type:       Item_Type,
	place:      Item_Place,
	durability: i16,
	owner:      u16, // the cell (Ground) or the character (Carried, Equipped)
	next:       Item_Id,
}

// ---- characters ---------------------------------------------------------------------------------------------------
Char_Id :: distinct u16 // 0 = none
MAX_CHARS :: 128

// What the original kept as flags on the avatar (set by the Druid, the Innkeeper and the rat quest).
Char_Flag :: enum u8 { Knows_Foraging, Knows_Twine_Making, Knows_Rock_Sharpening, Knows_Fire_Making, Knows_Torch_Making, Knows_Hatchet_Making, Rat_Quest, Paid_Innkeeper }

Character :: struct {
	in_use:       bool,
	type:         Character_Type,
	cell:         Cell_Id,
	stats:        Character_Stats,
	flags:        bit_set[Char_Flag],
	first_item:   Item_Id, // carried items, oldest first
	last_item:    Item_Id,
	equipped:     [Equip_Slot]Item_Id,
	next_in_cell: Char_Id,
}

// ---- cells ----------------------------------------------------------------------------------------------------------
Cell :: struct {
	terrain:          Terrain_Type,
	forage_remaining: i16,
	effect:           Effect_Ref,
	first_item:       Item_Id, // items on the ground, oldest first
	last_item:        Item_Id,
	first_char:       Char_Id, // characters here, oldest first
}

// ---- messages (the original's queue of dialogs) -------------------------------------------------------------------
MAX_MESSAGES :: 32
MAX_LINES :: 12
MAX_CHOICES :: 8
LINE_CAP :: 56 // the screen is 48 characters wide; longer text is cut
CHOICE_CAP :: 48 // the screen is 48 characters wide

Text_Line :: struct { hue: Hue, len: u8, buf: [LINE_CAP]u8 }

// A choice runs `effect` for the avatar when picked. item, number and again carry what the original stored as choice
// metadata and statistics (for example Buy: which item, the price, and which dialog to reopen).
Msg_Choice :: struct {
	len:    u8,
	buf:    [CHOICE_CAP]u8,
	effect: Effect_Kind,
	item:   Item_Type,
	number: i32,
	again:  Effect_Kind,
}

Message :: struct {
	sfx:          Sfx_Id,
	line_count:   u8,
	choice_count: u8,
	lines:        [MAX_LINES]Text_Line,
	choices:      [MAX_CHOICES]Msg_Choice,
	overflow:     Msg_Choice, // where a choice beyond MAX_CHOICES is written and ignored
}

// ---- the world ----------------------------------------------------------------------------------------------------------
World :: struct {
	rng:          Rng,
	cells:        [TOTAL_CELLS]Cell,
	effects:      [MAX_EFFECTS]Cell_Effect, // index 0 is unused
	effect_count: int,
	items:        [MAX_ITEMS]Item, // index 0 is unused
	free_items:   Item_Id,         // head of the free list (linked through `next`)
	item_count:   int,             // items in use
	chars:        [MAX_CHARS]Character, // index 0 is unused
	avatar:       Char_Id,
	messages:     [MAX_MESSAGES]Message, // a ring: the oldest is shown first
	msg_head:     int,
	msg_count:    int,
	msg_scratch:  Message, // where messages go when the queue is full (they are dropped)
	sfx_queue:    [MAX_SFX]Sfx_Id, // sounds to play now (those with no message to carry them)
	sfx_count:    int,
}

MAX_SFX :: 16

// ---- item and character pools ---------------------------------------------------------------------------------------
cell_ptr :: proc(w: ^World, c: Cell_Id) -> ^Cell { return &w.cells[c] }

item_create :: proc(w: ^World, type: Item_Type) -> Item_Id {
	id := w.free_items
	if id == 0 { return 0 } // the pool is full
	it := &w.items[id]
	w.free_items = it.next
	it^ = {type = type, place = .Ground, durability = i16(ITEMS[type].stats.durability)}
	w.item_count += 1
	return id
}

// Detach an item from wherever it is, leaving it in no list (place Free is NOT set: callers re-home it or recycle it).
item_detach :: proc(w: ^World, id: Item_Id) {
	it := &w.items[id]
	switch it.place {
	case .Free:
	case .Ground:
		c := &w.cells[it.owner]
		list_remove(w, &c.first_item, &c.last_item, id)
	case .Carried:
		ch := &w.chars[it.owner]
		list_remove(w, &ch.first_item, &ch.last_item, id)
	case .Equipped:
		ch := &w.chars[it.owner]
		for slot in Equip_Slot { if ch.equipped[slot] == id { ch.equipped[slot] = 0 } }
	}
	it.next = 0
}

item_recycle :: proc(w: ^World, id: Item_Id) {
	item_detach(w, id)
	it := &w.items[id]
	it^ = {place = .Free, next = w.free_items}
	w.free_items = id
	w.item_count -= 1
}

list_append :: proc(w: ^World, first, last: ^Item_Id, id: Item_Id) {
	w.items[id].next = 0
	if first^ == 0 { first^ = id } else { w.items[last^].next = id }
	last^ = id
}

list_remove :: proc(w: ^World, first, last: ^Item_Id, id: Item_Id) {
	prev := Item_Id(0)
	cur := first^
	for cur != 0 {
		if cur == id {
			next := w.items[cur].next
			if prev == 0 { first^ = next } else { w.items[prev].next = next }
			if last^ == id { last^ = prev }
			return
		}
		prev = cur
		cur = w.items[cur].next
	}
}

give_item :: proc(w: ^World, c: Char_Id, id: Item_Id) {
	item_detach(w, id)
	ch := &w.chars[c]
	it := &w.items[id]
	it.place, it.owner = .Carried, u16(c)
	list_append(w, &ch.first_item, &ch.last_item, id)
}

put_item_on_ground :: proc(w: ^World, cell: Cell_Id, id: Item_Id) {
	item_detach(w, id)
	cp := &w.cells[cell]
	it := &w.items[id]
	it.place, it.owner = .Ground, u16(cell)
	list_append(w, &cp.first_item, &cp.last_item, id)
}

// ---- characters ----------------------------------------------------------------------------------------------------
char_create :: proc(w: ^World, type: Character_Type, cell: Cell_Id) -> Char_Id {
	for i in 1 ..< MAX_CHARS {
		if !w.chars[i].in_use {
			w.chars[i] = {in_use = true, type = type, cell = cell, stats = CHARACTERS[type].stats}
			char_enter_cell_list(w, Char_Id(i), cell)
			return Char_Id(i)
		}
	}
	return 0
}

char_enter_cell_list :: proc(w: ^World, c: Char_Id, cell: Cell_Id) {
	w.chars[c].cell = cell
	w.chars[c].next_in_cell = 0
	cp := &w.cells[cell]
	if cp.first_char == 0 { cp.first_char = c; return }
	cur := cp.first_char
	for w.chars[cur].next_in_cell != 0 { cur = w.chars[cur].next_in_cell }
	w.chars[cur].next_in_cell = c
}

char_leave_cell_list :: proc(w: ^World, c: Char_Id) {
	cp := &w.cells[w.chars[c].cell]
	prev := Char_Id(0)
	cur := cp.first_char
	for cur != 0 {
		if cur == c {
			next := w.chars[cur].next_in_cell
			if prev == 0 { cp.first_char = next } else { w.chars[prev].next_in_cell = next }
			w.chars[c].next_in_cell = 0
			return
		}
		prev = cur
		cur = w.chars[cur].next_in_cell
	}
}

// Characters in the avatar's cell other than `c`, oldest first (at most `out` capacity).
others_in_cell :: proc(w: ^World, c: Char_Id, out: []Char_Id) -> int {
	n := 0
	cur := w.cells[w.chars[c].cell].first_char
	for cur != 0 && n < len(out) {
		if cur != c { out[n] = cur; n += 1 }
		cur = w.chars[cur].next_in_cell
	}
	return n
}

// ---- cell effects ---------------------------------------------------------------------------------------------------
effect_add :: proc(w: ^World, e: Cell_Effect) -> Effect_Ref {
	if w.effect_count == 0 { w.effect_count = 1 } // index 0 stays unused
	assert(w.effect_count < MAX_EFFECTS, "too many cell effects")
	w.effects[w.effect_count] = e
	w.effect_count += 1
	return Effect_Ref(w.effect_count - 1)
}

terrain_flag :: proc(w: ^World, cell: Cell_Id, flag: Terrain_Flag) -> bool {
	return flag in TERRAINS[w.cells[cell].terrain].flags
}
