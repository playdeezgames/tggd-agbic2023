#+build !js
package tests

// Compares the port's screens with frames recorded from the original VB game (docs/reference/vb, made by tools/oracle.sh).
// A reference file is 216 lines of 384 hue digits (hex).

import "core:testing"
import "bq:game"

parse_reference :: proc(text: string, out: ^game.Hue_Frame) -> bool {
	i := 0
	for ch in transmute([]u8)text {
		d: int
		switch ch {
		case '0' ..= '9': d = int(ch - '0')
		case 'A' ..= 'F': d = int(ch - 'A') + 10
		case '\n', '\r': continue
		case: return false
		}
		if i >= len(out) { return false }
		out[i] = game.Hue(d)
		i += 1
	}
	return i == len(out)
}

// Counts the pixels that differ and reports the first one.
expect_frame :: proc(t: ^testing.T, name: string, got, want: ^game.Hue_Frame, loc := #caller_location) {
	diff, first := 0, -1
	for i in 0 ..< len(got) {
		if got[i] != want[i] {
			diff += 1
			if first < 0 { first = i }
		}
	}
	testing.expectf(t, diff == 0, "%s: %d pixels differ (first at x=%d y=%d: got hue %v, want %v)", name, diff,
		first % game.FRAME_WIDTH, first / game.FRAME_WIDTH, got[max(first, 0)], want[max(first, 0)], loc = loc)
}

new_core :: proc() -> ^game.Core {
	core := new(game.Core)
	game.core_init(core, game.Services{})
	return core
}

step :: proc(core: ^game.Core, commands: ..game.Command) -> game.Step_Output {
	out: game.Step_Output
	events := make([dynamic]game.Input_Event, context.temp_allocator)
	for c in commands { append(&events, game.Input_Event{kind = .Command, command = c}) }
	game.core_step(core, {events = events[:]}, &out)
	return out
}

@(test)
splash_matches_vb :: proc(t: ^testing.T) {
	want := new(game.Hue_Frame)
	defer free(want)
	testing.expect(t, parse_reference(#load("../../docs/reference/vb/splash.txt", string), want), "splash.txt is malformed")
	core := new_core()
	defer free(core)
	out := step(core)
	testing.expect(t, out.frame_changed, "the first step must draw")
	expect_frame(t, "splash", &core.hues, want)
	// the RGBA frame is the hue frame through the palette
	for i in 0 ..< len(core.hues) {
		if out.frame[i] != game.PALETTE[core.hues[i]] { testing.fail_now(t, "RGBA frame does not match the hue frame"); }
	}
}
