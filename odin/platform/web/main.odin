#+build js
package main

// Browser platform (js_wasm32). odin.js runs `main` once (init); then page/platform.js calls `platform_frame` every
// animation frame. Everything crossing the boundary is a number, a (ptr, len) string, or a pointer into wasm memory.
import "base:runtime"
import "bq:game"

foreign import platform_env "platform"

@(default_calling_convention = "contextless")
foreign platform_env {
	js_entropy_u32 :: proc() -> u32 ---
	js_log :: proc(message: string) ---
	js_download_text :: proc(filename, text: string) --- // offers a file to save
	js_request_file :: proc() ---                        // shows a file chooser; the choice comes back through platform_file
}

core: ^game.Core // allocated once at start; never on the stack
out: game.Step_Output
events: [64]game.Input_Event
event_count: int

push_event :: proc "contextless" (e: game.Input_Event) {
	if event_count < len(events) { events[event_count] = e; event_count += 1 }
}

main :: proc() {
	core = new(game.Core)
	game.core_init(core, game.Services{
		entropy = proc() -> u64 {
			high := js_entropy_u32() // two statements: the order of two calls in one expression is not guaranteed
			low := js_entropy_u32()
			return u64(high) << 32 | u64(low)
		},
		log = proc(message: string) { js_log(message) },
		storage_get = storage_get, storage_set = storage_set, storage_remove = storage_remove,
		download_text = proc(filename, text: string) { js_download_text(filename, text) },
		request_file = proc() { js_request_file() },
	})
}

// ---- exports called by page/platform.js ---------------------------------------------------------
@(export) platform_command :: proc "c" (command: i32) { push_event({kind = .Command, command = game.Command(command)}) }
@(export) platform_tap :: proc "c" (x, y: i32, precise: bool) { push_event({kind = .Tap, x = i16(x), y = i16(y), precise = precise}) }
// now_ms is the wall clock in milliseconds (an f64: int is 32 bits here)
@(export) platform_frame :: proc "c" (dt, now_ms: f64) {
	context = runtime.default_context()
	if core == nil { return }
	game.core_step(core, {dt = dt, now_ms = now_ms, events = events[:event_count]}, &out)
	event_count = 0
	if file_buf != nil { delete(file_buf); file_buf = nil }
	free_all(context.temp_allocator) // the core builds its strings there
}
// A chosen file: the page asks for a buffer, copies the bytes in, then announces it. The buffer is freed after the step.
file_buf: []u8
@(export) platform_alloc :: proc "c" (n: i32) -> rawptr {
	context = runtime.default_context()
	if file_buf != nil { delete(file_buf) }
	file_buf = make([]u8, int(n))
	return raw_data(file_buf)
}
@(export) platform_file :: proc "c" (n: i32) { push_event({kind = .File, data = file_buf[:int(n)]}) }
// what the step asked for in sound and window terms (see page/platform.js)
@(export) platform_sfx_count :: proc "c" () -> i32 { return i32(out.sfx_count) }
@(export) platform_sfx :: proc "c" (i: i32) -> i32 { return i32(out.sfx[i]) }
@(export) platform_mux :: proc "c" () -> i32 { return i32(out.mux) }
@(export) platform_sfx_volume :: proc "c" () -> f32 { return out.sfx_volume }
@(export) platform_mux_volume :: proc "c" () -> f32 { return out.mux_volume }
@(export) platform_fullscreen :: proc "c" () -> bool { return out.fullscreen }
// a number that says what state the game is in (QA and the parity test)
@(export) platform_state_hash :: proc "c" () -> u32 {
	context = runtime.default_context()
	h := game.core_state_hash(core)
	free_all(context.temp_allocator)
	return h
}
@(export) platform_frame_ptr :: proc "c" () -> rawptr { return out.frame }
@(export) platform_frame_changed :: proc "c" () -> bool { return out.frame_changed }
@(export) platform_quit :: proc "c" () -> bool { return out.quit_requested }
