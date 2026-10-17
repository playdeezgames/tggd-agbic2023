package game

import "base:intrinsics"
import "core:encoding/json"
import "core:fmt"

// The boilerplate states of AOS.UI: splash, main menu, options, volume, window size, save and load, confirmations, about.

// What a menu entry means (the value stored with it).
Mi :: enum i32 {
	None, Embark, Scum_Load, Load, Options, About, Quit, Continue_Game, Scum_Save, Save, Abandon_Game,
	Toggle_Full_Screen, Window_Size, Sfx_Volume, Mux_Volume, No, Yes,
	// the action menu and the craft menu
	Ground, Inventory, Equipment, Sleep, Forage, Craft, Put_Out_Fire, Statistics,
	Make_Twine, Build_Fire, Make_Torch, Build_Furnace, Cook_Bagel, Knap, Make_Hatchet,
	// inventory detail
	Drop_All, Drop_Half, Drop_One, Equip_It,
	// saves as files
	Export, Import,
}

WINDOW_SCALES := [9]int{3, 4, 5, 9, 10, 14, 15, 19, 20}
EFFECT_VALUE_BASE :: 1000 // inventory detail: an item verb is stored as this plus its Effect_Kind

// ---- save slots and settings (Services storage; slot 0 is the "scum" quick save) -------------------------------------------------------------------
SETTINGS_KEY :: "bq:config"
SAVE_FILENAME :: "bagel-quest-save.json"

slot_key :: proc(slot: int) -> string { return fmt.tprintf("bq:slot%d", slot) }

slot_text :: proc(core: ^Core, slot: int) -> (string, bool) {
	if core.services.storage_get == nil { return "", false }
	return core.services.storage_get(slot_key(slot), context.temp_allocator)
}

slot_exists :: proc(core: ^Core, slot: int) -> bool {
	_, ok := slot_text(core, slot)
	return ok
}

slot_save :: proc(core: ^Core, slot: int) {
	if core.services.storage_set == nil || !core.world_ready { return }
	if text, ok := world_to_save_text(&core.world, context.temp_allocator); ok { core.services.storage_set(slot_key(slot), text) }
}

// Replace the live world by the one in `text`, or change nothing and say why.
load_save_text :: proc(core: ^Core, text: string) -> (ok: bool, why: string) {
	scratch := new(World)
	defer free(scratch)
	ok, why = world_from_save_text(scratch, text)
	if !ok { return }
	intrinsics.mem_copy(&core.world, scratch, size_of(World))
	core.world_ready = true
	core.ui.enemy_index = 0
	return true, ""
}

slot_load :: proc(core: ^Core, slot: int) -> bool {
	text, ok := slot_text(core, slot)
	if !ok { return false }
	good, _ := load_save_text(core, text)
	return good
}

Settings_File :: struct { sfx: int, mux: int, fullscreen: bool, scale: int }

save_settings :: proc(core: ^Core) {
	if core.services.storage_set == nil { return }
	u := &core.ui
	data, err := json.marshal(Settings_File{u.sfx_tenths, u.mux_tenths, u.fullscreen, u.window_scale}, {}, context.temp_allocator)
	if err == nil { core.services.storage_set(SETTINGS_KEY, string(data)) }
}

// Settings that are missing or out of range keep their defaults.
load_settings :: proc(core: ^Core) {
	if core.services.storage_get == nil { return }
	text, ok := core.services.storage_get(SETTINGS_KEY, context.temp_allocator)
	if !ok { return }
	s: Settings_File
	if json.unmarshal(transmute([]byte)text, &s, .JSON, context.temp_allocator) != nil { return }
	u := &core.ui
	if s.sfx >= 0 && s.sfx <= 10 { u.sfx_tenths = s.sfx }
	if s.mux >= 0 && s.mux <= 10 { u.mux_tenths = s.mux }
	u.fullscreen = s.fullscreen
	for m in WINDOW_SCALES { if m == s.scale { u.window_scale = m } }
}

// ---- starting each state ---------------------------------------------------------------------------------------------------------------------------
start_menu :: proc(core: ^Core, s: UI_State) {
	u := &core.ui
	#partial switch s {
	case .Main_Menu:
		p := picker_begin(core, "Main Menu", "Select", "Quit")
		pick_add(p, i32(Mi.Embark), "Embark!")
		pick_add(p, i32(Mi.Scum_Load), "Scum Load")
		pick_add(p, i32(Mi.Load), "Load...")
		pick_add(p, i32(Mi.Options), "Options...")
		pick_add(p, i32(Mi.About), "About...")
		pick_add(p, i32(Mi.Quit), "Quit")
	case .Game_Menu:
		p := picker_begin(core, "Game Menu", "Select", "Cancel")
		pick_add(p, i32(Mi.Continue_Game), "Continue Game")
		pick_add(p, i32(Mi.Scum_Save), "Scum Save")
		pick_add(p, i32(Mi.Save), "Save...")
		pick_add(p, i32(Mi.Scum_Load), "Scum Load")
		pick_add(p, i32(Mi.Options), "Options...")
		pick_add(p, i32(Mi.Abandon_Game), "Abandon Game")
	case .Options:
		p := picker_begin(core, "Options", "Select", "Cancel")
		pick_add(p, i32(Mi.Toggle_Full_Screen), "Toggle Full Screen")
		if core.services.desktop { pick_add(p, i32(Mi.Window_Size), "Window Size...") } // meaningless in a browser tab
		pick_add(p, i32(Mi.Sfx_Volume), "Sfx Volume...")
		pick_add(p, i32(Mi.Mux_Volume), "Mux Volume...")
	case .Window_Size:
		p := picker_begin(core, "", "Select", "Cancel")
		text_set(&p.header, "Current Size: %dx%d", FRAME_WIDTH * u.window_scale, FRAME_HEIGHT * u.window_scale)
		for m, i in WINDOW_SCALES {
			pick_add(p, i32(m), "%dx%d", FRAME_WIDTH * m, FRAME_HEIGHT * m)
			if m == u.window_scale { p.index = i }
		}
	case .Sfx_Volume:
		p := picker_begin(core, "", "Select", "Cancel")
		text_set(&p.header, "Sfx Volume (Currently: %d%%)", u.sfx_tenths * 10)
		for i in 0 ..= 10 { pick_add(p, i32(i), "%d%%", i * 10) }
		p.index = u.sfx_tenths
	case .Mux_Volume:
		p := picker_begin(core, "", "Select", "Cancel")
		text_set(&p.header, "Mux Volume (Currently: %d%%)", u.mux_tenths * 10)
		for i in 0 ..= 10 { pick_add(p, i32(i), "%d%%", i * 10) }
		p.index = u.mux_tenths
	case .Load:
		p := picker_begin(core, "Load Game", "Select", "Cancel")
		for slot in 1 ..= 5 { if slot_exists(core, slot) { pick_add(p, i32(slot), "Slot %d", slot) } }
		if p.count == 0 { pick_add(p, 0, "No Saves Exist!") }
		if core.services.request_file != nil { pick_add(p, i32(Mi.Import) * -1, "Import...") }
	case .Save:
		p := picker_begin(core, "Save Game", "Select", "Cancel")
		for slot in 1 ..= 5 { pick_add(p, i32(slot), "Slot %d%s", slot, "(will overwrite)" if slot_exists(core, slot) else "") }
		if core.services.download_text != nil { pick_add(p, i32(Mi.Export) * -1, "Export...") }
	case .Confirm_Quit:
		p := picker_begin(core, "Are you sure you want to quit the game?", "Select", "Cancel")
		pick_add(p, i32(Mi.No), "No")
		pick_add(p, i32(Mi.Yes), "Yes")
	case .Confirm_Abandon:
		p := picker_begin(core, "Are you sure you want to abandon the game?", "Select", "Cancel")
		pick_add(p, i32(Mi.No), "No")
		pick_add(p, i32(Mi.Yes), "Yes")
	}
}

// ---- activating an entry --------------------------------------------------------------------------------------------------------------------------
activate_menu :: proc(core: ^Core, s: UI_State, item: Pick_Item) {
	u := &core.ui
	#partial switch s {
	case .Main_Menu:
		#partial switch Mi(item.value) {
		case .Quit: set_state(core, .Confirm_Quit)
		case .Options: set_states(core, .Options, .Main_Menu)
		case .About: set_state(core, .About)
		case .Embark: set_state(core, .Embark)
		case .Load: set_state(core, .Load)
		case .Scum_Load: set_state(core, .Scum_Load_Main_Menu)
		}
	case .Game_Menu:
		#partial switch Mi(item.value) {
		case .Continue_Game: set_state(core, .Neutral)
		case .Scum_Save: set_state(core, .Scum_Save)
		case .Scum_Load: set_state(core, .Scum_Load_Game_Menu)
		case .Save: set_state(core, .Save)
		case .Options: set_states(core, .Options, .Game_Menu)
		case .Abandon_Game: set_state(core, .Confirm_Abandon)
		}
	case .Options:
		#partial switch Mi(item.value) {
		case .Toggle_Full_Screen: u.fullscreen = !u.fullscreen; save_settings(core)
		case .Window_Size: set_state(core, .Window_Size)
		case .Sfx_Volume: set_state(core, .Sfx_Volume)
		case .Mux_Volume: set_state(core, .Mux_Volume)
		}
	case .Window_Size:
		u.window_scale = int(item.value)
		save_settings(core)
		text_set(&u.pick.header, "Current Size: %dx%d", FRAME_WIDTH * u.window_scale, FRAME_HEIGHT * u.window_scale)
	case .Sfx_Volume:
		u.sfx_tenths = int(item.value)
		ui_play_sfx(core, .Sfx_Volume_Test)
		save_settings(core)
		text_set(&u.pick.header, "Sfx Volume (Currently: %d%%)", u.sfx_tenths * 10)
	case .Mux_Volume:
		u.mux_tenths = int(item.value)
		u.mux = .Main_Theme
		save_settings(core)
		text_set(&u.pick.header, "Mux Volume (Currently: %d%%)", u.mux_tenths * 10)
	case .Load:
		if item.value == -i32(Mi.Import) {
			u.awaiting_import = true
			core.services.request_file()
		} else if item.value > 0 && slot_load(core, int(item.value)) {
			set_state(core, .Neutral)
		} else {
			ui_play_sfx(core, .Failure)
			set_state(core, .Main_Menu)
		}
	case .Save:
		if item.value == -i32(Mi.Export) {
			if text, ok := world_to_save_text(&core.world, context.temp_allocator); ok { core.services.download_text(SAVE_FILENAME, text) }
		} else {
			slot_save(core, int(item.value))
			set_state(core, .Game_Menu)
		}
	case .Confirm_Quit:
		if Mi(item.value) == .No { set_state(core, .Main_Menu) } else { set_state(core, .None) }
	case .Confirm_Abandon:
		if Mi(item.value) == .No {
			set_state(core, .Game_Menu)
		} else {
			core.world_ready = false // AbandonGame
			set_state(core, .Main_Menu)
		}
	}
}

// ---- splash and about --------------------------------------------------------------------------------------------------------------------------------
ABOUT_LINES := [?]struct { line: int, text: string, hue: Hue }{
	{0, "About Bagel Quest", .Orange}, {2, "Art:", .White}, {3, "https://kenney.nl/assets/1-bit-pack", .White},
	{5, "A Production of TheGrumpyGameDev", .White}, {7, "For A Game By Its Cover 2023", .White}
}

render_about :: proc(core: ^Core) {
	fill_all(&core.hues, .Black)
	for l in ABOUT_LINES { draw_ui_text(&core.hues, 0, UI_FONT_CELL * l.line, l.text, l.hue) }
}
