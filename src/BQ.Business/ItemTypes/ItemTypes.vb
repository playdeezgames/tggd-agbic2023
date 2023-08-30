Friend Module ItemTypes
    Friend Const Wheat = "Wheat"
    Friend Const Flour = "Flour"
    Friend Const Dough = "Dough"
    Friend Const SmokedPepper = "SmokedPepper"
    Friend Const Paprika = "Paprika"
    Friend Const SeasonedRat = "SeasonedRat"
    Friend Const Bagel = "Bagel"
    Friend Sub Save(filename As String)
        System.IO.File.WriteAllText(filename, JsonSerializer.Serialize(descriptors))
    End Sub
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, ItemTypeDescriptor) =
        New Dictionary(Of String, ItemTypeDescriptor) From
        {
            {"PlantFiber", New ItemTypeDescriptor("Plant Fiber", ChrW(&H21), LightGreen)},
            {Wheat, New WheatDescriptor()},
            {Flour, New FlourDescriptor()},
            {Dough, New ItemTypeDescriptor("Dough", ChrW(&H44), Tan)},
            {Bagel, New ItemTypeDescriptor("Bagel", ChrW(&H44), Brown)},
            {Paprika, New ItemTypeDescriptor("Paprika", ChrW(&H43), Orange)},
            {SmokedPepper, New SmokedPepperDescriptor()},
            {"RatBody", New RatBodyDescriptor()},
            {"RatTail", New ItemTypeDescriptor("Rat Tail", ChrW(&H2E), DarkGray)},
            {"CookedRat", New CookedRatDescriptor()},
            {SeasonedRat, New SeasonedRatDescriptor()},
            {"RatCorpse", New RatCorpseDescriptor()},
            {"Rock", New ItemTypeDescriptor("Rock", ChrW(&H30), DarkGray)},
            {"Clay", New ItemTypeDescriptor("Clay", ChrW(&H30), Tan)},
            {"Pepper", New PepperDescriptor()},
            {"SharpRock", New SharpRockDescriptor()},
            {"Twine", New ItemTypeDescriptor("Twine", ChrW(&H21), Tan)},
            {"Torch", New ItemTypeDescriptor("Torch", ChrW(&H34), Red)},
            {"Charcoal", New ItemTypeDescriptor("Charcoal", ChrW(&H36), DarkGray)},
            {"StrawHat", New StrawHatDescriptor()},
            {"UnfiredPot", New ItemTypeDescriptor("Unfired Pot", ChrW(&H3F), Tan)},
            {"ClayPot", New ItemTypeDescriptor("Clay Pot", ChrW(&H3F), Brown)},
            {"CrackedPot", New ItemTypeDescriptor("Cracked Pot", ChrW(&H40), Brown)},
            {"WaterPot", New ItemTypeDescriptor("Water-Filled Pot", ChrW(&H40), Brown)},
            {"Hatchet", New HatchetDescriptor()},
            {"Stick", New StickDescriptor()},
            {"EnergyHerb", New EnergyHerbDescriptor()}
        }

    Friend Function ToItemTypeDescriptor(itemType As String) As ItemTypeDescriptor
        Return If(descriptors.ContainsKey(itemType), descriptors(itemType), Nothing)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
