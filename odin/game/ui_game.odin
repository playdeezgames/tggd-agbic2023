package game

// The game's own states (BQ.Presentation/States).

import "core:fmt"

// Layout constants of Navigation (BQ.Presentation Constants): 16 by 16 cells, the avatar's cell in the middle.
CELL_W :: 16
CELL_H :: 16
CENTER_CELL_X :: FRAME_WIDTH / 2 - CELL_W / 2   // 184
CENTER_CELL_Y :: FRAME_HEIGHT / 2 - CELL_H / 2  // 100
LEFT_COLUMN :: -((CENTER_CELL_X + CELL_W - 1) / CELL_W)       // -12
TOP_ROW :: -((CENTER_CELL_Y + CELL_H - 1) / CELL_H)           // -7
MAP_COLUMNS :: (FRAME_WIDTH - CENTER_CELL_X + CELL_W - 1) / CELL_W - LEFT_COLUMN  // 25
MAP_ROWS :: (FRAME_HEIGHT - CENTER_CELL_Y + CELL_H - 1) / CELL_H - TOP_ROW        // 15

avatar_cell :: proc(core: ^Core) -> Cell_Id { return core.world.chars[core.world.avatar].cell }

// ---- Neutral: decides what to show next ------------------------------------------------------------------------------------------------------
neutral_start :: proc(core: ^Core) {
	w := &core.world
	switch {
	case msg_has(w): set_state(core, .Message)
	case is_dead(w, w.avatar): set_state(core, .Dead)
	case combat_exists(w): set_state(core, .Combat)
	case has_won(w): set_state(core, .Winner)
	case: set_state(core, .Navigation)
	}
}

// ---- action states: work in on_start, then switch away -------------------------------------------------------------------------------------------
run_action :: proc(core: ^Core, s: UI_State) {
	w := &core.world
	a := w.avatar
	next := UI_State.Neutral
	#partial switch s {
	case .Move_Up: move(w, a, 0, -1)
	case .Move_Down: move(w, a, 0, 1)
	case .Move_Left: move(w, a, -1, 0)
	case .Move_Right: move(w, a, 1, 0)
	case .Take: take_items(w, core.ui.sel_type, core.ui.sel_count); next = .Ground
	case .Drop: drop_items(w, core.ui.sel_type, core.ui.sel_count); next = .Inventory
	case .Sleep: sleep(w, a)
	case .Build_Fire: do_terrain_verb(w, .Build_Fire)
	case .Build_Furnace: do_terrain_verb(w, .Build_Furnace)
	case .Cook_Bagel: do_terrain_verb(w, .Cook_Bagel)
	case .Make_Torch: do_terrain_verb(w, .Make_Torch)
	case .Put_Out_Fire: do_terrain_verb(w, .Put_Out_Fire)
	case .Knap: do_make_item(w, a, .Sharp_Rock, "a sharp rock")
	case .Make_Hatchet: do_make_item(w, a, .Hatchet, "a hatchet")
	case .Make_Twine: do_make_item(w, a, .Twine, "twine")
	case .Run: combat_run(w)
	}
	set_state(core, next)
}

// ---- Navigation ----------------------------------------------------------------------------------------------------------------------------------------
navigation_command :: proc(core: ^Core, cmd: Command) {
	#partial switch cmd {
	case .Confirm: set_state(core, .Action_Menu)
	case .Cancel: set_state(core, .Game_Menu)
	case .Up: set_state(core, .Move_Up)
	case .Down: set_state(core, .Move_Down)
	case .Left: set_state(core, .Move_Left)
	case .Right: set_state(core, .Move_Right)
	}
}

render_navigation :: proc(core: ^Core) {
	f := &core.hues
	w := &core.world
	fill_all(f, .Dark_Gray)
	pos := cell_pos(avatar_cell(core))
	for col in LEFT_COLUMN ..< LEFT_COLUMN + MAP_COLUMNS {
		for row in TOP_ROW ..< TOP_ROW + MAP_ROWS {
			id := cell_at(pos.map_id, pos.col + col, pos.row + row)
			if id == NO_CELL { continue }
			px, py := col * CELL_W + CENTER_CELL_X, row * CELL_H + CENTER_CELL_Y
			cell := &w.cells[id]
			fill_rect(f, px, py, CELL_W, CELL_H, .Black)
			t := TERRAINS[cell.terrain]
			draw_tile(f, px, py, t.glyph, t.hue)
			seen: bit_set[Item_Type]
			for it := cell.first_item; it != 0; it = w.items[it].next {
				type := w.items[it].type
				if type in seen { continue }
				seen += {type}
				draw_tile(f, px, py, ITEMS[type].glyph, ITEMS[type].hue)
			}
			if cell.first_char != 0 {
				ch := CHARACTERS[w.chars[cell.first_char].type]
				draw_tile(f, px, py, ch.mask_glyph, ch.mask_hue)
				draw_tile(f, px, py, ch.glyph, ch.hue)
			}
		}
	}
	a := w.avatar
	y := 0
	stat :: proc(f: ^Hue_Frame, y: ^int, text: string, hue: Hue) {
		draw_ui_text(f, 1, y^ + 1, text, .Black)
		draw_ui_text(f, 0, y^, text, hue)
		y^ += UI_FONT_CELL
	}
	stat(f, &y, health_display(w, a), .Pink)
	stat(f, &y, energy_display(w, a), .Blue)
	stat(f, &y, level_display(w, a), .Purple)
	stat(f, &y, xp_display(w, a), .Cyan)
	stat(f, &y, fmt.tprintf(" %s", jools_display(w, a)), .Light_Green) // a leading space, as the original has
	draw_status_bar(f, controls_text("Actions", "Game Menu"), .Black, .Light_Gray)
}

// ---- the Actions and Craft menus --------------------------------------------------------------------------------------------------------------------
start_action_menu :: proc(core: ^Core) {
	w := &core.world
	a := w.avatar
	p := picker_begin(core, "Actions", "Select", "Cancel")
	if w.cells[avatar_cell(core)].first_item != 0 { pick_add(p, i32(Mi.Ground), "Ground...") }
	if w.chars[a].first_item != 0 { pick_add(p, i32(Mi.Inventory), "Inventory") }
	if has_equipment(w, a) { pick_add(p, i32(Mi.Equipment), "Equipment") }
	if can_sleep_now(w) { pick_add(p, i32(Mi.Sleep), "Sleep") }
	if forage_can(w) { pick_add(p, i32(Mi.Forage), "Forage...") }
	if can_craft_anything(w, a) { pick_add(p, i32(Mi.Craft), "Craft...") }
	if can_put_out_fire(w, a) { pick_add(p, i32(Mi.Put_Out_Fire), "Put Out Fire") }
	pick_add(p, i32(Mi.Statistics), "Statistics")
}

start_craft :: proc(core: ^Core) {
	w := &core.world
	a := w.avatar
	p := picker_begin(core, "Craft...", "Select", "Cancel")
	if can_make_twine(w, a) { pick_add(p, i32(Mi.Make_Twine), "Make Twine") }
	if can_build_fire(w, a) { pick_add(p, i32(Mi.Build_Fire), "Build Fire") }
	if can_make_torch(w, a) { pick_add(p, i32(Mi.Make_Torch), "Make Torch") }
	if can_build_furnace(w, a) { pick_add(p, i32(Mi.Build_Furnace), "Build Furnace") }
	if can_cook_bagel(w, a) { pick_add(p, i32(Mi.Cook_Bagel), "Cook Bagel") }
	if can_knap(w, a) { pick_add(p, i32(Mi.Knap), "Knap") }
	if can_make_hatchet(w, a) { pick_add(p, i32(Mi.Make_Hatchet), "Make Hatchet") }
}

activate_game_menu :: proc(core: ^Core, s: UI_State, item: Pick_Item) {
	u := &core.ui
	w := &core.world
	#partial switch s {
	case .Action_Menu:
		#partial switch Mi(item.value) {
		case .Ground: set_state(core, .Ground)
		case .Inventory: set_state(core, .Inventory)
		case .Equipment: set_state(core, .Equipment)
		case .Sleep: set_state(core, .Sleep)
		case .Forage: set_state(core, .Forage)
		case .Craft: set_state(core, .Craft)
		case .Put_Out_Fire: set_state(core, .Put_Out_Fire)
		case .Statistics: set_state(core, .Statistics)
		}
	case .Craft:
		#partial switch Mi(item.value) {
		case .Make_Twine: set_state(core, .Make_Twine)
		case .Build_Fire: set_state(core, .Build_Fire)
		case .Make_Torch: set_state(core, .Make_Torch)
		case .Build_Furnace: set_state(core, .Build_Furnace)
		case .Cook_Bagel: set_state(core, .Cook_Bagel)
		case .Knap: set_state(core, .Knap)
		case .Make_Hatchet: set_state(core, .Make_Hatchet)
		}
	case .Ground:
		u.sel_type = Item_Type(item.value)
		set_state(core, .Ground_Detail)
	case .Ground_Detail:
		u.sel_count = int(item.value)
		set_state(core, .Take)
	case .Inventory_Detail:
		v := int(item.value)
		switch {
		case v >= EFFECT_VALUE_BASE:
			do_item_effect(w, u.sel_type, Effect_Kind(v - EFFECT_VALUE_BASE))
			set_state(core, .Neutral)
		case Mi(v) == .Drop_All:
			u.sel_count = item_count_of(w, w.avatar, u.sel_type)
			set_state(core, .Drop)
		case Mi(v) == .Drop_Half:
			u.sel_count = item_count_of(w, w.avatar, u.sel_type) / 2
			set_state(core, .Drop)
		case Mi(v) == .Drop_One:
			u.sel_count = 1
			set_state(core, .Drop)
		case Mi(v) == .Equip_It:
			set_state(core, .Equip)
		}
	case .Equipment:
		slot := Equip_Slot(item.value)
		unequip(w, w.avatar, slot)
		msg_line(msg_begin(w), .Light_Gray, "%s unequips %s", char_name(w, w.avatar), EQUIP_SLOT_NAMES[slot])
		set_state(core, .Neutral)
	case .Equip:
		do_equip(core, Item_Id(item.value))
		set_state(core, .Neutral)
	}
}

do_equip :: proc(core: ^Core, id: Item_Id) {
	w := &core.world
	equip(w, w.avatar, id)
	msg_line(msg_begin(w), .Light_Gray, "%s equips %s", char_name(w, w.avatar), item_name(w, id))
}

// ---- Ground and Inventory detail menus ---------------------------------------------------------------------------------------------------------------
stack_count :: proc(stacks: []Stack, type: Item_Type) -> int {
	for s in stacks { if s.type == type { return s.count } }
	return 0
}

start_ground :: proc(core: ^Core) {
	w := &core.world
	p := picker_begin(core, "On the Ground", "Select", "Cancel")
	stacks: [32]Stack
	n := ground_stacks(w, stacks[:])
	for i in 0 ..< n { pick_add(p, i32(stacks[i].type), "%s(x%d)", ITEMS[stacks[i].type].name, stacks[i].count) }
	if n == 0 { set_state(core, .Action_Menu) }
}

start_ground_detail :: proc(core: ^Core) {
	u := &core.ui
	w := &core.world
	stacks: [32]Stack
	n := ground_stacks(w, stacks[:])
	count := stack_count(stacks[:n], u.sel_type)
	p := picker_begin(core, "", "Select", "Cancel")
	text_set(&p.header, "%s(x%d)", ITEMS[u.sel_type].name, count)
	if count > 1 { pick_add(p, i32(count), "Take All") }
	if count > 2 { pick_add(p, i32(count / 2), "Take Half") }
	pick_add(p, 1, "Take One")
	switch count {
	case 0: set_state(core, .Ground)
	case 1: u.sel_count = 1; set_state(core, .Take)
	}
}

start_inventory_detail :: proc(core: ^Core) {
	u := &core.ui
	w := &core.world
	count := item_count_of(w, w.avatar, u.sel_type)
	p := picker_begin(core, "", "Select", "Cancel")
	text_set(&p.header, "%s(x%d)", ITEMS[u.sel_type].name, count)
	if count > 1 { pick_add(p, i32(Mi.Drop_All), "Drop All") }
	if count > 2 { pick_add(p, i32(Mi.Drop_Half), "Drop Half") }
	pick_add(p, i32(Mi.Drop_One), "Drop One")
	if ITEMS[u.sel_type].slot != .None { pick_add(p, i32(Mi.Equip_It), "Equip") }
	for kind in ITEMS[u.sel_type].effects { if kind != .None { pick_add(p, i32(EFFECT_VALUE_BASE) + i32(kind), "%s", EFFECT_NAMES[kind]) } }
	if count == 0 { set_state(core, .Inventory) }
}

start_equipment :: proc(core: ^Core) {
	w := &core.world
	p := picker_begin(core, "Equipment", "Unequip", "Cancel")
	for slot in Equip_Slot {
		if id := w.chars[w.avatar].equipped[slot]; id != 0 { pick_add(p, i32(slot), "%s: %s", EQUIP_SLOT_NAMES[slot], item_full_name(w, id)) }
	}
}

// The original's header here is the leftover "<placeholder>" (audit D16); the item's name is shown instead.
start_equip :: proc(core: ^Core) {
	u := &core.ui
	w := &core.world
	p := picker_begin(core, ITEMS[u.sel_type].name, "Select", "Cancel")
	n := 0
	only := Item_Id(0)
	for id := w.chars[w.avatar].first_item; id != 0; id = w.items[id].next {
		if w.items[id].type == u.sel_type && ITEMS[u.sel_type].slot != .None {
			pick_add(p, i32(id), "%s", item_full_name(w, id))
			n += 1
			only = id
		}
	}
	switch n {
	case 0: set_state(core, .Inventory)
	case 1: do_equip(core, only); set_state(core, .Neutral)
	}
}

// ---- Inventory (the grid) ---------------------------------------------------------------------------------------------------------------------------------
start_inventory :: proc(core: ^Core) {
	w := &core.world
	inv := &core.ui.inv
	inv.count = inventory_stacks(w, inv.stacks[:])
	inv.cols, inv.rows = grid_size(inv.count)
	inv.cur = inv.count / 2
	// (the original crashes drawing an empty inventory, which is what dropping your last stack leads to: audit D15)
	if inv.count == 0 { set_state(core, .Action_Menu) }
}

inventory_command :: proc(core: ^Core, cmd: Command) {
	inv := &core.ui.inv
	if inv.count == 0 { return }
	#partial switch cmd {
	case .Cancel: set_state(core, .Action_Menu)
	case .Confirm:
		core.ui.sel_type = inv.stacks[inv.cur].type
		set_state(core, .Inventory_Detail)
	case .Up:
		inv.cur -= inv.cols
		if inv.cur < 0 { inv.cur += inv.count }
	case .Down:
		inv.cur += inv.cols
		if inv.cur >= inv.count { inv.cur -= inv.count }
	case .Left: inv.cur = (inv.cur + inv.count - 1) % inv.count
	case .Right: inv.cur = (inv.cur + 1) % inv.count
	}
}

render_inventory :: proc(core: ^Core) {
	f := &core.hues
	inv := &core.ui.inv
	fill_all(f, .Black)
	row_stride, col_stride := TILE_SIZE * 2, TILE_SIZE * 2
	y := FRAME_HEIGHT / 2 - inv.rows * row_stride / 2
	index := 0
	for _ in 0 ..< inv.rows {
		x := FRAME_WIDTH / 2 - inv.cols * col_stride / 2
		for _ in 0 ..< inv.cols {
			if index < inv.count {
				type := inv.stacks[index].type
				draw_tile(f, x + col_stride / 4, y + row_stride / 4, ITEMS[type].glyph, ITEMS[type].hue)
				text := fmt.tprintf("x%d", inv.stacks[index].count)
				draw_ui_text(f, x + col_stride / 2 - ui_text_width(text) / 2, y + TILE_SIZE + row_stride / 4, text, .Light_Gray)
			}
			if index == inv.cur { draw_tile(f, x + col_stride / 4, y + row_stride / 4, 255, .White) }
			index += 1
			x += col_stride
		}
		y += row_stride
	}
	draw_header(f, ITEMS[inv.stacks[inv.cur].type].name, .Orange, .Black)
	draw_status_bar(f, controls_text("Select", "Cancel"), .Black, .Light_Gray)
}

// ---- Forage (a grid of hidden squares) --------------------------------------------------------------------------------------------------------------------------
start_forage :: proc(core: ^Core) {
	w := &core.world
	fg := &core.ui.forage
	fg^ = {}
	remaining := forage_remaining(w)
	fg.cols, fg.rows = grid_size(remaining)
	if fg.cols == 0 || fg.cols * fg.rows > MAX_FORAGE_GRID { set_state(core, .Neutral); return }
	fg.cur_col, fg.cur_row = fg.cols / 2, fg.rows / 2
	// hide `remaining` squares at random (bounded: the original retries without limit)
	left := remaining
	for tries := 0; left > 0 && tries < 100000; tries += 1 {
		i := rng_range(&w.rng, 0, fg.cols - 1) + rng_range(&w.rng, 0, fg.rows - 1) * fg.cols
		if !fg.hidden[i] { fg.hidden[i] = true; left -= 1 }
	}
	for i in 0 ..< fg.cols * fg.rows { if left > 0 && !fg.hidden[i] { fg.hidden[i] = true; left -= 1 } }
}

forage_command :: proc(core: ^Core, cmd: Command) {
	w := &core.world
	fg := &core.ui.forage
	#partial switch cmd {
	case .Cancel:
		if fg.loot_count > 0 {
			m := msg_begin(w)
			msg_line(m, .Light_Gray, "%s forages:", char_name(w, w.avatar))
			stacks: [32]Stack
			n := 0
			for i in 0 ..< fg.loot_count {
				found := false
				for k in 0 ..< n { if stacks[k].type == fg.loot[i] { stacks[k].count += 1; found = true; break } }
				if !found { stacks[n] = {fg.loot[i], 1}; n += 1 }
			}
			for k in 0 ..< n { msg_line(m, .Light_Gray, "%s(x%d)", ITEMS[stacks[k].type].name, stacks[k].count) }
		}
		set_state(core, .Neutral)
	case .Up: fg.cur_row = (fg.cur_row + fg.rows - 1) % fg.rows
	case .Down: fg.cur_row = (fg.cur_row + 1) % fg.rows
	case .Left: fg.cur_col = (fg.cur_col + fg.cols - 1) % fg.cols
	case .Right: fg.cur_col = (fg.cur_col + 1) % fg.cols
	case .Confirm:
		i := fg.cur_col + fg.cur_row * fg.cols
		if fg.hidden[i] && forage_can(w) {
			fg.hidden[i] = false
			if id := forage_one(w); id != 0 {
				fg.found[i] = w.items[id].type
				if fg.loot_count < MAX_FORAGE_GRID { fg.loot[fg.loot_count] = w.items[id].type; fg.loot_count += 1 }
			}
		}
	}
}

render_forage :: proc(core: ^Core) {
	f := &core.hues
	w := &core.world
	fg := &core.ui.forage
	fill_all(f, .Black)
	y := FRAME_HEIGHT / 2 - fg.rows * CELL_H / 2
	for row in 0 ..< fg.rows {
		x := FRAME_WIDTH / 2 - fg.cols * CELL_W / 2
		for col in 0 ..< fg.cols {
			i := col + row * fg.cols
			if fg.hidden[i] {
				draw_tile(f, x, y, 254, .Yellow)
			} else if fg.found[i] != .None {
				draw_tile(f, x, y, ITEMS[fg.found[i]].glyph, ITEMS[fg.found[i]].hue)
			}
			if col == fg.cur_col && row == fg.cur_row { draw_tile(f, x, y, 255, .Green if fg.hidden[i] && forage_can(w) else .Red) }
			x += CELL_W
		}
		y += CELL_H
	}
	// what has been found so far, down the left edge
	stacks: [32]Stack
	n := 0
	for i in 0 ..< fg.loot_count {
		found := false
		for k in 0 ..< n { if stacks[k].type == fg.loot[i] { stacks[k].count += 1; found = true; break } }
		if !found { stacks[n] = {fg.loot[i], 1}; n += 1 }
	}
	ly := FRAME_HEIGHT / 2 - TILE_SIZE / 2 * n
	for k in 0 ..< n {
		draw_tile(f, 0, ly, ITEMS[stacks[k].type].glyph, ITEMS[stacks[k].type].hue)
		draw_ui_text(f, TILE_SIZE, ly + TILE_SIZE / 2 - UI_FONT_CELL / 2, fmt.tprintf("x%d", stacks[k].count), .Light_Gray)
		ly += TILE_SIZE
	}
	draw_header(f, energy_display(w, w.avatar), .Orange, .Black)
	draw_status_bar(f, controls_text("Forage", "Exit"), .Black, .Light_Gray)
}

// ---- Combat ---------------------------------------------------------------------------------------------------------------------------------------------------------------
combat_start :: proc(core: ^Core) {
	u := &core.ui
	if !combat_exists(&core.world) { set_state(core, .Neutral); return }
	if u.enemy_index >= combat_count(&core.world) { u.enemy_index = 0 }
	u.menu_index = 0
	u.mux = .Combat_Theme
}

combat_command :: proc(core: ^Core, cmd: Command) {
	u := &core.ui
	n := combat_count(&core.world)
	#partial switch cmd {
	case .Up, .Down: u.menu_index = (u.menu_index + 1) % 2 // two entries: either direction swaps
	case .Left: u.enemy_index = (u.enemy_index + n - 1) % n
	case .Right: u.enemy_index = (u.enemy_index + 1) % n
	case .Confirm:
		if u.menu_index == 0 {
			combat_attack(&core.world, u.enemy_index)
			set_state(core, .Neutral)
		} else {
			set_state(core, .Run)
		}
	}
}

draw_character :: proc(f: ^Hue_Frame, x, y: int, type: Character_Type) {
	ch := CHARACTERS[type]
	draw_tile(f, x, y, ch.mask_glyph, ch.mask_hue)
	draw_tile(f, x, y, ch.glyph, ch.hue)
}

render_combat :: proc(core: ^Core) {
	f := &core.hues
	w := &core.world
	u := &core.ui
	fill_all(f, .Black)
	third_w, two_thirds_w := FRAME_WIDTH / 3, FRAME_WIDTH * 2 / 3
	third_h, sixth_h := FRAME_HEIGHT / 3, FRAME_HEIGHT / 6
	half_tile := TILE_SIZE / 2
	half_ui := UI_FONT_CELL / 2
	// the avatar and its numbers
	draw_character(f, third_w - half_tile, third_h - half_tile, w.chars[w.avatar].type)
	y := sixth_h - half_ui * 3
	y = centered_text(f, third_w, y, char_name(w, w.avatar), .Light_Gray)
	y = centered_text(f, third_w, y, health_display(w, w.avatar), .Pink)
	centered_text(f, third_w, y, energy_display(w, w.avatar), .Blue)
	// the enemies in a row, with a pointer under the chosen one
	enemies: [MAX_COMBATANTS]Char_Id
	n := others_in_cell(w, w.avatar, enemies[:])
	x := two_thirds_w - half_tile * n
	ey := third_h - half_tile * 2
	draw_tile(f, x + TILE_SIZE * u.enemy_index, ey + TILE_SIZE, 0x1C, .White)
	for i in 0 ..< n {
		draw_character(f, x, ey, w.chars[enemies[i]].type)
		x += TILE_SIZE
	}
	// the chosen enemy's numbers
	e := enemies[min(u.enemy_index, max(n - 1, 0))]
	ey2 := sixth_h - half_ui * 2
	ey2 = centered_text(f, two_thirds_w, ey2, char_name(w, e), .Light_Gray)
	centered_text(f, two_thirds_w, ey2, health_display(w, e), .Pink)
	// the menu
	my := FRAME_HEIGHT * 3 / 4 - half_ui * 2
	fill_rect(f, 0, my, FRAME_WIDTH, UI_FONT_CELL, .Blue)
	my -= u.menu_index * UI_FONT_CELL
	items := [2]string{"Attack!", "Run!"}
	for i in 0 ..< 2 { my = centered_text(f, FRAME_WIDTH / 2, my, items[i], .Black if i == u.menu_index else .Blue) }
}

// ---- Messages ---------------------------------------------------------------------------------------------------------------------------------------------------------------
message_start :: proc(core: ^Core) {
	w := &core.world
	if !msg_has(w) { set_state(core, .Neutral); return }
	ui_play_sfx(core, msg_current(w).sfx)
	core.ui.choice_index = 0
}

message_command :: proc(core: ^Core, cmd: Command) {
	u := &core.ui
	w := &core.world
	m := msg_current(w)
	if m == nil { message_start(core); return }
	n := int(m.choice_count)
	if n == 0 {
		if cmd == .Confirm || cmd == .Cancel { msg_dismiss(w); message_start(core) }
		return
	}
	#partial switch cmd {
	case .Up: u.choice_index = (u.choice_index + n - 1) % n
	case .Down: u.choice_index = (u.choice_index + 1) % n
	case .Confirm:
		message_choose(w, u.choice_index)
		message_start(core)
	case .Cancel: // the first press moves to the cancel choice (the first), the second takes it
		if u.choice_index != 0 { u.choice_index = 0; return }
		message_choose(w, 0)
		message_start(core)
	}
}

render_message :: proc(core: ^Core) {
	f := &core.hues
	m := msg_current(&core.world)
	fill_all(f, .Black)
	if m == nil { return }
	has_choices := m.choice_count > 0
	y := (FRAME_HEIGHT / 3 if has_choices else FRAME_HEIGHT / 2) - UI_FONT_CELL * int(m.line_count) / 2
	for i in 0 ..< m.line_count {
		text := line_text(&m.lines[i])
		draw_ui_text(f, FRAME_WIDTH / 2 - ui_text_width(text) / 2, y, text, m.lines[i].hue)
		y += UI_FONT_CELL
	}
	if has_choices {
		cy := FRAME_HEIGHT * 2 / 3 - UI_FONT_CELL / 2
		fill_rect(f, 0, cy, FRAME_WIDTH, UI_FONT_CELL, .Blue)
		cy -= core.ui.choice_index * UI_FONT_CELL
		for i in 0 ..< m.choice_count {
			text := choice_text(&m.choices[i])
			draw_ui_text(f, FRAME_WIDTH / 2 - ui_text_width(text) / 2, cy, text, .Black if int(i) == core.ui.choice_index else .Blue)
			cy += UI_FONT_CELL
		}
	}
	draw_status_bar(f, controls_text("Select" if has_choices else "Continue", ""), .Black, .Light_Gray)
}

// ---- Statistics, Dead, Winner --------------------------------------------------------------------------------------------------------------------------------------------
render_statistics :: proc(core: ^Core) {
	f := &core.hues
	w := &core.world
	a := w.avatar
	fill_all(f, .Black)
	y := FRAME_HEIGHT / 2 - UI_FONT_CELL * 8 / 2
	y = centered_text(f, FRAME_WIDTH / 2, y, health_display(w, a), .Pink)
	y = centered_text(f, FRAME_WIDTH / 2, y, energy_display(w, a), .Blue)
	y = centered_text(f, FRAME_WIDTH / 2, y, attack_display(w, a), .Red)
	y = centered_text(f, FRAME_WIDTH / 2, y, defend_display(w, a), .Green)
	y = centered_text(f, FRAME_WIDTH / 2, y, xp_display(w, a), .Cyan)
	y = centered_text(f, FRAME_WIDTH / 2, y, level_display(w, a), .Purple)
	y = centered_text(f, FRAME_WIDTH / 2, y, ap_display(w, a), .Yellow)
	centered_text(f, FRAME_WIDTH / 2, y, jools_display(w, a), .Light_Green)
	draw_header(f, "Statistics", .Black, .Orange) // black on orange here, unlike the pickers
	draw_status_bar(f, controls_text("", "Go Back"), .Black, .Light_Gray)
}

END_SCREEN_DELAY_MS :: 1000

end_screen_command :: proc(core: ^Core, cmd: Command) {
	if (cmd == .Confirm || cmd == .Cancel) && core.now_ms >= core.ui.show_until {
		core.world_ready = false
		set_state(core, .Main_Menu)
	}
}

render_end_screen :: proc(core: ^Core, text: string, hue: Hue, button: string) {
	f := &core.hues
	fill_all(f, .Black)
	draw_ui_text(f, FRAME_WIDTH / 2 - ui_text_width(text) / 2, FRAME_HEIGHT / 2 - UI_FONT_CELL / 2, text, hue)
	if core.now_ms >= core.ui.show_until { draw_status_bar(f, controls_text(button, ""), .Black, .Light_Gray) }
}
