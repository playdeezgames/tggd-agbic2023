
character.World:CreateMessage():
    AddLine(7, "Well, there are a bunch of rats in the cellar."):
    AddLine(7, "I'll pay you 1 jools for each rat tail."):
    AddLine(7, "I only accept the tails, no corpses."):
    AddLine(7, "So you'll need to cut them off first."):
    AddChoice("I'm on it!", "AcceptRatQuest"):
    AddChoice("Mebbe later?", "ExitDialog")