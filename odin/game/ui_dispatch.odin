package game

// The switches that route the controller's calls to the right state.

on_start :: proc(core: ^Core, s: UI_State) {
	u := &core.ui
	#partial switch s {
	case .Splash:
	case .Main_Menu, .Game_Menu, .Options, .Window_Size, .Sfx_Volume, .Mux_Volume, .Load, .Save, .Confirm_Quit, .Confirm_Abandon:
		start_menu(core, s)
	case .Embark:
		seed := core.services.entropy() if core.services.entropy != nil else core.embark_count * 7919 + 1
		core.embark_count += 1
		world_new(&core.world, seed)
		core.world_ready = true
		u.enemy_index = 0
		set_state(core, .Neutral)
	case .Scum_Save:
		slot_save(core, 0)
		set_state(core, .Neutral)
	case .Scum_Load_Main_Menu, .Scum_Load_Game_Menu:
		if slot_load(core, 0) {
			set_state(core, .Neutral)
		} else {
			set_state(core, .Main_Menu if s == .Scum_Load_Main_Menu else .Game_Menu)
		}
	case .Neutral: neutral_start(core)
	case .Navigation: u.mux = .Main_Theme
	case .Action_Menu: start_action_menu(core)
	case .Craft: start_craft(core)
	case .Ground: start_ground(core)
	case .Ground_Detail: start_ground_detail(core)
	case .Inventory: start_inventory(core)
	case .Inventory_Detail: start_inventory_detail(core)
	case .Equipment: start_equipment(core)
	case .Equip: start_equip(core)
	case .Forage: start_forage(core)
	case .Combat: combat_start(core)
	case .Message: message_start(core)
	case .Dead:
		u.mux = .Death_Theme
		u.show_until = core.now_ms + END_SCREEN_DELAY_MS
	case .Winner:
		u.mux = .Victory_Theme
		u.show_until = core.now_ms + END_SCREEN_DELAY_MS
		ui_play_sfx(core, .Woo_Hoo)
	case .Move_Up, .Move_Down, .Move_Left, .Move_Right, .Take, .Drop, .Sleep, .Build_Fire, .Build_Furnace, .Cook_Bagel, .Knap,
	     .Make_Hatchet, .Make_Torch, .Make_Twine, .Put_Out_Fire, .Run:
		run_action(core, s)
	}
}

picker_activate :: proc(core: ^Core, s: UI_State, item: Pick_Item) {
	#partial switch s {
	case .Main_Menu, .Game_Menu, .Options, .Window_Size, .Sfx_Volume, .Mux_Volume, .Load, .Save, .Confirm_Quit, .Confirm_Abandon:
		activate_menu(core, s, item)
	case:
		activate_game_menu(core, s, item)
	}
}

ui_command :: proc(core: ^Core, cmd: Command) {
	s := ui_top(core)
	if is_picker(s) { picker_command(core, s, cmd); return }
	#partial switch s {
	case .Splash: if cmd == .Confirm { set_state(core, .Main_Menu) }
	case .About: set_state(core, .Main_Menu)
	case .Navigation: navigation_command(core, cmd)
	case .Inventory: inventory_command(core, cmd)
	case .Forage: forage_command(core, cmd)
	case .Combat: combat_command(core, cmd)
	case .Message: message_command(core, cmd)
	case .Statistics: if cmd == .Confirm || cmd == .Cancel { set_state(core, .Action_Menu) }
	case .Dead, .Winner: end_screen_command(core, cmd)
	case .Closed: if cmd == .Confirm { ui_init(core); load_settings(core) }
	}
}

ui_render :: proc(core: ^Core) {
	s := ui_top(core)
	if is_picker(s) { picker_render(core); return }
	#partial switch s {
	case .Splash: draw_splash(&core.hues)
	case .About: render_about(core)
	case .Navigation: render_navigation(core)
	case .Inventory: render_inventory(core)
	case .Forage: render_forage(core)
	case .Combat: render_combat(core)
	case .Message: render_message(core)
	case .Statistics: render_statistics(core)
	case .Dead: render_end_screen(core, "Yer dead!", .Red, "Continue")
	case .Winner: render_end_screen(core, "You Win!", .Light_Green, "Main Menu")
	case .Closed:
		fill_all(&core.hues, .Black)
		centered_text(&core.hues, FRAME_WIDTH / 2, FRAME_HEIGHT / 2 - UI_FONT_CELL, "Thanks for playing!", .Orange)
		centered_text(&core.hues, FRAME_WIDTH / 2, FRAME_HEIGHT / 2 + UI_FONT_CELL, "You can close this tab.", .Light_Gray)
		draw_status_bar(&core.hues, controls_text("Play again", ""), .Black, .Light_Gray)
	case: fill_all(&core.hues, .Black) // a state that never renders (the original throws)
	}
}

// A file the player chose for the Load screen's Import entry.
ui_file :: proc(core: ^Core, data: []u8) {
	u := &core.ui
	if !u.awaiting_import || ui_top(core) != .Load { return }
	u.awaiting_import = false
	if ok, why := load_save_text(core, string(data)); ok {
		set_state(core, .Neutral)
	} else {
		ui_play_sfx(core, .Failure)
		text_set(&u.pick.header, "Import failed: %s", why)
	}
}
