package game

import "base:runtime"

// The platform interface: everything the portable core exchanges with a platform.
// The core never imports a platform package; platforms never reach into core state.
//
//   platform -> core:  Input_Event list per frame, dt, the wall clock, and Services supplied once at init
//   core -> platform:  Step_Output per frame (pixels, quit flag)
//
// Same shape as Murder Hobo and Kordanor's Cabal (vault: Tech/Odin wasm recipe.md).

// The original's 384 by 216 view, shown with square pixels.
FRAME_WIDTH :: 384
FRAME_HEIGHT :: 216

// The six commands of the original (AOS.UI Command: A, B, Up, Down, Left, Right).
Command :: enum u8 { None, Up, Down, Left, Right, Confirm, Cancel }

Input_Kind :: enum u8 {
	None,
	Command, // a key or gamepad button, already mapped
	File,    // the player chose a file to import: its text is in `data` (valid for this step only)
	Tap,     // a mouse press or touch: x, y are FRAME pixel coordinates (may be outside the frame)
	         // `precise` is true for a mouse or pen; a finger is not precise (first tap selects, a tap on the selection confirms)
}

Input_Event :: struct {
	kind:    Input_Kind,
	command: Command,
	x, y:    i16,
	precise: bool,
	data:    []u8, // File only
}

Step_Output :: struct {
	frame:          ^[FRAME_WIDTH * FRAME_HEIGHT]u32, // bytes in memory order R,G,B,A (little-endian u32 0xAABBGGRR)
	frame_changed:  bool,
	quit_requested: bool, // desktop only: the web build shows a closing screen instead
	// sound: effects to play now (at the volume below) and the music wanted (the platform switches only when it changes)
	sfx:            [MAX_SFX]Sfx_Id,
	sfx_count:      int,
	mux:            Mux_Id,
	sfx_volume:     f32, // 0 to 1
	mux_volume:     f32,
	// desktop only: what the Options menu asks for
	fullscreen:     bool,
	window_scale:   int, // the window is the view times this
}

Step_Input :: struct {
	dt:     f64, // seconds since the previous step (presentation only)
	now_ms: f64, // wall clock, milliseconds since the Unix epoch; f64 because int is 32 bits on wasm
	events: []Input_Event,
}

// Platform functions the core calls synchronously. Supplied once to core_init, so the core has no platform imports
// and tests can pass fakes. Storage joins this struct in phase 6 (saves).
Services :: struct {
	entropy:        proc() -> u64, // for seeding; not reproducible
	log:            proc(message: string),
	desktop:        bool,          // true on the native client
	// storage for saves and settings (nil when the platform has none: the Save and Load screens then show no slots)
	storage_get:    proc(key: string, allocator: runtime.Allocator) -> (value: string, ok: bool),
	storage_set:    proc(key, value: string) -> bool, // false on quota or when storage is unavailable
	storage_remove: proc(key: string),
	// export and import of a save as a file (nil when the platform cannot): the Save and Load screens add an entry for each
	download_text:  proc(filename, text: string),
	request_file:   proc(), // opens a file picker; the choice arrives later as an Input_Event of kind File
}

// core_step is called once per presented frame; it must not block, must not keep pointers into `input` after returning,
// and fills `out` completely every call.
