Imports System.Data

Friend Class MapModel
    Implements IMapModel

    Private ReadOnly map As IMap
    Private ReadOnly translation As (column As Integer, row As Integer)

    Public Sub New(map As IMap, translation As (column As Integer, row As Integer))
        Me.map = map
        Me.translation = translation
    End Sub

    Public ReadOnly Property Terrain(cellXY As (column As Integer, row As Integer)) As (Glyph As Char, Hue As Integer) Implements IMapModel.Terrain
        Get
            cellXY = Translate(cellXY)
            Dim descriptor = TerrainTypes.Descriptor(map.GetCell(cellXY.column, cellXY.row))
            Return (descriptor.Glyph, descriptor.Hue)
        End Get
    End Property

    Private Function Translate(cellXY As (column As Integer, row As Integer)) As (column As Integer, row As Integer)
        Return (translation.column + cellXY.column, translation.row + cellXY.row)
    End Function

    Public ReadOnly Property Character(cellXY As (column As Integer, row As Integer)) As (Glyph As Char, Hue As Integer, MaskGlyph As Char, MaskHue As Integer)? Implements IMapModel.Character
        Get
            cellXY = Translate(cellXY)
            Dim descriptor = map.GetCell(cellXY.column, cellXY.row).Characters.FirstOrDefault?.CharacterType?.ToCharacterTypeDescriptor
            If descriptor Is Nothing Then
                Return Nothing
            End If
            Return (descriptor.Glyph, descriptor.Hue, descriptor.MaskGlyph, descriptor.MaskHue)
        End Get
    End Property

    Public ReadOnly Property Items(cellXY As (column As Integer, row As Integer)) As IEnumerable(Of (Glyph As Char, Hue As Integer)) Implements IMapModel.Items
        Get
            cellXY = Translate(cellXY)
            Dim cell = map.GetCell(cellXY.column, cellXY.row)
            Return cell.Items.Select(Function(x) x.ItemType).Distinct().Select(Function(x) (ToItemTypeDescriptor(x).Glyph, ToItemTypeDescriptor(x).Hue))
        End Get
    End Property

    Public ReadOnly Property HasItems(cellXY As (column As Integer, row As Integer)) As Boolean Implements IMapModel.HasItems
        Get
            cellXY = Translate(cellXY)
            Return map.GetCell(cellXY.column, cellXY.row).HasItems
        End Get
    End Property

    Public ReadOnly Property GroundItems(cellXY As (column As Integer, row As Integer)) As List(Of (String, String)) Implements IMapModel.GroundItems
        Get
            cellXY = Translate(cellXY)
            Return map.
                GetCell(cellXY.column, cellXY.row).Items.
                GroupBy(Function(x) ItemExtensions.Name(x)).
                Select(Function(x) ($"{x.Key}(x{x.Count})", x.Key)).
                ToList
        End Get
    End Property

    Public ReadOnly Property ItemCount(cellXY As (column As Integer, row As Integer), itemName As String) As Integer Implements IMapModel.ItemCount
        Get
            cellXY = Translate(cellXY)
            Return map.
                GetCell(cellXY.column, cellXY.row).
                Items.
                Count(Function(x) ItemExtensions.Name(x) = itemName)
        End Get
    End Property

    Public Function CellExists(cellXY As (column As Integer, row As Integer)) As Boolean Implements IMapModel.CellExists
        cellXY = Translate(cellXY)
        Return cellXY.column >= 0 AndAlso cellXY.row >= 0 AndAlso cellXY.column < map.Columns AndAlso cellXY.row < map.Rows
    End Function

    Public Function FormatItemCount(cellXY As (column As Integer, row As Integer), itemName As String) As String Implements IMapModel.FormatItemCount
        Return $"{itemName}(x{ItemCount(cellXY, itemName)})"
    End Function
End Class
