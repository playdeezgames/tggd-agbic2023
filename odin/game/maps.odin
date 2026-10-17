package game

// World generation: the nine maps, their stitching, and the avatar. A port of MapInitializer, WorldInitializer,
// WildernessInitializer, TownInitializer and the room initializers.
// Randomness comes only from w.rng, in a fixed order, so a seed gives the same world on every target.

import "base:intrinsics"

// An empty world: nothing on the map, every item slot free.
world_blank :: proc(w: ^World) {
	intrinsics.mem_zero(w, size_of(World)) // not `w^ = {}`: no chance of a 250 KB temporary on the wasm stack
	// item pool: a free list through all slots but 0
	for i in 1 ..< MAX_ITEMS - 1 { w.items[i].next = Item_Id(i + 1) }
	w.items[MAX_ITEMS - 1].next = 0
	w.free_items = 1
	for id in 0 ..< len(w.items) { w.items[id].place = .Free }
	for &c in w.cells { c.terrain = .Empty }
}

world_new :: proc(w: ^World, seed: u64) {
	world_blank(w)
	rng_seed(&w.rng, seed)
	for m in Map_Id {
		build_map(w, m)
		init_cells(w, m)
		if m == .Town { spawn_loxy(w) }
	}
	stitch_maps(w)
}

// ---- helpers ----------------------------------------------------------------------------------------------------------
set_terrain :: proc(w: ^World, m: Map_Id, col, row: int, t: Terrain_Type) { w.cells[cell_at(m, col, row)].terrain = t }

fill_map :: proc(w: ^World, m: Map_Id, t: Terrain_Type) {
	s := MAP_SIZES[m]
	for row in 0 ..< s.rows { for col in 0 ..< s.cols { set_terrain(w, m, col, row, t) } }
}

// Walls on the border of a room.
wall_room :: proc(w: ^World, m: Map_Id) {
	s := MAP_SIZES[m]
	for col in 0 ..< s.cols { set_terrain(w, m, col, 0, .Wall); set_terrain(w, m, col, s.rows - 1, .Wall) }
	for row in 1 ..< s.rows - 1 { set_terrain(w, m, 0, row, .Wall); set_terrain(w, m, s.cols - 1, row, .Wall) }
}

set_effect :: proc(w: ^World, m: Map_Id, col, row: int, e: Cell_Effect) {
	w.cells[cell_at(m, col, row)].effect = effect_add(w, e)
}

// Cells run their terrain's Initialize script after the map is built: it only sets how much can be foraged.
init_cells :: proc(w: ^World, m: Map_Id) {
	s := MAP_SIZES[m]
	for row in 0 ..< s.rows {
		for col in 0 ..< s.cols {
			c := &w.cells[cell_at(m, col, row)]
			c.forage_remaining = i16(TERRAINS[c.terrain].forage_count)
		}
	}
}

// ---- the maps ------------------------------------------------------------------------------------------------------------
build_map :: proc(w: ^World, m: Map_Id) {
	switch m {
	case .Town:
		build_town(w)
	case .Wilderness:
		build_wilderness(w)
	case .Healer:
		fill_map(w, m, .Empty)
		wall_room(w, m)
		set_terrain(w, m, 3, 6, .Door)
		set_terrain(w, m, 3, 1, .Basin)
		set_effect(w, m, 3, 1, {kind = .Heal, number = 5})
		set_terrain(w, m, 5, 5, .Old_Man)
		set_effect(w, m, 5, 5, {kind = .Healer_Talk})
	case .Health_Trainer:
		fill_map(w, m, .Empty)
		wall_room(w, m)
		set_terrain(w, m, 3, 6, .Door)
		set_terrain(w, m, 3, 1, .Strong_Man)
		set_effect(w, m, 3, 1, {kind = .Health_Trainer_Talk})
	case .Potter:
		fill_map(w, m, .Empty)
		wall_room(w, m)
		set_terrain(w, m, 3, 6, .Door)
		set_terrain(w, m, 3, 1, .Potter)
		set_effect(w, m, 3, 1, {kind = .Potter_Talk})
	case .Druid_House:
		fill_map(w, m, .Empty)
		wall_room(w, m)
		set_terrain(w, m, 0, 3, .Door)
		set_terrain(w, m, 5, 3, .Druid)
		set_effect(w, m, 5, 3, {kind = .Druid_Talk})
	case .Energy_Trainer:
		fill_map(w, m, .Empty)
		wall_room(w, m)
		set_terrain(w, m, 3, 0, .Door)
		set_terrain(w, m, 3, 5, .Energy_Trainer)
		set_effect(w, m, 3, 5, {kind = .Energy_Trainer_Talk})
	case .Inn:
		build_inn(w)
	case .Cellar:
		fill_map(w, m, .Cellar_Floor)
		wall_room(w, m)
		set_terrain(w, m, 1, 5, .Stairs_Up)
		// its Teleport effect is given its destination when the maps are stitched
		set_effect(w, m, 1, 5, {kind = .Teleport})
	}
}

TOWN_MINIMAP := [TOWN_SIZE]string{
	"!!!!!!!!*!!!!!!!!",
	"!.......*.......!",
	"!.......*.......!",
	"!.+^*********^+.!",
	"!..**..+^...**..!",
	"!..*.........*..!",
	"!..*.+^***^+.*..!",
	"!..*..**.**..*..!",
	"*******...*******",
	"!..*..**.**..*..!",
	"!..*.+^***^+.*..!",
	"!..*.........*..!",
	"!..**...^+..**..!",
	"!.+^*********^+.!",
	"!.......*.......!",
	"!.......*.......!",
	"!!!!!!!!*!!!!!!!!",
}

Town_Sign :: struct { col, row: int, message: Message_Type }
TOWN_SIGNS := [10]Town_Sign{
	{2, 13, .Healer_Sign}, {2, 3, .Inn_Sign}, {14, 3, .Town_Sign2}, {7, 4, .Health_Trainer_Sign}, {5, 6, .Energy_Trainer_Sign},
	{11, 6, .Town_Sign5}, {5, 10, .Town_Sign6}, {11, 10, .Druid_Sign}, {9, 12, .Town_Sign8}, {14, 13, .Potter_Sign},
}

build_town :: proc(w: ^World) {
	for line, row in TOWN_MINIMAP {
		for ch, col in line {
			t: Terrain_Type
			switch ch {
			case '!': t = .Fence
			case '.': t = .Grass
			case '*': t = .Gravel
			case '^': t = .House
			case '+': t = .Sign
			case: panic("unknown town minimap character")
			}
			set_terrain(w, .Town, col, row, t)
		}
	}
	for s in TOWN_SIGNS { set_effect(w, .Town, s.col, s.row, {kind = .Message, message = s.message}) }
}

INN_MINIMAP := [ROOM_SIZE]string{
	"#######",
	"#b # b#",
	"#  #  #",
	"##D#D##",
	"#     #",
	"#v   !#",
	"###D###",
}

build_inn :: proc(w: ^World) {
	for line, row in INN_MINIMAP {
		for ch, col in line {
			t: Terrain_Type
			switch ch {
			case '#': t = .Wall
			case 'b': t = .Bed
			case ' ': t = .Empty
			case 'D': t = .Door
			case '!': t = .Gorachan
			case 'v': t = .Stairs_Down
			case: panic("unknown inn minimap character")
			}
			set_terrain(w, .Inn, col, row, t)
		}
	}
	for row in 0 ..< ROOM_SIZE {
		for col in 0 ..< ROOM_SIZE {
			id := cell_at(.Inn, col, row)
			if w.cells[id].terrain == .Gorachan { set_effect(w, .Inn, col, row, {kind = .Gorachan_Talk}) }
		}
	}
	// all four beds share one effect
	sleep := effect_add(w, {kind = .Sleep_At_Inn})
	for row in 0 ..< ROOM_SIZE {
		for col in 0 ..< ROOM_SIZE {
			id := cell_at(.Inn, col, row)
			if w.cells[id].terrain == .Bed { w.cells[id].effect = sleep }
		}
	}
	// the stairs down: EnterCellar, with its destination given when the maps are stitched
	set_effect(w, .Inn, 1, 5, {kind = .Enter_Cellar})
}

// ---- the wilderness -------------------------------------------------------------------------------------------------------
MAZE_SIZE :: 5
MAZE_CELL :: 15 // tiles per maze cell

Direction :: enum u8 { North, East, South, West }
DIRECTION_DELTA := [Direction][2]int{.North = {0, -1}, .East = {1, 0}, .South = {0, 1}, .West = {-1, 0}}
direction_opposite :: proc(d: Direction) -> Direction { return Direction((int(d) + 2) % 4) }

Maze :: struct { open: [MAZE_SIZE][MAZE_SIZE][Direction]bool } // open[col][row][d]: the door of that cell towards d is open

// A random spanning tree (the original's frontier method): start at a random cell; repeatedly take a random frontier cell
// and open a door to a random neighbour that is already inside.
maze_generate :: proc(r: ^Rng, maze: ^Maze) {
	maze^ = {}
	inside: [MAZE_SIZE][MAZE_SIZE]bool
	queued: [MAZE_SIZE][MAZE_SIZE]bool
	frontier: [MAZE_SIZE * MAZE_SIZE][2]int
	n := 0
	add_neighbours :: proc(cell: [2]int, inside, queued: ^[MAZE_SIZE][MAZE_SIZE]bool, frontier: ^[MAZE_SIZE * MAZE_SIZE][2]int, n: ^int) {
		for d in Direction {
			nb := cell + DIRECTION_DELTA[d]
			if nb.x < 0 || nb.y < 0 || nb.x >= MAZE_SIZE || nb.y >= MAZE_SIZE { continue }
			if inside[nb.x][nb.y] || queued[nb.x][nb.y] { continue }
			queued[nb.x][nb.y] = true
			frontier[n^] = nb
			n^ += 1
		}
	}
	start := [2]int{rng_range(r, 0, MAZE_SIZE - 1), rng_range(r, 0, MAZE_SIZE - 1)}
	inside[start.x][start.y] = true
	add_neighbours(start, &inside, &queued, &frontier, &n)
	for n > 0 {
		pick := rng_range(r, 0, n - 1)
		cell := frontier[pick]
		frontier[pick] = frontier[n - 1]
		n -= 1
		candidates: [4]Direction
		nc := 0
		for d in Direction {
			nb := cell + DIRECTION_DELTA[d]
			if nb.x < 0 || nb.y < 0 || nb.x >= MAZE_SIZE || nb.y >= MAZE_SIZE { continue }
			if inside[nb.x][nb.y] { candidates[nc] = d; nc += 1 }
		}
		d := candidates[rng_range(r, 0, nc - 1)]
		nb := cell + DIRECTION_DELTA[d]
		maze.open[cell.x][cell.y][d] = true
		maze.open[nb.x][nb.y][direction_opposite(d)] = true
		inside[cell.x][cell.y] = true
		add_neighbours(cell, &inside, &queued, &frontier, &n)
	}
}

// What the wilderness is painted with, by weight (the original's terrainTable).
WILDERNESS_TERRAIN := [?]struct { terrain: Terrain_Type, weight: int }{
	{.Empty, 100}, {.Grass, 50}, {.Tree, 150}, {.Farm, 10}, {.Clay_Pit, 1}, {.Rock_Quarry, 5},
}

// River tile by which doors are open: north 1, east 2, south 4, west 8.
RIVER_TILE := [16]Terrain_Type{
	1 = .River_N, 2 = .River_E, 3 = .River_NE, 4 = .River_S, 5 = .River_NS, 6 = .River_SE, 7 = .River_NES, 8 = .River_W,
	9 = .River_NW, 10 = .River_EW, 11 = .River_WNE, 12 = .River_SW, 13 = .River_SWN, 14 = .River_ESW, 15 = .River_NESW,
}

build_wilderness :: proc(w: ^World) {
	m :: Map_Id.Wilderness
	total := 0
	for e in WILDERNESS_TERRAIN { total += e.weight }
	for row in 0 ..< WILDERNESS_SIZE {
		for col in 0 ..< WILDERNESS_SIZE {
			roll := rng_range(&w.rng, 0, total - 1)
			for e in WILDERNESS_TERRAIN {
				roll -= e.weight
				if roll < 0 { set_terrain(w, m, col, row, e.terrain); break }
			}
		}
	}
	// rivers: the maze drawn as water, centre tile and straight lines to each open side
	maze: Maze
	maze_generate(&w.rng, &maze)
	half :: MAZE_CELL / 2
	for mc in 0 ..< MAZE_SIZE {
		for mr in 0 ..< MAZE_SIZE {
			ox, oy := mc * MAZE_CELL, mr * MAZE_CELL
			flags := 0
			if maze.open[mc][mr][.North] { for y in oy ..< oy + half { set_terrain(w, m, ox + half, y, .River_NS) }; flags += 1 }
			if maze.open[mc][mr][.South] { for y in oy + half + 1 ..< oy + half + 1 + half { set_terrain(w, m, ox + half, y, .River_NS) }; flags += 4 }
			if maze.open[mc][mr][.West] { for x in ox ..< ox + half { set_terrain(w, m, x, oy + half, .River_EW) }; flags += 8 }
			if maze.open[mc][mr][.East] { for x in ox + half + 1 ..< ox + half + 1 + half { set_terrain(w, m, x, oy + half, .River_EW) }; flags += 2 }
			set_terrain(w, m, ox + half, oy + half, RIVER_TILE[flags])
		}
	}
	// every river tile shares one effect: bumping the bank
	bump := effect_add(w, {kind = .Bump_River})
	for row in 0 ..< WILDERNESS_SIZE {
		for col in 0 ..< WILDERNESS_SIZE {
			id := cell_at(m, col, row)
			if terrain_flag(w, id, .Is_Water_Source) { w.cells[id].effect = bump }
		}
	}
	// the town: a random tenable tile (the original retries without a limit; here it is bounded, then scans)
	placed := false
	for _ in 0 ..< 10000 {
		id := cell_at(m, rng_range(&w.rng, 0, WILDERNESS_SIZE - 1), rng_range(&w.rng, 0, WILDERNESS_SIZE - 1))
		if terrain_flag(w, id, .Tenable) { w.cells[id].terrain = .Town; placed = true; break }
	}
	if !placed {
		for i in 0 ..< WILDERNESS_SIZE * WILDERNESS_SIZE {
			id := Cell_Id(map_offset(m) + i)
			if terrain_flag(w, id, .Tenable) { w.cells[id].terrain = .Town; break }
		}
	}
}

// ---- characters at creation -------------------------------------------------------------------------------------------------
// The original ran an Initialize script per character type; each is a line or two, so they are plain code here.
init_character :: proc(w: ^World, c: Char_Id) {
	ch := &w.chars[c]
	switch ch.type {
	case .Loxy:
		if it := item_create(w, .Plant_Fiber); it != 0 { give_item(w, c, it) }
	case .Rat:
		if it := item_create(w, .Rat_Corpse); it != 0 { give_item(w, c, it) }
	case .Scarecrow:
		if roll_dice(&w.rng, 1, 5, 5) > 0 { // 1d5/5: a one in five chance
			if it := item_create(w, .Straw_Hat); it != 0 { give_item(w, c, it) }
		}
	case .Olive_Glop:
		ch.stats.jools = i32(roll_dice(&w.rng, 3, 6, 6))
	case .Cherry_Glop:
		ch.stats.jools = i32(roll_dice(&w.rng, 6, 6, 6))
	}
}

// Create a character in a cell and run its initializer (what the original's CreateCharacter did).
spawn_character :: proc(w: ^World, type: Character_Type, cell: Cell_Id) -> Char_Id {
	c := char_create(w, type, cell)
	if c != 0 { init_character(w, c) }
	return c
}

// Loxy starts in a random tenable town cell with no one in it.
spawn_loxy :: proc(w: ^World) {
	candidates: [TOWN_SIZE * TOWN_SIZE]Cell_Id
	n := 0
	for row in 0 ..< TOWN_SIZE {
		for col in 0 ..< TOWN_SIZE {
			id := cell_at(.Town, col, row)
			if terrain_flag(w, id, .Tenable) && w.cells[id].first_char == 0 { candidates[n] = id; n += 1 }
		}
	}
	w.avatar = spawn_character(w, .Loxy, candidates[rng_range(&w.rng, 0, n - 1)])
}

// ---- stitching ---------------------------------------------------------------------------------------------------------------
teleport_to :: proc(w: ^World, m: Map_Id, col, row: int, dest: Cell_Id) {
	set_effect(w, m, col, row, {kind = .Teleport, dest = dest})
}

stitch_maps :: proc(w: ^World) {
	mid :: ROOM_SIZE / 2
	last :: ROOM_SIZE - 1
	// each building: the town door cell -> inside; the room's door cell -> outside the town door
	pair :: proc(w: ^World, room: Map_Id, town_door: [2]int, room_in, room_door: [2]int, town_out: [2]int) {
		teleport_to(w, .Town, town_door.x, town_door.y, cell_at(room, room_in.x, room_in.y))
		teleport_to(w, room, room_door.x, room_door.y, cell_at(.Town, town_out.x, town_out.y))
	}
	pair(w, .Potter, {13, 13}, {mid, last - 1}, {mid, last}, {13, 12})
	pair(w, .Energy_Trainer, {6, 6}, {mid, 1}, {mid, 0}, {6, 5})
	pair(w, .Inn, {3, 3}, {mid, last - 1}, {mid, last}, {3, 2})
	pair(w, .Druid_House, {10, 10}, {1, mid}, {0, mid}, {9, 10})
	pair(w, .Health_Trainer, {8, 4}, {mid, last - 1}, {mid, last}, {8, 3})
	pair(w, .Healer, {3, 13}, {mid, last - 1}, {mid, last}, {4, 13})
	// inn stairs <-> cellar stairs
	inn_stairs, cellar_stairs := cell_at(.Inn, 1, 5), cell_at(.Cellar, 1, 5)
	w.effects[w.cells[inn_stairs].effect].dest = cellar_stairs
	w.effects[w.cells[cellar_stairs].effect].dest = inn_stairs
	// wilderness <-> town
	wild_town := NO_CELL
	for i in 0 ..< WILDERNESS_SIZE * WILDERNESS_SIZE {
		id := Cell_Id(map_offset(.Wilderness) + i)
		if w.cells[id].terrain == .Town { wild_town = id; break }
	}
	assert(wild_town != NO_CELL, "the wilderness has no town tile")
	teleport_to(w, .Wilderness, cell_pos(wild_town).col, cell_pos(wild_town).row, cell_at(.Town, TOWN_SIZE / 2, TOWN_SIZE - 2))
	teleport_to(w, .Town, TOWN_SIZE / 2, 0, wild_town)
	teleport_to(w, .Town, TOWN_SIZE / 2, TOWN_SIZE - 1, wild_town)
	teleport_to(w, .Town, 0, TOWN_SIZE / 2, wild_town)
	teleport_to(w, .Town, TOWN_SIZE - 1, TOWN_SIZE / 2, wild_town)
}
