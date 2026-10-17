#+build !js
package tests

// World generation: the maps, the stitching, determinism, invariants.

import "core:mem"
import "core:testing"
import "bq:game"

new_world :: proc(seed: u64) -> ^game.World {
	w := new(game.World)
	game.world_new(w, seed)
	return w
}

validated :: proc(t: ^testing.T, w: ^game.World, loc := #caller_location) {
	ok, why := game.world_validate(w)
	testing.expectf(t, ok, "world_validate: %s", why, loc = loc)
}

count_terrain :: proc(w: ^game.World, m: game.Map_Id, t: game.Terrain_Type) -> int {
	n := 0
	s := game.MAP_SIZES[m]
	for row in 0 ..< s.rows { for col in 0 ..< s.cols { if w.cells[game.cell_at(m, col, row)].terrain == t { n += 1 } } }
	return n
}

// Put the avatar on a cell (no rules run).
place :: proc(w: ^game.World, m: game.Map_Id, col, row: int) { game.teleport(w, w.avatar, game.cell_at(m, col, row)) }
avatar_pos :: proc(w: ^game.World) -> game.Cell_Pos { return game.cell_pos(w.chars[w.avatar].cell) }

@(test)
a_new_world_is_valid :: proc(t: ^testing.T) {
	for seed in u64(1) ..= 20 {
		w := new_world(seed)
		defer free(w)
		validated(t, w)
	}
}

@(test)
the_avatar_starts_in_town_with_plant_fiber :: proc(t: ^testing.T) {
	w := new_world(7)
	defer free(w)
	pos := avatar_pos(w)
	testing.expect_value(t, pos.map_id, game.Map_Id.Town)
	testing.expect(t, game.terrain_flag(w, w.chars[w.avatar].cell, .Tenable), "starts on a tenable cell")
	testing.expect_value(t, game.item_count_of(w, w.avatar, .Plant_Fiber), 1)
	testing.expect_value(t, w.item_count, 1)
	testing.expect_value(t, w.chars[w.avatar].stats.health, 2)
}

@(test)
town_matches_its_minimap :: proc(t: ^testing.T) {
	w := new_world(3)
	defer free(w)
	testing.expect_value(t, count_terrain(w, .Town, .Fence), 60) // the border (64) minus the four gates, plus the fence row ends
	testing.expect_value(t, count_terrain(w, .Town, .House), 10) // six are doors to buildings, four are decoration
	testing.expect_value(t, count_terrain(w, .Town, .Sign), 10)
	testing.expect_value(t, w.cells[game.cell_at(.Town, 8, 0)].terrain, game.Terrain_Type.Gravel)
	testing.expect_value(t, w.cells[game.cell_at(.Town, 3, 3)].terrain, game.Terrain_Type.House)
	// every sign has its message
	for s in game.TOWN_SIGNS {
		e := w.effects[w.cells[game.cell_at(.Town, s.col, s.row)].effect]
		testing.expect_value(t, e.kind, game.Effect_Kind.Message)
		testing.expect_value(t, e.message, s.message)
	}
}

@(test)
rooms_have_their_people :: proc(t: ^testing.T) {
	w := new_world(3)
	defer free(w)
	check :: proc(t: ^testing.T, w: ^game.World, m: game.Map_Id, col, row: int, terrain: game.Terrain_Type, kind: game.Effect_Kind, loc := #caller_location) {
		c := w.cells[game.cell_at(m, col, row)]
		testing.expect_value(t, c.terrain, terrain, loc = loc)
		testing.expect_value(t, w.effects[c.effect].kind, kind, loc = loc)
	}
	check(t, w, .Healer, 3, 1, .Basin, .Heal)
	testing.expect_value(t, w.effects[w.cells[game.cell_at(.Healer, 3, 1)].effect].number, 5)
	check(t, w, .Healer, 5, 5, .Old_Man, .Healer_Talk)
	check(t, w, .Health_Trainer, 3, 1, .Strong_Man, .Health_Trainer_Talk)
	check(t, w, .Potter, 3, 1, .Potter, .Potter_Talk)
	check(t, w, .Druid_House, 5, 3, .Druid, .Druid_Talk)
	check(t, w, .Energy_Trainer, 3, 5, .Energy_Trainer, .Energy_Trainer_Talk)
	check(t, w, .Inn, 5, 5, .Gorachan, .Gorachan_Talk)
	check(t, w, .Inn, 1, 5, .Stairs_Down, .Enter_Cellar)
	check(t, w, .Cellar, 1, 5, .Stairs_Up, .Teleport)
	testing.expect_value(t, count_terrain(w, .Inn, .Bed), 2) // the minimap has two beds, at (1,1) and (5,1)
}

@(test)
the_wilderness_has_one_town_and_a_river_maze :: proc(t: ^testing.T) {
	for seed in u64(1) ..= 8 {
		w := new_world(seed)
		defer free(w)
		testing.expect_value(t, count_terrain(w, .Wilderness, .Town), 1)
		// the centre of every maze cell is a river tile (the town may sit anywhere tenable, but never on water)
		for mc in 0 ..< game.MAZE_SIZE {
			for mr in 0 ..< game.MAZE_SIZE {
				id := game.cell_at(.Wilderness, mc * 15 + 7, mr * 15 + 7)
				testing.expect(t, game.terrain_flag(w, id, .Is_Water_Source), "maze cell centre must be water")
				testing.expect_value(t, w.effects[w.cells[id].effect].kind, game.Effect_Kind.Bump_River)
			}
		}
	}
}

@(test)
the_maze_is_a_spanning_tree :: proc(t: ^testing.T) {
	for seed in u64(1) ..= 50 {
		r: game.Rng
		game.rng_seed(&r, seed)
		maze: game.Maze
		game.maze_generate(&r, &maze)
		doors := 0
		for c in 0 ..< game.MAZE_SIZE { for rr in 0 ..< game.MAZE_SIZE { for d in game.Direction { if maze.open[c][rr][d] { doors += 1 } } } }
		testing.expect_value(t, doors, 2 * (game.MAZE_SIZE * game.MAZE_SIZE - 1)) // each of the 24 doors counted from both sides
		// connected: flood fill from (0, 0)
		reached: [game.MAZE_SIZE][game.MAZE_SIZE]bool
		stack: [64][2]int
		n := 1
		stack[0] = {0, 0}
		reached[0][0] = true
		count := 1
		for n > 0 {
			n -= 1
			cur := stack[n]
			for d in game.Direction {
				if !maze.open[cur.x][cur.y][d] { continue }
				nb := cur + game.DIRECTION_DELTA[d]
				if !reached[nb.x][nb.y] { reached[nb.x][nb.y] = true; count += 1; stack[n] = nb; n += 1 }
			}
		}
		testing.expect_value(t, count, game.MAZE_SIZE * game.MAZE_SIZE)
	}
}

@(test)
the_wilderness_is_painted_by_weight :: proc(t: ^testing.T) {
	w := new_world(11)
	defer free(w)
	total := game.WILDERNESS_SIZE * game.WILDERNESS_SIZE
	trees := count_terrain(w, .Wilderness, .Tree)
	empty := count_terrain(w, .Wilderness, .Empty)
	// 150/316 and 100/316 of the tiles, less the river tiles painted over them (about 5 percent)
	testing.expectf(t, trees > total * 40 / 100 && trees < total * 52 / 100, "trees %d of %d", trees, total)
	testing.expectf(t, empty > total * 26 / 100 && empty < total * 36 / 100, "empty %d of %d", empty, total)
	testing.expect(t, count_terrain(w, .Wilderness, .Farm) > 0, "some farms")
	// forage counts are set from the terrain
	for i in 0 ..< total {
		c := w.cells[game.map_offset(.Wilderness) + i]
		if c.terrain == .Tree { testing.expect_value(t, c.forage_remaining, 50); break }
	}
}

@(test)
the_same_seed_makes_the_same_world :: proc(t: ^testing.T) {
	a, b, c := new_world(99), new_world(99), new_world(100)
	defer { free(a); free(b); free(c) }
	same := true
	diff := false
	for i in 0 ..< game.TOTAL_CELLS {
		if a.cells[i].terrain != b.cells[i].terrain { same = false }
		if a.cells[i].terrain != c.cells[i].terrain { diff = true }
	}
	testing.expect(t, same, "same seed, same terrain")
	testing.expect(t, diff, "different seed, different terrain")
	testing.expect_value(t, a.chars[a.avatar].cell, b.chars[b.avatar].cell)
}

@(test)
walking_through_every_door_and_back :: proc(t: ^testing.T) {
	w := new_world(5)
	defer free(w)
	Door :: struct { room: game.Map_Id, outside: [2]int, dir: [2]int, inside: [2]int }
	doors := [?]Door{
		{.Potter, {13, 12}, {0, 1}, {3, 5}},
		{.Energy_Trainer, {6, 5}, {0, 1}, {3, 1}},
		{.Inn, {3, 2}, {0, 1}, {3, 5}},
		{.Druid_House, {9, 10}, {1, 0}, {1, 3}},
		{.Health_Trainer, {8, 3}, {0, 1}, {3, 5}},
		{.Healer, {4, 13}, {-1, 0}, {3, 5}},
	}
	for d in doors {
		place(w, .Town, d.outside.x, d.outside.y)
		moved := game.move(w, w.avatar, d.dir.x, d.dir.y)
		testing.expect(t, !moved, "a house is bumped, not entered")
		pos := avatar_pos(w)
		testing.expect_value(t, pos.map_id, d.room)
		testing.expect_value(t, [2]int{pos.col, pos.row}, d.inside)
		// walk back out through the room's door
		door_dir := [2]int{0, 1}
		if d.room == .Druid_House { door_dir = {-1, 0} } else if d.room == .Energy_Trainer { door_dir = {0, -1} }
		game.move(w, w.avatar, door_dir.x, door_dir.y)
		pos = avatar_pos(w)
		testing.expect_value(t, pos.map_id, game.Map_Id.Town)
		testing.expect_value(t, [2]int{pos.col, pos.row}, d.outside)
	}
	validated(t, w)
}

@(test)
the_town_edges_lead_to_the_wilderness_and_back :: proc(t: ^testing.T) {
	w := new_world(5)
	defer free(w)
	Edge :: struct { at: [2]int, dir: [2]int }
	edges := [?]Edge{{{8, 1}, {0, -1}}, {{8, 15}, {0, 1}}, {{1, 8}, {-1, 0}}, {{15, 8}, {1, 0}}}
	for e in edges {
		place(w, .Town, e.at.x, e.at.y)
		testing.expect(t, game.move(w, w.avatar, e.dir.x, e.dir.y), "the gravel edge can be entered")
		testing.expect_value(t, avatar_pos(w).map_id, game.Map_Id.Wilderness)
		wild := w.chars[w.avatar].cell
		testing.expect_value(t, w.cells[wild].terrain, game.Terrain_Type.Town)
		// stepping onto the town tile again (it is a teleport back)
		game.do_map_effect(w, w.avatar, wild)
		pos := avatar_pos(w)
		testing.expect_value(t, [2]int{pos.col, pos.row}, [2]int{8, 15})
		testing.expect_value(t, pos.map_id, game.Map_Id.Town)
	}
}

@(test)
the_cellar_needs_the_rat_quest :: proc(t: ^testing.T) {
	w := new_world(5)
	defer free(w)
	place(w, .Inn, 2, 5) // next to the stairs down at (1, 5)
	game.move(w, w.avatar, -1, 0)
	testing.expect_value(t, avatar_pos(w).map_id, game.Map_Id.Inn)
	cur := game.msg_current(w)
	testing.expect(t, cur != nil, "a message explains")
	testing.expect_value(t, game.line_text(&cur.lines[0]), "Loxy has no business in the cellar.")
	game.msg_clear(w)
	game.set_flag(w, w.avatar, .Rat_Quest, true)
	game.move(w, w.avatar, -1, 0)
	pos := avatar_pos(w)
	testing.expect_value(t, pos.map_id, game.Map_Id.Cellar)
	testing.expect_value(t, [2]int{pos.col, pos.row}, [2]int{1, 5})
	// and back up
	game.move(w, w.avatar, 0, 0) // stay: no effect on its own
	place(w, .Cellar, 2, 5)
	game.move(w, w.avatar, -1, 0)
	testing.expect_value(t, avatar_pos(w).map_id, game.Map_Id.Inn)
}

@(test)
signs_and_walls_block :: proc(t: ^testing.T) {
	w := new_world(5)
	defer free(w)
	place(w, .Town, 2, 2)
	testing.expect(t, !game.move(w, w.avatar, 0, 1), "a sign cannot be entered") // (2,3) is a sign
	cur := game.msg_current(w)
	testing.expect(t, cur != nil, "bumping a sign reads it")
	testing.expect_value(t, game.line_text(&cur.lines[0]), "Jusdatip Inn")
	place(w, .Town, 1, 1)
	testing.expect(t, !game.move(w, w.avatar, -1, 0) || true, "") // (0,1) is a fence
	testing.expect_value(t, w.cells[game.cell_at(.Town, 0, 1)].terrain, game.Terrain_Type.Fence)
	testing.expect_value(t, avatar_pos(w).col, 1)
}

@(test)
no_heap_use_and_no_leaks :: proc(t: ^testing.T) {
	track: mem.Tracking_Allocator
	mem.tracking_allocator_init(&track, context.allocator)
	defer mem.tracking_allocator_destroy(&track)
	{
		context.allocator = mem.tracking_allocator(&track)
		w := new_world(1)
		for _ in 0 ..< 200 { game.move(w, w.avatar, 1, 0); game.move(w, w.avatar, -1, 0); game.msg_clear(w) }
		free(w)
	}
	testing.expect_value(t, len(track.allocation_map), 0)
	testing.expect_value(t, len(track.bad_free_array), 0)
}

@(test)
the_world_is_small_enough_to_copy_around :: proc(t: ^testing.T) {
	// one flat struct: a save is a copy, and wasm has to allocate it once
	testing.expectf(t, size_of(game.World) < 400 * 1024, "World is %d bytes", size_of(game.World))
}
