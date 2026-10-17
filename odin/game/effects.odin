package game

// Effects: what bumping a cell, picking a dialogue choice, using an item or doing a crafting verb does for the avatar.
// The original ran a Lua script per effect type (the avatar's script table, Content/Scripts/Characters/Loxy); here each is a case
// of one switch over Effect_Kind, which tools/gen/gen_content.py generates from exactly that table, so a missing case is a
// compile error. Texts are the originals', with the fixes listed in docs/defect-audit.md (D1 to D5, D13, D14).
// The sounds are the C6 decision: each message carries the sound that fits (see the audit's sound map).

// Everything an effect may need. Cell effects fill dest/message/number/cell; item verbs fill item_id; choices fill item/number/again.
Effect_Call :: struct {
	kind:    Effect_Kind,
	dest:    Cell_Id,
	message: Message_Type,
	number:  i32,
	cell:    Cell_Id,
	item_id: Item_Id,
	item:    Item_Type,
	again:   Effect_Kind,
}

run_effect :: proc(w: ^World, c: Char_Id, call: Effect_Call) {
	switch call.kind {
	case .None, .Exit_Dialog:
	// ---- plumbing
	case .Teleport: teleport(w, c, call.dest)
	case .Message: msg_canned(w, call.message)
	case .Enter_Cellar: enter_cellar(w, c, call)
	case .Forage: // superseded by the forage grid (the original's cells never carried this effect)
	// ---- dialogue only
	case .Gorachan_Talk: gorachan_talk(w, c)
	case .Pervert_Innkeeper: msg_line(msg_line(msg_begin(w), .Light_Gray, "I'm not a pervert!"), .Light_Gray, "I'm just Australian!")
	case .Druid_Talk: druid_talk(w)
	case .Druid_Allergies: msg_line(msg_begin(w), .Light_Gray, "Alas, I have allergies.")
	case .Druid_Prices: druid_prices(w, c)
	case .Healer_Talk: healer_talk(w)
	case .Nihilist_Prices: msg_line(msg_line(msg_begin(w), .Light_Gray, "I don't sell anything."), .Light_Gray, "I'm a nihilist, remember?")
	case .Energy_Trainer_Talk: energy_trainer_talk(w, c)
	case .Health_Trainer_Talk: health_trainer_talk(w, c)
	case .Potter_Talk: potter_talk(w)
	case .Potter_Flavor_Text: potter_flavor_text(w)
	case .Start_Rat_Quest: start_rat_quest(w)
	// ---- quest and economy
	case .Accept_Rat_Quest: set_flag(w, c, .Rat_Quest, true)
	case .Complete_Rat_Quest: complete_rat_quest(w, c)
	case .Pay_Innkeeper: pay_innkeeper(w, c)
	case .Sleep_At_Inn: sleep_at_inn(w, c)
	case .Buy: buy(w, c, call)
	case .Heal: heal_at_basin(w, c, call.number)
	case .Train_Energy: train_energy(w, c)
	case .Train_Health: train_health(w, c)
	case .Potter_Make_Pot: potter_make_pot(w, c)
	case .Bump_River: bump_river(w, c)
	case .Fill_Clay_Pot: fill_clay_pot(w, c)
	// ---- skills
	case .Druid_Teach_Menu: druid_teach_menu(w, c)
	case .Learn_Foraging, .Learn_Knapping, .Learn_Twine_Making, .Learn_Fire_Making, .Learn_Torch_Making, .Learn_Hatched_Making:
		learn_skill(w, c, call.kind)
	// ---- crafting verbs
	case .Build_Fire: build_fire(w, c)
	case .Build_Furnace: build_furnace(w, c)
	case .Put_Out_Fire: put_out_fire(w, c, call.cell)
	case .Make_Torch: make_torch(w, c)
	case .Cook_Bagel: cook_furnace_recipe(w, c, .Bagel, "cook a bagel", "cooks a bagel")
	case .Cook_Rat_Body: cook_recipe(w, c, .Cooked_Rat_Body, "cook a rat body", "cooks a rat body")
	case .Cook_Rat_Corpse: cook_recipe(w, c, .Cooked_Rat_Corpse, "cook a rat corpse", "cooks a rat corpse")
	case .Smoke_Pepper: cook_recipe(w, c, .Smoked_Pepper, "smoke a pepper", "smokes a pepper")
	case .Season_Rat: do_recipe(w, c, 0, .Seasoned_Rat, "season a rat", "seasons a rat")
	case .Make_Hatchet: do_recipe(w, c, 1, .Hatchet, "make a hatchet", "makes a hatchet")
	case .Make_Paprika: do_recipe(w, c, 1, .Paprika, "make paprika", "makes paprika")
	case .Mill_Wheat: do_recipe(w, c, 1, .Flour, "make flour", "makes flour")
	case .Make_Dough: do_recipe(w, c, 2, .Dough, "make dough", "makes dough")
	case .Cut_Off_Tail: do_recipe(w, c, 2, .Rat_Tail, "cut off a rat's tail", "cut off a rat's tail")
	// ---- eating
	case .Eat_Cooked_Rat: eat_cooked(w, c, call.item_id)
	case .Eat_Seasoned_Rat: eat_seasoned(w, c, call.item_id)
	case .Eat_Rat_Corpse: eat_rat_corpse(w, c, call.item_id)
	case .Eat_Pepper: eat_pepper(w, c, call.item_id, "pepper")
	case .Eat_Smoked_Pepper: eat_pepper(w, c, call.item_id, "smoked pepper")
	case .Use_Energy_Herb: use_energy_herb(w, c, call.item_id)
	}
}

// Move the character to another cell without any of the movement rules (the original's doTeleport).
teleport :: proc(w: ^World, c: Char_Id, dest: Cell_Id) {
	if dest == NO_CELL { return }
	char_leave_cell_list(w, c)
	char_enter_cell_list(w, c, dest)
}

// The avatar picks choice `index` of the current message: its effect runs, then the message is dismissed (so anything the
// effect said is shown next). The same order as the original's AvatarModel.MakeChoice.
message_choose :: proc(w: ^World, index: int) {
	m := msg_current(w)
	if m == nil || index < 0 || index >= int(m.choice_count) { return }
	ch := m.choices[index] // a copy: the effect may add messages
	run_effect(w, w.avatar, {kind = ch.effect, item = ch.item, number = ch.number, again = ch.again})
	msg_dismiss(w)
}

// ---- entry points for the screens -----------------------------------------------------------------------------------------------
TERRAIN_VERB_EFFECT := [Terrain_Verb]Effect_Kind{
	.Build_Fire = .Build_Fire, .Build_Furnace = .Build_Furnace, .Put_Out_Fire = .Put_Out_Fire, .Make_Torch = .Make_Torch, .Cook_Bagel = .Cook_Bagel,
}

// A crafting verb done where the avatar stands: the terrain must offer it, else "Nothing happens."
do_terrain_verb :: proc(w: ^World, verb: Terrain_Verb) {
	cell := w.chars[w.avatar].cell
	if !cell_has_verb(w, cell, verb) { msg_canned(w, .Nothing_Happens); return }
	run_effect(w, w.avatar, {kind = TERRAIN_VERB_EFFECT[verb], cell = cell})
}

// An item verb from the inventory menu (Eat, Cook, Mill, ...) on the first carried item of that type.
do_item_effect :: proc(w: ^World, type: Item_Type, kind: Effect_Kind) {
	for id := w.chars[w.avatar].first_item; id != 0; id = w.items[id].next {
		if w.items[id].type == type { run_effect(w, w.avatar, {kind = kind, item_id = id}); return }
	}
}

// ---- helpers --------------------------------------------------------------------------------------------------------------------------
// "Plant Fiber: 3/5" for each input of a recipe.
need_lines :: proc(w: ^World, c: Char_Id, m: ^Message, r: Recipe_Type) {
	for in_ in recipe_inputs(r) {
		msg_line(m, .Light_Gray, "%s: %d/%d", ITEMS[in_.item].name, item_count_of(w, c, in_.item), in_.count)
	}
}

// A refusal: one message, with the "Shucks" sound.
refuse :: proc(w: ^World) -> ^Message {
	m := msg_begin(w)
	m.sfx = SFX_FAILURE
	return m
}

consume_energy :: proc(w: ^World, c: Char_Id, cost: i32, action: string) -> bool {
	if w.chars[c].stats.energy < cost {
		msg_line(refuse(w), .Light_Gray, "%s doesn't have the energy to %s.", char_name(w, c), action)
		return false
	}
	add_energy(w, c, -cost)
	return true
}

// "To make dough, Loxy needs: ..." unless the recipe can be made.
can_do_recipe :: proc(w: ^World, c: Char_Id, r: Recipe_Type, task: string) -> bool {
	if can_craft(w, c, r) { return true }
	m := refuse(w)
	msg_line(m, .Light_Gray, "To %s,", task)
	msg_line(m, .Light_Gray, "%s needs:", char_name(w, c))
	need_lines(w, c, m, r)
	return false
}

done_message :: proc(w: ^World, c: Char_Id, sfx: Sfx_Id, format: string, args: ..any) {
	m := msg_begin(w)
	m.sfx = sfx
	msg_line(m, .Light_Gray, format, ..args)
}

do_recipe :: proc(w: ^World, c: Char_Id, cost: i32, r: Recipe_Type, task, result: string) {
	if !can_do_recipe(w, c, r, task) { return }
	if !consume_energy(w, c, cost, task) { return }
	craft(w, c, r)
	done_message(w, c, .Craft, "%s %s.", char_name(w, c), result)
}

cook_recipe :: proc(w: ^World, c: Char_Id, r: Recipe_Type, task, result: string) {
	if !terrain_flag(w, w.chars[c].cell, .Has_Fire) {
		msg_line(refuse(w), .Light_Gray, "%s needs a fire to %s.", char_name(w, c), task)
		return
	}
	do_recipe(w, c, 0, r, task, result)
}

cook_furnace_recipe :: proc(w: ^World, c: Char_Id, r: Recipe_Type, task, result: string) {
	if !terrain_flag(w, w.chars[c].cell, .Is_Furnace) {
		msg_line(refuse(w), .Light_Gray, "%s needs a furnace to %s.", char_name(w, c), task)
		return
	}
	do_recipe(w, c, 0, r, task, result)
}

// ---- the dialogues ----------------------------------------------------------------------------------------------------------------------
enter_cellar :: proc(w: ^World, c: Char_Id, call: Effect_Call) {
	if has_flag(w, c, .Rat_Quest) {
		teleport(w, c, call.dest)
	} else {
		msg_line(msg_begin(w), .Light_Gray, "%s has no business in the cellar.", char_name(w, c))
	}
}

gorachan_talk :: proc(w: ^World, c: Char_Id) {
	m := msg_begin(w)
	msg_line(m, .Light_Gray, "Welcome to Jusdatip Inn!")
	msg_line(m, .Light_Gray, "I'm Gorachan.")
	msg_line(m, .Light_Gray, "You can rest in a bed for 1 jools.")
	msg_line(m, .Light_Gray, "I'd offer to join you,")
	msg_line(m, .Light_Gray, "but then you wouldn't get any rest!")
	msg_choice(m, .Exit_Dialog, "Cool story, bro!")
	msg_choice(m, .Pervert_Innkeeper, "Yer a pervert!")
	msg_choice(m, .Pay_Innkeeper, "I'll take a bed.")
	if has_flag(w, c, .Rat_Quest) {
		if item_count_of(w, c, .Rat_Tail) > 0 { msg_choice(m, .Complete_Rat_Quest, "Here's some rat tails!") }
	} else {
		msg_choice(m, .Start_Rat_Quest, "I need a job!")
	}
}

start_rat_quest :: proc(w: ^World) {
	m := msg_begin(w)
	msg_line(m, .Light_Gray, "Well, there are a bunch of rats in the cellar.")
	msg_line(m, .Light_Gray, "I'll pay you 1 jools for each rat tail.")
	msg_line(m, .Light_Gray, "I only accept the tails, no corpses.")
	msg_line(m, .Light_Gray, "So you'll need to cut them off first.")
	msg_choice(m, .Accept_Rat_Quest, "I'm on it!")
	msg_choice(m, .Exit_Dialog, "Mebbe later?")
}

complete_rat_quest :: proc(w: ^World, c: Char_Id) {
	jools: i32
	for id := w.chars[c].first_item; id != 0; {
		next := w.items[id].next
		if w.items[id].type == .Rat_Tail { jools += 1; item_recycle(w, id) }
		id = next
	}
	add_jools(w, c, jools)
	done_message(w, c, .Woo_Hoo, "%s receives %d jools.", char_name(w, c), jools)
}

pay_innkeeper :: proc(w: ^World, c: Char_Id) {
	BED_COST :: 1
	if has_flag(w, c, .Paid_Innkeeper) { msg_line(msg_begin(w), .Light_Gray, "You've already paid!"); return }
	if w.chars[c].stats.jools < BED_COST { msg_line(refuse(w), .Light_Gray, "Sorry! No jools, no bed!"); return }
	add_jools(w, c, -BED_COST)
	set_flag(w, c, .Paid_Innkeeper, true)
	m := msg_begin(w)
	msg_line(m, .Light_Gray, "Thanks for yer business.")
	msg_line(m, .Light_Gray, "Choose any bed you like.")
}

sleep_at_inn :: proc(w: ^World, c: Char_Id) {
	if !has_flag(w, c, .Paid_Innkeeper) {
		msg_line(refuse(w), .Light_Gray, "%s needs to pay Gorachan first!", char_name(w, c))
		return
	}
	set_flag(w, c, .Paid_Innkeeper, false)
	s := &w.chars[c].stats
	add_energy(w, c, s.max_energy - s.energy)
	m := msg_begin(w)
	msg_line(m, .Light_Gray, "%s rests and feels refreshed!", char_name(w, c))
	msg_line(m, .Light_Gray, "%s has %d/%d energy.", char_name(w, c), s.energy, s.max_energy)
}

druid_talk :: proc(w: ^World) {
	m := msg_begin(w)
	msg_line(m, .Light_Gray, "Greetings! I am Marcus, the hippy druid.")
	msg_line(m, .Light_Gray, "I can help you learn nature's way.")
	msg_choice(m, .Exit_Dialog, "Cool story, bro!")
	msg_choice(m, .Druid_Allergies, "Don't druids live in the woods?")
	msg_choice(m, .Druid_Teach_Menu, "Teach me!")
	msg_choice(m, .Druid_Prices, "What's for sale?")
}

ENERGY_HERB_PRICE :: 5

druid_prices :: proc(w: ^World, c: Char_Id) {
	m := msg_begin(w)
	msg_line(m, .Light_Gray, "I sell a variety of herbs.")
	msg_line(m, .Light_Gray, "(%s has %d jools)", char_name(w, c), w.chars[c].stats.jools)
	msg_choice(m, .Exit_Dialog, "Good to know!")
	buy_choice := msg_choice(m, .Buy, "Buy Energy Herb(%d jools)", ENERGY_HERB_PRICE)
	buy_choice.item, buy_choice.number, buy_choice.again = .Energy_Herb, ENERGY_HERB_PRICE, .Druid_Prices
}

// A purchase: the choice carries what, the price and which dialogue to reopen.
buy :: proc(w: ^World, c: Char_Id, call: Effect_Call) {
	if w.chars[c].stats.jools < call.number {
		m := refuse(w)
		msg_line(m, .Light_Gray, "You don't have enough!")
		msg_choice(m, call.again, "Shucks!")
		return
	}
	add_jools(w, c, -call.number)
	if id := item_create(w, call.item); id != 0 { give_item(w, c, id) }
	m := msg_begin(w)
	m.sfx = .Yoink
	msg_line(m, .Light_Gray, "Thank you for yer purchase!")
	msg_choice(m, call.again, "No worries!")
}

healer_talk :: proc(w: ^World) {
	m := msg_begin(w)
	msg_line(m, .Light_Gray, "Welcome to the Nihilistic House of Healing.")
	msg_line(m, .Light_Gray, "If you go to the basin and wash,")
	msg_line(m, .Light_Gray, "you will be healed,")
	msg_line(m, .Light_Gray, "but it will cost you half of yer jools.")
	msg_line(m, .Light_Gray, "Not that I care or anything,")
	msg_line(m, .Light_Gray, "because I'm a nihilist.")
	msg_choice(m, .Exit_Dialog, "Cool story, bro!")
	msg_choice(m, .Nihilist_Prices, "What's for sale?")
}

// The basin heals up to `cap` health (5 at the town healer, whatever the character's maximum) and takes half the jools.
heal_at_basin :: proc(w: ^World, c: Char_Id, cap: i32) {
	s := &w.chars[c].stats
	target := min(s.max_health, cap)
	if s.health >= target { msg_line(msg_begin(w), .Light_Gray, "Nothing happens!"); return } // (with its own "!", unlike the canned message)
	set_health(w, c, target)
	m := msg_begin(w)
	m.sfx = .Woo_Hoo
	msg_line(m, .Light_Gray, "%s is healed!", char_name(w, c))
	msg_line(m, .Light_Gray, "%s now has %d health.", char_name(w, c), s.health)
	jools := s.jools / 2
	add_jools(w, c, -jools)
	if jools > 0 { msg_line(m, .Red, "%s loses %d jools!", char_name(w, c), jools) }
}

energy_trainer_talk :: proc(w: ^World, c: Char_Id) {
	m := msg_begin(w)
	msg_line(m, .Light_Gray, "I am the endurance trainer.")
	msg_line(m, .Light_Gray, "I can increase yer energy")
	msg_line(m, .Light_Gray, "for the cost of 1AP and %d jools.", w.chars[c].stats.max_energy * 2)
	msg_choice(m, .Exit_Dialog, "Cool story, bro!")
	msg_choice(m, .Train_Energy, "Train me!")
}

train_energy :: proc(w: ^World, c: Char_Id) {
	s := &w.chars[c].stats
	if s.ap < 1 {
		m := refuse(w)
		msg_line(m, .Light_Gray, "You need at least 1 AP.")
		msg_line(m, .Light_Gray, "Come back when yer more experienced!")
		return
	}
	cost := 2 * s.max_energy
	if s.jools < cost {
		m := refuse(w)
		msg_line(m, .Light_Gray, "The price is %d jools.", cost)
		msg_line(m, .Light_Gray, "I have overhead, you know.")
		return
	}
	add_ap(w, c, -1)
	add_jools(w, c, -cost)
	s.max_energy += 1
	add_energy(w, c, 1)
	m := msg_begin(w)
	m.sfx = .Woo_Hoo
	msg_line(m, .Red, "%s loses 1 AP", char_name(w, c))
	msg_line(m, .Red, "%s loses %d jools", char_name(w, c), cost)
	msg_line(m, .Green, "%s adds 1 Maximum Energy", char_name(w, c))
	msg_line(m, .Light_Gray, "Yer now at %d Maximum Energy.", s.max_energy)
	msg_line(m, .Light_Gray, "Persistence is futile!")
}

health_trainer_talk :: proc(w: ^World, c: Char_Id) {
	m := msg_begin(w)
	msg_line(m, .Light_Gray, "I am the health trainer!")
	msg_line(m, .Light_Gray, "I can help you increase yer health.")
	msg_line(m, .Light_Gray, "The cost is %d AP.", w.chars[c].stats.max_health * 5)
	msg_choice(m, .Exit_Dialog, "Cool story, bro!")
	msg_choice(m, .Train_Health, "Train me!")
}

train_health :: proc(w: ^World, c: Char_Id) {
	s := &w.chars[c].stats
	cost := 5 * s.max_health
	if s.ap < cost {
		m := refuse(w)
		msg_line(m, .Light_Gray, "To go from %d to %d Maximum Health,", s.max_health, s.max_health + 1)
		msg_line(m, .Light_Gray, "you need %d AP, but you have %d.", cost, s.ap)
		msg_line(m, .Light_Gray, "Come back when yer more experienced!")
		return
	}
	add_ap(w, c, -cost)
	set_max_health(w, c, s.max_health + 1)
	set_health(w, c, s.health + 1)
	m := msg_begin(w)
	m.sfx = .Woo_Hoo
	msg_line(m, .Red, "%s loses %d AP", char_name(w, c), cost)
	msg_line(m, .Green, "%s adds 1 Maximum Health", char_name(w, c))
	msg_line(m, .Light_Gray, "Yer now at %d Maximum Health.", s.max_health)
	msg_line(m, .Light_Gray, "Remember! if you don't have yer health,")
	msg_line(m, .Light_Gray, "you don't really have anything!")
}

potter_talk :: proc(w: ^World) {
	m := msg_begin(w)
	msg_line(m, .Light_Gray, "Greetings! I am Harold, the Potter.")
	msg_line(m, .Light_Gray, "I make pots! For jools!")
	msg_choice(m, .Exit_Dialog, "Cool story, bro!")
	msg_choice(m, .Potter_Flavor_Text, "I loved yer movies!")
	msg_choice(m, .Potter_Make_Pot, "Make me a pot!")
}

potter_flavor_text :: proc(w: ^World) {
	m := msg_begin(w)
	msg_line(m, .Light_Gray, "Um. Thanks!")
	msg_line(m, .Light_Gray, "...")
	msg_line(m, .Light_Gray, "What's a 'Movie'?")
	msg_choice(m, .Exit_Dialog, "Nevermind!")
}

POT_PRICE :: 20

potter_make_pot :: proc(w: ^World, c: Char_Id) {
	if w.chars[c].stats.jools < POT_PRICE {
		m := refuse(w)
		msg_line(m, .Light_Gray, "The price is %d jools.", POT_PRICE)
		msg_line(m, .Light_Gray, "%s has %d jools", char_name(w, c), w.chars[c].stats.jools)
		return
	}
	if !can_craft(w, c, .Unfired_Pot) {
		m := refuse(w)
		msg_line(m, .Light_Gray, "To make a pot, I need:")
		need_lines(w, c, m, .Unfired_Pot)
		return
	}
	add_jools(w, c, -POT_PRICE)
	craft(w, c, .Unfired_Pot)
	craft(w, c, .Clay_Pot)
	m := msg_begin(w)
	m.sfx = .Yoink
	msg_line(m, .Red, "%s loses %d jools", char_name(w, c), POT_PRICE)
	msg_line(m, .Light_Green, "%s gains 1 %s", char_name(w, c), ITEMS[.Clay_Pot].name)
}

bump_river :: proc(w: ^World, c: Char_Id) {
	m := msg_begin(w)
	msg_line(m, .Light_Gray, "%s visits the river bank.", char_name(w, c))
	msg_choice(m, .Exit_Dialog, "Cool story, bro!")
	if item_count_of(w, c, .Clay_Pot) > 0 { msg_choice(m, .Fill_Clay_Pot, "Fill Clay Pot with Water") }
}

fill_clay_pot :: proc(w: ^World, c: Char_Id) {
	if !craft(w, c, .Water_Pot) { return } // the pot is gone: nothing to say (the original claimed it was filled)
	done_message(w, c, .Craft, "%s fills a %s with water.", char_name(w, c), ITEMS[.Clay_Pot].name)
}

// ---- skills (the Druid) ---------------------------------------------------------------------------------------------------------------------
Skill :: struct {
	flag:     Char_Flag,
	task:     string, // "make a fire": "To learn to make a fire,"
	action:   string, // the Craft menu entry it unlocks
	label:    string, // the Druid's menu text
	recipe:   Recipe_Type,
	by_doing: bool,   // learning it also makes one (and uses the materials up)
	caveat:   string,
}

SKILL_AP_COST :: 1

// In the Druid's menu order.
SKILL_KINDS := [6]Effect_Kind{.Learn_Foraging, .Learn_Twine_Making, .Learn_Knapping, .Learn_Fire_Making, .Learn_Torch_Making, .Learn_Hatched_Making}

skill_of :: proc(kind: Effect_Kind) -> Skill {
	#partial switch kind {
	case .Learn_Foraging: return {.Knows_Foraging, "forage", "Forage...", "Foraging(-1AP)", .Foraging, false, ""}
	case .Learn_Twine_Making: return {.Knows_Twine_Making, "make twine", "Make Twine", "Twine Making(-1AP,-2 Plant Fiber)", .Twine, true, ""}
	case .Learn_Knapping: return {.Knows_Rock_Sharpening, "knap", "Knap", "Knapping(-1AP,-2 Rock)", .Sharp_Rock, true, ""}
	case .Learn_Fire_Making: return {.Knows_Fire_Making, "make a fire", "Build Fire", "Fire Making(-1AP, -5 Rock, -5 Sticks)", .Cooking_Fire, false, "(only works in clear areas in the wilderness)"}
	case .Learn_Torch_Making: return {.Knows_Torch_Making, "make a torch", "Make Torch", "Torch Making(-1AP, -1 Stick, -1 Charcoal)", .Torch, false, "(only works with a source of flames)"}
	case .Learn_Hatched_Making: return {.Knows_Hatchet_Making, "make a hatchet", "Make Hatchet", "Hatchet Making(-1AP,-1Stick,-1S.Rock,-1Twine)", .Hatchet, true, ""}
	}
	return {}
}

druid_teach_menu :: proc(w: ^World, c: Char_Id) {
	m := msg_begin(w)
	any_left := false
	for k in SKILL_KINDS { if !has_flag(w, c, skill_of(k).flag) { any_left = true } }
	if !any_left { msg_line(m, .Light_Gray, "You have learned all I have to teach you."); return }
	msg_line(m, .Light_Gray, "I can teach you these things:")
	msg_choice(m, .Exit_Dialog, "Good to know!")
	for k in SKILL_KINDS { if !has_flag(w, c, skill_of(k).flag) { msg_choice(m, k, "%s", skill_of(k).label) } }
}

learn_skill :: proc(w: ^World, c: Char_Id, kind: Effect_Kind) {
	sk := skill_of(kind)
	name := char_name(w, c)
	m := msg_begin(w)
	if has_flag(w, c, sk.flag) { msg_line(m, .Light_Gray, "%s already knows how to %s!", name, sk.task); return }
	if !can_craft(w, c, sk.recipe) {
		m.sfx = SFX_FAILURE
		msg_line(m, .Light_Gray, "To learn to %s,", sk.task)
		msg_line(m, .Light_Gray, "%s needs:", name)
		need_lines(w, c, m, sk.recipe)
		return
	}
	if w.chars[c].stats.ap < SKILL_AP_COST {
		m.sfx = SFX_FAILURE
		msg_line(m, .Light_Gray, "To learn to %s,", sk.task)
		msg_line(m, .Light_Gray, "%s needs %d AP,", name, SKILL_AP_COST)
		msg_line(m, .Light_Gray, "but has %d!", w.chars[c].stats.ap)
		return
	}
	add_ap(w, c, -SKILL_AP_COST)
	set_flag(w, c, sk.flag, true)
	if sk.by_doing { craft(w, c, sk.recipe) }
	m.sfx = .Woo_Hoo
	msg_line(m, .Light_Gray, "You now know how to %s!", sk.task)
	msg_line(m, .Light_Gray, "To do so, simply select '%s'", sk.action)
	msg_line(m, .Light_Gray, "from the Actions menu.")
	if len(sk.caveat) > 0 { msg_line(m, .Light_Gray, "%s", sk.caveat) }
}

// ---- building ----------------------------------------------------------------------------------------------------------------------------------
// Fire, furnace and torch spend their energy BEFORE checking for materials, as the original did (audit C14).
build_fire :: proc(w: ^World, c: Char_Id) {
	if !consume_energy(w, c, 1, "build a fire") { return }
	if !can_do_recipe(w, c, .Cooking_Fire, "build a fire") { return }
	craft(w, c, .Cooking_Fire)
	done_message(w, c, .Craft, "%s builds a small fire.", char_name(w, c))
	w.cells[w.chars[c].cell].terrain = .Cooking_Fire
}

build_furnace :: proc(w: ^World, c: Char_Id) {
	if !consume_energy(w, c, 1, "build a furnace") { return }
	if !can_do_recipe(w, c, .Furnace, "build a furnace") { return }
	craft(w, c, .Furnace)
	done_message(w, c, .Craft, "%s builds a furnace.", char_name(w, c))
	w.cells[w.chars[c].cell].terrain = .Furnace
}

make_torch :: proc(w: ^World, c: Char_Id) {
	if !consume_energy(w, c, 1, "make a torch") { return }
	if !can_do_recipe(w, c, .Torch, "make a torch") { return }
	craft(w, c, .Torch)
	done_message(w, c, .Craft, "%s makes a torch.", char_name(w, c))
}

// Put the fire out: charcoal (the terrain's data says what and into what) is left on the ground.
put_out_fire :: proc(w: ^World, c: Char_Id, cell: Cell_Id) {
	if !consume_energy(w, c, 1, "put out a fire") { return }
	done_message(w, c, .Craft, "%s extinguishes the fire.", char_name(w, c))
	info := TERRAINS[w.cells[cell].terrain]
	if info.put_out_drops != .None { if id := item_create(w, info.put_out_drops); id != 0 { put_item_on_ground(w, cell, id) } }
	w.cells[cell].terrain = info.put_out_into
}

// ---- eating ----------------------------------------------------------------------------------------------------------------------------------------
// Remove the item for good; returns its name (the table's string outlives the item).
consume_item :: proc(w: ^World, id: Item_Id) -> string {
	name := item_name(w, id)
	item_recycle(w, id)
	return name
}

do_healing :: proc(w: ^World, c: Char_Id, item: string, amount: i32, sfx: Sfx_Id) {
	set_health(w, c, w.chars[c].stats.health + amount)
	m := msg_begin(w)
	m.sfx = sfx
	msg_line(m, .Light_Gray, "%s restores %d health!", item, amount)
	msg_line(m, .Light_Gray, "%s now has %d/%d health", char_name(w, c), w.chars[c].stats.health, w.chars[c].stats.max_health)
}

// Half the time a spicy meal is worth 1 XP.
determine_spiciness :: proc(w: ^World, c: Char_Id, m: ^Message) {
	if rng_range(&w.rng, 0, 1) == 0 {
		msg_line(m, .Orange, "That was a spicy one!")
		award_xp(w, c, m, 1)
	}
}

eat_cooked :: proc(w: ^World, c: Char_Id, id: Item_Id) {
	if id == 0 { return }
	do_healing(w, c, consume_item(w, id), 2, .Tasty)
}

// (the original always added "That was a spicy one!" here and sometimes again: audit D13)
eat_seasoned :: proc(w: ^World, c: Char_Id, id: Item_Id) {
	if id == 0 { return }
	do_healing(w, c, consume_item(w, id), 2, .Tasty)
	if rng_range(&w.rng, 0, 1) == 0 {
		m := msg_begin(w)
		msg_line(m, .Orange, "That was a spicy one!")
		award_xp(w, c, m, 1)
	}
}

eat_pepper :: proc(w: ^World, c: Char_Id, id: Item_Id, what: string) {
	if id == 0 { return }
	consume_item(w, id)
	m := msg_begin(w)
	m.sfx = .Eat
	msg_line(m, .Light_Gray, "%s eats the %s.", char_name(w, c), what)
	determine_spiciness(w, c, m)
}

// A corpse or a body: half the time it heals 1, half the time it is tainted and costs 1 health (which can kill).
eat_rat_corpse :: proc(w: ^World, c: Char_Id, id: Item_Id) {
	if id == 0 { return }
	name := consume_item(w, id)
	if rng_range(&w.rng, 0, 1) == 0 {
		do_healing(w, c, name, 1, .Eat)
		return
	}
	set_health(w, c, w.chars[c].stats.health - 1)
	m := msg_begin(w)
	m.sfx = .Eat
	msg_line(m, .Light_Gray, "%s is tainted!", name)
	msg_line(m, .Light_Gray, "%s loses 1 health!", char_name(w, c))
	if is_dead(w, c) {
		msg_line(m, .Red, "%s dies.", char_name(w, c))
	} else {
		msg_line(m, .Light_Gray, "%s now has %d/%d health", char_name(w, c), w.chars[c].stats.health, w.chars[c].stats.max_health)
	}
}

ENERGY_HERB_BENEFIT :: 10

use_energy_herb :: proc(w: ^World, c: Char_Id, id: Item_Id) {
	if id == 0 { return }
	name := consume_item(w, id)
	add_energy(w, c, ENERGY_HERB_BENEFIT)
	m := msg_begin(w)
	m.sfx = .Eat
	msg_line(m, .Light_Gray, "%s eats the %s.", char_name(w, c), name)
	msg_line(m, .Light_Gray, "%s regains energy!", char_name(w, c))
	msg_line(m, .Light_Gray, "%s now has %d/%d energy.", char_name(w, c), w.chars[c].stats.energy, w.chars[c].stats.max_energy)
}

// The Craft menu's twine, sharp rock and hatchet (the original's DoMakeItem): no energy, and the usual "needs" list when short.
// (The Make_Hatchet effect above, which costs 1 energy, is never reached from a menu: audit D17.)
do_make_item :: proc(w: ^World, c: Char_Id, r: Recipe_Type, noun: string) {
	if !can_craft(w, c, r) {
		m := refuse(w)
		msg_line(m, .Light_Gray, "To make %s,", noun)
		msg_line(m, .Light_Gray, "%s needs:", char_name(w, c))
		need_lines(w, c, m, r)
		return
	}
	craft(w, c, r)
	done_message(w, c, .Craft, "%s makes %s.", char_name(w, c), noun)
}
