﻿Friend Class RatCorpseDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Rat Corpse",
            ChrW(&H2D),
            DarkGray,
            verbTypes:=New Dictionary(Of String, Action(Of ICharacter, IItem)) From
            {
                {Business.VerbTypes.CutOffTail, AddressOf DoCutOffTail}
            })
    End Sub

    Private Shared Sub DoCutOffTail(character As ICharacter, item As IItem)
        If Not character.HasCuttingTool Then
            character.World.CreateMessage().AddLine(LightGray, $"{character.Name} needs a cutting tool for that!")
            Return
        End If
        character.RemoveItem(item)
        character.AddItem(ItemInitializer.CreateItem(character.World, ItemTypes.RatBody))
        character.AddItem(ItemInitializer.CreateItem(character.World, ItemTypes.RatTail))
    End Sub
End Class