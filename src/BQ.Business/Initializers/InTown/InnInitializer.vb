﻿Friend Module InnInitializer
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
        InitializeStairs(map)
    End Sub

    Private Sub InitializeStairs(map As IMap)
        map.GetCell(1, map.Rows - 2).Effect = map.CreateEffect()
        SetEffectType(map.GetCell(1, map.Rows - 2).Effect, "EnterCellar")
    End Sub

    Private Sub InitializeBeds(map As IMap)
        Dim bedCells = map.Cells.Where(Function(x) x.TerrainType = TerrainTypes.Bed)
        Dim trigger = map.CreateEffect()
        SetEffectType(trigger, EffectTypes.SleepAtInn)
        For Each bedCell In bedCells
            bedCell.Effect = trigger
        Next
    End Sub

    Private Sub InitializeGorachan(map As IMap)
        Dim gorachanCell = map.Cells.Single(Function(x) x.TerrainType = TerrainTypes.Gorachan)
        gorachanCell.Effect = map.CreateEffect()
        SetEffectType(gorachanCell.Effect, EffectTypes.GorachanTalk)
    End Sub
End Module
