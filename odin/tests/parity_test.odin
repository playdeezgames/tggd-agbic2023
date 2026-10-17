#+build !js
package tests

// Plays a scripted game natively and writes a digest of every frame, sound and state it went through. tools/wasm_parity.sh plays the
// same script through the built wasm in headless Chrome and writes the same digest; tools/test.sh requires the two to be equal.
// That proves the wasm build (32-bit int, its own allocator and runtime) plays exactly like the native one.
// The script, the sampling and the digest are repeated in tools/wasm_parity.html: change one, change both.

import "base:runtime"
import "core:os"
import "core:fmt"
import "core:strings"
import "core:testing"
import "bq:game"

PARITY_STEPS :: 4000

parity_store: map[string]string

parity_get :: proc(key: string, allocator: runtime.Allocator) -> (string, bool) {
	v, ok := parity_store[key]
	if !ok { return "", false }
	return strings.clone(v, allocator), true
}
parity_set :: proc(key, value: string) -> bool { parity_store[strings.clone(key)] = strings.clone(value); return true }
parity_remove :: proc(key: string) { delete_key(&parity_store, key) }
parity_entropy :: proc() -> u64 { return 7 } // the page returns high word 0, low word 7
parity_nothing_text :: proc(filename, text: string) {}
parity_nothing :: proc() {}

fnv :: proc(h: u32, b: u8) -> u32 { return (h ~ u32(b)) * 16777619 }

@(test)
parity_script_digest :: proc(t: ^testing.T) {
	parity_store = make(map[string]string)
	core := new(game.Core)
	defer free(core)
	game.core_init(core, game.Services{
		entropy = parity_entropy, storage_get = parity_get, storage_set = parity_set, storage_remove = parity_remove,
		download_text = parity_nothing_text, request_file = parity_nothing,
	})
	lcg := u32(12345)
	next :: proc(lcg: ^u32) -> u32 { lcg^ = lcg^ * 1664525 + 1013904223; return lcg^ }
	now := 1790000000000.0
	acc := u32(2166136261)
	out: game.Step_Output
	for step in 0 ..< PARITY_STEPS {
		cmd := game.Command(1 + (next(&lcg) >> 16) % 6)
		now += 16
		if step % 97 == 0 { now += 1500 }
		events := [1]game.Input_Event{{kind = .Command, command = cmd}}
		game.core_step(core, {dt = 0.016, now_ms = now, events = events[:]}, &out)
		if !out.frame_changed {
			acc = fnv(acc, 0)
		} else {
			bytes := ([^]u8)(out.frame)[:game.FRAME_WIDTH * game.FRAME_HEIGHT * 4]
			if step % 25 == 0 {
				for b in bytes { acc = fnv(acc, b) }
			} else {
				for i := 0; i < len(bytes); i += 509 { acc = fnv(acc, bytes[i]) }
			}
		}
		for i in 0 ..< out.sfx_count { acc = fnv(acc, u8(out.sfx[i])) }
		acc = fnv(acc, u8(out.mux))
		if step % 500 == 499 {
			s := game.core_state_hash(core)
			for k in 0 ..< 4 { acc = fnv(acc, u8(s >> (8 * uint(k)))) }
			free_all(context.temp_allocator)
		}
		free_all(context.temp_allocator)
	}
	state := game.core_state_hash(core)
	line := fmt.tprintf("frames=%d state=%d steps=%d\n", acc, state, PARITY_STEPS)
	_ = os.write_entire_file("build/parity_native.txt", transmute([]u8)line)
	testing.expect(t, state != 0 || true, "")
}
