Friend Module CharacterTypes
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, CharacterTypeDescriptor) =
        New Dictionary(Of String, CharacterTypeDescriptor) From
        {
            {
                "Loxy",
                New CharacterTypeDescriptor(
                    "Loxy",
                    ChrW(2),
                    12,
                    ChrW(1),
                    0,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {"Health", 2},
                        {"MaximumHealth", 2},
                        {"AttackDice", 2},
                        {"MaximumAttack", 1},
                        {"DefendDice", 4},
                        {"MaximumDefend", 2},
                        {"XP", 0},
                        {"XPGoal", 10},
                        {"XPLevel", 1},
                        {"AdvancementPointsPerLevel", 10},
                        {"AdvancementPoints", 0},
                        {"Energy", 10},
                        {"MaximumEnergy", 10},
                        {"Jools", 20}
                    },
                    effectHandlers:=LoxyEffectHandlers.All,
                    initializeScript:="
character:AddItem(ItemInitializer.CreateItem(character.World,""ClayPot""))",
                    effectScripts:=New Dictionary(Of String, String) From
                    {
                        {
                            "BumpRiver",
                            "
        local msg = character.World:CreateMessage():
            AddLine(7, CharacterExtensions.Name(character) .. "" visits the river bank.""):
            AddChoice(""Cool story, bro!"", ""ExitDialog"")
        if CharacterExtensions.HasItemTypeInInventory(character, ""ClayPot"") then
            msg:AddChoice(""Fill Clay Pot with Water"", ""FillClayPot"")
        end"
                        },
                        {
                            "CompleteRatQuest",
                            "
local jools = 0
local items = CharacterExtensions.ItemsOfItemType(character, ""RatTail"")
for i = 0,items.Length-1 do
    local item = items[i]
    jools = jools + 1
    character:RemoveItem(item)
    item:Recycle()
end
CharacterExtensions.AddJools(character, jools)
character.World:CreateMessage():AddLine(7, CharacterExtensions.Name(character) .. "" receives "" .. jools .. "" jools."")"
                        },
                        {
                            "PotterMakePot",
                            "
local price = 20
local RecipeType = ""UnfiredPot""
if CharacterExtensions.Jools(character) < price then
    character.World:CreateMessage():
                AddLine(7, ""The price is "" .. price .. "" jools.""):
                AddLine(7, CharacterExtensions.Name(character) .. "" has "" .. CharacterExtensions.Jools(character) .. "" jools)"")
    return
end
if not RecipeTypes.CanCraft(RecipeType, character) then
    local msg = character.World:CreateMessage():AddLine(7, ""To make a pot, I need:"")
    CharacterExtensions.AddRecipeInputs(character, msg, RecipeType)
    return
end
CharacterExtensions.AddJools(character, -price)
RecipeTypes.Craft(RecipeType, character)
RecipeTypes.Craft(""ClayPot"",character)
character.World:CreateMessage():
    AddLine(4, CharacterExtensions.Name(character) .. "" loses "" .. price .. "" jools""):AddLine(10, CharacterExtensions.Name(character) .. "" gains 1 "" .. ItemTypes.ToItemTypeDescriptor(""ClayPot"").Name)"
                        },
                        {
                            "PayInnkeeper",
                            "
if character:GetFlag(""PaidInnkeeper"") then
    character.World:CreateMessage():
                AddLine(7, ""You've already paid!"")
    return
end
local bedCost = 1
if CharacterExtensions.Jools(character) < bedCost then
    character.World:CreateMessage():
                AddLine(7, ""Sorry! No jools, no bed!"")
    return
end
CharacterExtensions.AddJools(character, -bedCost)
character:SetFlag(""PaidInnkeeper"", true)
character.World:CreateMessage():
                AddLine(7, ""Thanks for yer business.""):
                AddLine(7, ""Choose any bed you like."")"
                        },
                        {
                            "PervertInnkeeper",
                            "
character.World:CreateMessage():
    AddLine(7, ""I'm not a pervert!""):
    AddLine(7, ""I'm just Australian!"")"
                        },
                        {
                            "StartRatQuest",
                            "
character.World:CreateMessage():
    AddLine(7, ""Well, there are a bunch of rats in the cellar.""):
    AddLine(7, ""I'll pay you 1 jools for each rat tail.""):
    AddLine(7, ""I only accept the tails, no corpses.""):
    AddLine(7, ""So you'll need to cut them off first.""):
    AddChoice(""I'm on it!"", ""AcceptRatQuest""):
    AddChoice(""Mebbe later?"", ""ExitDialog"")"
                        },
                        {
                            "GorachanTalk",
                            "
local msg = character.World:CreateMessage():
                AddLine(7, ""Welcome to Jusdatip Inn!""):
                AddLine(7, ""I'm Gorachan.""):
                AddLine(7, ""You can rest in a bed for 1 jools.""):
                AddLine(7, ""I'd offer to join you,""):
                AddLine(7, ""but then you wouldn't get any rest!""):
                AddChoice(""Cool story, bro!"", ""ExitDialog""):
                AddChoice(""Yer a pervert!"", ""PervertInnkeeper""):
                AddChoice(""I'll take a bed."", ""PayInnkeeper"")
if character:GetFlag(""RatQuest"") then
    if CharacterExtensions.HasItemTypeInInventory(character, ""RatTail"") then
        msg:AddChoice(""Here's some rat tails!"", ""CompleteRatQuest"")
    end
else
    msg:AddChoice(""I need a job!"", ""StartRatQuest"")
end"
                        },
                        {
                            "TrainHealth",
                            "
local msg = character.World:CreateMessage()
local Multiplier = 5
local TrainingCost = Multiplier * CharacterExtensions.MaximumHealth(character)
if CharacterExtensions.AdvancementPoints(character) < TrainingCost then
    msg:AddLine(7, ""To go from "" .. CharacterExtensions.MaximumHealth(character) .. "" to "" .. CharacterExtensions.MaximumHealth(character) + 1 .. "" Maximum Health,"")
    msg:AddLine(7, ""you need "" .. TrainingCost .. "" AP, but you have "" .. CharacterExtensions.AdvancementPoints(character) .. ""."")
    msg:AddLine(7, ""Come back when yer more experienced!"")
    return
end
CharacterExtensions.AddAdvancementPoints(character, -TrainingCost)
CharacterExtensions.SetMaximumHealth(character, CharacterExtensions.MaximumHealth(character) + 1)
CharacterExtensions.SetHealth(character, CharacterExtensions.Health(character) + 1)
msg:AddLine(4, CharacterExtensions.Name(character) .. "" loses "" .. TrainingCost .. "" AP"")
msg:AddLine(2, CharacterExtensions.Name(character) .. "" adds 1 Maximum Health"")
msg:AddLine(7, ""Yer now at "" .. CharacterExtensions.MaximumHealth(character) .. "" Maximum Health."")
msg:AddLine(7, ""Remember! if you don't have yer health,"")
msg:AddLine(7, ""you don't really have anything!"")"
                        },
                        {
                            "HealthTrainerTalk",
                            "
character.World:CreateMessage():
    AddLine(7, ""I am the health trainer!""):
    AddLine(7, ""I can help you increase yer health.""):
    AddLine(7, ""The cost is "" .. CharacterExtensions.MaximumHealth(character) * 5 .. "" AP.""):
    AddChoice(""Cool story, bro!"", ""ExitDialog""):
    AddChoice(""Train me!"", ""TrainHealth"")"
                        },
                        {
                            "Heal",
                            "
local maximumHealth = math.min(CharacterExtensions.MaximumHealth(character), effect:GetStatistic(""MaximumHealth""))
if CharacterExtensions.Health(character) >= maximumHealth then
    character.World:CreateMessage():AddLine(7, ""Nothing happens!"")
    return
end
CharacterExtensions.SetHealth(character, maximumHealth)
local msg =
    character.World:
        CreateMessage():
        AddLine(7, CharacterExtensions.Name(character) .. "" is healed!""):
        AddLine(7, CharacterExtensions.Name(character) .. "" now has "" .. CharacterExtensions.Health(character) .. "" health."")
local jools = math.floor(CharacterExtensions.Jools(character)/2)
CharacterExtensions.AddJools(character, -jools)
if jools > 0 then
    msg:AddLine(4, CharacterExtensions.Name(character) .. "" loses {jools} jools!"")
end"
                        },
                        {
                            "NihilistPrices",
                            "
character.World:CreateMessage():
    AddLine(7, ""I don't sell anything.""):
    AddLine(7, ""I'm a nihilist, remember?"")"
                        },
                        {
                            "FillClayPot",
                            "
RecipeTypes.Craft(""WaterPot"", character)
character.World:CreateMessage():AddLine(7, CharacterExtensions.Name(character) .. "" fills a "" .. ItemTypes.ToItemTypeDescriptor(""ClayPot"").Name .. "" with water."")"
                        },
                        {
                            "EnergyTrainerTalk",
                            "
local trainCost = CharacterExtensions.MaximumEnergy(character) * 2
character.World:CreateMessage():
    AddLine(7, ""I am the endurance trainer.""):
    AddLine(7, ""I can increase yer energy""):
    AddLine(7, ""for the cost of 1AP and "" .. trainCost .. "" jools.""):
    AddChoice(""Cool story, bro!"", ""ExitDialog""):
    AddChoice(""Train me!"", ""TrainEnergy"")"
                        },
                        {
                            "TrainEnergy",
                            "
local msg = character.World:CreateMessage()
if CharacterExtensions.AdvancementPoints(character) < 1 then
    msg:AddLine(7, ""You need at least 1 AP."")
    msg:AddLine(7, ""Come back when yer more experienced!"")
    return
end
local Multiplier = 2
local TrainingCost = Multiplier * CharacterExtensions.MaximumEnergy(character)
if CharacterExtensions.Jools(character) < TrainingCost then
    msg:
        AddLine(7, ""The price is "" .. TrainingCost .. "" jools.""):
        AddLine(7, ""I have overhead, you know."")
    return
end
CharacterExtensions.AddAdvancementPoints(character, -1)
CharacterExtensions.AddJools(character, -TrainingCost)
CharacterExtensions.SetMaximumEnergy(character, CharacterExtensions.MaximumEnergy(character) + 1)
CharacterExtensions.AddEnergy(character, 1)
msg:AddLine(4, CharacterExtensions.Name(character) .. "" loses 1 AP"")
msg:AddLine(4, CharacterExtensions.Name(character) .. "" loses "" .. TrainingCost .. "" jools"")
msg:AddLine(2, CharacterExtensions.Name(character) .. "" adds 1 Maximum Energy"")
msg:AddLine(7, ""Yer now at "" .. CharacterExtensions.MaximumEnergy(character) .. "" Maximum Energy."")
msg:AddLine(7, ""Persistence is futile!"")"
                        },
                        {
                            "HealerTalk",
                            "
character.World:CreateMessage():
        AddLine(7, ""Welcome to the Nihilistic House of Healing.""):
        AddLine(7, ""If you go to the basin And wash,""):
        AddLine(7, ""you will be healed,""):
        AddLine(7, ""but it will cost you half of yer jools.""):
        AddLine(7, ""Not that I care or anything,""):
        AddLine(7, ""because I'm a nihilist.""):
        AddChoice(""Cool story, bro!"", ""ExitDialog""):
        AddChoice(""What's for sale?"", ""NihilistPrices"")"
                        },
                        {
                            "ExitDialog",
                            ""
                        },
                        {
                            "Message",
                            "
MessageTypes.ToMessageTypeDescriptor(effect:GetMetadata(""MessageType"")):CreateMessage(character.World)"
                        },
                        {
                            "DruidTeachMenu",
                            "
local canLearnForaging = not character:GetFlag(""KnowsForaging"")
local canLearnTwineMaking = not character:GetFlag(""KnowsTwineMaking"")
local canLearnKnapping = not character:GetFlag(""KnowsRockSharpening"")
local canLearnFireMaking = not character:GetFlag(""KnowsFireMaking"")
local canLearnTorchMaking = not character:GetFlag(""KnowsTorchMaking"")
local canLearnHatchetMaking = not character:GetFlag(""KnowsHatchetMaking"")
local canLearn = canLearnForaging or canLearnTwineMaking or canLearnKnapping or canLearnFireMaking or canLearnTorchMaking or canLearnHatchetMaking
local msg = character.World:CreateMessage()
if not canLearn then
    msg:AddLine(7, ""You have learned all I have to teach you."")
    return
end
msg:AddLine(7, ""I can teach you these things:"")
msg:AddChoice(""Good to know!"", ""ExitDialog"")
if canLearnForaging then
    msg:AddChoice(
        ""Foraging(-1AP)"",
        ""LearnForaging"")
    msg.LastChoice:SetStatistic(""AdvancementPoints"", 1)
    msg.LastChoice:SetMetadata(""FlagType"", ""KnowsForaging"")
    msg.LastChoice:SetMetadata(""TaskName"", ""forage"")
    msg.LastChoice:SetMetadata(""ActionName"", ""Forage..."")
    msg.LastChoice:SetMetadata(""RecipeType"", ""Foraging"")
end
if canLearnTwineMaking then
    msg:AddChoice(
        ""Twine Making(-1AP,-2 Plant Fiber)"",
        ""LearnTwineMaking"")
    msg.LastChoice:SetStatistic(""AdvancementPoints"", 1)
    msg.LastChoice:SetMetadata(""FlagType"", ""KnowsTwineMaking"")
    msg.LastChoice:SetMetadata(""TaskName"", ""make twine"")
    msg.LastChoice:SetMetadata(""ActionName"", ""Make Twine"")
    msg.LastChoice:SetMetadata(""RecipeType"", ""Twine"")
    msg.LastChoice:SetFlag(""LearnByDoing"", true)
end
if canLearnKnapping then
    msg:AddChoice(
        ""Knapping(-1AP,-2 Rock)"",
        ""LearnKnapping"")
    msg.LastChoice:SetStatistic(""AdvancementPoints"", 1)
    msg.LastChoice:SetMetadata(""FlagType"", ""KnowsRockSharpening"")
    msg.LastChoice:SetMetadata(""TaskName"", ""knap"")
    msg.LastChoice:SetMetadata(""ActionName"", ""Knap"")
    msg.LastChoice:SetMetadata(""RecipeType"", ""SharpRock"")
    msg.LastChoice:SetFlag(""LearnByDoing"", true)
end
if canLearnFireMaking then
    msg:AddChoice(
        ""Fire Making(-1AP, -5 Rock, -5 Sticks)"",
        ""LearnFireMaking"")
    msg.LastChoice:SetStatistic(""AdvancementPoints"", 1)
    msg.LastChoice:SetMetadata(""FlagType"", ""KnowsFireMaking"")
    msg.LastChoice:SetMetadata(""TaskName"", ""make a fire"")
    msg.LastChoice:SetMetadata(""ActionName"", ""Build Fire"")
    msg.LastChoice:SetMetadata(""RecipeType"", ""CookingFire"")
    msg.LastChoice:SetMetadata(""Caveat"", ""(only works in clear areas in the wilderness)"")
    msg.LastChoice:SetFlag(""LearnByDoing"", false)
end
if canLearnTorchMaking then
    msg:AddChoice(
        ""Torch Making(-1AP, -1 Stick, -1 Charcoal)"",
        ""LearnTorchMaking"")
    msg.LastChoice:SetStatistic(""AdvancementPoints"", 1)
    msg.LastChoice:SetMetadata(""FlagType"", ""KnowsTorchMaking"")
    msg.LastChoice:SetMetadata(""TaskName"", ""make a torch"")
    msg.LastChoice:SetMetadata(""ActionName"", ""Make Torch"")
    msg.LastChoice:SetMetadata(""RecipeType"", ""Torch"")
    msg.LastChoice:SetMetadata(""Caveat"", ""(only works with a source of flames)"")
    msg.LastChoice:SetFlag(""LearnByDoing"", false)
end
if canLearnHatchetMaking then
    msg:AddChoice(
        ""Hatchet Making(-1AP,-1Stick,-1S.Rock,-1Twine)"",
        ""LearnHatchedMaking"")
    msg.LastChoice:SetStatistic(""AdvancementPoints"", 1)
    msg.LastChoice:SetMetadata(""FlagType"", ""KnowsHatchetMaking"")
    msg.LastChoice:SetMetadata(""TaskName"", ""make a hatchet"")
    msg.LastChoice:SetMetadata(""ActionName"", ""Make Hatchet"")
    msg.LastChoice:SetMetadata(""RecipeType"", ""Hatchet"")
    msg.LastChoice:SetFlag(""LearnByDoing"", true)
end"
                        },
                        {
                            "DruidPrices",
                            "
local msg = character.World:CreateMessage():
                AddLine(7, ""I sell a variety of herbs.""):
                AddLine(7, ""("" .. CharacterExtensions.Name(character) .. "" has "" .. CharacterExtensions.Jools(character) .. "" jools)""):
                AddChoice(""Good to know!"", ""ExitDialog""):
                AddChoice(""Buy Energy Herb(5 jools)"",""Buy"")
msg.LastChoice:SetMetadata(""ItemType"", ""EnergyHerb"")
msg.LastChoice:SetStatistic(""Price"", 5)
msg.LastChoice:SetMetadata(""EffectType"", ""DruidPrices"")"
                        },
                        {
                            "MakeHatchet",
                            "
CharacterExtensions.DoRecipe(character, 1, ""Hatchet"", ""make a hatchet"", ""makes a hatchet"")"
                        },
                        {
                            "SeasonRat",
                            "
CharacterExtensions.DoRecipe(character, 0, ""SeasonedRat"", ""season a rat"", ""seasons a rat"")"
                        },
                        {
                            "MakePaprika",
                            "
CharacterExtensions.DoRecipe(character, 1, ""Paprika"", ""make paprika"", ""makes paprika"")"
                        },
                        {
                            "SmokePepper",
                            "
CharacterExtensions.CookRecipe(character, ""SmokedPepper"", ""smoke a pepper"", ""smokes a pepper"")"
                        },
                        {
                            "MillWheat",
                            "
CharacterExtensions.DoRecipe(character, 1, ""Flour"", ""make flour"", ""makes flour"")"
                        },
                        {
                            "MakeDough",
                            "
CharacterExtensions.DoRecipe(character, 2, ""Dough"", ""make dough"", ""makes dough"")"
                        },
                        {
                            "CookRatBody",
                            "
        CharacterExtensions.CookRecipe(character, ""CookedRatBody"", ""cook a rat body"", ""cooks a rat body"")"
                        },
                        {
                            "CookRatCorpse",
                            "
CharacterExtensions.CookRecipe(character, ""CookedRatCorpse"", ""cook a rat corpse"", ""cooks a rat corpse"")"
                        },
                        {
                            "CutOffTail",
                            "
CharacterExtensions.DoRecipe(character, 2, ""RatTail"", ""cut off a rat's tail"", ""cut off a rat's tail"")"
                        },
                        {"BuildFurnace", "
        if not CharacterExtensions.ConsumeEnergy(character, 1, ""build a furnace"") then
            return
        end
        if not RecipeTypes.CanCraft(""Furnace"", character) then
            local msg = character.World:CreateMessage():
                AddLine(7, ""To build a furnace,""):
                AddLine(7, CharacterExtensions.Name(character) .. "" needs:"")
            local recipeName = ""Furnace""
            CharacterExtensions.ReportNeededRecipeInputs(character, msg, recipeName)
            return
        end
        RecipeTypes.Craft(""Furnace"", character)
        character.World:CreateMessage():
                AddLine(7, CharacterExtensions.Name(character) .. "" builds a furnace."")
        character.Cell.TerrainType = ""Furnace"""},
                        {"BuildFire", "
if not CharacterExtensions.ConsumeEnergy(character, 1, ""build a fire"") then
    return
end
if not RecipeTypes.CanCraft(""CookingFire"", character) then
    local msg = character.World:CreateMessage():
        AddLine(7, ""To build a fire,""):
        AddLine(7, CharacterExtensions.Name(character) .. "" needs:"")
    local inputs = RecipeTypes.Inputs(""CookingFire"")
    for i = 0, inputs.Length-1 do
        local input = inputs[i]
        msg:AddLine(7, ItemTypes.ToItemTypeDescriptor(input.ItemType).Name .. "": "" .. character:ItemTypeCount(input.ItemType) .. ""/"" .. input.Count)
    end
    return
end
RecipeTypes.Craft(""CookingFire"", character)
character.World:CreateMessage():
        AddLine(7, CharacterExtensions.Name(character) .. "" builds a small fire."")
character.Cell.TerrainType = ""CookingFire"""
                        },
                        {"PutOutFire", "
if not CharacterExtensions.ConsumeEnergy(character, 1, ""put out a fire"") then
    return
end
character.World:CreateMessage():
        AddLine(7, CharacterExtensions.Name(character) .. "" extinguishes the fire."")
if effect:HasMetadata(""ItemType"") then
    character.Cell:AddItem(ItemInitializer.CreateItem(character.World, effect:GetMetadata(""ItemType"")))
end
character.Cell.TerrainType = effect:GetMetadata(""TerrainType"")"},
                        {"MakeTorch", "
if not CharacterExtensions.ConsumeEnergy(character, 1, ""make a torch"") then
    return
end
if not RecipeTypes.CanCraft(""Torch"", character) then
    local msg = character.World:CreateMessage():
        AddLine(7, ""To make a torch,""):
        AddLine(7, CharacterExtensions.Name(character) .. "" needs:"")
    local inputs = RecipeTypes.Inputs(""Torch"")
    for i = 0,inputs.Length-1 do
        msg:AddLine(7, ItemTypes.ToItemTypeDescriptor(inputs[i].ItemType).Name .. "": "" .. character:ItemTypeCount(inputs[i].ItemType) .. ""/"" .. inputs[i].Count)
    end
    return
end
RecipeTypes.Craft(""Torch"", character)
character.World:CreateMessage():
        AddLine(7, CharacterExtensions.Name(character) .. "" makes a torch."")"
                        },
                        {"CookBagel", "CharacterExtensions.CookFurnaceRecipe(character, ""Bagel"", ""cook a bagel"", ""cooks a bagel"")"},
                        {"LearnForaging", "CharacterExtensions.DoLearnSkill(character,effect)"},
                        {"LearnKnapping", "CharacterExtensions.DoLearnSkill(character,effect)"},
                        {"LearnTwineMaking", "CharacterExtensions.DoLearnSkill(character,effect)"},
                        {"LearnFireMaking", "CharacterExtensions.DoLearnSkill(character,effect)"},
                        {"LearnTorchMaking", "CharacterExtensions.DoLearnSkill(character,effect)"},
                        {"LearnHatchedMaking", "CharacterExtensions.DoLearnSkill(character,effect)"},
                        {
                            "DruidAllergies",
                            "        
character.World:CreateMessage():
    AddLine(7, ""Alas, I have allergies."")"
                        },
                        {
                            "DruidTalk",
                            "
character.World:CreateMessage():
    AddLine(7, ""Greetings! I am Marcus, the hippy druid.""):
    AddLine(7, ""I can help you learn nature's way.""):
    AddChoice(""Cool story, bro!"", ""ExitDialog""):
    AddChoice(""Don't druids live in the woods?"", ""DruidAllergies""):
    AddChoice(""Teach me!"", ""DruidTeachMenu""):
    AddChoice(""What's for sale?"", ""DruidPrices"")"},
                        {
                            "EatSeasonedRat",
                            "
local item = CharacterExtensions.ConsumedItem(character, effect)
CharacterExtensions.DoHealing(character, item, 2)
CharacterExtensions.DetermineSpiciness(character, character.World:CreateMessage():AddLine(11, ""That was a spicy one!""))"
                        },
                        {
                            "EatCookedRat",
                            "
local item = CharacterExtensions.ConsumedItem(character, effect)
CharacterExtensions.DoHealing(character, item, 2)"
                        },
                        {
                            "EatPepper",
                            "        
local item = CharacterExtensions.ConsumedItem(character, effect)
local msg = character.World:CreateMessage():AddLine(7, CharacterExtensions.Name(character) .. "" eats the pepper."")
CharacterExtensions.DetermineSpiciness(character, msg)"
                        },
                        {
                            "EatSmokedPepper",
                            "        
local item = CharacterExtensions.ConsumedItem(character, effect)
local msg = character.World:CreateMessage():AddLine(7, CharacterExtensions.Name(character) .. "" eats the smoked pepper."")
CharacterExtensions.DetermineSpiciness(character, msg)"
                        },
                        {
                            "SleepAtInn",
                            "
if character:GetFlag(""PaidInnkeeper"") then
    character:SetFlag(""PaidInnkeeper"", false)
    CharacterExtensions.AddEnergy(character, CharacterExtensions.MaximumEnergy(character) - CharacterExtensions.Energy(character))
    character.World:CreateMessage():
                AddLine(7, CharacterExtensions.Name(character) .. "" rests and feels refreshed!""):
                AddLine(7, CharacterExtensions.Name(character) .. "" has "" .. CharacterExtensions.Energy(character) .. ""/"" .. CharacterExtensions.MaximumEnergy(character) .. "" energy."")
else
    character.World:CreateMessage():
                AddLine(7, CharacterExtensions.Name(character) .. "" needs to pay Gorachan first!"")
end"
                        },
                        {
                            "PotterFlavorText",
                            "world:CreateMessage():AddLine(7, ""Um. Thanks!""):AddLine(7, ""...""):AddLine(7, ""What's a 'Movie'?""):AddChoice(""Nevermind!"", ""ExitDialog"")"
                        },
                        {
                            "Teleport",
"
require('Content.Scripts.teleport')
doTeleport(character,effect)"
                        },
                        {
                            "EnterCellar",
"
require('Content.Scripts.teleport')
if character:GetFlag(""RatQuest"") then
    doTeleport(character,effect)
else
    character.World:CreateMessage():AddLine(7, CharacterExtensions.Name(character) .. "" has no business in the cellar."")
end"
                        },
                        {
                            "PotterTalk",
"
world:CreateMessage():
    AddLine(7, ""Greetings! I am Harold, the Potter.""):
    AddLine(7, ""I make pots! For jools!""):
    AddChoice(""Cool story, bro!"", ""ExitDialog""):
    AddChoice(""I loved yer movies!"", ""PotterFlavorText""):
    AddChoice(""Make me a pot!"", ""PotterMakePot"")"
                        },
                        {
                            "AcceptRatQuest",
                            "character:SetFlag(""RatQuest"", true)"
                        },
                        {
                            "EatRatCorpse",
                            "
local item = CharacterExtensions.ConsumedItem(character, effect)
if RNG.GenerateBoolean(50, 50) then
    CharacterExtensions.DoHealing(character, item, 1)
else
    CharacterExtensions.SetHealth(character, CharacterExtensions.Health(character) - 1)
    local msg = character.World:CreateMessage():
        AddLine(7, ItemExtensions.Name(item) .. "" is tainted!""):
        AddLine(7, CharacterExtensions.Name(character) .. "" loses 1 health!"")
    if CharacterExtensions.IsDead(character) then
        msg:AddLine(4, CharacterExtensions.Name(character) .. "" dies."")
    else
        msg:AddLine(7, CharacterExtensions.Name(character) .. "" now has "" .. CharacterExtensions.Health(character) .. ""/"" .. CharacterExtensions.MaximumHealth(character) .. "" health"")
    end
end"
                        },
                        {
                            "UseEnergyHerb",
                            "
local item = CharacterExtensions.ConsumedItem(character, effect)
local energyBenefit = 10
CharacterExtensions.AddEnergy(character, energyBenefit)
character.World:
    CreateMessage():
    AddLine(7, CharacterExtensions.Name(character) .. "" eats the "" .. ItemExtensions.Name(item) .. "".""):
    AddLine(7, CharacterExtensions.Name(character) .. "" regains energy!""):
    AddLine(7, CharacterExtensions.Name(character) .. "" now has "" .. CharacterExtensions.Energy(character) .. ""/"" .. CharacterExtensions.MaximumEnergy(character) .. "" energy."")"
                        }
                    })
            },
            {
                "OliveGlop",
                New CharacterTypeDescriptor(
                    "Olive Glop",
                    ChrW(&H1B),
                    10,
                    ChrW(&H1A),
                    0,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {"Health", 1},
                        {"MaximumHealth", 1},
                        {"AttackDice", 2},
                        {"MaximumAttack", 1},
                        {"DefendDice", 1},
                        {"MaximumDefend", 1},
                        {"Peril", 5},
                        {"XP", 1}
                    },
                    initializeScript:="character:SetStatistic('Jools', RNG.RollDice('3d6/6'))")
            },
            {
                "Rat",
                New CharacterTypeDescriptor(
                    "Rat",
                    ChrW(&H2A),
                    6,
                    ChrW(&H32),
                    0,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {"Health", 1},
                        {"MaximumHealth", 1},
                        {"AttackDice", 1},
                        {"MaximumAttack", 1},
                        {"DefendDice", 1},
                        {"MaximumDefend", 1},
                        {"Peril", 3},
                        {"XP", 0}
                    },
                    initializeScript:="character:AddItem(ItemInitializer.CreateItem(character.World, 'RatCorpse'))")
            },
            {
                "Scarecrow",
                New CharacterTypeDescriptor(
                    "Scarecrow",
                    ChrW(&H3A),
                    14,
                    ChrW(&H3C),
                    0,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {"Health", 3},
                        {"MaximumHealth", 3},
                        {"AttackDice", 4},
                        {"MaximumAttack", 2},
                        {"DefendDice", 3},
                        {"MaximumDefend", 3},
                        {"Peril", 7},
                        {"XP", 3}
                    },
initializeScript:="if RNG.RollDice('1d5/5') > 0 then
    character:AddItem(ItemInitializer.CreateItem(character.World, 'StrawHat'))
end")
            },
            {
                "CherryGlop",
                New CharacterTypeDescriptor(
                    "Cherry Glop",
                    ChrW(&H1B),
                    4,
                    ChrW(&H1A),
                    0,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {"Health", 2},
                        {"MaximumHealth", 2},
                        {"AttackDice", 4},
                        {"MaximumAttack", 2},
                        {"DefendDice", 1},
                        {"MaximumDefend", 1},
                        {"Peril", 10},
                        {"XP", 2}
                    },
                    initializeScript:="character:SetStatistic('Jools', RNG.RollDice('6d6/6'))")
            }
        }

    <Extension>
    Friend Function ToCharacterTypeDescriptor(characterType As String) As CharacterTypeDescriptor
        Return descriptors(characterType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
