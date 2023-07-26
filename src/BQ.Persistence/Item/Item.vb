﻿Imports System.Net.NetworkInformation

Friend Class Item
    Inherits ItemDataClient
    Implements IItem
    Public Sub New(worldData As Data.WorldData, itemId As Integer)
        MyBase.New(worldData, itemId)
    End Sub
    Public ReadOnly Property Id As Integer Implements IItem.Id
        Get
            Return ItemId
        End Get
    End Property

    Public ReadOnly Property ItemType As String Implements IItem.ItemType
        Get
            Return ItemData.ItemType
        End Get
    End Property

    Public Property Statistic(statisticType As String) As Integer Implements IItem.Statistic
        Get
            Return ItemData.Statistics(statisticType)
        End Get
        Set(value As Integer)
            ItemData.Statistics(statisticType) = value
        End Set
    End Property

    Public Property Flag(flagType As String) As Boolean Implements IFlagHolder.Flag
        Get
            Return ItemData.Flags.Contains(flagType)
        End Get
        Set(value As Boolean)
            If value Then
                ItemData.Flags.Add(flagType)
            Else
                ItemData.Flags.Remove(flagType)
            End If
        End Set
    End Property

    Public Sub Recycle() Implements IItem.Recycle
        ItemData.Recycled = True
    End Sub

    Public Sub RemoveStatistic(statisticType As String) Implements IStatisticsHolder.RemoveStatistic
        ItemData.Statistics.Remove(statisticType)
    End Sub

    Public Function HasStatistic(statisticType As String) As Boolean Implements IStatisticsHolder.HasStatistic
        Return ItemData.Statistics.ContainsKey(statisticType)
    End Function
End Class
