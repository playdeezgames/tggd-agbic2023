Imports System.Runtime.CompilerServices

Friend Module CharacterExtensions
    <Extension>
    Friend Function Name(character As ICharacter) As String
        Return character.CharacterType.ToCharacterTypeDescriptor.Name
    End Function
    <Extension>
    Friend Function Verbs(character As ICharacter) As IEnumerable(Of String)
        Return character.CharacterType.ToCharacterTypeDescriptor.AvailableVerbs
    End Function
    <Extension>
    Friend Sub DoVerb(target As ICharacter, verbType As String, source As ICharacter)
        target.CharacterType.ToCharacterTypeDescriptor.Verbs(verbType).Invoke(source, target)
    End Sub
    <Extension>
    Private Function Weapons(character As ICharacter) As IEnumerable(Of IItem)
        Return character.EquippedItems.Where(Function(x) x.IsWeapon)
    End Function
    <Extension>
    Private Function Armors(character As ICharacter) As IEnumerable(Of IItem)
        Return character.EquippedItems.Where(Function(x) x.IsArmor)
    End Function
    <Extension>
    Friend Sub TakeItem(character As ICharacter, item As IItem)
        character.Cell.RemoveItem(item)
        character.AddItem(item)
    End Sub
    <Extension>
    Friend Sub DropItem(character As ICharacter, item As IItem)
        character.Cell.AddItem(item)
        character.RemoveItem(item)
    End Sub
    <Extension>
    Private Function CanEnter(character As ICharacter, cell As ICell) As Boolean
        Return cell IsNot Nothing AndAlso cell.IsTenable
    End Function
    <Extension>
    Private Sub DoTrigger(character As ICharacter, cell As ICell)
        If cell IsNot Nothing AndAlso cell.HasTrigger Then
            cell.DoTrigger(character)
        End If
    End Sub
    <Extension>
    Friend Sub Move(character As ICharacter, delta As (x As Integer, y As Integer))
        Dim cell = character.Cell
        Dim nextCell = cell.Map.GetCell(cell.Column + delta.x, cell.Row + delta.y)
        If Not character.CanEnter(nextCell) Then
            character.DoTrigger(nextCell)
            Return
        End If
        nextCell.Character = character
        character.Cell.Character = Nothing
        character.Cell = nextCell
        character.DoTrigger(nextCell)
    End Sub
End Module
