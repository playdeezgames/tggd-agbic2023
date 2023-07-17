Imports SPLORR.Game

Friend Module TownInitializer
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
    End Sub
End Module
