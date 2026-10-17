#+build !js
package tests

import "core:testing"
import "bq:game"

@(test)
palette_matches_original :: proc(t: ^testing.T) {
	// spot checks against hue.json: Blue is R=42 G=75 B=215, White is all 255
	testing.expect_value(t, game.PALETTE[.Blue], u32(0xFFD74B2A))
	testing.expect_value(t, game.PALETTE[.White], u32(0xFFFFFFFF))
	testing.expect_value(t, game.PALETTE[.Black], u32(0xFF000000))
}

@(test)
idle_steps_do_not_redraw :: proc(t: ^testing.T) {
	core := new_core()
	defer free(core)
	step(core)
	out := step(core)
	testing.expect(t, !out.frame_changed, "a step with no input and no change must not report a changed frame")
}

@(test)
text_stays_inside_the_frame :: proc(t: ^testing.T) {
	// drawing far outside the view must be ignored, not crash (the original's SetPixel ignores out-of-range pixels)
	f := new(game.Hue_Frame)
	defer free(f)
	game.draw_ui_text(f, -100, -100, "hello", .White)
	game.draw_ui_text(f, game.FRAME_WIDTH - 4, game.FRAME_HEIGHT - 4, "hello", .White)
	game.draw_tile(f, -8, -8, 2, .White)
}

@(test)
controls_text_matches_original :: proc(t: ^testing.T) {
	testing.expect_value(t, game.controls_text("Continue", ""), "Space/(A) - Continue")
	testing.expect_value(t, game.controls_text("Select", "Cancel"), "Space/(A) - Select | Esc/(B) - Cancel")
	testing.expect_value(t, game.controls_text("", "Go Back"), "Esc/(B) - Go Back")
}
