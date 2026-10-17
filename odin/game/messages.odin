package game

import "core:fmt"

// The message queue (the original's world messages): rules push dialogs; the UI shows the oldest until it is dismissed.
// A message has hued lines, optional choices (each runs an effect for the avatar) and an optional sound.

// The original's PlayerHit sound is stored under the key "SfxVolumeTest" and Shucks under "Failure".
SFX_PLAYER_HIT :: Sfx_Id.Sfx_Volume_Test
SFX_FAILURE :: Sfx_Id.Failure

// Start a new message at the back of the queue. When the queue is full the message goes to a scratch slot and is dropped,
// so callers never need to check.
msg_begin :: proc(w: ^World) -> ^Message {
	if w.msg_count >= MAX_MESSAGES {
		w.msg_scratch = {}
		return &w.msg_scratch
	}
	i := (w.msg_head + w.msg_count) % MAX_MESSAGES
	w.messages[i] = {}
	w.msg_count += 1
	return &w.messages[i]
}

msg_has :: proc(w: ^World) -> bool { return w.msg_count > 0 }

msg_current :: proc(w: ^World) -> ^Message {
	if w.msg_count == 0 { return nil }
	return &w.messages[w.msg_head]
}

msg_dismiss :: proc(w: ^World) {
	if w.msg_count == 0 { return }
	w.msg_head = (w.msg_head + 1) % MAX_MESSAGES
	w.msg_count -= 1
}

msg_clear :: proc(w: ^World) { for msg_has(w) { msg_dismiss(w) } }

// Add a line (formatted like fmt.bprintf; text longer than LINE_CAP is cut). Returns the message so calls can chain.
msg_line :: proc(m: ^Message, hue: Hue, format: string, args: ..any) -> ^Message {
	if int(m.line_count) >= MAX_LINES { return m }
	l := &m.lines[m.line_count]
	text := fmt.bprintf(l.buf[:], format, ..args)
	l.hue, l.len = hue, u8(len(text))
	m.line_count += 1
	return m
}

// Add a choice that runs `effect` when picked. The caller sets item, number and again on the result when needed.
msg_choice :: proc(m: ^Message, effect: Effect_Kind, format: string, args: ..any) -> ^Msg_Choice {
	if int(m.choice_count) >= MAX_CHOICES { return &m.overflow } // written and ignored
	c := &m.choices[m.choice_count]
	text := fmt.bprintf(c.buf[:], format, ..args)
	c.len, c.effect = u8(len(text)), effect
	m.choice_count += 1
	return c
}

line_text :: proc(l: ^Text_Line) -> string { return string(l.buf[:l.len]) }
choice_text :: proc(c: ^Msg_Choice) -> string { return string(c.buf[:c.len]) }

// A canned message from the content tables (signs, "Nothing happens.").
msg_canned :: proc(w: ^World, type: Message_Type) -> ^Message {
	m := msg_begin(w)
	info := MESSAGES[type]
	m.sfx = info.sfx
	for l in MESSAGE_LINES[info.lines.start:info.lines.start + info.lines.count] { msg_line(m, l.hue, "%s", l.text) }
	for ch in MESSAGE_CHOICES[info.choices.start:info.choices.start + info.choices.count] { msg_choice(m, ch.effect, "%s", ch.text) }
	return m
}

// Sounds with no message to carry them (picking things up, levelling up) are queued; the platform plays them after the step.
sfx_play :: proc(w: ^World, id: Sfx_Id) {
	if w.sfx_count < MAX_SFX { w.sfx_queue[w.sfx_count] = id; w.sfx_count += 1 }
}

// Hand over the queued sounds and empty the queue. Returns how many were copied to `out`.
sfx_drain :: proc(w: ^World, out: []Sfx_Id) -> int {
	n := min(w.sfx_count, len(out))
	for i in 0 ..< n { out[i] = w.sfx_queue[i] }
	w.sfx_count = 0
	return n
}
