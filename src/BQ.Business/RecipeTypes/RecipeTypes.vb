Imports System.Text.Json

Friend Module RecipeTypes
    Friend Sub Load(filename As String)
        Descriptors = JsonSerializer.Deserialize(Of Dictionary(Of String, RecipeDescriptor))(System.IO.File.ReadAllText(filename))
    End Sub
    Private Descriptors As IReadOnlyDictionary(Of String, RecipeDescriptor)
    Public Function CanCraft(recipeName As String, character As ICharacter) As Boolean
        Return Descriptors(recipeName).CanCraft(character)
    End Function
    Public Function Craft(recipeName As String, character As ICharacter) As Boolean
        If CanCraft(recipeName, character) Then
            Descriptors(recipeName).Craft(character)
            Return True
        End If
        Return False
    End Function
    Public Function Inputs(recipeName As String) As Ingredient()
        Return Descriptors(recipeName).Inputs.Select(Function(x) New Ingredient With {.ItemType = x.Key, .Count = x.Value}).ToArray
    End Function
End Module
