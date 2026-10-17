#!/usr/bin/env python3
"""Generates odin/game/content_data.odin and odin/platform/web/page/keys.js from the original game's data (stdlib only).

Strict by design (HamQuest pattern): every property of every JSON object is consumed through a schema, so an unknown
property fails the run; every cross reference (item, terrain, character, effect, sound, message) is checked; Lua files that
carry data are parsed with a fixed pattern and anything else fails. The lists IGNORED and DATA_FIXES below are the COMPLETE
set of departures from the source data. Behaviour that lives in Lua (the effect scripts) is NOT generated here: it is
written by hand in Odin (phase 4), keyed by the Effect_Kind enum this file produces.

Sources: src/BagelQuest/Content/{itemTypes,terrainTypes,characterTypes,effectTypes,equipSlots,recipes,messages,sfx,mux,keys}.json
and the Scripts/ tree. CONTENT_HASH (FNV-1a 64 of the generated text) goes into saves so a save only loads against the
content that made it."""
import os, re, sys
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from common import ROOT, CONTENT, HUE_NAMES, load, read_text, snake, Strict

# ---- departures from the source data (complete list) ---------------------------------------------------------------
IGNORED = {
    # dead in the original (docs/defect-audit.md, "Dead code")
    "terrain effect Forage": "superseded by the forage grid; ForagingModel does not read the terrain effect",
    "messages HealerTalk, HealthTrainerTalk, DruidTalk, TownSign4": "stale: the Lua scripts replace the first three (different text); TownSign4 is never placed",
    "Content/Config/keys.json": "an older, unused copy of the key table (lacks Q and Z); only checked to be a subset of keys.json",
    "characterTypes InitializeScript": "hand-written in Odin (maps.odin init_character): give items, roll jools; the files are checked to exist",
    "effectTypes entries without an item that uses them": "kept as display names only for item effects",
    "font.json, hue.json, Fonts/*": "handled by gen_assets.py",
}
DATA_FIXES = {
    "Items/RatCorpse/FullName.lua": 'returns "test"; treated as no script (plain name), defect D6',
}
# User decision C2 (phase 4): the four placeholder signs get real text. They stand next to the four decorative houses.
MESSAGE_TEXT_FIXES = {
    "TownSign2": ["Home of Barry the Forager", "(Barry is out foraging. Again.)"],
    "TownSign5": ["For Rent: One Hut", "Inquire within. Nobody is within."],
    "TownSign6": ["Beware of Rat", "(it is a very small rat)"],
    "TownSign8": ["Bagel Appreciation Society", "Meetings every never."],
}
DATA_FIXES["messages TownSign2, TownSign5, TownSign6, TownSign8"] = "placeholder text 'This is sign #N' replaced (user decision C2)"
DEAD_MESSAGES = {"HealerTalk", "HealthTrainerTalk", "DruidTalk", "TownSign4"}

def fnv1a64(text):
    h = 0xCBF29CE484222325
    for b in text.encode("utf-8"):
        h = ((h ^ b) * 0x100000001B3) & 0xFFFFFFFFFFFFFFFF
    return h

def ostr(s):
    return '"' + s.replace("\\", "\\\\").replace('"', '\\"') + '"'

def glyph(g, where):
    assert isinstance(g, str) and len(g) == 1 and ord(g) < 256, f"{where}: glyph must be one character code below 256, got {g!r}"
    return ord(g)

def hue(h, where):
    assert isinstance(h, int) and 0 <= h < 16, f"{where}: hue out of range {h!r}"
    return f".{HUE_NAMES[h]}"

def fields(**kw):
    """A struct literal with only the non-default fields (None, 0, False, empty are left out)."""
    parts = [f"{k} = {v}" for k, v in kw.items() if v not in (None, 0, "", False, "{}", "nil")]
    return "{" + ", ".join(parts) + "}"

def script_path(path, where):
    assert path.startswith("./Content/"), f"{where}: unexpected script path {path!r}"
    rel = path[len("./Content/"):]
    assert os.path.exists(os.path.join(CONTENT, rel)), f"{where}: script {rel} does not exist"
    return rel

# ---- load ------------------------------------------------------------------------------------------------------------
effect_names_json = load("effectTypes.json")
equip_json = load("equipSlots.json")
items_json = load("itemTypes.json")
chars_json = load("characterTypes.json")
terrain_json = load("terrainTypes.json")
recipes_json = load("recipes.json")
messages_json = load("messages.json")
sfx_json = load("sfx.json")
mux_json = load("mux.json")
keys_json = load("keys.json")

# ---- enums -----------------------------------------------------------------------------------------------------------
ITEM = {n: snake(n) for n in items_json}
TERRAIN = {n: snake(n) for n in terrain_json}
CHAR = {n: snake(n) for n in chars_json}
RECIPE = {n: snake(n) for n in recipes_json}
MESSAGE = {n: snake(n) for n in messages_json if n not in DEAD_MESSAGES}
assert DEAD_MESSAGES <= set(messages_json)
SLOT = {n: snake(n) for n in equip_json}
SFX = {n: snake(n) for n in sfx_json}
MUX = {n: snake(n) for n in mux_json}

# Effects: the avatar's script table is the authority (every message choice, item verb and terrain verb is run through it).
loxy = Strict(chars_json["Loxy"], "characterTypes.Loxy")
loxy_scripts = loxy.data["EffectScripts"]
for other in chars_json:
    if other != "Loxy":
        assert chars_json[other]["EffectScripts"] == {}, f"{other} has effect scripts; only Loxy is expected to"
EFFECT = {n: snake(n) for n in loxy_scripts}
referenced = {script_path(p, f"Loxy.EffectScripts.{n}") for n, p in loxy_scripts.items()}
on_disk = {f"Scripts/Characters/Loxy/{f}" for f in os.listdir(os.path.join(CONTENT, "Scripts", "Characters", "Loxy")) if f.endswith(".lua")}
assert on_disk - referenced == {"Scripts/Characters/Loxy/Initialize.lua"}, f"unreferenced Loxy scripts: {sorted(on_disk - referenced)}"
for n in effect_names_json:
    assert n in EFFECT, f"effectTypes.json names {n}, which no script handles"

def effect(n, where):
    assert n in EFFECT, f"{where}: effect {n!r} has no script"
    return f".{EFFECT[n]}"

def ref(table, n, where, allow_empty=False):
    if allow_empty and n in ("", None):
        return ".None"
    assert n in table, f"{where}: unknown reference {n!r}"
    return f".{table[n]}"

def enum_decl(name, members, base="u8", none=False):
    names = (["None"] if none else []) + list(members)
    return f"{name} :: enum {base} {{ " + ", ".join(names) + " }"

out = []
def emit(*lines):
    out.extend(lines)

emit("package game", "",
     "// GENERATED by tools/gen/gen_content.py from src/BagelQuest/Content (itemTypes, terrainTypes, characterTypes, effectTypes,",
     "// equipSlots, recipes, messages, sfx, mux and the Scripts tree). Do not edit. The generator lists every departure from the data.",
     "",
     "@@HASH@@", "",
     "// Index and count of a run of entries in one of the pool arrays below (weighted lists, ingredient lists, text lines).",
     "Range :: struct { start, count: u16 }", "")

# ---- effects ---------------------------------------------------------------------------------------------------------
emit(f"// The {len(EFFECT)} effect scripts of the original's avatar (Content/Scripts/Characters/Loxy): every verb, dialogue step and quest",
     "// action. Behaviour is written by hand in Odin (phase 4); this enum is the exact set it must cover.",
     enum_decl("Effect_Kind", EFFECT.values(), none=True), "")
emit("// Display names for the effects that appear in the inventory item menu.", "EFFECT_NAMES := #partial [Effect_Kind]string{")
for n, s in EFFECT.items():
    if n in effect_names_json:
        e = Strict(effect_names_json[n], f"effectTypes.{n}")
        emit(f"\t.{s} = {ostr(e.take('Name'))},")
        e.finish()
emit("}", "")

# ---- equip slots -----------------------------------------------------------------------------------------------------
emit(enum_decl("Equip_Slot", SLOT.values(), none=True), "", "EQUIP_SLOT_NAMES := #partial [Equip_Slot]string{")
for n, s in SLOT.items():
    e = Strict(equip_json[n], f"equipSlots.{n}")
    emit(f"\t.{s} = {ostr(e.take('Name'))},")
    e.finish()
emit("}", "")

# ---- sound -----------------------------------------------------------------------------------------------------------
SOUND_LISTS = {}

def files(table_json, prefix, ids, enum, const):
    emit(enum_decl(enum, ids.values(), none=True), "", f"// File names under the page's {prefix.lower()} folder (the originals, copied by the build).",
         f"{const} := #partial [{enum}]string{{")
    for n, s in ids.items():
        path = table_json[n]
        assert path.startswith(f"Content/Audio/{prefix}/"), f"{const}.{n}: unexpected path {path!r}"
        assert os.path.exists(os.path.join(CONTENT, path[len("Content/"):])), f"{const}.{n}: {path} does not exist"
        emit(f"\t.{s} = {ostr(path.rsplit('/', 1)[1])},")
        SOUND_LISTS.setdefault(const, []).append(path.rsplit('/', 1)[1])
    emit("}", "")
files(sfx_json, "Sfx", SFX, "Sfx_Id", "SFX_FILES")
files(mux_json, "Mux", MUX, "Mux_Id", "MUX_FILES")

# ---- items -----------------------------------------------------------------------------------------------------------
ITEM_STATS = {"AttackDice": "attack_dice", "MaximumAttack": "max_attack", "DefendDice": "defend_dice",
              "MaximumDefend": "max_defend", "Durability": "durability", "MaximumDurability": "max_durability"}
item_flags = sorted({f for v in items_json.values() for f in v["Flags"]})
emit(enum_decl("Item_Type", ITEM.values(), none=True), "",
     enum_decl("Item_Flag", [snake(f) for f in item_flags]), "",
     "// How an item is named in lists (FullName.lua): the plain name, or the name with durability: Stick(10/10).",
     "Full_Name :: enum u8 { Plain, With_Durability }", "",
     "Item_Stats :: struct { attack_dice, max_attack, defend_dice, max_defend, durability, max_durability: i32 }", "",
     "MAX_ITEM_EFFECTS :: 3", "",
     "Item_Info :: struct {", "\tname:      string,", "\tglyph:     u8,", "\thue:       Hue,", "\tslot:      Equip_Slot,",
     "\tflags:     bit_set[Item_Flag],", "\tstats:     Item_Stats, // what a new item starts with", "\tfull_name: Full_Name,",
     "\teffects:   [MAX_ITEM_EFFECTS]Effect_Kind, // the item menu's verbs, in the original's order", "}", "",
     "ITEMS := #partial [Item_Type]Item_Info{")
DURABILITY_NAME = 'return ItemExtensions.Name(item) .. "(" .. ItemExtensions.Durability(item) .. "/" .. ItemExtensions.MaximumDurability(item) .. ")"'
for n, s in ITEM.items():
    w = f"itemTypes.{n}"
    d = Strict(items_json[n], w)
    name, g, h = d.take("Name"), glyph(d.take("Glyph"), w), hue(d.take("Hue"), w)
    slot = d.take("EquipSlotType")
    slot = ref(SLOT, slot, w) if slot else None
    flags = "{" + ", ".join("." + snake(f) for f in d.take("Flags")) + "}"
    stats = d.take("Statistics")
    for k in stats:
        assert k in ITEM_STATS, f"{w}.Statistics: unknown statistic {k}"
    stat_lit = fields(**{ITEM_STATS[k]: v for k, v in stats.items()})
    fn_path = script_path(d.take("FullNameScript"), w)
    fn_text = read_text(fn_path).strip()
    if fn_text == "":
        full = "Plain"
    elif fn_text == DURABILITY_NAME:
        full = "With_Durability"
    elif fn_path.replace("Scripts/", "") in DATA_FIXES:
        full = "Plain"
    else:
        raise SystemExit(f"{w}: FullName.lua has an unknown form: {fn_text!r}")
    effs = d.take("Effects")
    assert len(effs) <= 3, f"{w}: more than MAX_ITEM_EFFECTS effects"
    effect_lits = []
    for en, ed in effs.items():
        e = Strict(ed, f"{w}.Effects.{en}")
        et = e.take("EffectType")
        assert et in (None, "", en), f"{w}.Effects.{en}: EffectType is {et!r}"
        for k in ("Statistics", "Flags", "Metadatas"):
            e.take_empty(k)
        e.finish()
        assert en in effect_names_json, f"{w}: item effect {en} has no display name in effectTypes.json"
        effect_lits.append(effect(en, w))
    d.finish()
    emit(f"\t.{s} = " + fields(name=ostr(name), glyph=g, hue=h if h != ".Black" else None, slot=slot,
                               flags=flags if flags != "{}" else None, stats=stat_lit, full_name=f".{full}" if full != "Plain" else None,
                               effects="{" + ", ".join(f"{i} = {e}" for i, e in enumerate(effect_lits)) + "}" if effect_lits else None) + ",")
emit("}", "")

# ---- recipes ---------------------------------------------------------------------------------------------------------
emit(enum_decl("Recipe_Type", RECIPE.values()), "",
     "Ingredient :: struct { item: Item_Type, count: u16 }", "",
     "Recipe_Info :: struct { name: string, inputs, outputs: Range } // ranges into RECIPE_INGREDIENTS", "")
ingredients = []
recipe_lits = []
for n, s in RECIPE.items():
    w = f"recipes.{n}"
    d = Strict(recipes_json[n], w)
    ranges = {}
    for key in ("Inputs", "Outputs"):
        start = len(ingredients)
        for it, c in d.take(key).items():
            assert isinstance(c, int) and c > 0, f"{w}.{key}.{it}: bad count"
            ingredients.append(f"{{{ref(ITEM, it, w + '.' + key)}, {c}}}")
        ranges[key] = (start, len(ingredients) - start)
    recipe_lits.append(f"\t.{s} = {{name = {ostr(d.take('Name'))}, inputs = {{{ranges['Inputs'][0]}, {ranges['Inputs'][1]}}}, outputs = {{{ranges['Outputs'][0]}, {ranges['Outputs'][1]}}}}},")
    d.finish()
emit("RECIPES := [Recipe_Type]Recipe_Info{", *recipe_lits, "}", "",
     f"RECIPE_INGREDIENTS := [{len(ingredients)}]Ingredient{{", *[f"\t{i}," for i in ingredients], "}", "")

# ---- characters ------------------------------------------------------------------------------------------------------
CHAR_STATS = {"Health": "health", "MaximumHealth": "max_health", "AttackDice": "attack_dice", "MaximumAttack": "max_attack",
              "DefendDice": "defend_dice", "MaximumDefend": "max_defend", "XP": "xp", "XPGoal": "xp_goal", "XPLevel": "xp_level",
              "AdvancementPointsPerLevel": "ap_per_level", "AdvancementPoints": "ap", "Energy": "energy",
              "MaximumEnergy": "max_energy", "Jools": "jools", "Peril": "peril"}
emit(enum_decl("Character_Type", CHAR.values()), "",
     "Character_Stats :: struct {", "\thealth, max_health, attack_dice, max_attack, defend_dice, max_defend: i32,",
     "\txp, xp_goal, xp_level, ap_per_level, ap, energy, max_energy, jools, peril: i32,", "}", "",
     "// mask_glyph is drawn first in mask_hue (black) to cut the character out of the terrain, then glyph in hue.",
     "Character_Info :: struct { name: string, glyph: u8, hue: Hue, mask_glyph: u8, mask_hue: Hue, stats: Character_Stats }", "",
     "CHARACTERS := [Character_Type]Character_Info{")
for n, s in CHAR.items():
    w = f"characterTypes.{n}"
    d = Strict(chars_json[n], w)
    name, g, h = d.take("Name"), glyph(d.take("Glyph"), w), hue(d.take("Hue"), w)
    mg, mh = glyph(d.take("MaskGlyph"), w), hue(d.take("MaskHue"), w)
    stats = d.take("Statistics")
    for k in stats:
        assert k in CHAR_STATS, f"{w}.Statistics: unknown statistic {k}"
    script_path(d.take("InitializeScript"), w)  # hand-written in phase 4 (IGNORED)
    d.take("EffectScripts")                     # Loxy's table is the Effect_Kind enum
    d.finish()
    emit(f"\t.{s} = " + fields(name=ostr(name), glyph=g, hue=h if h != ".Black" else None, mask_glyph=mg,
                               mask_hue=mh if mh != ".Black" else None,
                               stats=fields(**{CHAR_STATS[k]: v for k, v in stats.items()})) + ",")
emit("}", "")

# ---- terrain ---------------------------------------------------------------------------------------------------------
TERRAIN_VERBS = {"BuildFire": "Build_Fire", "BuildFurnace": "Build_Furnace", "PutOutFire": "Put_Out_Fire",
                 "MakeTorch": "Make_Torch", "CookBagel": "Cook_Bagel"}
terrain_flags = sorted({f for v in terrain_json.values() for f in v["Flags"]})
emit(enum_decl("Terrain_Type", TERRAIN.values()), "",
     enum_decl("Terrain_Flag", [snake(f) for f in terrain_flags]),
     "// Tenable: can be walked on. Peril is the chance weight of meeting a creature (see docs/game-reference.md).", "",
     enum_decl("Terrain_Verb", TERRAIN_VERBS.values()), "",
     "Creature_Weight :: struct { type: Character_Type, weight: i32 }",
     "Forage_Weight :: struct { item: Item_Type, weight: i32 } // item None means a forage that finds nothing", "",
     "Terrain_Info :: struct {", "\tname:           string,", "\tglyph:          u8,", "\thue:            Hue,",
     "\tflags:          bit_set[Terrain_Flag],", "\tverbs:          bit_set[Terrain_Verb],", "\tperil:          i32,",
     "\tcreatures:      Range, // into CREATURE_WEIGHTS", "\tforage:         Range, // into FORAGE_WEIGHTS",
     "\tforage_count:   i32,   // what a new cell starts with (the original's Initialize.lua)",
     "\tdepleted:       Terrain_Type, // the original lists it but never uses it (docs/defect-audit.md C1); itself when none",
     "\tput_out_into:   Terrain_Type, // Put_Out_Fire only", "\tput_out_drops:  Item_Type,    // Put_Out_Fire only", "}", "")
creature_pool, forage_pool, terrain_lits = [], [], []
INIT_FORAGE = re.compile(r'^cell:SetStatistic\("ForageRemaining", (\d+)\)$')
for n, s in TERRAIN.items():
    w = f"terrainTypes.{n}"
    d = Strict(terrain_json[n], w)
    name, g, h = d.take("Name"), glyph(d.take("Glyph"), w), hue(d.take("Hue"), w)
    flags = "{" + ", ".join("." + snake(f) for f in d.take("Flags")) + "}"
    stats = d.take("Statistics")
    assert set(stats) <= {"Peril"}, f"{w}.Statistics: unknown {set(stats)}"
    metas = d.take("Metadatas")
    assert set(metas) <= {"DepletedTerrainType"}, f"{w}.Metadatas: unknown {set(metas)}"
    cstart = len(creature_pool)
    for cn, wt in d.take("CreatureTypeGenerator").items():
        creature_pool.append(f"{{{ref(CHAR, cn, w + '.CreatureTypeGenerator')}, {wt}}}")
    fstart = len(forage_pool)
    for itn, wt in d.take("Foragables").items():
        forage_pool.append(f"{{{ref(ITEM, itn, w + '.Foragables', allow_empty=True)}, {wt}}}")
    verbs, put_out_into, put_out_drops = [], None, None
    for en, ed in d.take("Effects").items():
        if en == "Forage":
            continue  # IGNORED
        assert en in TERRAIN_VERBS, f"{w}: unknown terrain effect {en}"
        e = Strict(ed, f"{w}.Effects.{en}")
        e.take_empty("EffectType"); e.take_empty("Statistics"); e.take_empty("Flags")
        m = e.take("Metadatas")
        if en == "PutOutFire":
            assert set(m) == {"TerrainType", "ItemType"}, f"{w}.PutOutFire metadata {set(m)}"
            put_out_into, put_out_drops = ref(TERRAIN, m["TerrainType"], w), ref(ITEM, m["ItemType"], w)
        else:
            assert m == {}, f"{w}.{en}: unexpected metadata"
        e.finish()
        verbs.append("." + TERRAIN_VERBS[en])
        assert effect(en, w)
    init = read_text(script_path(d.take("InitializerScript"), w)).strip()
    forage_count = 0
    if init:
        m = INIT_FORAGE.match(init)
        assert m, f"{w}: Initialize.lua has an unknown form: {init!r}"
        forage_count = int(m.group(1))
    d.finish()
    depleted = ref(TERRAIN, metas["DepletedTerrainType"], w) if "DepletedTerrainType" in metas else f".{s}"
    terrain_lits.append(f"\t.{s} = " + fields(
        name=ostr(name), glyph=g, hue=h if h != ".Black" else None, flags=flags if flags != "{}" else None,
        verbs="{" + ", ".join(verbs) + "}" if verbs else None, peril=stats.get("Peril"),
        creatures=f"{{{cstart}, {len(creature_pool) - cstart}}}" if len(creature_pool) > cstart else None,
        forage=f"{{{fstart}, {len(forage_pool) - fstart}}}", forage_count=forage_count,
        depleted=depleted if depleted != f".{s}" else None, put_out_into=put_out_into, put_out_drops=put_out_drops) + ",")
emit("TERRAINS := [Terrain_Type]Terrain_Info{", *terrain_lits, "}", "",
     f"CREATURE_WEIGHTS := [{len(creature_pool)}]Creature_Weight{{", *[f"\t{c}," for c in creature_pool], "}", "",
     f"FORAGE_WEIGHTS := [{len(forage_pool)}]Forage_Weight{{", *[f"\t{c}," for c in forage_pool], "}", "")

# ---- canned messages ---------------------------------------------------------------------------------------------------
emit(enum_decl("Message_Type", MESSAGE.values()), "",
     "Message_Line :: struct { hue: Hue, text: string }",
     "Message_Choice :: struct { text: string, effect: Effect_Kind }",
     "Message_Info :: struct { sfx: Sfx_Id, lines, choices: Range } // ranges into MESSAGE_LINES and MESSAGE_CHOICES", "")
lines_pool, choices_pool, msg_lits = [], [], []
for n, s in MESSAGE.items():
    w = f"messages.{n}"
    d = Strict(messages_json[n], w)
    sfx = d.take("Sfx")
    sfx = ref(SFX, sfx, w) if sfx else None
    ls = len(lines_pool)
    src_lines = d.take("Lines")
    if n in MESSAGE_TEXT_FIXES:
        assert len(src_lines) == 1 and src_lines[0]["Text"].startswith("This is sign #"), f"{w}: not the placeholder the fix expects"
        src_lines = [{"Hue": src_lines[0]["Hue"], "Text": t} for t in MESSAGE_TEXT_FIXES[n]]
    for ln in src_lines:
        l = Strict(ln, w + ".Lines")
        lines_pool.append(f"{{{hue(l.take('Hue'), w)}, {ostr(l.take('Text'))}}}")
        l.finish()
    cs = len(choices_pool)
    for ch in d.take("Choices"):
        c = Strict(ch, w + ".Choices")
        choices_pool.append(f"{{{ostr(c.take('Text'))}, {effect(c.take('EffectType'), w)}}}")
        c.take_empty("Statistics"); c.take_empty("Flags"); c.take_empty("Metadatas")
        c.finish()
    d.finish()
    msg_lits.append(f"\t.{s} = " + fields(sfx=sfx, lines=f"{{{ls}, {len(lines_pool) - ls}}}" if len(lines_pool) > ls else None,
                                           choices=f"{{{cs}, {len(choices_pool) - cs}}}" if len(choices_pool) > cs else None) + ",")
emit("MESSAGES := [Message_Type]Message_Info{", *msg_lits, "}", "",
     f"MESSAGE_LINES := [{len(lines_pool)}]Message_Line{{", *[f"\t{c}," for c in lines_pool], "}", "",
     f"MESSAGE_CHOICES := [{max(len(choices_pool), 1)}]Message_Choice{{", *[f"\t{c}," for c in choices_pool], "}", "")

body = "\n".join(out) + "\n"
h = fnv1a64(body)  # the placeholder line is part of the hashed text, so the hash covers everything else exactly
text = body.replace("@@HASH@@", f"// Hash of the generated text below; saves record it so a save only loads against the content that made it.\nCONTENT_HASH :: u64(0x{h:016X})", 1)
with open(os.path.join(ROOT, "odin", "game", "content_data.odin"), "w") as f:
    f.write(text)

# ---- key map for the web page ----------------------------------------------------------------------------------------
cmd_names = {"Up": "up", "Down": "down", "Left": "left", "Right": "right", "A": "confirm", "B": "cancel"}
by_key, by_code = {}, {}
NAMED = {"Space": " ", "Enter": "Enter", "Escape": "Escape", "Up": "ArrowUp", "Down": "ArrowDown", "Left": "ArrowLeft", "Right": "ArrowRight"}
for k, cmd in keys_json.items():
    assert cmd in cmd_names, f"keys.json: unknown command {cmd}"
    if k in NAMED:
        by_key[NAMED[k]] = cmd_names[cmd]
    elif re.fullmatch(r"NumPad\d", k):
        by_code["Numpad" + k[-1]] = cmd_names[cmd]
    elif re.fullmatch(r"[A-Z]", k):
        by_key[k.lower()] = by_key[k] = cmd_names[cmd]
    else:
        raise SystemExit(f"keys.json: no browser mapping for key {k}")
# Config/keys.json is an older copy of the table the other way round (Program.vb loads Content/keys.json, which adds Q and Z
# for AZERTY keyboards); every entry of the old copy must still be in the real one
cfg = load("Config/keys.json")
assert {(k, c) for c, ks in cfg.items() for k in ks} <= set(keys_json.items()), "Content/Config/keys.json has entries Content/keys.json lacks"
def js(d):
    return "{ " + ", ".join(f'"{k}": "{v}"' for k, v in d.items()) + " }"
with open(os.path.join(ROOT, "odin", "platform", "web", "page", "keys.js"), "w") as f:
    f.write("// GENERATED by tools/gen/gen_content.py from src/BagelQuest/Content/keys.json. Do not edit.\n"
            "// The logical key (e.key) is read before the physical code (e.code); see the vault's Gotchas.\n"
            f"const KEYS_BY_KEY = {js(by_key)};\nconst KEYS_BY_CODE = {js(by_code)};\n")
def jlist(names):
    return "[null, " + ", ".join(f'"{n}"' for n in names) + "]"
with open(os.path.join(ROOT, "odin", "platform", "web", "page", "sounds.js"), "w") as f:
    f.write("// GENERATED by tools/gen/gen_content.py from sfx.json and mux.json. Do not edit.\n"
            "// Indexed by the core's Sfx_Id (0 is none); the files are copied from the original's Content/Audio/Sfx.\n// The original's music files are NOT shipped: page/music.js synthesizes the themes (user decision, phase 7).\n"
            f"const SFX_FILES = {jlist(SOUND_LISTS['SFX_FILES'])};\n")
print("wrote odin/game/content_data.odin (hash %016X) and odin/platform/web/page/keys.js" % h)
