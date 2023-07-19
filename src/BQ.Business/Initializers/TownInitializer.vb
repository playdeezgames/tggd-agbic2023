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
        map.GetCell(2, 13).Trigger =
            map.CreateTrigger().
            SetTriggerType(TriggerTypes.Message).
            AddMessageLine(LightGray, "This is sign #9.")
        map.GetCell(14, 13).Trigger =
            map.CreateTrigger().
            SetTriggerType(TriggerTypes.Message).
            AddMessageLine(LightGray, "This is sign #10.")

        map.GetCell(map.Columns \ 2, map.Rows - 1).Trigger =
            map.CreateTrigger().
            SetTriggerType(TriggerTypes.Teleport).
            SetDestination(map.GetCell(map.Columns \ 2, 0))
        map.GetCell(map.Columns \ 2, 0).Trigger =
            map.CreateTrigger().
            SetTriggerType(TriggerTypes.Teleport).
            SetDestination(map.GetCell(map.Columns \ 2, map.Rows - 1))
        map.GetCell(0, map.Rows \ 2).Trigger =
            map.CreateTrigger().
            SetTriggerType(TriggerTypes.Teleport).
            SetDestination(map.GetCell(map.Columns - 1, map.Rows \ 2))
        map.GetCell(map.Columns - 1, map.Rows \ 2).Trigger =
            map.CreateTrigger().
            SetTriggerType(TriggerTypes.Teleport).
            SetDestination(map.GetCell(0, map.Rows \ 2))
    End Sub
End Module
