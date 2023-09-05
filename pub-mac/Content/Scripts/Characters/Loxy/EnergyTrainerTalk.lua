
local trainCost = CharacterExtensions.MaximumEnergy(character) * 2
character.World:CreateMessage():
    AddLine(7, "I am the endurance trainer."):
    AddLine(7, "I can increase yer energy"):
    AddLine(7, "for the cost of 1AP and " .. trainCost .. " jools."):
    AddChoice("Cool story, bro!", "ExitDialog"):
    AddChoice("Train me!", "TrainEnergy")