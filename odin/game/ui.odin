package game

// The screens: a port of AOS.UI's controller and boilerplate states and BQ.Presentation's game states.
// The original keeps a stack of named states; each has HandleCommand, Render and OnStart. Action states (a move, a drop, a
// craft) do their work in OnStart and immediately switch to another state. The same shape is kept here so the sequences of
// OnStart calls (which re-build menus and reset their cursors) match the original.

import "core:fmt"

UI_State :: enum u8 {
	None, // "no state": setting it pops the stack (the original's Nothing)
	Splash, Main_Menu, Confirm_Quit, About, Options, Window_Size, Sfx_Volume, Mux_Volume, Load, Save, Confirm_Abandon, Game_Menu,
	Scum_Save, Scum_Load_Main_Menu, Scum_Load_Game_Menu, Embark,
	Neutral, Navigation, Action_Menu, Craft, Ground, Ground_Detail, Inventory, Inventory_Detail, Equipment, Equip, Forage,
	Combat, Message, Statistics, Dead, Winner,
	// action states: they act in on_start and switch away at once
	Move_Up, Move_Down, Move_Left, Move_Right, Take, Drop, Sleep, Build_Fire, Build_Furnace, Cook_Bagel, Knap, Make_Hatchet,
	Make_Torch, Make_Twine, Put_Out_Fire, Run,
	Closed, // the web build's end of the game (there is no window to close)
}

// ---- text in fixed buffers -------------------------------------------------------------------------------------
Text :: struct { len: u8, buf: [64]u8 }

text_set :: proc(t: ^Text, format: string, args: ..any) {
	s := fmt.bprintf(t.buf[:], format, ..args)
	t.len = u8(len(s))
}
text_str :: proc(t: ^Text) -> string { return string(t.buf[:t.len]) }

// ---- pickers: the original's BasePickerState -----------------------------------------------------------------------------
PICK_MAX :: 32

Pick_Item :: struct { text: Text, value: i32 }

Picker :: struct {
	header: Text,
	status: Text,
	items:  [PICK_MAX]Pick_Item,
	count:  int,
	index:  int,
}

pick_add :: proc(p: ^Picker, value: i32, format: string, args: ..any) {
	if p.count >= PICK_MAX { return }
	text_set(&p.items[p.count].text, format, ..args)
	p.items[p.count].value = value
	p.count += 1
}

// ---- state data ------------------------------------------------------------------------------------------------------------------------
Forage_Ui :: struct {
	cols, rows:       int,
	cur_col, cur_row: int,
	hidden:           [MAX_FORAGE_GRID]bool,
	found:            [MAX_FORAGE_GRID]Item_Type, // what each revealed cell gave
	loot:             [MAX_FORAGE_GRID]Item_Type, // everything found, in order
	loot_count:       int,
}
MAX_FORAGE_GRID :: 64 // 50 finds at most (a tree or a quarry), laid out about 8 by 7

Inventory_Ui :: struct {
	stacks:     [32]Stack,
	count:      int,
	cols, rows: int,
	cur:        int,
}

UI :: struct {
	stack:          [4]UI_State,
	depth:          int,
	start_enabled:  bool,
	pick:           Picker,
	// settings (saved in phase 6)
	sfx_tenths:     int, // 0 to 10
	mux_tenths:     int,
	fullscreen:     bool,
	window_scale:   int, // desktop: the window is the view times this
	// the music wanted now (the platform switches only when it changes)
	mux:            Mux_Id,
	// screens
	choice_index:   int, // Message
	inv:            Inventory_Ui,
	sel_type:       Item_Type, // the stack chosen in the inventory or on the ground (the original's Item.Name)
	sel_count:      int,
	forage:         Forage_Ui,
	enemy_index:    int,   // Combat
	menu_index:     int,
	show_until:     f64,   // Dead and Winner: input is ignored until then (ms)
	pending_sfx:    [MAX_SFX]Sfx_Id, // sounds asked for by states (the platform plays them after the step)
	pending_count:  int,
	awaiting_import: bool, // the Load screen asked for a file and is waiting for it
}

ui_play_sfx :: proc(core: ^Core, id: Sfx_Id) {
	if id == .None || core.ui.pending_count >= MAX_SFX { return }
	core.ui.pending_sfx[core.ui.pending_count] = id
	core.ui.pending_count += 1
}

ui_top :: proc(core: ^Core) -> UI_State {
	if core.ui.depth == 0 { return .None }
	return core.ui.stack[core.ui.depth - 1]
}

// ---- the state stack (BaseGameController) ---------------------------------------------------------------------------------------
pop_state :: proc(core: ^Core) {
	u := &core.ui
	if u.depth > 0 { u.depth -= 1 }
	if u.depth > 0 && u.start_enabled { on_start(core, u.stack[u.depth - 1]) }
}

set_current :: proc(core: ^Core, state: UI_State, push: bool) {
	u := &core.ui
	if !push { pop_state(core) }
	if state != .None {
		assert(u.depth < len(u.stack), "state stack overflow")
		u.stack[u.depth] = state
		u.depth += 1
		if u.start_enabled { on_start(core, state) }
	}
}

set_state :: proc(core: ^Core, state: UI_State) { set_current(core, state, false) }

// Replace the current state by `next` without starting it, then push `pushed` (the original's SetStates).
set_states :: proc(core: ^Core, pushed, next: UI_State) {
	core.ui.start_enabled = false
	set_current(core, next, false)
	core.ui.start_enabled = true
	set_current(core, pushed, true)
}

ui_init :: proc(core: ^Core) {
	u := &core.ui
	u^ = {}
	u.start_enabled = true
	u.sfx_tenths = 5
	u.mux_tenths = 3
	u.window_scale = 3
	set_state(core, .Splash)
}

// ---- the picker's own commands and drawing ----------------------------------------------------------------------------------------------
// The state a picker goes to on Cancel (the original passes it to the constructor).
picker_cancel :: proc(s: UI_State) -> UI_State {
	#partial switch s {
	case .Main_Menu: return .Confirm_Quit
	case .Confirm_Quit: return .Main_Menu
	case .Options: return .None
	case .Window_Size, .Sfx_Volume, .Mux_Volume: return .Options
	case .Load: return .Main_Menu
	case .Save: return .Game_Menu
	case .Confirm_Abandon: return .Game_Menu
	case .Game_Menu: return .Neutral
	case .Action_Menu: return .Neutral
	case .Craft: return .Action_Menu
	case .Ground: return .Action_Menu
	case .Ground_Detail: return .Ground
	case .Inventory_Detail: return .Inventory
	case .Equipment: return .Action_Menu
	case .Equip: return .Inventory_Detail
	}
	return .None
}

is_picker :: proc(s: UI_State) -> bool {
	#partial switch s {
	case .Main_Menu, .Confirm_Quit, .Options, .Window_Size, .Sfx_Volume, .Mux_Volume, .Load, .Save, .Confirm_Abandon, .Game_Menu,
	     .Action_Menu, .Craft, .Ground, .Ground_Detail, .Inventory_Detail, .Equipment, .Equip:
		return true
	}
	return false
}

picker_command :: proc(core: ^Core, s: UI_State, cmd: Command) {
	p := &core.ui.pick
	#partial switch cmd {
	case .Cancel: set_state(core, picker_cancel(s))
	case .Confirm: if p.count > 0 { picker_activate(core, s, p.items[p.index]) }
	case .Up: if p.count > 0 { p.index = (p.index + p.count - 1) % p.count }
	case .Down: if p.count > 0 { p.index = (p.index + 1) % p.count }
	}
}

// BasePickerState.Render: black screen, a blue bar through the middle, the list scrolled so the selection sits on the bar,
// then the header (orange on black) and the status bar over it.
picker_render :: proc(core: ^Core) {
	f := &core.hues
	p := &core.ui.pick
	fill_all(f, .Black)
	fill_rect(f, 0, FRAME_HEIGHT / 2 - UI_FONT_CELL / 2, FRAME_WIDTH, UI_FONT_CELL, .Blue)
	y := FRAME_HEIGHT / 2 - UI_FONT_CELL / 2 - p.index * UI_FONT_CELL
	for i in 0 ..< p.count {
		text := text_str(&p.items[i].text)
		hue := Hue.Black if i == p.index else Hue.Blue
		draw_ui_text(f, FRAME_WIDTH / 2 - ui_text_width(text) / 2, y, text, hue)
		y += UI_FONT_CELL
	}
	draw_header(f, text_str(&p.header), .Orange, .Black)
	draw_status_bar(f, text_str(&p.status), .Black, .Light_Gray)
}

// The common start of a picker: cursor at the top, menu rebuilt (the state's own code then fills the items).
picker_begin :: proc(core: ^Core, header: string, status_a, status_b: string) -> ^Picker {
	p := &core.ui.pick
	p^ = {}
	text_set(&p.header, "%s", header)
	text_set(&p.status, "%s", controls_text(status_a, status_b))
	return p
}

centered_text :: proc(f: ^Hue_Frame, cx, y: int, text: string, hue: Hue) -> int {
	draw_ui_text(f, cx - ui_text_width(text) / 2, y, text, hue)
	return y + UI_FONT_CELL
}
