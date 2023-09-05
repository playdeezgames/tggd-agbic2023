
character.World:CreateMessage():
    AddLine(7, "I am the health trainer!"):
    AddLine(7, "I can help you increase yer health."):
    AddLine(7, "The cost is " .. CharacterExtensions.MaximumHealth(character) * 5 .. " AP."):
    AddChoice("Cool story, bro!", "ExitDialog"):
    AddChoice("Train me!", "TrainHealth")