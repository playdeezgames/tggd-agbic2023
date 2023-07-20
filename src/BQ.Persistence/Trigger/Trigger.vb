Imports System.Runtime.CompilerServices
Imports BQ.Data

Friend Class Trigger
    Inherits TriggerDataClient
    Implements ITrigger
    Public Sub New(worldData As WorldData, mapId As Integer, triggerId As Integer)
        MyBase.New(worldData, mapId, triggerId)
    End Sub
    Public ReadOnly Property Id As Integer Implements ITrigger.Id
        Get
            Return TriggerId
        End Get
    End Property

    Public ReadOnly Property TriggerType As String Implements ITrigger.TriggerType
        Get
            Return TriggerData.TriggerType
        End Get
    End Property

    Public Sub Execute(character As ICharacter) Implements ITrigger.Execute
        Select Case TriggerType
            Case TriggerTypes.Message
                ExecuteMessage(character)
            Case TriggerTypes.Teleport
                ExecuteTeleport(character)
        End Select
    End Sub

    Private Sub ExecuteTeleport(character As ICharacter)
        Dim nextCell = character.World.Map(TriggerData.Destination.MapId).GetCell(TriggerData.Destination.Column, TriggerData.Destination.Row)
        nextCell.AddCharacter(character)
        character.Cell.RemoveCharacter(character)
        character.Cell = nextCell
    End Sub

    Private Sub ExecuteMessage(character As ICharacter)
        Dim message = character.World.CreateMessage()
        For Each messageLine In TriggerData.MessageLines
            message.AddLine(messageLine.Hue, messageLine.Text)
        Next
    End Sub

    Public Function SetTriggerType(triggerType As String) As ITrigger Implements ITrigger.SetTriggerType
        TriggerData.TriggerType = triggerType
        Return Me
    End Function

    Public Function AddMessageLine(hue As Integer, text As String) As ITrigger Implements ITrigger.AddMessageLine
        TriggerData.MessageLines.Add(New MessageLineData With {.Hue = hue, .Text = text})
        Return Me
    End Function

    Public Function SetDestination(cell As ICell) As ITrigger Implements ITrigger.SetDestination
        If cell Is Nothing Then
            TriggerData.Destination = Nothing
            Return Me
        End If
        TriggerData.Destination = New TriggerDestinationData With {.MapId = cell.Map.Id, .Column = cell.Column, .Row = cell.Row}
        Return Me
    End Function
End Class
