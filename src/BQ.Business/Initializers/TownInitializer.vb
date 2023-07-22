﻿Imports SPLORR.Game

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
            {"!"c, TerrainTypes.Fence},
            {"."c, TerrainTypes.Grass},
            {"*"c, TerrainTypes.Gravel},
            {"^"c, TerrainTypes.House},
            {"+"c, TerrainTypes.Sign}
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
        map.GetCell(2, 3).Trigger =
            map.CreateTrigger().
            SetTriggerType(TriggerTypes.Message).
            AddMessageLine(LightGray, "This is sign #1.")
        map.GetCell(14, 3).Trigger =
            map.CreateTrigger().
            SetTriggerType(TriggerTypes.Message).
            AddMessageLine(LightGray, "This is sign #2.")
        map.GetCell(7, 4).Trigger =
            map.CreateTrigger().
            SetTriggerType(TriggerTypes.Message).
            AddMessageLine(LightGray, "This is sign #3.")
        map.GetCell(5, 6).Trigger =
            map.CreateTrigger().
            SetTriggerType(TriggerTypes.Message).
            AddMessageLine(LightGray, "This is sign #4.")
        map.GetCell(11, 6).Trigger =
            map.CreateTrigger().
            SetTriggerType(TriggerTypes.Message).
            AddMessageLine(LightGray, "This is sign #5.")
        map.GetCell(5, 10).Trigger =
            map.CreateTrigger().
            SetTriggerType(TriggerTypes.Message).
            AddMessageLine(LightGray, "This is sign #6.")
        map.GetCell(11, 10).Trigger =
            map.CreateTrigger().
            SetTriggerType(TriggerTypes.Message).
            AddMessageLine(LightGray, "This is sign #7.")
        map.GetCell(9, 12).Trigger =
            map.CreateTrigger().
            SetTriggerType(TriggerTypes.Message).
            AddMessageLine(LightGray, "This is sign #8.")
        map.GetCell(14, 13).Trigger =
            map.CreateTrigger().
            SetTriggerType(TriggerTypes.Message).
            AddMessageLine(LightGray, "This is sign #10.")
    End Sub

    Private Sub InitializeHealer(townMap As IMap)
        townMap.GetCell(2, 13).Trigger =
            townMap.CreateTrigger().
            SetTriggerType(TriggerTypes.Message).
            AddMessageLine(LightGray, "House of Nihilistic Healing")
    End Sub
End Module
