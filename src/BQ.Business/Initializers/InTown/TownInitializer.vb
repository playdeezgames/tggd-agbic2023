Imports SPLORR.Game

Friend Module TownInitializer
    Friend Const TownColumns = 17
    Friend Const TownRows = 17
    Private ReadOnly townMinimap As IReadOnlyList(Of String) =
        New List(Of String) From
        {
            "!!!!!!!!*!!!!!!!!",
            "!.......*.......!",
            "!.......*.......!",
            "!.+^*********^+.!",
            "!..**..+^...**..!",
            "!..*.........*..!",
            "!..*.+^***^+.*..!",
            "!..*..**.**..*..!",
            "*******...*******",
            "!..*..**.**..*..!",
            "!..*.+^***^+.*..!",
            "!..*.........*..!",
            "!..**...^+..**..!",
            "!.+^*********^+.!",
            "!.......*.......!",
            "!.......*.......!",
            "!!!!!!!!*!!!!!!!!"
        }
    Private ReadOnly table As IReadOnlyDictionary(Of Char, String) =
        New Dictionary(Of Char, String) From
        {
            {"!"c, "Fence"},
            {"."c, "Grass"},
            {"*"c, "Gravel"},
            {"^"c, "House"},
            {"+"c, "Sign"}
        }

    Friend Sub Initialize(map As IMap)
        Dim row = 0
        For Each line In townMinimap
            Dim column = 0
            For Each character In line
                map.GetCell(column, row).TerrainType = table(character)
                column += 1
            Next
            row += 1
        Next

        InitializeHealer(map)
        map.GetCell(2, 3).Effect =
            map.CreateEffect()
        SetEffectType(map.GetCell(2, 3).Effect, "Message")
        map.GetCell(2, 3).Effect.SetMetadata("MessageType", "InnSign")
        map.GetCell(14, 3).Effect =
            map.CreateEffect()
        SetEffectType(map.GetCell(14, 3).Effect, "Message")
        map.GetCell(14, 3).Effect.SetMetadata("MessageType", "TownSign2")
        map.GetCell(7, 4).Effect =
            map.CreateEffect()
        SetEffectType(map.GetCell(7, 4).Effect, "Message")
        map.GetCell(7, 4).Effect.SetMetadata("MessageType", "HealthTrainerSign")
        map.GetCell(5, 6).Effect =
            map.CreateEffect()
        SetEffectType(map.GetCell(5, 6).Effect, "Message")
        map.GetCell(5, 6).Effect.SetMetadata("MessageType", "EnergyTrainerSign")
        map.GetCell(11, 6).Effect =
            map.CreateEffect()
        SetEffectType(map.GetCell(11, 6).Effect, "Message")
        map.GetCell(11, 6).Effect.SetMetadata("MessageType", "TownSign5")
        map.GetCell(5, 10).Effect =
            map.CreateEffect()
        SetEffectType(map.GetCell(5, 10).Effect, "Message")
        map.GetCell(5, 10).Effect.SetMetadata("MessageType", "TownSign6")
        map.GetCell(11, 10).Effect =
            map.CreateEffect()
        SetEffectType(map.GetCell(11, 10).Effect, "Message")
        map.GetCell(11, 10).Effect.SetMetadata("MessageType", "DruidSign")
        map.GetCell(9, 12).Effect =
            map.CreateEffect()
        SetEffectType(map.GetCell(11, 10).Effect, "Message")
        map.GetCell(9, 12).Effect.SetMetadata("MessageType", "TownSign8")
        map.GetCell(14, 13).Effect =
            map.CreateEffect()
        SetEffectType(map.GetCell(14, 13).Effect, "Message")
        map.GetCell(14, 13).Effect.SetMetadata("MessageType", "PotterSign")
    End Sub

    Private Sub InitializeHealer(townMap As IMap)
        townMap.GetCell(2, 13).Effect =
            townMap.CreateEffect()
        SetEffectType(townMap.GetCell(2, 13).Effect, "Message")
        townMap.GetCell(2, 13).Effect.SetMetadata("MessageType", "HealerSign")
    End Sub
End Module
