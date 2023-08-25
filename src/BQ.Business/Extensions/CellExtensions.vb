Friend Module CellExtensions
    <Extension>
    Friend Function CanBuildFurnace(cell As ICell) As Boolean
        Return cell.Descriptor.CanBuildFurnace
    End Function
    <Extension>
    Friend Function CanCookBagel(cell As ICell) As Boolean
        Return cell.Descriptor.HasEffect(EffectTypes.CookBagel)
    End Function
    <Extension>
    Friend Function HasFire(cell As ICell) As Boolean
        Return cell.Descriptor.HasFire
    End Function
    <Extension>
    Friend Function CanSleep(cell As ICell) As Boolean
        Return cell.Descriptor.CanSleep
    End Function
    <Extension>
    Friend Function CanMakeTorch(cell As ICell) As Boolean
        Return cell.Descriptor.HasEffect(EffectTypes.MakeTorch)
    End Function
    <Extension>
    Friend Function CanBuildFire(cell As ICell) As Boolean
        Return cell.Map.GetFlag(FlagTypes.AllowFireBuilding) AndAlso cell.Descriptor.HasEffect(EffectTypes.BuildFire)
    End Function
    <Extension>
    Friend Function GenerateForageItemType(cell As ICell, Optional r As Random = Nothing) As String
        Dim descriptor = cell.Descriptor
        Return RNG.FromGenerator(descriptor.Foragables, r)
    End Function
    <Extension>
    Friend Function CanForage(cell As ICell) As Boolean
        Return cell.Descriptor.HasEffect(EffectTypes.Forage) AndAlso cell.TryGetStatistic(StatisticTypes.ForageRemaining) > 0
    End Function
    <Extension>
    Friend Function IsTenable(cell As ICell) As Boolean
        Return cell.Descriptor.Tenable
    End Function
    <Extension>
    Friend Function Peril(cell As ICell) As Integer
        Return cell.Descriptor.Peril
    End Function
    <Extension>
    Friend Sub DoEffect(cell As ICell, effectType As String, character As ICharacter)
        Dim descriptor = cell.Descriptor
        If Not descriptor.HasEffect(effectType) Then
            MessageTypes.NothingHappens.ToMessageTypeDescriptor.CreateMessage(character.World)
            Return
        End If
        descriptor.DoEffect(character, effectType, cell)
    End Sub
End Module
