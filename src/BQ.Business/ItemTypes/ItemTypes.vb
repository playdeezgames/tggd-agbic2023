Imports System.ComponentModel
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
    Friend Const Torch = "Torch"
    Friend Const Charcoal = "Charcoal"
    Friend Const CookedRat = "CookedRat"
    Friend Const Pepper = "Pepper"
    Friend Const StrawHat = "StrawHat"
    Friend Const Hatchet = "Hatchet"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, ItemTypeDescriptor) =
        New Dictionary(Of String, ItemTypeDescriptor) From
        {
            {PlantFiber, New ItemTypeDescriptor("Plant Fiber", ChrW(&H21), LightGreen)},
            {RatBody, New RatBodyDescriptor()},
            {RatTail, New ItemTypeDescriptor("Rat Tail", ChrW(&H2E), DarkGray)},
            {CookedRat, New CookedRatDescriptor()},
            {RatCorpse, New RatCorpseDescriptor()},
            {Rock, New ItemTypeDescriptor("Rock", ChrW(&H30), LightGray)},
            {Pepper, New ItemTypeDescriptor("Pepper", ChrW(&H39), Red)},
            {SharpRock, New SharpRockDescriptor()},
            {Twine, New ItemTypeDescriptor("Twine", ChrW(&H21), Tan)},
            {Torch, New ItemTypeDescriptor("Torch", ChrW(&H34), Red)},
            {Charcoal, New ItemTypeDescriptor("Charcoal", ChrW(&H36), DarkGray)},
            {StrawHat, New ItemTypeDescriptor("Straw Hat", ChrW(&H3B), Yellow)},
            {Hatchet, New ItemTypeDescriptor("Hatchet", ChrW(&H3D), DarkGray)},
            {Stick, New StickDescriptor()},
            {EnergyHerb, New EnergyHerbDescriptor()}
        }

    <Extension>
    Friend Function ToItemTypeDescriptor(itemType As String) As ItemTypeDescriptor
        Return If(descriptors.ContainsKey(itemType), descriptors(itemType), Nothing)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
