#+build !js
package tests

// Saves: round trips, rejection of bad files, a fuzz run, and the screens that use them (slots, settings, export and import).

import "base:intrinsics"
import "core:mem"
import "core:strings"
import "core:testing"
import "bq:game"

// A fake platform storage (the tests run one at a time).
store: map[string]string
downloaded: string
file_requested: bool

fake_get :: proc(key: string, allocator: mem.Allocator) -> (string, bool) {
	v, ok := store[key]
	if !ok { return "", false }
	return strings.clone(v, allocator), true
}
fake_set :: proc(key, value: string) -> bool { store[strings.clone(key)] = strings.clone(value); return true }
fake_remove :: proc(key: string) { delete_key(&store, key) }
fake_download :: proc(filename, text: string) { downloaded = strings.clone(text) }
fake_request :: proc() { file_requested = true }

reset_store :: proc() {
	store = make(map[string]string)
	downloaded = ""
	file_requested = false
}

platform :: proc() -> game.Services {
	return {storage_get = fake_get, storage_set = fake_set, storage_remove = fake_remove, download_text = fake_download, request_file = fake_request}
}

new_core_with :: proc(s: game.Services) -> ^game.Core {
	core := new(game.Core)
	game.core_init(core, s)
	return core
}

press :: proc(core: ^game.Core, cmds: ..game.Command) {
	for c in cmds { step(core, c) }
}

// Play a little, deterministically, so a world has items, wear, a fight or two and changed ground.
play_a_bit :: proc(w: ^game.World, seed: u64, steps: int) {
	r: game.Rng
	game.rng_seed(&r, seed)
	for f in game.Char_Flag { game.set_flag(w, w.avatar, f, true) }
	w.chars[w.avatar].stats.max_health = 30
	for _ in 0 ..< steps {
		a := w.avatar
		if game.is_dead(w, a) { game.set_health(w, a, 30) }
		if w.chars[a].stats.energy < 2 { game.add_energy(w, a, 10) }
		if game.msg_has(w) { game.msg_dismiss(w); continue }
		if game.combat_exists(w) { game.combat_attack(w, 0); continue }
		switch game.rng_range(&r, 0, 5) {
		case 0 ..= 2: d := game.RUN_DELTAS[game.rng_range(&r, 0, 3)]; game.move(w, a, d.x, d.y)
		case 3: if game.forage_can(w) { game.forage_one(w) }
		case 4:
			type := game.Item_Type(game.rng_range(&r, 1, len(game.Item_Type) - 1))
			if it := game.item_create(w, type); it != 0 { game.give_item(w, a, it) }
		case 5:
			stacks: [32]game.Stack
			n := game.inventory_stacks(w, stacks[:])
			if n > 0 { game.drop_items(w, stacks[game.rng_range(&r, 0, n - 1)].type, 2) }
		}
	}
	game.msg_clear(w)
	w.sfx_count = 0
}

@(test)
a_save_round_trips :: proc(t: ^testing.T) {
	for seed in u64(1) ..= 6 {
		a := new_world(seed)
		b := new(game.World)
		defer { free(a); free(b) }
		play_a_bit(a, seed, 3000)
		text1, ok1 := game.world_to_save_text(a, context.temp_allocator)
		testing.expect(t, ok1, "saved")
		ok, why := game.world_from_save_text(b, text1)
		testing.expectf(t, ok, "loaded: %s", why)
		validated(t, b)
		text2, _ := game.world_to_save_text(b, context.temp_allocator)
		testing.expect(t, text1 == text2, "saving what was loaded gives the same text")
		// the parts a player sees
		testing.expect_value(t, b.chars[b.avatar].stats, a.chars[a.avatar].stats)
		testing.expect_value(t, b.chars[b.avatar].cell, a.chars[a.avatar].cell)
		testing.expect_value(t, b.item_count, a.item_count)
		for i in 0 ..< game.TOTAL_CELLS {
			if a.cells[i].terrain != b.cells[i].terrain || a.cells[i].forage_remaining != b.cells[i].forage_remaining || a.cells[i].effect != b.cells[i].effect {
				testing.fail_now(t, "a cell differs after loading")
			}
		}
		// and play goes on the same way: the same moves from the same random state
		play_a_bit(a, 99, 300)
		play_a_bit(b, 99, 300)
		ta, _ := game.world_to_save_text(a, context.temp_allocator)
		tb, _ := game.world_to_save_text(b, context.temp_allocator)
		testing.expect(t, ta == tb, "a loaded game plays on exactly like the original")
		free_all(context.temp_allocator)
	}
}

@(test)
a_save_is_small :: proc(t: ^testing.T) {
	w := new_world(3)
	defer free(w)
	text, _ := game.world_to_save_text(w, context.temp_allocator)
	testing.expectf(t, len(text) < 20000, "a fresh game saves in %d bytes", len(text))
}

@(test)
bad_saves_are_refused_and_change_nothing :: proc(t: ^testing.T) {
	w := new_world(3)
	live := new_world(8)
	defer { free(w); free(live) }
	good, _ := game.world_to_save_text(w, context.temp_allocator)
	core := new_core_with({})
	defer free(core)
	game.world_new(&core.world, 8)
	core.world_ready = true
	before := new(game.World)
	defer free(before)
	intrinsics.mem_copy(before, &core.world, size_of(game.World))
	Case :: struct { name, text: string }
	cases := make([dynamic]Case, context.temp_allocator)
	swap :: proc(text, from, to: string) -> string { r, _ := strings.replace(text, from, to, 1, context.temp_allocator); return r }
	append(&cases, Case{"empty", ""})
	append(&cases, Case{"not json", "hello"})
	append(&cases, Case{"truncated", good[:len(good) / 2]})
	append(&cases, Case{"wrong format", swap(good, "\"bagel-quest\"", "\"other\"")})
	append(&cases, Case{"wrong version", swap(good, "\"version\":1", "\"version\":2")})
	append(&cases, Case{"other content", swap(good, "\"content\":\"", "\"content\":\"0")})
	append(&cases, Case{"unknown item", swap(good, "\"Plant_Fiber\"", "\"Gold_Bar\"")})
	append(&cases, Case{"no avatar", swap(good, "\"avatar\":1", "\"avatar\":0")})
	append(&cases, Case{"short terrain", swap(good, "\"terrain\":\"", "\"terrain\":\"x")})
	for c in cases {
		ok, _ := game.load_save_text(core, c.text)
		testing.expectf(t, !ok, "%s must be refused", c.name)
		testing.expectf(t, mem.compare_ptrs(&core.world, before, size_of(game.World)) == 0, "%s must change nothing", c.name)
	}
	ok, why := game.load_save_text(core, good)
	testing.expectf(t, ok, "the good save loads: %s", why)
}

// Flip, drop, duplicate and swap bytes of a good save; every result is refused or a valid world, never a crash.
@(test)
fuzzed_saves_never_break_the_world :: proc(t: ^testing.T) {
	w := new_world(5)
	defer free(w)
	play_a_bit(w, 5, 500)
	good, _ := game.world_to_save_text(w, context.temp_allocator)
	r: game.Rng
	game.rng_seed(&r, 4242)
	scratch := new(game.World)
	defer free(scratch)
	accepted := 0
	for i in 0 ..< 1500 {
		data := make([]u8, len(good), context.temp_allocator)
		copy(data, good)
		for _ in 0 ..< game.rng_range(&r, 1, 6) {
			pos := game.rng_range(&r, 0, len(data) - 1)
			switch game.rng_range(&r, 0, 3) {
			case 0: data[pos] = u8(game.rng_range(&r, 32, 126))
			case 1: data[pos] = '0' + u8(game.rng_range(&r, 0, 9))
			case 2: data[pos] = data[game.rng_range(&r, 0, len(data) - 1)]
			case 3: if pos + 12 < len(data) { for k in 0 ..< 12 { data[pos + k] = ' ' } }
			}
		}
		ok, _ := game.world_from_save_text(scratch, string(data))
		if ok {
			accepted += 1
			good_world, why := game.world_validate(scratch)
			testing.expectf(t, good_world, "an accepted mutation must still be valid: %s", why)
		}
		if i % 100 == 0 { free_all(context.temp_allocator); /* the good text lives there too: rebuild */ }
		if i % 100 == 0 { good, _ = game.world_to_save_text(w, context.temp_allocator) }
	}
	testing.expectf(t, accepted < 1500, "some mutations must be refused (%d accepted)", accepted)
}

@(test)
slots_through_the_screens :: proc(t: ^testing.T) {
	reset_store()
	core := new_core_with(platform())
	defer free(core)
	press(core, .Confirm, .Confirm) // splash, embark
	game.teleport(&core.world, core.world.avatar, game.cell_at(.Town, 8, 8))
	core.world.chars[core.world.avatar].stats.jools = 77
	// Game Menu -> Save... -> Slot 2
	press(core, .Cancel, .Down, .Down, .Confirm, .Down, .Confirm)
	_, saved := store["bq:slot2"]
	testing.expect(t, saved, "slot 2 holds the save")
	testing.expect_value(t, game.ui_top(core), game.UI_State.Game_Menu)
	// Abandon, then Load... shows the slot and loads it
	core.world.chars[core.world.avatar].stats.jools = 1
	press(core, .Down, .Down, .Down, .Down, .Down, .Confirm, .Down, .Confirm) // Abandon Game, Yes
	testing.expect_value(t, game.ui_top(core), game.UI_State.Main_Menu)
	press(core, .Down, .Down, .Confirm) // Load...
	testing.expect_value(t, core.ui.pick.count, 2) // Slot 2 and Import...
	testing.expect_value(t, game.text_str(&core.ui.pick.items[0].text), "Slot 2")
	press(core, .Confirm)
	testing.expect_value(t, game.ui_top(core), game.UI_State.Navigation)
	testing.expect_value(t, core.world.chars[core.world.avatar].stats.jools, 77)
	// a damaged slot is refused with the failure sound, and we are back at the main menu
	store["bq:slot2"] = "{broken"
	press(core, .Cancel, .Down, .Down, .Down, .Down, .Down, .Confirm, .Down, .Confirm)
	press(core, .Down, .Down, .Confirm, .Confirm)
	testing.expect_value(t, game.ui_top(core), game.UI_State.Main_Menu)
}

@(test)
scum_save_and_load :: proc(t: ^testing.T) {
	reset_store()
	core := new_core_with(platform())
	defer free(core)
	press(core, .Confirm, .Confirm)
	core.world.chars[core.world.avatar].stats.jools = 31
	press(core, .Cancel, .Down, .Confirm) // Game Menu -> Scum Save
	_, saved := store["bq:slot0"]
	testing.expect(t, saved, "the quick save is slot 0")
	core.world.chars[core.world.avatar].stats.jools = 2
	press(core, .Cancel, .Down, .Down, .Down, .Confirm) // Scum Load
	testing.expect_value(t, core.world.chars[core.world.avatar].stats.jools, 31)
	testing.expect_value(t, game.ui_top(core), game.UI_State.Navigation)
}

@(test)
settings_are_remembered :: proc(t: ^testing.T) {
	reset_store()
	core := new_core_with(platform())
	press(core, .Confirm, .Down, .Down, .Down, .Confirm) // Options (web: Full Screen, Sfx, Mux)
	press(core, .Down, .Confirm)                         // Sfx Volume...
	press(core, .Down, .Down, .Confirm)                  // 50% -> 70%
	testing.expect_value(t, core.ui.sfx_tenths, 7)
	free(core)
	core2 := new_core_with(platform())
	defer free(core2)
	testing.expect_value(t, core2.ui.sfx_tenths, 7)
	testing.expect_value(t, core2.ui.mux_tenths, 3)
	// junk in the config keeps the defaults
	store["bq:config"] = "{\"sfx\":99,\"mux\":-4,\"fullscreen\":true,\"scale\":7}"
	core3 := new_core_with(platform())
	defer free(core3)
	testing.expect_value(t, core3.ui.sfx_tenths, 5)
	testing.expect_value(t, core3.ui.mux_tenths, 3)
	testing.expect_value(t, core3.ui.window_scale, 3)
	testing.expect(t, core3.ui.fullscreen, "full screen was valid")
}

@(test)
export_and_import_a_save_file :: proc(t: ^testing.T) {
	reset_store()
	core := new_core_with(platform())
	defer free(core)
	press(core, .Confirm, .Confirm)
	core.world.chars[core.world.avatar].stats.jools = 55
	press(core, .Cancel, .Down, .Down, .Confirm) // Game Menu -> Save...
	press(core, .Up, .Confirm)                   // the last entry: Export...
	testing.expect(t, len(downloaded) > 100, "a file was offered")
	// wreck the game, abandon, import the file at the Load screen
	core.world.chars[core.world.avatar].stats.jools = 3
	press(core, .Cancel, .Down, .Down, .Down, .Down, .Down, .Confirm, .Down, .Confirm)
	press(core, .Down, .Down, .Confirm) // Load...
	testing.expect_value(t, game.text_str(&core.ui.pick.items[0].text), "No Saves Exist!")
	press(core, .Down, .Confirm) // Import...
	testing.expect(t, file_requested, "a file picker was requested")
	// a bad file first
	out: game.Step_Output
	bad := transmute([]u8)string("nope")
	game.core_step(core, {events = {{kind = .File, data = bad}}}, &out)
	testing.expect_value(t, game.ui_top(core), game.UI_State.Load)
	testing.expect(t, strings.has_prefix(game.text_str(&core.ui.pick.header), "Import failed"), "the failure is shown")
	press(core, .Confirm) // Import... again
	game.core_step(core, {events = {{kind = .File, data = transmute([]u8)downloaded}}}, &out)
	testing.expect_value(t, game.ui_top(core), game.UI_State.Navigation)
	testing.expect_value(t, core.world.chars[core.world.avatar].stats.jools, 55)
	// a file nobody asked for is ignored
	game.core_step(core, {events = {{kind = .File, data = transmute([]u8)downloaded}}}, &out)
	testing.expect_value(t, game.ui_top(core), game.UI_State.Navigation)
}
