
character.World:CreateMessage():
    AddLine(7, "Greetings! I am Marcus, the hippy druid."):
    AddLine(7, "I can help you learn nature's way."):
    AddChoice("Cool story, bro!", "ExitDialog"):
    AddChoice("Don't druids live in the woods?", "DruidAllergies"):
    AddChoice("Teach me!", "DruidTeachMenu"):
    AddChoice("What's for sale?", "DruidPrices")