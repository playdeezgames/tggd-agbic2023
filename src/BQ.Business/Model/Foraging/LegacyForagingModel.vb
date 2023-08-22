Friend Class LegacyForagingModel
    Implements ILegacyForagingModel

    Private ReadOnly world As IWorld

    Public Sub New(world As IWorld)
        Me.world = world
    End Sub

    Public ReadOnly Property GridSize As (columns As Integer, rows As Integer) Implements ILegacyForagingModel.GridSize
        Get
            Dim forageRemaining = world.Avatar.Cell.Statistic(StatisticTypes.ForageRemaining)
            Dim rows = CInt(Math.Sqrt(forageRemaining))
            Return ((forageRemaining + rows - 1) \ rows, rows)
        End Get
    End Property

    Public Function LegacyForageItemType(itemType As String) As Boolean Implements ILegacyForagingModel.LegacyForageItemType
        If world.Avatar.Energy > 0 AndAlso world.Avatar.Cell.Statistic(StatisticTypes.ForageRemaining) > 0 Then
            If Not String.IsNullOrEmpty(itemType) Then
                world.Avatar.AddItem(ItemInitializer.CreateItem(world, itemType))
            End If
            world.Avatar.Cell.SetStatistic(StatisticTypes.ForageRemaining, world.Avatar.Cell.Statistic(StatisticTypes.ForageRemaining) - 1)
            If world.Avatar.Cell.Statistic(StatisticTypes.ForageRemaining) <= 0 Then
                world.Avatar.Cell.TerrainType = world.Avatar.Cell.Descriptor.DepletedTerrainType
            End If
            world.Avatar.AddEnergy(-1)
            Return True
        End If
        Return False
    End Function

    Public Function LegacyGenerateGrid() As (glyph As Char, hue As Integer, itemType As String)(,) Implements ILegacyForagingModel.LegacyGenerateGrid
        Dim mapCell = world.Avatar.Cell
        Dim forageRemaining = mapCell.Statistic(StatisticTypes.ForageRemaining)
        Dim size = GridSize
        Dim cells(size.columns - 1, size.rows - 1) As (glyph As Char, hue As Integer, itemType As String)
        For Each index In Enumerable.Range(0, size.columns * size.rows)
            Dim row = index \ size.columns
            Dim column = index Mod size.columns
            If index < forageRemaining Then
                Dim itemType = mapCell.GenerateForageItemType()
                Dim descriptor = itemType?.ToItemTypeDescriptor
                cells(column, row) = (If(descriptor?.Glyph, ChrW(0)), If(descriptor?.Hue, Black), itemType)
            Else
                Dim swapRow = RNG.FromRange(0, size.rows - 1)
                Dim swapColumn = RNG.FromRange(0, size.columns - 1)
                cells(column, row) = cells(swapColumn, swapRow)
                cells(swapColumn, swapRow) = (ChrW(0), Black, Nothing)
            End If
        Next
        Return cells
    End Function
End Class
