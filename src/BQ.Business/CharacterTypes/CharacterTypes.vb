Friend Module CharacterTypes
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, CharacterTypeDescriptor) =
        New Dictionary(Of String, CharacterTypeDescriptor) From
        {
            {
                "Loxy",
                New CharacterTypeDescriptor(
                    "Loxy",
                    ChrW(2),
                    Pink,
                    ChrW(1),
                    Black,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Health, 2},
                        {StatisticTypes.MaximumHealth, 2},
                        {StatisticTypes.AttackDice, 2},
                        {StatisticTypes.MaximumAttack, 1},
                        {StatisticTypes.DefendDice, 4},
                        {StatisticTypes.MaximumDefend, 2},
                        {StatisticTypes.XP, 0},
                        {StatisticTypes.XPGoal, 10},
                        {StatisticTypes.XPLevel, 1},
                        {StatisticTypes.AdvancementPointsPerLevel, 10},
                        {StatisticTypes.AdvancementPoints, 0},
                        {StatisticTypes.Energy, 10},
                        {StatisticTypes.MaximumEnergy, 10}
                    },
                    effectHandlers:=LoxyEffectHandlers.All,
                    initializeScript:="
--character:AddItem(ItemInitializer.CreateItem(character.World,""RatCorpse""))",
                    effectScripts:=New Dictionary(Of String, String) From
                    {
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
                    Hue.LightGreen,
                    ChrW(&H1A),
                    Hue.Black,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Health, 1},
                        {StatisticTypes.MaximumHealth, 1},
                        {StatisticTypes.AttackDice, 2},
                        {StatisticTypes.MaximumAttack, 1},
                        {StatisticTypes.DefendDice, 1},
                        {StatisticTypes.MaximumDefend, 1},
                        {StatisticTypes.Peril, 5},
                        {StatisticTypes.XP, 1}
                    },
                    initializeScript:="character:SetStatistic('Jools', RNG.RollDice('3d6/6'))")
            },
            {
                "Rat",
                New CharacterTypeDescriptor(
                    "Rat",
                    ChrW(&H2A),
                    Hue.Brown,
                    ChrW(&H32),
                    Hue.Black,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Health, 1},
                        {StatisticTypes.MaximumHealth, 1},
                        {StatisticTypes.AttackDice, 1},
                        {StatisticTypes.MaximumAttack, 1},
                        {StatisticTypes.DefendDice, 1},
                        {StatisticTypes.MaximumDefend, 1},
                        {StatisticTypes.Peril, 3},
                        {StatisticTypes.XP, 0}
                    },
                    initializeScript:="character:AddItem(ItemInitializer.CreateItem(character.World, 'RatCorpse'))")
            },
            {
                "Scarecrow",
                New CharacterTypeDescriptor(
                    "Scarecrow",
                    ChrW(&H3A),
                    Hue.Yellow,
                    ChrW(&H3C),
                    Hue.Black,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Health, 3},
                        {StatisticTypes.MaximumHealth, 3},
                        {StatisticTypes.AttackDice, 4},
                        {StatisticTypes.MaximumAttack, 2},
                        {StatisticTypes.DefendDice, 3},
                        {StatisticTypes.MaximumDefend, 3},
                        {StatisticTypes.Peril, 7},
                        {StatisticTypes.XP, 3}
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
                    Hue.Red,
                    ChrW(&H1A),
                    Hue.Black,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Health, 2},
                        {StatisticTypes.MaximumHealth, 2},
                        {StatisticTypes.AttackDice, 4},
                        {StatisticTypes.MaximumAttack, 2},
                        {StatisticTypes.DefendDice, 1},
                        {StatisticTypes.MaximumDefend, 1},
                        {StatisticTypes.Peril, 10},
                        {StatisticTypes.XP, 2}
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
