﻿Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports BQ.Persistence

Friend Module ItemTypes
    Friend Const PlantFiber = "PlantFiber"
    Friend Const Twine = "Twine"
    Friend Const Stick = "Stick"
    Friend Const EnergyHerb = "EnergyHerb"
    Friend Const RatCorpse = "RatCorpse"
    Friend Const Rock = "Rock"
    Friend Const SharpRock = "Sharp Rock"
    Friend Const RatBody = "RatBody"
    Friend Const RatTail = "RatTail"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, ItemTypeDescriptor) =
        New Dictionary(Of String, ItemTypeDescriptor) From
        {
            {PlantFiber, New ItemTypeDescriptor("Plant Fiber", ChrW(&H23), LightGreen)},
            {RatBody, New RatBodyDescriptor()},
            {RatTail, New ItemTypeDescriptor("Rat Tail", ChrW(&H2E), DarkGray)},
            {RatCorpse, New RatCorpseDescriptor()},
            {Rock, New ItemTypeDescriptor("Rock", ChrW(&H30), LightGray)},
            {SharpRock, New SharpRockDescriptor()},
            {Twine, New ItemTypeDescriptor("Twine", ChrW(&H21), Tan)},
            {Stick, New StickDescriptor()},
            {EnergyHerb, New EnergyHerbDescriptor()}
        }

    <Extension>
    Friend Function ToItemTypeDescriptor(itemType As String) As ItemTypeDescriptor
        Return descriptors(itemType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
