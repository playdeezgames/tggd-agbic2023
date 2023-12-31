﻿Imports BQ.Data

Friend Class Message
    Inherits MessageDataClient
    Implements IMessage

    Public Sub New(worldData As Data.WorldData, messageId As Integer)
        MyBase.New(worldData, messageId)
    End Sub

    Public ReadOnly Property LineCount As Integer Implements IMessage.LineCount
        Get
            Return MessageData.Lines.Count
        End Get
    End Property

    Private ReadOnly Property ChoiceCount As Integer Implements IMessage.ChoiceCount
        Get
            Return MessageData.Choices.Count
        End Get
    End Property

    Public ReadOnly Property Lines As IEnumerable(Of IMessageLine) Implements IMessage.Lines
        Get
            Return Enumerable.Range(0, LineCount).Select(Function(x) New MessageLine(WorldData, MessageId, x))
        End Get
    End Property

    Public Property Sfx As String Implements IMessage.Sfx
        Get
            Return MessageData.Sfx
        End Get
        Set(value As String)
            MessageData.Sfx = value
        End Set
    End Property

    Public ReadOnly Property HasChoices As Boolean Implements IMessage.HasChoices
        Get
            Return MessageData.Choices.Any
        End Get
    End Property

    Public ReadOnly Property Choices As IEnumerable(Of IMessageChoice) Implements IMessage.Choices
        Get
            Return Enumerable.Range(0, ChoiceCount).Select(Function(x) New MessageChoice(WorldData, MessageId, x))
        End Get
    End Property

    Public Property CancelChoice As Integer Implements IMessage.CancelChoice
        Get
            Return MessageData.CancelChoice
        End Get
        Set(value As Integer)
            MessageData.CancelChoice = value
        End Set
    End Property

    Public ReadOnly Property LastChoice As IMessageChoice Implements IMessage.LastChoice
        Get
            If ChoiceCount > 0 Then
                Return Choice(ChoiceCount - 1)
            End If
            Return Nothing
        End Get
    End Property

    Public Function AddLine(hue As Integer, text As String) As IMessage Implements IMessage.AddLine
        MessageData.Lines.Add(New Data.MessageLineData With
                              {
                                .Text = text,
                                .Hue = hue
                              })
        Return Me
    End Function

    Public Function SetSfx(sfx As String) As IMessage Implements IMessage.SetSfx
        Me.Sfx = sfx
        Return Me
    End Function

    Public Function AddChoice(
                             text As String,
                             effectType As String) As IMessage Implements IMessage.AddChoice
        Dim id = MessageData.Choices.Count
        MessageData.Choices.Add(New Data.MessageChoiceData With
            {
                .Text = text,
                .EffectType = effectType
            })
        Return Me
    End Function

    Public Function Choice(index As Integer) As IMessageChoice Implements IMessage.Choice
        Return New MessageChoice(WorldData, MessageId, index)
    End Function

    Public Sub RemoveStatistic(statisticType As String) Implements IStatisticsHolder.RemoveStatistic
        MessageData.Statistics.Remove(statisticType)
    End Sub

    Public Function HasStatistic(statisticType As String) As Boolean Implements IStatisticsHolder.HasStatistic
        Return MessageData.Statistics.ContainsKey(statisticType)
    End Function

    Public Function HasMetadata(identifier As String) As Boolean Implements IMetadataHolder.HasMetadata
        Return MessageData.Metadatas.ContainsKey(identifier)
    End Function

    Public Sub RemoveMetadata(identifier As String) Implements IMetadataHolder.RemoveMetadata
        MessageData.Metadatas.Remove(identifier)
    End Sub

    Public Function GetStatistic(statisticType As String, Optional defaultValue As Integer = 0) As Integer Implements IStatisticsHolder.GetStatistic
        Return If(HasStatistic(statisticType), MessageData.Statistics(statisticType), defaultValue)
    End Function

    Public Sub SetStatistic(statisticType As String, value As Integer) Implements IStatisticsHolder.SetStatistic
        MessageData.Statistics(statisticType) = value
    End Sub

    Public Function AddStatistic(statisticType As String, delta As Integer) As Integer Implements IStatisticsHolder.AddStatistic
        SetStatistic(statisticType, GetStatistic(statisticType) + delta)
        Return GetStatistic(statisticType)
    End Function

    Public Sub SetFlag(flagType As String, value As Boolean) Implements IFlagHolder.SetFlag
        If value Then
            MessageData.Flags.Add(flagType)
        Else
            MessageData.Flags.Remove(flagType)
        End If
    End Sub

    Public Function GetFlag(flagType As String) As Boolean Implements IFlagHolder.GetFlag
        Return MessageData.Flags.Contains(flagType)
    End Function

    Public Sub SetMetadata(identifier As String, value As String) Implements IMetadataHolder.SetMetadata
        MessageData.Metadatas(identifier) = value
    End Sub

    Public Function GetMetadata(identifier As String) As String Implements IMetadataHolder.GetMetadata
        Return MessageData.Metadatas(identifier)
    End Function
End Class
