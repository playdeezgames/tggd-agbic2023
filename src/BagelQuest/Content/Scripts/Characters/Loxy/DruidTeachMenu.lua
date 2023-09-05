
local canLearnForaging = not character:GetFlag("KnowsForaging")
local canLearnTwineMaking = not character:GetFlag("KnowsTwineMaking")
local canLearnKnapping = not character:GetFlag("KnowsRockSharpening")
local canLearnFireMaking = not character:GetFlag("KnowsFireMaking")
local canLearnTorchMaking = not character:GetFlag("KnowsTorchMaking")
local canLearnHatchetMaking = not character:GetFlag("KnowsHatchetMaking")
local canLearn = canLearnForaging or canLearnTwineMaking or canLearnKnapping or canLearnFireMaking or canLearnTorchMaking or canLearnHatchetMaking
local msg = character.World:CreateMessage()
if not canLearn then
    msg:AddLine(7, "You have learned all I have to teach you.")
    return
end
msg:AddLine(7, "I can teach you these things:")
msg:AddChoice("Good to know!", "ExitDialog")
if canLearnForaging then
    msg:AddChoice(
        "Foraging(-1AP)",
        "LearnForaging")
    msg.LastChoice:SetStatistic("AdvancementPoints", 1)
    msg.LastChoice:SetMetadata("FlagType", "KnowsForaging")
    msg.LastChoice:SetMetadata("TaskName", "forage")
    msg.LastChoice:SetMetadata("ActionName", "Forage...")
    msg.LastChoice:SetMetadata("RecipeType", "Foraging")
end
if canLearnTwineMaking then
    msg:AddChoice(
        "Twine Making(-1AP,-2 Plant Fiber)",
        "LearnTwineMaking")
    msg.LastChoice:SetStatistic("AdvancementPoints", 1)
    msg.LastChoice:SetMetadata("FlagType", "KnowsTwineMaking")
    msg.LastChoice:SetMetadata("TaskName", "make twine")
    msg.LastChoice:SetMetadata("ActionName", "Make Twine")
    msg.LastChoice:SetMetadata("RecipeType", "Twine")
    msg.LastChoice:SetFlag("LearnByDoing", true)
end
if canLearnKnapping then
    msg:AddChoice(
        "Knapping(-1AP,-2 Rock)",
        "LearnKnapping")
    msg.LastChoice:SetStatistic("AdvancementPoints", 1)
    msg.LastChoice:SetMetadata("FlagType", "KnowsRockSharpening")
    msg.LastChoice:SetMetadata("TaskName", "knap")
    msg.LastChoice:SetMetadata("ActionName", "Knap")
    msg.LastChoice:SetMetadata("RecipeType", "SharpRock")
    msg.LastChoice:SetFlag("LearnByDoing", true)
end
if canLearnFireMaking then
    msg:AddChoice(
        "Fire Making(-1AP, -5 Rock, -5 Sticks)",
        "LearnFireMaking")
    msg.LastChoice:SetStatistic("AdvancementPoints", 1)
    msg.LastChoice:SetMetadata("FlagType", "KnowsFireMaking")
    msg.LastChoice:SetMetadata("TaskName", "make a fire")
    msg.LastChoice:SetMetadata("ActionName", "Build Fire")
    msg.LastChoice:SetMetadata("RecipeType", "CookingFire")
    msg.LastChoice:SetMetadata("Caveat", "(only works in clear areas in the wilderness)")
    msg.LastChoice:SetFlag("LearnByDoing", false)
end
if canLearnTorchMaking then
    msg:AddChoice(
        "Torch Making(-1AP, -1 Stick, -1 Charcoal)",
        "LearnTorchMaking")
    msg.LastChoice:SetStatistic("AdvancementPoints", 1)
    msg.LastChoice:SetMetadata("FlagType", "KnowsTorchMaking")
    msg.LastChoice:SetMetadata("TaskName", "make a torch")
    msg.LastChoice:SetMetadata("ActionName", "Make Torch")
    msg.LastChoice:SetMetadata("RecipeType", "Torch")
    msg.LastChoice:SetMetadata("Caveat", "(only works with a source of flames)")
    msg.LastChoice:SetFlag("LearnByDoing", false)
end
if canLearnHatchetMaking then
    msg:AddChoice(
        "Hatchet Making(-1AP,-1Stick,-1S.Rock,-1Twine)",
        "LearnHatchedMaking")
    msg.LastChoice:SetStatistic("AdvancementPoints", 1)
    msg.LastChoice:SetMetadata("FlagType", "KnowsHatchetMaking")
    msg.LastChoice:SetMetadata("TaskName", "make a hatchet")
    msg.LastChoice:SetMetadata("ActionName", "Make Hatchet")
    msg.LastChoice:SetMetadata("RecipeType", "Hatchet")
    msg.LastChoice:SetFlag("LearnByDoing", true)
end