Friend Module InnInitializer
    Friend Const InnColumns = 7
    Friend Const InnRows = 7
    Private ReadOnly innMinimap As IReadOnlyList(Of String) =
        New List(Of String) From
        {
            "#######",
            "#b # b#",
            "#  #  #",
            "##D#D##",
            "#     #",
            "#    !#",
            "###D###"
        }
    Private ReadOnly table As IReadOnlyDictionary(Of Char, String) =
        New Dictionary(Of Char, String) From
        {
            {"#"c, TerrainTypes.Wall},
            {"b"c, TerrainTypes.Bed},
            {" "c, TerrainTypes.Empty},
            {"D"c, TerrainTypes.Door},
            {"!"c, TerrainTypes.Gorachan}
        }
    Friend Sub Initialize(map As IMap)
        Dim row As Integer = 0
        For Each mapLine In innMinimap
            Dim column As Integer = 0
            For Each mapCell In mapLine
                map.GetCell(column, row).TerrainType = table(mapCell)
                column += 1
            Next
            row += 1
        Next
    End Sub
End Module
