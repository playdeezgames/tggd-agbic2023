#+build !js
package tests

// The screens against the original: replay docs/reference/scenes.txt on the port and compare every shot with the frame the VB
// game drew (docs/reference/vb, recorded by tools/oracle.sh). Both sides read the same scene file.

import "core:fmt"
import "core:os"
import "core:strings"
import "core:testing"
import "bq:game"

SCENES :: #load("../../docs/reference/scenes.txt", string)
VB_FRAMES := #load_directory("../../docs/reference/vb")

// "RockQuarry" for Rock_Quarry: the VB names have no underscores.
no_underscores :: proc(s: string) -> string {
	b := strings.builder_make(context.temp_allocator)
	for ch in s { if ch != '_' { strings.write_rune(&b, ch) } }
	return strings.to_string(b)
}

by_name :: proc($E: typeid, name: string) -> (E, bool) {
	for v in E { if no_underscores(fmt.tprintf("%v", v)) == name { return v, true } }
	return {}, false
}

must :: proc($E: typeid, name: string) -> E {
	v, ok := by_name(E, name)
	if !ok { panic(fmt.tprintf("scene names an unknown %v: %s", typeid_of(E), name)) }
	return v
}

command_of :: proc(name: string) -> game.Command {
	switch name {
	case "Up": return .Up
	case "Down": return .Down
	case "Left": return .Left
	case "Right": return .Right
	case "A": return .Confirm
	case "B": return .Cancel
	}
	panic(fmt.tprintf("unknown command %s", name))
}

Runner :: struct {
	core:  ^game.Core,
	now:   f64,
	t:     ^testing.T,
	scene: string,
}

runner_step :: proc(r: ^Runner, commands: ..game.Command) {
	events := make([dynamic]game.Input_Event, context.temp_allocator)
	for c in commands { append(&events, game.Input_Event{kind = .Command, command = c}) }
	out: game.Step_Output
	game.core_step(r.core, {dt = 0.016, now_ms = r.now, events = events[:]}, &out)
	r.now += 16
}

runner_new_scene :: proc(r: ^Runner, name: string) {
	if r.core != nil { free(r.core) }
	r.core = new(game.Core)
	game.core_init(r.core, game.Services{desktop = true})
	r.scene = name
	runner_step(r)
}

reference_frame :: proc(name: string) -> (frame: ^game.Hue_Frame, ok: bool) {
	file := fmt.tprintf("%s.txt", name)
	for f in VB_FRAMES {
		if f.name == file {
			frame = new(game.Hue_Frame, context.temp_allocator)
			return frame, parse_reference(string(f.data), frame)
		}
	}
	return nil, false
}

set_stat :: proc(w: ^game.World, name: string, value: i32) {
	s := &w.chars[w.avatar].stats
	switch name {
	case "Health": s.health = value
	case "MaximumHealth": s.max_health = value
	case "Energy": s.energy = value
	case "MaximumEnergy": s.max_energy = value
	case "Jools": s.jools = value
	case "AdvancementPoints": s.ap = value
	case "XP": s.xp = value
	case "XPGoal": s.xp_goal = value
	case "XPLevel": s.xp_level = value
	case "AttackDice": s.attack_dice = value
	case "MaximumAttack": s.max_attack = value
	case "DefendDice": s.defend_dice = value
	case "MaximumDefend": s.max_defend = value
	case: panic(fmt.tprintf("unknown statistic %s", name))
	}
}

atoi :: proc(s: string) -> int {
	n := 0
	for ch in s { n = n * 10 + int(ch - '0') }
	return n
}

// Run one line of the scene file.
run_line :: proc(r: ^Runner, line: string) -> (shot: string) {
	f := strings.fields(line, context.temp_allocator)
	if len(f) == 0 || f[0][0] == '#' { return }
	w := &r.core.world if r.core != nil else nil
	switch f[0] {
	case "scene": runner_new_scene(r, f[1])
	case "keys": for k in f[1:] { runner_step(r, command_of(k)) }
	case "embark": runner_step(r, .Confirm); runner_step(r, .Confirm)
	case "neutral": game.set_state(r.core, .Neutral); runner_step(r)
	case "wait": r.now += f64(atoi(f[1])); runner_step(r)
	case "shot": runner_step(r); return f[1]
	case "avatar": game.teleport(w, w.avatar, game.cell_at(must(game.Map_Id, f[1]), atoi(f[2]), atoi(f[3])))
	case "terrain": w.cells[game.cell_at(must(game.Map_Id, f[1]), atoi(f[2]), atoi(f[3]))].terrain = must(game.Terrain_Type, f[4])
	case "fill":
		m := must(game.Map_Id, f[1])
		for col in atoi(f[2]) ..= atoi(f[4]) { for row in atoi(f[3]) ..= atoi(f[5]) { w.cells[game.cell_at(m, col, row)].terrain = must(game.Terrain_Type, f[6]) } }
	case "give":
		for _ in 0 ..< (atoi(f[2]) if len(f) > 2 else 1) { game.give_item(w, w.avatar, game.item_create(w, must(game.Item_Type, f[1]))) }
	case "ground":
		for _ in 0 ..< (atoi(f[2]) if len(f) > 2 else 1) { game.put_item_on_ground(w, w.chars[w.avatar].cell, game.item_create(w, must(game.Item_Type, f[1]))) }
	case "equip":
		id := game.item_create(w, must(game.Item_Type, f[2]))
		game.give_item(w, w.avatar, id)
		game.equip(w, w.avatar, id)
	case "stat": set_stat(w, f[1], i32(atoi(f[2])))
	case "flag": game.set_flag(w, w.avatar, must(game.Char_Flag, f[1]), f[2] == "1")
	case "forage": w.cells[w.chars[w.avatar].cell].forage_remaining = i16(atoi(f[1]))
	case "enemy": game.spawn_character(w, must(game.Character_Type, f[1]), w.chars[w.avatar].cell)
	case: panic(fmt.tprintf("unknown scene op %s", f[0]))
	}
	return
}

// Where the port deliberately differs from the original: the exact number of differing pixels, and why. A change in the
// number fails the test, so nothing else can hide behind these.
Known_Difference :: struct { shot: string, pixels: int, why: string }
KNOWN_DIFFERENCES := [?]Known_Difference{
	{"healer_talk", 18, "D5: 'And wash' is 'and wash'"},
	{"healer_talk_down", 18, "D5: 'And wash' is 'and wash'"},
	{"basin_heals", 604, "D1: the original shows '{jools}' literally"},
	{"equip_list", 1088, "D16 and D18: the original shows '<placeholder>' and 'System.Object[]' for the item names"},
	{"unequip_list", 592, "D18: the original shows 'System.Object[]' for the item name"},
	{"equipment_list", 1040, "D18: the original shows 'System.Object[]' for items with a durability"},
}

// The frame as hue digits, for tools/frame2png.py.
dump_frame :: proc(name: string, f: ^game.Hue_Frame) {
	os.make_directory("build/screens_failed")
	digits := "0123456789ABCDEF"
	b := strings.builder_make(context.temp_allocator)
	for y in 0 ..< game.FRAME_HEIGHT {
		for x in 0 ..< game.FRAME_WIDTH { strings.write_byte(&b, digits[int(f[y * game.FRAME_WIDTH + x])]) }
		strings.write_byte(&b, '\n')
	}
	_ = os.write_entire_file(fmt.tprintf("build/screens_failed/%s.txt", name), transmute([]u8)strings.to_string(b))
}

@(test)
screens_match_the_original :: proc(t: ^testing.T) {
	r := Runner{t = t}
	defer if r.core != nil { free(r.core) }
	checked, bad := 0, 0
	rest := SCENES
	for line in strings.split_lines_iterator(&rest) {
		shot := run_line(&r, strings.trim_space(line))
		if len(shot) == 0 { continue }
		want, ok := reference_frame(shot)
		if !ok { testing.expectf(t, false, "no reference frame for shot %s", shot); bad += 1; continue }
		checked += 1
		diff, first := 0, -1
		for i in 0 ..< len(r.core.hues) {
			if r.core.hues[i] != want[i] { diff += 1; if first < 0 { first = i } }
		}
		known := -1
		for k in KNOWN_DIFFERENCES { if k.shot == shot { known = k.pixels } }
		if diff != 0 && diff == known { continue }
		if diff != 0 {
			bad += 1
			dump_frame(shot, &r.core.hues)
			testing.expectf(t, false, "%s / %s: %d pixels differ (first at x=%d y=%d: got %v, want %v)", r.scene, shot, diff,
				first % game.FRAME_WIDTH, first / game.FRAME_WIDTH, r.core.hues[first], want[first])
		}
	}
	testing.expectf(t, checked > 80, "only %d shots were checked", checked)
	testing.expectf(t, bad == 0, "%d of %d screens differ from the original", bad, checked + bad)
}
