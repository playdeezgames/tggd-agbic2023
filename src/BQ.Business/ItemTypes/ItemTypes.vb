Public Module ItemTypes
    Friend Sub Save(filename As String)
        File.WriteAllText(filename, JsonSerializer.Serialize(descriptors))
    End Sub
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, ItemTypeDescriptor) =
        New Dictionary(Of String, ItemTypeDescriptor) From
        {
            {"PlantFiber", New ItemTypeDescriptor("Plant Fiber", ChrW(&H21), 10)},
            {"Wheat", New WheatDescriptor()},
            {"Flour", New FlourDescriptor()},
            {"Dough", New ItemTypeDescriptor("Dough", ChrW(&H44), 13)},
            {"Bagel", New ItemTypeDescriptor("Bagel", ChrW(&H44), 6)},
            {"Paprika", New ItemTypeDescriptor("Paprika", ChrW(&H43), 11)},
            {"SmokedPepper", New SmokedPepperDescriptor()},
            {"RatBody", New RatBodyDescriptor()},
            {"RatTail", New ItemTypeDescriptor("Rat Tail", ChrW(&H2E), 8)},
            {"CookedRat", New CookedRatDescriptor()},
            {"SeasonedRat", New SeasonedRatDescriptor()},
            {"RatCorpse", New RatCorpseDescriptor()},
            {"Rock", New ItemTypeDescriptor("Rock", ChrW(&H30), 8)},
            {"Clay", New ItemTypeDescriptor("Clay", ChrW(&H30), 13)},
            {"Pepper", New PepperDescriptor()},
            {"SharpRock", New SharpRockDescriptor()},
            {"Twine", New ItemTypeDescriptor("Twine", ChrW(&H21), 13)},
            {"Torch", New ItemTypeDescriptor("Torch", ChrW(&H34), Red)},
            {"Charcoal", New ItemTypeDescriptor("Charcoal", ChrW(&H36), 8)},
            {"StrawHat", New StrawHatDescriptor()},
            {"UnfiredPot", New ItemTypeDescriptor("Unfired Pot", ChrW(&H3F), 13)},
            {"ClayPot", New ItemTypeDescriptor("Clay Pot", ChrW(&H3F), 6)},
            {"CrackedPot", New ItemTypeDescriptor("Cracked Pot", ChrW(&H40), 6)},
            {"WaterPot", New ItemTypeDescriptor("Water-Filled Pot", ChrW(&H40), 6)},
            {"Hatchet", New HatchetDescriptor()},
            {"Stick", New StickDescriptor()},
            {"EnergyHerb", New EnergyHerbDescriptor()}
        }

    Public Function ToItemTypeDescriptor(itemType As String) As ItemTypeDescriptor
        Return If(descriptors.ContainsKey(itemType), descriptors(itemType), Nothing)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
