﻿Imports SPLORR.Game

Friend Class TreeDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
                    "Tree",
                    ChrW(&HA),
                    Business.Hue.Green,
                    True,
                    cellInitializer:=AddressOf InitializeTree,
                    verbTypes:=New Dictionary(Of String, Action(Of ICharacter, ICell)) From
                    {
                        {Business.VerbTypes.Forage, AddressOf DoForage}
                    },
                    foragables:=New Dictionary(Of String, Integer) From
                    {
                        {String.Empty, 75},
                        {ItemTypes.Stick, 100},
                        {ItemTypes.Rock, 25}
                    })
    End Sub

    Private Shared Sub InitializeTree(cell As ICell)
        cell.Statistic(StatisticTypes.Peril) = 1
    End Sub
End Class
