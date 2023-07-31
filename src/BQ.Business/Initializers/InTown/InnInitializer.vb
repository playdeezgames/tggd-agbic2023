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
            "#v   !#",
            "###D###"
        }
    Private ReadOnly table As IReadOnlyDictionary(Of Char, String) =
        New Dictionary(Of Char, String) From
        {
            {"#"c, TerrainTypes.Wall},
            {"b"c, TerrainTypes.Bed},
            {" "c, TerrainTypes.Empty},
            {"D"c, TerrainTypes.Door},
            {"!"c, TerrainTypes.Gorachan},
            {"v"c, TerrainTypes.StairsDown}
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
        InitializeGorachan(map)
        InitializeBeds(map)
    End Sub

    Private Sub InitializeBeds(map As IMap)
        Dim bedCells = map.Cells.Where(Function(x) x.TerrainType = TerrainTypes.Bed)
        Dim trigger = map.CreateTrigger().SetTriggerType(TriggerTypes.SleepAtInn)
        For Each bedCell In bedCells
            bedCell.Trigger = trigger
        Next
    End Sub

    Private Sub InitializeGorachan(map As IMap)
        Dim gorachanCell = map.Cells.Single(Function(x) x.TerrainType = TerrainTypes.Gorachan)
        gorachanCell.Trigger = map.CreateTrigger().SetTriggerType(TriggerTypes.GorachanTalk)
    End Sub
End Module
