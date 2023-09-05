local msg = character.World:CreateMessage():
                AddLine(7, "Welcome to Jusdatip Inn!"):
                AddLine(7, "I'm Gorachan."):
                AddLine(7, "You can rest in a bed for 1 jools."):
                AddLine(7, "I'd offer to join you,"):
                AddLine(7, "but then you wouldn't get any rest!"):
                AddChoice("Cool story, bro!", "ExitDialog"):
                AddChoice("Yer a pervert!", "PervertInnkeeper"):
                AddChoice("I'll take a bed.", "PayInnkeeper")
if character:GetFlag("RatQuest") then
    if CharacterExtensions.HasItemTypeInInventory(character, "RatTail") then
        msg:AddChoice("Here's some rat tails!", "CompleteRatQuest")
    end
else
    msg:AddChoice("I need a job!", "StartRatQuest")
end