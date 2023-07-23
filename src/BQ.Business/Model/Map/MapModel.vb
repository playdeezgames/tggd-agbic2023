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
            Dim descriptor = map.GetCell(cellXY.column, cellXY.row).TerrainType.ToTerrainTypeDescriptor
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
            Return cell.Items.Select(Function(x) x.ItemType).Distinct().Select(Function(x) (x.ToItemTypeDescriptor.Glyph, x.ToItemTypeDescriptor.Hue))
        End Get
    End Property

    Public Function CellExists(cellXY As (column As Integer, row As Integer)) As Boolean Implements IMapModel.CellExists
        cellXY = Translate(cellXY)
        Return cellXY.column >= 0 AndAlso cellXY.row >= 0 AndAlso cellXY.column < map.Columns AndAlso cellXY.row < map.Rows
    End Function
End Class
