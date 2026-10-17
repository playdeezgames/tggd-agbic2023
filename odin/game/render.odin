package game

import "core:fmt"

// The software rasterizer: a port of the original's pixel sink (BasePixelSink, OffscreenBuffer), Font and the UIContext
// helpers. The picture is kept as hue indexes, like the original, and converted to RGBA at the end, so tests can compare
// it with the frames recorded from the VB game (docs/reference/vb, made by tools/oracle.sh).

Hue_Frame :: [FRAME_WIDTH * FRAME_HEIGHT]Hue

// A constant array cannot be indexed at run time; these are plain variables in assets_data.odin.

// Pixels outside the view are ignored (as in the original's DisplayBuffer.SetPixel).
set_pixel :: proc(f: ^Hue_Frame, x, y: int, hue: Hue) {
	if x < 0 || y < 0 || x >= FRAME_WIDTH || y >= FRAME_HEIGHT { return }
	f[y * FRAME_WIDTH + x] = hue
}

fill_rect :: proc(f: ^Hue_Frame, x, y, w, h: int, hue: Hue) {
	for yy in max(y, 0) ..< min(y + h, FRAME_HEIGHT) {
		for xx in max(x, 0) ..< min(x + w, FRAME_WIDTH) { f[yy * FRAME_WIDTH + xx] = hue }
	}
}

fill_all :: proc(f: ^Hue_Frame, hue: Hue) { for i in 0 ..< len(f) { f[i] = hue } }

// ---- the UI font (CyFont8x8): fixed 8 by 8 cells, ASCII 32 to 127. The original throws on any other character;
// here such a character is drawn as a blank cell.

ui_text_width :: proc(text: string) -> int { return len(text) * UI_FONT_CELL }

// Draws `text` with its top-left at (x, y); only the set pixels are drawn (the rest stays transparent). Returns the end x.
draw_ui_text :: proc(f: ^Hue_Frame, x, y: int, text: string, hue: Hue) -> int {
	x := x
	for i in 0 ..< len(text) {
		code := int(text[i])
		if code >= UI_FONT_FIRST && code < UI_FONT_FIRST + UI_FONT_COUNT {
			glyph := &UI_FONT_ROWS[code - UI_FONT_FIRST]
			for row in 0 ..< UI_FONT_CELL {
				bits := glyph[row]
				for col in 0 ..< UI_FONT_CELL {
					if bits & (1 << uint(col)) != 0 { set_pixel(f, x + col, y + row, hue) }
				}
			}
		}
		x += UI_FONT_CELL
	}
	return x
}

// ---- the game art (BagelQuest font): 16 by 16 tiles indexed by character code.
draw_tile :: proc(f: ^Hue_Frame, x, y: int, code: u8, hue: Hue) {
	glyph := &TILE_ROWS[code]
	for row in 0 ..< TILE_SIZE {
		bits := glyph[row]
		for col in 0 ..< TILE_SIZE {
			if bits & (1 << uint(col)) != 0 { set_pixel(f, x + col, y + row, hue) }
		}
	}
}

// ---- UIContext helpers
// ShowStatusBar: a bar the height of the font at the bottom, text centred.
draw_status_bar :: proc(f: ^Hue_Frame, text: string, foreground, background: Hue) {
	fill_rect(f, 0, FRAME_HEIGHT - UI_FONT_CELL, FRAME_WIDTH, UI_FONT_CELL, background)
	draw_ui_text(f, FRAME_WIDTH / 2 - ui_text_width(text) / 2, FRAME_HEIGHT - UI_FONT_CELL, text, foreground)
}

// ShowHeader: a bar the height of the font at the top, text centred.
draw_header :: proc(f: ^Hue_Frame, text: string, foreground, background: Hue) {
	fill_rect(f, 0, 0, FRAME_WIDTH, UI_FONT_CELL, background)
	draw_ui_text(f, FRAME_WIDTH / 2 - ui_text_width(text) / 2, 0, text, foreground)
}

// UIContext.ControlsText. The strings live in the temp allocator (the platform frees it each frame).
controls_text :: proc(a_text, b_text: string) -> string {
	result := ""
	if len(a_text) > 0 { result = fmt.tprintf("Space/(A) - %s", a_text) }
	if len(b_text) > 0 {
		if len(result) > 0 { result = fmt.tprintf("%s | ", result) }
		result = fmt.tprintf("%sEsc/(B) - %s", result, b_text)
	}
	return result
}

// Hue indexes to RGBA (bytes R, G, B, A in memory).
to_rgba :: proc(hues: ^Hue_Frame, out: ^[FRAME_WIDTH * FRAME_HEIGHT]u32) {
	for i in 0 ..< len(hues) { out[i] = PALETTE[hues[i]] }
}
