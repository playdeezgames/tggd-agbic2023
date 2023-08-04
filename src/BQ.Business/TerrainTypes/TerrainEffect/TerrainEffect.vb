Friend Class TerrainEffect
    Inherits BaseEffect
    Implements ITerrainEffect

    Public Sub New(effectType As String, effectData As BaseData, cell As ICell)
        MyBase.New(effectType, effectData)
        Me.Cell = cell
    End Sub

    Public ReadOnly Property Cell As ICell Implements ITerrainEffect.Cell
End Class
