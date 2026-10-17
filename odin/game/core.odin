package game

import "base:intrinsics"

// The portable core. One call to core_step per presented frame.

Core :: struct {
	services:    Services,
	ui:          UI,
	world:       World,
	world_ready: bool,                     // false until Embark; false again after Abandon, death or a win
	embark_count: u64,                     // seeds the world when the platform gives no entropy (tests)
	now_ms:      f64,
	hues:        Hue_Frame,                // the picture as hue indexes (what the tests compare with the VB frames)
	prev_hues:   Hue_Frame,
	frame:       [FRAME_WIDTH * FRAME_HEIGHT]u32, // the same picture as RGBA, what the platform shows
	drawn:       bool,
}

core_init :: proc(core: ^Core, services: Services) {
	intrinsics.mem_zero(core, size_of(Core)) // not `core^ = {}`: no chance of a large temporary on the wasm stack
	core.services = services
	ui_init(core)
	load_settings(core)
	core.ui.mux = .Main_Theme // the splash starts the music on its first render
}

core_step :: proc(core: ^Core, input: Step_Input, out: ^Step_Output) {
	core.now_ms = input.now_ms
	for e in input.events {
		#partial switch e.kind {
		case .Command: ui_command(core, e.command)
		case .File: ui_file(core, e.data)
		}
	}
	// the web build has no window to close: the end of the main menu's Quit is a closing screen
	if core.ui.depth == 0 && !core.services.desktop { set_state(core, .Closed) }

	ui_render(core)
	out.frame_changed = !core.drawn || core.hues != core.prev_hues
	if out.frame_changed {
		to_rgba(&core.hues, &core.frame)
		core.prev_hues = core.hues
		core.drawn = true
	}
	out.frame = &core.frame

	// sounds: those states asked for, then those the rules queued (picking up, levelling up)
	out.sfx_count = 0
	for i in 0 ..< core.ui.pending_count { out.sfx[out.sfx_count] = core.ui.pending_sfx[i]; out.sfx_count += 1 }
	core.ui.pending_count = 0
	if core.world_ready && out.sfx_count < MAX_SFX {
		queued: [MAX_SFX]Sfx_Id
		n := sfx_drain(&core.world, queued[:])
		for i in 0 ..< n { if out.sfx_count < MAX_SFX { out.sfx[out.sfx_count] = queued[i]; out.sfx_count += 1 } }
	}
	out.mux = core.ui.mux
	out.sfx_volume = f32(core.ui.sfx_tenths) / 10
	out.mux_volume = f32(core.ui.mux_tenths) / 10
	out.quit_requested = core.ui.depth == 0 && core.services.desktop
	out.fullscreen = core.ui.fullscreen
	out.window_scale = core.ui.window_scale
}

// BagelQuestContext.ShowSplashContent: the title in the 8 by 8 font centred on black, a one pixel tan outline (the eight
// neighbours, drawn first) under brown fill, and the status bar "Space/(A) - Continue".
SPLASH_TITLE :: "Bagel Quest"
splash_outline := [8][2]int{{-1, -1}, {0, -1}, {1, -1}, {-1, 0}, {1, 0}, {-1, 1}, {0, 1}, {1, 1}}

draw_splash :: proc(f: ^Hue_Frame) {
	fill_all(f, .Black)
	x := FRAME_WIDTH / 2 - ui_text_width(SPLASH_TITLE) / 2
	y := FRAME_HEIGHT / 2 - UI_FONT_CELL / 2
	for d in splash_outline { draw_ui_text(f, x + d.x, y + d.y, SPLASH_TITLE, .Tan) }
	draw_ui_text(f, x, y, SPLASH_TITLE, .Brown)
	draw_status_bar(f, controls_text("Continue", ""), .Black, .Light_Gray)
}

// FNV-1a over the saved form of the world (0 when there is no game): one number that says whether two runs ended in the same state.
core_state_hash :: proc(core: ^Core) -> u32 {
	if !core.world_ready { return 0 }
	text, ok := world_to_save_text(&core.world, context.temp_allocator)
	if !ok { return 0 }
	h := u32(2166136261)
	for b in transmute([]u8)text { h = (h ~ u32(b)) * 16777619 }
	return h
}
