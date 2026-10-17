#+build !js
package tests

// The scripted behaviour: dialogues, quests, shops, skills, crafting verbs, eating, and the whole way to a bagel.

import "core:strings"
import "core:testing"
import "bq:game"

// The current message as text: its lines joined with " | ".
said :: proc(w: ^game.World) -> string {
	m := game.msg_current(w)
	if m == nil { return "(no message)" }
	parts := make([dynamic]string, context.temp_allocator)
	for i in 0 ..< m.line_count { append(&parts, game.line_text(&m.lines[i])) }
	return strings.join(parts[:], " | ", context.temp_allocator)
}

choice_index :: proc(w: ^game.World, text: string) -> int {
	m := game.msg_current(w)
	if m == nil { return -1 }
	for i in 0 ..< int(m.choice_count) { if game.choice_text(&m.choices[i]) == text { return i } }
	return -1
}

choices_of :: proc(w: ^game.World) -> string {
	m := game.msg_current(w)
	if m == nil { return "(no message)" }
	parts := make([dynamic]string, context.temp_allocator)
	for i in 0 ..< int(m.choice_count) { append(&parts, game.choice_text(&m.choices[i])) }
	return strings.join(parts[:], " / ", context.temp_allocator)
}

// Pick a choice by its text; fails the test if it is not offered.
pick :: proc(t: ^testing.T, w: ^game.World, text: string, loc := #caller_location) {
	i := choice_index(w, text)
	testing.expectf(t, i >= 0, "no choice %q in: %s", text, choices_of(w), loc = loc)
	if i >= 0 { game.message_choose(w, i) }
}

// Run an effect for the avatar from a clean queue; the result is the first message it made.
fx :: proc(w: ^game.World, kind: game.Effect_Kind, call := game.Effect_Call{}) {
	game.msg_clear(w)
	c := call
	c.kind = kind
	game.run_effect(w, w.avatar, c)
}

sfx_of_current :: proc(w: ^game.World) -> game.Sfx_Id { return game.msg_current(w).sfx }

new_hero :: proc(seed: u64 = 1) -> ^game.World {
	w := new_world(seed)
	game.msg_clear(w)
	return w
}

stats :: proc(w: ^game.World) -> ^game.Character_Stats { return &w.chars[w.avatar].stats }

@(test)
the_rat_quest :: proc(t: ^testing.T) {
	w := new_hero()
	defer free(w)
	fx(w, .Gorachan_Talk)
	testing.expect_value(t, choices_of(w), "Cool story, bro! / Yer a pervert! / I'll take a bed. / I need a job!")
	pick(t, w, "I need a job!")
	testing.expect_value(t, said(w), "Well, there are a bunch of rats in the cellar. | I'll pay you 1 jools for each rat tail. | I only accept the tails, no corpses. | So you'll need to cut them off first.")
	testing.expect_value(t, choices_of(w), "I'm on it! / Mebbe later?")
	pick(t, w, "Mebbe later?")
	testing.expect(t, !game.has_flag(w, w.avatar, .Rat_Quest), "not accepted yet")
	game.msg_clear(w)
	fx(w, .Start_Rat_Quest)
	pick(t, w, "I'm on it!")
	testing.expect(t, game.has_flag(w, w.avatar, .Rat_Quest), "quest accepted")
	// with the quest, the job offer is gone, and the tails offer needs tails
	fx(w, .Gorachan_Talk)
	testing.expect_value(t, choices_of(w), "Cool story, bro! / Yer a pervert! / I'll take a bed.")
	give(w, .Rat_Tail, 3)
	fx(w, .Gorachan_Talk)
	testing.expect_value(t, choices_of(w), "Cool story, bro! / Yer a pervert! / I'll take a bed. / Here's some rat tails!")
	jools := stats(w).jools
	pick(t, w, "Here's some rat tails!")
	testing.expect_value(t, stats(w).jools, jools + 3)
	testing.expect_value(t, game.item_count_of(w, w.avatar, .Rat_Tail), 0)
	testing.expect_value(t, said(w), "Loxy receives 3 jools.")
	testing.expect_value(t, sfx_of_current(w), game.Sfx_Id.Woo_Hoo)
	validated(t, w)
}

@(test)
the_innkeeper :: proc(t: ^testing.T) {
	w := new_hero()
	defer free(w)
	fx(w, .Sleep_At_Inn)
	testing.expect_value(t, said(w), "Loxy needs to pay Gorachan first!")
	testing.expect_value(t, sfx_of_current(w), game.Sfx_Id.Failure)
	stats(w).energy = 3
	fx(w, .Pay_Innkeeper)
	testing.expect_value(t, said(w), "Thanks for yer business. | Choose any bed you like.")
	testing.expect_value(t, stats(w).jools, 19)
	fx(w, .Pay_Innkeeper)
	testing.expect_value(t, said(w), "You've already paid!")
	testing.expect_value(t, stats(w).jools, 19)
	fx(w, .Sleep_At_Inn)
	testing.expect_value(t, said(w), "Loxy rests and feels refreshed! | Loxy has 10/10 energy.")
	testing.expect(t, !game.has_flag(w, w.avatar, .Paid_Innkeeper), "the bed is used up")
	stats(w).jools = 0
	fx(w, .Pay_Innkeeper)
	testing.expect_value(t, said(w), "Sorry! No jools, no bed!")
	fx(w, .Pervert_Innkeeper)
	testing.expect_value(t, said(w), "I'm not a pervert! | I'm just Australian!")
}

@(test)
the_druid_teaches_six_skills :: proc(t: ^testing.T) {
	w := new_hero()
	defer free(w)
	fx(w, .Druid_Talk)
	testing.expect_value(t, said(w), "Greetings! I am Marcus, the hippy druid. | I can help you learn nature's way.")
	pick(t, w, "Don't druids live in the woods?")
	testing.expect_value(t, said(w), "Alas, I have allergies.")
	game.msg_clear(w)
	fx(w, .Druid_Teach_Menu)
	testing.expect_value(t, choices_of(w), "Good to know! / Foraging(-1AP) / Twine Making(-1AP,-2 Plant Fiber) / Knapping(-1AP,-2 Rock) / Fire Making(-1AP, -5 Rock, -5 Sticks) / Torch Making(-1AP, -1 Stick, -1 Charcoal) / Hatchet Making(-1AP,-1Stick,-1S.Rock,-1Twine)")
	// foraging costs the one AP
	pick(t, w, "Foraging(-1AP)")
	testing.expect_value(t, said(w), "You now know how to forage! | To do so, simply select 'Forage...' | from the Actions menu.")
	testing.expect(t, game.has_flag(w, w.avatar, .Knows_Foraging), "knows foraging")
	testing.expect_value(t, stats(w).ap, 0)
	testing.expect_value(t, sfx_of_current(w), game.Sfx_Id.Woo_Hoo)
	// no AP left
	fx(w, .Learn_Knapping)
	testing.expect_value(t, said(w), "To learn to knap, | Loxy needs: | Rock: 0/2") // materials are checked before AP
	give(w, .Rock, 2)
	fx(w, .Learn_Knapping)
	testing.expect_value(t, said(w), "To learn to knap, | Loxy needs 1 AP, | but has 0!")
	stats(w).ap = 5
	fx(w, .Learn_Knapping)
	testing.expect_value(t, said(w), "You now know how to knap! | To do so, simply select 'Knap' | from the Actions menu.")
	testing.expect_value(t, game.item_count_of(w, w.avatar, .Sharp_Rock), 1) // learned by doing: a rock was sharpened
	testing.expect_value(t, game.item_count_of(w, w.avatar, .Rock), 1)
	fx(w, .Learn_Knapping)
	testing.expect_value(t, said(w), "Loxy already knows how to knap!")
	// twine: the avatar starts with one fibre, needs two
	fx(w, .Learn_Twine_Making)
	testing.expect_value(t, said(w), "To learn to make twine, | Loxy needs: | Plant Fiber: 1/2")
	// fire and torch only need the materials held (audit C13)
	fx(w, .Learn_Fire_Making)
	testing.expect_value(t, said(w), "To learn to make a fire, | Loxy needs: | Rock: 1/5 | Stick: 0/5")
	give(w, .Rock, 4); give(w, .Stick, 5)
	fx(w, .Learn_Fire_Making)
	testing.expect_value(t, said(w), "You now know how to make a fire! | To do so, simply select 'Build Fire' | from the Actions menu. | (only works in clear areas in the wilderness)")
	testing.expect_value(t, game.item_count_of(w, w.avatar, .Rock), 5) // nothing was used up
	// teach everything, then the menu says so
	for k in game.SKILL_KINDS { game.set_flag(w, w.avatar, game.skill_of(k).flag, true) }
	fx(w, .Druid_Teach_Menu)
	testing.expect_value(t, said(w), "You have learned all I have to teach you.")
	validated(t, w)
}

@(test)
the_druid_sells_energy_herbs :: proc(t: ^testing.T) {
	w := new_hero()
	defer free(w)
	fx(w, .Druid_Prices)
	testing.expect_value(t, said(w), "I sell a variety of herbs. | (Loxy has 20 jools)")
	testing.expect_value(t, choices_of(w), "Good to know! / Buy Energy Herb(5 jools)")
	pick(t, w, "Buy Energy Herb(5 jools)")
	testing.expect_value(t, stats(w).jools, 15)
	testing.expect_value(t, game.item_count_of(w, w.avatar, .Energy_Herb), 1)
	testing.expect_value(t, said(w), "Thank you for yer purchase!")
	testing.expect_value(t, sfx_of_current(w), game.Sfx_Id.Yoink)
	pick(t, w, "No worries!") // reopens the shop
	testing.expect_value(t, said(w), "I sell a variety of herbs. | (Loxy has 15 jools)")
	stats(w).jools = 4
	pick(t, w, "Buy Energy Herb(5 jools)")
	testing.expect_value(t, said(w), "You don't have enough!")
	testing.expect_value(t, sfx_of_current(w), game.Sfx_Id.Failure)
	testing.expect_value(t, stats(w).jools, 4)
	pick(t, w, "Shucks!")
	testing.expect_value(t, said(w), "I sell a variety of herbs. | (Loxy has 4 jools)")
}

@(test)
the_healer_basin :: proc(t: ^testing.T) {
	w := new_hero()
	defer free(w)
	fx(w, .Healer_Talk)
	testing.expect_value(t, said(w), "Welcome to the Nihilistic House of Healing. | If you go to the basin and wash, | you will be healed, | but it will cost you half of yer jools. | Not that I care or anything, | because I'm a nihilist.")
	pick(t, w, "What's for sale?")
	testing.expect_value(t, said(w), "I don't sell anything. | I'm a nihilist, remember?")
	game.msg_clear(w)
	fx(w, .Heal, {number = 5})
	testing.expect_value(t, said(w), "Nothing happens!") // already at full health
	stats(w).health = 1
	fx(w, .Heal, {number = 5})
	testing.expect_value(t, said(w), "Loxy is healed! | Loxy now has 2 health. | Loxy loses 10 jools!")
	testing.expect_value(t, stats(w).jools, 10)
	// the basin only heals up to 5, whatever the maximum (audit C15)
	game.set_max_health(w, w.avatar, 20)
	stats(w).health = 3
	fx(w, .Heal, {number = 5})
	testing.expect_value(t, stats(w).health, 5)
	stats(w).health = 5
	fx(w, .Heal, {number = 5})
	testing.expect_value(t, said(w), "Nothing happens!")
	// no jools: nothing is lost and no loss line
	stats(w).jools = 0
	stats(w).health = 1
	fx(w, .Heal, {number = 5})
	testing.expect_value(t, said(w), "Loxy is healed! | Loxy now has 5 health.")
}

@(test)
the_trainers :: proc(t: ^testing.T) {
	w := new_hero()
	defer free(w)
	fx(w, .Energy_Trainer_Talk)
	testing.expect_value(t, said(w), "I am the endurance trainer. | I can increase yer energy | for the cost of 1AP and 20 jools.")
	pick(t, w, "Train me!")
	testing.expect_value(t, said(w), "Loxy loses 1 AP | Loxy loses 20 jools | Loxy adds 1 Maximum Energy | Yer now at 11 Maximum Energy. | Persistence is futile!")
	testing.expect_value(t, stats(w).max_energy, 11)
	testing.expect_value(t, stats(w).energy, 11) // energy was 10 and gains 1 with the new maximum
	fx(w, .Train_Energy)
	testing.expect_value(t, said(w), "You need at least 1 AP. | Come back when yer more experienced!")
	stats(w).ap = 3
	fx(w, .Train_Energy)
	testing.expect_value(t, said(w), "The price is 22 jools. | I have overhead, you know.")
	// health
	fx(w, .Health_Trainer_Talk)
	testing.expect_value(t, said(w), "I am the health trainer! | I can help you increase yer health. | The cost is 10 AP.")
	fx(w, .Train_Health)
	testing.expect_value(t, said(w), "To go from 2 to 3 Maximum Health, | you need 10 AP, but you have 3. | Come back when yer more experienced!")
	stats(w).ap = 10
	fx(w, .Train_Health)
	testing.expect_value(t, said(w), "Loxy loses 10 AP | Loxy adds 1 Maximum Health | Yer now at 3 Maximum Health. | Remember! if you don't have yer health, | you don't really have anything!")
	testing.expect_value(t, stats(w).max_health, 3)
	testing.expect_value(t, stats(w).health, 3)
	testing.expect_value(t, stats(w).ap, 0)
	validated(t, w)
}

@(test)
the_potter :: proc(t: ^testing.T) {
	w := new_hero()
	defer free(w)
	fx(w, .Potter_Talk)
	pick(t, w, "I loved yer movies!")
	testing.expect_value(t, said(w), "Um. Thanks! | ... | What's a 'Movie'?")
	game.msg_clear(w)
	fx(w, .Potter_Make_Pot)
	testing.expect_value(t, said(w), "To make a pot, I need: | Plant Fiber: 1/5 | Clay: 0/5")
	stats(w).jools = 5
	fx(w, .Potter_Make_Pot)
	testing.expect_value(t, said(w), "The price is 20 jools. | Loxy has 5 jools")
	stats(w).jools = 20
	give(w, .Plant_Fiber, 4); give(w, .Clay, 5)
	fx(w, .Potter_Make_Pot)
	testing.expect_value(t, said(w), "Loxy loses 20 jools | Loxy gains 1 Clay Pot")
	testing.expect_value(t, game.item_count_of(w, w.avatar, .Clay_Pot), 1)
	testing.expect_value(t, game.item_count_of(w, w.avatar, .Clay), 0)
	testing.expect_value(t, game.item_count_of(w, w.avatar, .Unfired_Pot), 0)
	testing.expect_value(t, stats(w).jools, 0)
	validated(t, w)
}

@(test)
the_river_fills_pots :: proc(t: ^testing.T) {
	w := new_hero()
	defer free(w)
	fx(w, .Bump_River)
	testing.expect_value(t, choices_of(w), "Cool story, bro!")
	give(w, .Clay_Pot)
	fx(w, .Bump_River)
	testing.expect_value(t, said(w), "Loxy visits the river bank.")
	testing.expect_value(t, choices_of(w), "Cool story, bro! / Fill Clay Pot with Water")
	pick(t, w, "Fill Clay Pot with Water")
	testing.expect_value(t, said(w), "Loxy fills a Clay Pot with water.")
	testing.expect_value(t, game.item_count_of(w, w.avatar, .Water_Pot), 1)
	testing.expect_value(t, game.item_count_of(w, w.avatar, .Clay_Pot), 0)
	fx(w, .Fill_Clay_Pot) // no pot left: silence, not a false claim
	testing.expect(t, !game.msg_has(w), "nothing to say")
}

@(test)
fire_torch_and_furnace :: proc(t: ^testing.T) {
	w := new_hero()
	defer free(w)
	a := w.avatar
	stand_on(w, .Empty)
	cell := w.chars[a].cell
	// the energy is spent before the materials are checked (audit C14)
	game.do_terrain_verb(w, .Build_Fire)
	testing.expect_value(t, said(w), "To build a fire, | Loxy needs: | Rock: 0/5 | Stick: 0/5")
	testing.expect_value(t, stats(w).energy, 9)
	game.msg_clear(w)
	give(w, .Rock, 5); give(w, .Stick, 6)
	game.do_terrain_verb(w, .Build_Fire)
	testing.expect_value(t, said(w), "Loxy builds a small fire.")
	testing.expect_value(t, w.cells[cell].terrain, game.Terrain_Type.Cooking_Fire)
	testing.expect_value(t, game.item_count_of(w, a, .Rock), 0)
	testing.expect_value(t, game.item_count_of(w, a, .Stick), 1)
	// a torch at the fire
	game.msg_clear(w)
	game.do_terrain_verb(w, .Make_Torch)
	testing.expect_value(t, said(w), "To make a torch, | Loxy needs: | Charcoal: 0/1 | Stick: 1/1")
	give(w, .Charcoal)
	game.msg_clear(w)
	game.do_terrain_verb(w, .Make_Torch)
	testing.expect_value(t, said(w), "Loxy makes a torch.")
	testing.expect_value(t, game.item_count_of(w, a, .Torch), 1)
	// cooking needs the fire
	give(w, .Rat_Corpse, 1)
	game.msg_clear(w)
	game.do_item_effect(w, .Rat_Corpse, .Cook_Rat_Corpse)
	testing.expect_value(t, said(w), "To cook a rat corpse, | Loxy needs: | Rat Corpse: 1/1 | Stick: 0/2") // the sticks went into the torch
	give(w, .Stick, 2)
	game.msg_clear(w)
	game.do_item_effect(w, .Rat_Corpse, .Cook_Rat_Corpse)
	testing.expect_value(t, said(w), "Loxy cooks a rat corpse.")
	testing.expect_value(t, game.item_count_of(w, a, .Cooked_Rat), 1)
	testing.expect_value(t, game.item_count_of(w, a, .Stick), 1) // two in, one back
	// put the fire out: charcoal on the ground, bare ground again
	game.msg_clear(w)
	game.do_terrain_verb(w, .Put_Out_Fire)
	testing.expect_value(t, said(w), "Loxy extinguishes the fire.")
	testing.expect_value(t, w.cells[cell].terrain, game.Terrain_Type.Empty)
	testing.expect_value(t, ground_count(w, cell, .Charcoal), 1)
	// a verb the ground does not offer
	game.msg_clear(w)
	game.do_terrain_verb(w, .Cook_Bagel)
	testing.expect_value(t, said(w), "Nothing happens.")
	// the furnace
	give(w, .Rock, 10); give(w, .Clay, 10); give(w, .Stick, 9)
	game.msg_clear(w)
	game.do_terrain_verb(w, .Build_Furnace)
	testing.expect_value(t, said(w), "Loxy builds a furnace.")
	testing.expect_value(t, w.cells[cell].terrain, game.Terrain_Type.Furnace)
	testing.expect_value(t, game.item_count_of(w, a, .Torch), 1) // the torch is kept
	validated(t, w)
}

@(test)
cooking_and_mixing :: proc(t: ^testing.T) {
	w := new_hero()
	defer free(w)
	a := w.avatar
	stats(w).energy = 10
	// flour: 5 wheat and 2 rock (the rocks stay), 1 energy
	give(w, .Wheat, 5); give(w, .Rock, 2)
	game.do_item_effect(w, .Wheat, .Mill_Wheat)
	testing.expect_value(t, said(w), "Loxy makes flour.")
	testing.expect_value(t, sfx_of_current(w), game.Sfx_Id.Craft)
	testing.expect_value(t, stats(w).energy, 9)
	testing.expect_value(t, game.item_count_of(w, a, .Rock), 2)
	// dough: flour + water pot, 2 energy; the pot comes back empty
	give(w, .Water_Pot)
	game.msg_clear(w)
	game.do_item_effect(w, .Flour, .Make_Dough)
	testing.expect_value(t, said(w), "Loxy makes dough.")
	testing.expect_value(t, game.item_count_of(w, a, .Clay_Pot), 1)
	testing.expect_value(t, stats(w).energy, 7)
	// not enough energy
	stats(w).energy = 1
	give(w, .Pepper)
	give(w, .Rock, 2)
	game.msg_clear(w)
	game.do_item_effect(w, .Smoked_Pepper, .Make_Paprika) // none yet: nothing to run
	testing.expect(t, !game.msg_has(w), "no smoked pepper to use")
	// a tail from a corpse needs a sharp rock and 2 energy
	give(w, .Rat_Corpse)
	game.msg_clear(w)
	game.do_item_effect(w, .Rat_Corpse, .Cut_Off_Tail)
	testing.expect_value(t, said(w), "To cut off a rat's tail, | Loxy needs: | Rat Corpse: 1/1 | Sharp Rock: 0/1")
	give(w, .Sharp_Rock)
	game.msg_clear(w)
	game.do_item_effect(w, .Rat_Corpse, .Cut_Off_Tail)
	testing.expect_value(t, said(w), "Loxy doesn't have the energy to cut off a rat's tail.")
	testing.expect_value(t, sfx_of_current(w), game.Sfx_Id.Failure)
	stats(w).energy = 5
	game.msg_clear(w)
	game.do_item_effect(w, .Rat_Corpse, .Cut_Off_Tail)
	testing.expect_value(t, said(w), "Loxy cut off a rat's tail.")
	testing.expect_value(t, game.item_count_of(w, a, .Rat_Tail), 1)
	testing.expect_value(t, game.item_count_of(w, a, .Rat_Body), 1)
	testing.expect_value(t, game.item_count_of(w, a, .Sharp_Rock), 1)
}

@(test)
eating :: proc(t: ^testing.T) {
	w := new_hero()
	defer free(w)
	a := w.avatar
	game.set_max_health(w, a, 10)
	stats(w).health = 3
	give(w, .Cooked_Rat)
	game.do_item_effect(w, .Cooked_Rat, .Eat_Cooked_Rat)
	testing.expect_value(t, said(w), "Cooked Rat restores 2 health! | Loxy now has 5/10 health")
	testing.expect_value(t, sfx_of_current(w), game.Sfx_Id.Tasty)
	testing.expect_value(t, game.item_count_of(w, a, .Cooked_Rat), 0)
	// the energy herb
	stats(w).energy = 2
	give(w, .Energy_Herb)
	game.msg_clear(w)
	game.do_item_effect(w, .Energy_Herb, .Use_Energy_Herb)
	testing.expect_value(t, said(w), "Loxy eats the Energy Herb. | Loxy regains energy! | Loxy now has 10/10 energy.")
	testing.expect_value(t, stats(w).energy, 10)
	// corpses heal or hurt by 1; both happen, and the avatar can die of it
	healed, hurt, died := 0, 0, 0
	for seed in u64(1) ..= 80 {
		w2 := new_hero(seed)
		game.set_max_health(w2, w2.avatar, 10)
		stats(w2).health = 5
		give(w2, .Rat_Corpse)
		game.do_item_effect(w2, .Rat_Corpse, .Eat_Rat_Corpse)
		line := said(w2)
		switch stats(w2).health {
		case 6: healed += 1
		case 4: hurt += 1
		}
		_ = line
		free(w2)
	}
	testing.expect(t, healed > 10 && hurt > 10, "a corpse is a coin toss")
	for seed in u64(1) ..= 80 {
		w2 := new_hero(seed)
		stats(w2).health = 1
		give(w2, .Rat_Body)
		game.do_item_effect(w2, .Rat_Body, .Eat_Rat_Corpse)
		if game.is_dead(w2, w2.avatar) {
			died += 1
			testing.expect_value(t, said(w2), "Rat Body is tainted! | Loxy loses 1 health! | Loxy dies.")
		}
		free(w2)
	}
	testing.expect(t, died > 10, "tainted food can kill")
}

@(test)
spicy_food_is_worth_xp_half_the_time :: proc(t: ^testing.T) {
	spicy, plain := 0, 0
	for seed in u64(1) ..= 100 {
		w := new_hero(seed)
		give(w, .Pepper)
		game.do_item_effect(w, .Pepper, .Eat_Pepper)
		text := said(w)
		if strings.contains(text, "spicy") {
			spicy += 1
			testing.expect_value(t, stats(w).xp, 1)
			testing.expect_value(t, text, "Loxy eats the pepper. | That was a spicy one! | Loxy gains 1 XP! | Loxy needs 9 for the next level.")
		} else {
			plain += 1
			testing.expect_value(t, text, "Loxy eats the pepper.")
		}
		free(w)
	}
	testing.expect(t, spicy > 25 && plain > 25, "about half")
	// seasoned rat: the spicy line shows once, in its own message, and only when spicy (audit D13)
	for seed in u64(1) ..= 60 {
		w := new_hero(seed)
		stats(w).health = 1
		give(w, .Seasoned_Rat)
		game.do_item_effect(w, .Seasoned_Rat, .Eat_Seasoned_Rat)
		game.msg_dismiss(w)
		if game.msg_has(w) {
			testing.expect_value(t, said(w), "That was a spicy one! | Loxy gains 1 XP! | Loxy needs 9 for the next level.")
		}
		free(w)
	}
}

@(test)
sounds_for_taking_and_levelling :: proc(t: ^testing.T) {
	w := new_hero()
	defer free(w)
	stand_on(w, .Grass)
	game.put_item_on_ground(w, w.chars[w.avatar].cell, game.item_create(w, .Rock))
	game.take_items(w, .Rock, 1)
	out: [game.MAX_SFX]game.Sfx_Id
	n := game.sfx_drain(w, out[:])
	testing.expect_value(t, n, 1)
	testing.expect_value(t, out[0], game.Sfx_Id.Take)
	testing.expect_value(t, game.sfx_drain(w, out[:]), 0)
	// a level up (through a kill message) plays WooHoo
	m := game.msg_begin(w)
	game.award_xp(w, w.avatar, m, 10)
	n = game.sfx_drain(w, out[:])
	testing.expect_value(t, n, 1)
	testing.expect_value(t, out[0], game.Sfx_Id.Woo_Hoo)
	// no gain, no sound
	game.award_xp(w, w.avatar, m, 1)
	testing.expect_value(t, game.sfx_drain(w, out[:]), 0)
}

@(test)
the_signs_have_real_text :: proc(t: ^testing.T) {
	w := new_hero()
	defer free(w)
	for s in game.TOWN_SIGNS {
		game.msg_clear(w)
		place(w, .Town, s.col, s.row)
		game.run_effect(w, w.avatar, {kind = .Message, message = s.message})
		testing.expect(t, !strings.contains(said(w), "This is sign"), "no placeholder text is left")
	}
	game.msg_clear(w)
	game.run_effect(w, w.avatar, {kind = .Message, message = .Town_Sign8})
	testing.expect_value(t, said(w), "Bagel Appreciation Society | Meetings every never.")
}

// All the way to the bagel, driven through the same calls the screens make, with the raw materials handed out.
@(test)
the_whole_way_to_a_bagel :: proc(t: ^testing.T) {
	w := new_hero(3)
	defer free(w)
	a := w.avatar
	stats(w).ap = 6
	// the Druid: every skill
	for k in game.SKILL_KINDS {
		stats(w).ap = max(stats(w).ap, 1)
		#partial switch k {
		case .Learn_Twine_Making: give(w, .Plant_Fiber, 2)
		case .Learn_Knapping: give(w, .Rock, 2)
		case .Learn_Fire_Making: give(w, .Rock, 5); give(w, .Stick, 5)
		case .Learn_Torch_Making: give(w, .Charcoal); give(w, .Stick)
		case .Learn_Hatched_Making: give(w, .Twine); give(w, .Sharp_Rock); give(w, .Stick)
		case:
		}
		game.run_effect(w, a, {kind = k})
		testing.expect(t, game.has_flag(w, a, game.skill_of(k).flag), "skill learned")
	}
	game.msg_clear(w)
	stats(w).max_energy = 40
	stats(w).energy = 40
	// start the wild part with a clean pack (the skills above were handed their materials)
	for id := w.chars[a].first_item; id != 0; { next := w.items[id].next; game.item_recycle(w, id); id = next }
	// the potter: a pot (20 jools, 5 fibre, 5 clay), filled at a river
	give(w, .Plant_Fiber, 5); give(w, .Clay, 5)
	game.run_effect(w, a, {kind = .Potter_Make_Pot})
	testing.expect_value(t, game.item_count_of(w, a, .Clay_Pot), 1)
	game.run_effect(w, a, {kind = .Fill_Clay_Pot})
	testing.expect_value(t, game.item_count_of(w, a, .Water_Pot), 1)
	// out in the wild: a fire, charcoal from it, a torch
	stand_on(w, .Empty)
	give(w, .Rock, 12); give(w, .Stick, 30); give(w, .Clay, 10); give(w, .Wheat, 5)
	game.do_terrain_verb(w, .Build_Fire)
	testing.expect_value(t, w.cells[w.chars[a].cell].terrain, game.Terrain_Type.Cooking_Fire)
	game.do_terrain_verb(w, .Put_Out_Fire)
	game.take_items(w, .Charcoal, 1)
	testing.expect_value(t, game.item_count_of(w, a, .Charcoal), 1)
	game.msg_clear(w)
	game.run_effect(w, a, {kind = .Build_Fire})   // a second fire to make the torch at
	game.do_terrain_verb(w, .Make_Torch)
	testing.expect_value(t, game.item_count_of(w, a, .Torch), 1)
	game.do_terrain_verb(w, .Put_Out_Fire)
	game.take_items(w, .Charcoal, 1)
	// a furnace on bare ground (needs the torch, which it keeps), then a second torch for the bagel
	give(w, .Rock, 8)
	game.do_terrain_verb(w, .Build_Furnace)
	testing.expect_value(t, w.cells[w.chars[a].cell].terrain, game.Terrain_Type.Furnace)
	// flour, dough (two rocks to grind with: the furnace took the rest)
	give(w, .Rock, 2)
	game.do_item_effect(w, .Wheat, .Mill_Wheat)
	game.do_item_effect(w, .Flour, .Make_Dough)
	testing.expect_value(t, game.item_count_of(w, a, .Dough), 1)
	// the bagel needs its own torch: make one at another fire
	stand_on(w, .Cooking_Fire)
	give(w, .Charcoal)
	game.do_terrain_verb(w, .Make_Torch)
	testing.expect_value(t, game.item_count_of(w, a, .Torch), 2)
	stand_on(w, .Furnace)
	game.msg_clear(w)
	testing.expect(t, game.can_cook_bagel(w, a), "ready to cook")
	testing.expect(t, !game.has_won(w), "not yet")
	game.do_terrain_verb(w, .Cook_Bagel)
	testing.expect_value(t, said(w), "Loxy cooks a bagel.")
	testing.expect(t, game.has_won(w), "a bagel in the pack is a win")
	validated(t, w)
}
