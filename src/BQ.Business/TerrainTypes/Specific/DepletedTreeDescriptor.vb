Friend Class DepletedTreeDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
            "Tree",
            ChrW(&HA),
            Business.Hue.Brown,
            True,
            cellInitializer:=AddressOf InitializeDepletedTree)
    End Sub

    Private Shared Sub InitializeDepletedTree(cell As ICell)
        cell.Statistic(StatisticTypes.Peril) = 1
    End Sub
End Class
