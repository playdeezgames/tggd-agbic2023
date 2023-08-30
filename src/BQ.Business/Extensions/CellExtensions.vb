Public Module CellExtensions
    Public Function CanBuildFurnace(cell As ICell) As Boolean
        Return cell.Descriptor.CanBuildFurnace
    End Function
    Public Function CanCookBagel(cell As ICell) As Boolean
        Return cell.Descriptor.HasEffect(EffectTypes.CookBagel)
    End Function
    Public Function HasFire(cell As ICell) As Boolean
        Return cell.Descriptor.HasFire
    End Function
    Public Function CanSleep(cell As ICell) As Boolean
        Return cell.Descriptor.CanSleep
    End Function
    Public Function CanMakeTorch(cell As ICell) As Boolean
        Return cell.Descriptor.HasEffect(EffectTypes.MakeTorch)
    End Function
    Public Function CanBuildFire(cell As ICell) As Boolean
        Return cell.Map.GetFlag("AllowFireBuilding") AndAlso cell.Descriptor.HasEffect(EffectTypes.BuildFire)
    End Function
    Public Function GenerateForageItemType(cell As ICell, Optional r As Random = Nothing) As String
        Dim descriptor = cell.Descriptor
        Return RNG.FromGenerator(descriptor.Foragables, r)
    End Function
    Public Function CanForage(cell As ICell) As Boolean
        Return cell.Descriptor.HasEffect(EffectTypes.Forage) AndAlso cell.GetStatistic(StatisticTypes.ForageRemaining) > 0
    End Function
    Public Function IsTenable(cell As ICell) As Boolean
        Return cell.Descriptor.Tenable
    End Function
    Public Function Peril(cell As ICell) As Integer
        Return cell.Descriptor.Peril
    End Function
    Public Sub DoEffect(cell As ICell, effectType As String, character As ICharacter)
        Dim descriptor = cell.Descriptor
        If Not descriptor.HasEffect(effectType) Then
            ToMessageTypeDescriptor("NothingHappens").CreateMessage(character.World)
            Return
        End If
        descriptor.DoEffect(character, effectType, cell)
    End Sub
End Module
