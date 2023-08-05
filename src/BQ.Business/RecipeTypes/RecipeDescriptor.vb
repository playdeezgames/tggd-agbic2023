﻿Friend Class RecipeDescriptor
    Friend ReadOnly Property Name As String
    Friend ReadOnly Property Inputs As IReadOnlyDictionary(Of String, Integer)
    Friend ReadOnly Property Outputs As IReadOnlyDictionary(Of String, Integer)
    Sub New(name As String, inputs As IReadOnlyDictionary(Of String, Integer), Optional outputs As IReadOnlyDictionary(Of String, Integer) = Nothing)
        Me.Name = name
        Me.Inputs = inputs
        Me.Outputs = If(outputs, New Dictionary(Of String, Integer))
    End Sub

    Friend Function CanCraft(character As ICharacter) As Boolean
        Dim itemStacks = character.Items.GroupBy(Function(x) x.ItemType).ToDictionary(Function(x) x.Key, Function(x) x.Count)
        For Each input In Inputs
            If Not itemStacks.ContainsKey(input.Key) OrElse itemStacks(input.Key) < input.Value Then
                Return False
            End If
        Next
        Return True
    End Function

    Friend Sub Craft(character As ICharacter)
        If Not CanCraft(character) Then
            Return
        End If
        Dim deltas = New Dictionary(Of String, Integer)(Outputs)
        For Each input In Inputs
            If deltas.ContainsKey(input.Key) Then
                deltas(input.Key) -= input.Value
            Else
                deltas(input.Key) = -input.Value
            End If
        Next
        For Each delta In deltas
            Select Case delta.Value
                Case Is < 0
                    For Each item In character.Items.Where(Function(x) x.ItemType = delta.Key).Take(-delta.Value)
                        character.RemoveItem(item)
                        item.Recycle()
                    Next
                Case Is > 0
                    For Each item In Enumerable.Range(1, delta.Value).Select(Function(x) ItemInitializer.CreateItem(character.World, delta.Key))
                        character.AddItem(item)
                    Next
            End Select
        Next
    End Sub

    Friend ReadOnly Property Description As String
        Get
            Dim inputText = String.Join("+", Inputs.Select(Function(x) $"{If(x.Value > 1, $"{x.Value} ", "")}{x.Key.ToItemTypeDescriptor.Name}").ToArray)
            Dim outputText = String.Join("+", Outputs.Select(Function(x) $"{If(x.Value > 1, $"{x.Value} ", "")}{x.Key.ToItemTypeDescriptor.Name}").ToArray)
            Return $"{inputText}->{outputText}"
        End Get
    End Property
End Class
