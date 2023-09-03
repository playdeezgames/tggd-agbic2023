﻿Module PotterEffectHandlers
    Friend Sub DoPotterMakePot(character As ICharacter, effect As IEffect)
        Const price = 20
        Const RecipeType = "UnfiredPot"
        If CharacterExtensions.Jools(character) < price Then
            character.World.CreateMessage().
                        AddLine(7, $"The price is {price} jools.").
                        AddLine(7, $"({CharacterExtensions.Name(character)} has {CharacterExtensions.Jools(character)} jools)")
            Return
        End If
        If Not RecipeTypes.CanCraft(RecipeType, character) Then
            Dim msg = character.World.CreateMessage().AddLine(7, "To make a pot, I need:")
            AddRecipeInputs(character, msg, RecipeType)
            Return
        End If
        CharacterExtensions.AddJools(character, -price)
        RecipeTypes.Craft(RecipeType, character)
        RecipeTypes.Craft("ClayPot", character)
        character.World.CreateMessage().
            AddLine(Red, $"{CharacterExtensions.Name(character)} loses {price} jools").AddLine(10, $"{CharacterExtensions.Name(character)} gains 1 {ToItemTypeDescriptor("ClayPot").Name}")
    End Sub
End Module
