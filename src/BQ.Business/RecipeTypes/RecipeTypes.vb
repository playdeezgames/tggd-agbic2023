Imports System.Text.Json

Friend Module RecipeTypes
    Friend Sub Load(filename As String)
        Descriptors = JsonSerializer.Deserialize(Of Dictionary(Of String, RecipeDescriptor))(System.IO.File.ReadAllText(filename))
    End Sub
    Private Descriptors As IReadOnlyDictionary(Of String, RecipeDescriptor)
    Friend Function CanCraft(recipeName As String, character As ICharacter) As Boolean
        Return Descriptors(recipeName).CanCraft(character)
    End Function
    Friend Function Craft(recipeName As String, character As ICharacter) As Boolean
        If CanCraft(recipeName, character) Then
            Descriptors(recipeName).Craft(character)
            Return True
        End If
        Return False
    End Function
    Friend Function Inputs(recipeName As String) As IEnumerable(Of (itemType As String, count As Integer))
        Return Descriptors(recipeName).Inputs.Select(Function(x) (x.Key, x.Value))
    End Function
End Module
