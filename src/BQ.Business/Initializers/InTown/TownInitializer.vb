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
    Private ReadOnly terrainTable As IReadOnlyDictionary(Of Char, String) =
        New Dictionary(Of Char, String) From
        {
            {"!"c, "Fence"},
            {"."c, "Grass"},
            {"*"c, "Gravel"},
            {"^"c, "House"},
            {"+"c, "Sign"}
        }
    Private ReadOnly signMessages As IReadOnlyList(Of (X As Integer, Y As Integer, MessageType As String)) =
        New List(Of (X As Integer, Y As Integer, MessageType As String)) From
        {
            (2, 13, "HealerSign"),
            (2, 3, "InnSign"),
            (14, 3, "TownSign2"),
            (7, 4, "HealthTrainerSign"),
            (5, 6, "EnergyTrainerSign"),
            (11, 6, "TownSign5"),
            (5, 10, "TownSign6"),
            (11, 10, "DruidSign"),
            (9, 12, "TownSign8"),
            (14, 13, "PotterSign")
        }

    Friend Sub Initialize(map As IMap)
        InitializeTerrain(map)
        For Each signMessage In signMessages
            CreateMessageEffect(map, signMessage.X, signMessage.Y, signMessage.MessageType)
        Next
    End Sub

    Private Sub InitializeTerrain(map As IMap)
        Dim row = 0
        For Each line In townMinimap
            Dim column = 0
            For Each character In line
                map.GetCell(column, row).TerrainType = terrainTable(character)
                column += 1
            Next
            row += 1
        Next
    End Sub

    Private Sub CreateMessageEffect(map As IMap, x As Integer, y As Integer, messageType As String)
        Dim cell = map.GetCell(x, y)
        cell.Effect =
            map.CreateEffect()
        SetEffectType(cell.Effect, "Message")
        cell.Effect.SetMetadata("MessageType", messageType)
    End Sub
End Module
