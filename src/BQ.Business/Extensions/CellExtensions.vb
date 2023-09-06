Public Module CellExtensions
    Public Function CanBuildFurnace(cell As ICell) As Boolean
        Return TerrainTypes.Descriptor(cell).GetFlag("CanBuildFurnace")
    End Function
    Public Function CanCookBagel(cell As ICell) As Boolean
        Return TerrainTypes.Descriptor(cell).HasEffect("CookBagel")
    End Function
    Public Function HasFire(cell As ICell) As Boolean
        Return TerrainTypes.Descriptor(cell).GetFlag("HasFire")
    End Function
    Public Function CanSleep(cell As ICell) As Boolean
        Return TerrainTypes.Descriptor(cell).GetFlag("CanSleep")
    End Function
    Public Function CanMakeTorch(cell As ICell) As Boolean
        Return TerrainTypes.Descriptor(cell).HasEffect("MakeTorch")
    End Function
    Public Function CanBuildFire(cell As ICell) As Boolean
        Return cell.Map.GetFlag("AllowFireBuilding") AndAlso TerrainTypes.Descriptor(cell).HasEffect("BuildFire")
    End Function
    Public Function GenerateForageItemType(cell As ICell, Optional r As Random = Nothing) As String
        Dim descriptor = TerrainTypes.Descriptor(cell)
        Return RNG.FromGenerator(descriptor.Foragables, r)
    End Function
    Public Function CanForage(cell As ICell) As Boolean
        Return TerrainTypes.Descriptor(cell).HasEffect("Forage") AndAlso cell.GetStatistic("ForageRemaining") > 0
    End Function
    Public Function IsTenable(cell As ICell) As Boolean
        Return TerrainTypes.Descriptor(cell).GetFlag("Tenable")
    End Function
    Public Function Peril(cell As ICell) As Integer
        Return TerrainTypes.Descriptor(cell).GetStatistic("Peril")
    End Function
    Public Sub DoEffect(cell As ICell, effectType As String, character As ICharacter)
        Dim descriptor = TerrainTypes.Descriptor(cell)
        If Not descriptor.HasEffect(effectType) Then
            ToMessageTypeDescriptor("NothingHappens").CreateMessage(character.World)
            Return
        End If
        descriptor.DoEffect(character, effectType, cell)
    End Sub
End Module
