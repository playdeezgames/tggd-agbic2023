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
    Friend Const Clay = "Clay"
    Friend Const UnfiredPot = "UnfiredPot"
    Friend Const ClayPot = "ClayPot"
    Friend Const CrackedPot = "CrackedPot"
    Friend Const WaterPot = "WaterPot"
    Friend Const Wheat = "Wheat"
    Friend Const Flour = "Flour"
    Friend Const Dough = "Dough"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, ItemTypeDescriptor) =
        New Dictionary(Of String, ItemTypeDescriptor) From
        {
            {PlantFiber, New ItemTypeDescriptor("Plant Fiber", ChrW(&H21), LightGreen)},
            {Wheat, New WheatDescriptor()},
            {Flour, New FlourDescriptor()},
            {Dough, New ItemTypeDescriptor("Dough", ChrW(&H44), Tan)},
            {RatBody, New RatBodyDescriptor()},
            {RatTail, New ItemTypeDescriptor("Rat Tail", ChrW(&H2E), DarkGray)},
            {CookedRat, New CookedRatDescriptor()},
            {RatCorpse, New RatCorpseDescriptor()},
            {Rock, New ItemTypeDescriptor("Rock", ChrW(&H30), DarkGray)},
            {Clay, New ItemTypeDescriptor("Clay", ChrW(&H30), Tan)},
            {Pepper, New ItemTypeDescriptor("Pepper", ChrW(&H39), Red)},
            {SharpRock, New SharpRockDescriptor()},
            {Twine, New ItemTypeDescriptor("Twine", ChrW(&H21), Tan)},
            {Torch, New ItemTypeDescriptor("Torch", ChrW(&H34), Red)},
            {Charcoal, New ItemTypeDescriptor("Charcoal", ChrW(&H36), DarkGray)},
            {StrawHat, New ItemTypeDescriptor("Straw Hat", ChrW(&H3B), Yellow)},
            {UnfiredPot, New ItemTypeDescriptor("Unfired Pot", ChrW(&H3F), Tan)},
            {ClayPot, New ItemTypeDescriptor("Clay Pot", ChrW(&H3F), Brown)},
            {CrackedPot, New ItemTypeDescriptor("Cracked Pot", ChrW(&H40), Brown)},
            {WaterPot, New ItemTypeDescriptor("Water-Filled Pot", ChrW(&H40), Brown)},
            {Hatchet, New HatchetDescriptor()},
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
