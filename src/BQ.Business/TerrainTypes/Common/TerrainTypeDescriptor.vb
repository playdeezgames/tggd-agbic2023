Public Class TerrainTypeDescriptor
    Inherits VisibleEntityDescriptor
    Implements IFlagHolder, IStatisticsHolder, IMetadataHolder
    Public Property Flags As New HashSet(Of String)
    Public Property Statistics As New Dictionary(Of String, Integer)
    Public Property Metadatas As New Dictionary(Of String, String)
    Public Property CreatureTypeGenerator As IReadOnlyDictionary(Of String, Integer)
    Public Property Effects As IReadOnlyDictionary(Of String, EffectData)
    Friend Function CanInteract() As Boolean
        Return Effects.Any
    End Function
    Public Property Foragables As IReadOnlyDictionary(Of String, Integer)
    Friend ReadOnly Property AllEffectTypes As IEnumerable(Of String)
        Get
            Return Effects.Keys
        End Get
    End Property
    Public Property InitializerScript As String
    Friend Sub Initialize(luaState As Lua, cell As ICell)
        If Not String.IsNullOrEmpty(InitializerScript) Then
            Dim oldCell = luaState("cell")
            luaState("cell") = cell
            luaState.DoString(InitializerScript)
            luaState("cell") = oldCell
            Return
        End If
    End Sub
    Friend Function HasEffect(effectType As String) As Boolean
        Return Effects.ContainsKey(effectType)
    End Function
    Friend Sub DoEffect(character As ICharacter, effectType As String, cell As ICell)
        Dim effect As IEffect = New TerrainEffect(effectType, Effects(effectType), cell)
        CharacterExtensions.Descriptor(character).RunEffectScript(WorldModel.LuaState, effect.EffectType, character, effect)
    End Sub
    Friend Function GenerateCreatureType() As String
        Return RNG.FromGenerator(CreatureTypeGenerator)
    End Function

    Public Sub SetFlag(flagType As String, value As Boolean) Implements IFlagHolder.SetFlag
        If value Then
            Flags.Add(flagType)
        Else
            Flags.Remove(flagType)
        End If
    End Sub

    Public Function GetFlag(flagType As String) As Boolean Implements IFlagHolder.GetFlag
        Return Flags.Contains(flagType)
    End Function

    Public Sub SetStatistic(statisticType As String, value As Integer) Implements IStatisticsHolder.SetStatistic
        Statistics(statisticType) = value
    End Sub

    Public Function AddStatistic(statisticType As String, delta As Integer) As Integer Implements IStatisticsHolder.AddStatistic
        SetStatistic(statisticType, GetStatistic(statisticType) + delta)
        Return GetStatistic(statisticType)
    End Function

    Public Sub RemoveStatistic(statisticType As String) Implements IStatisticsHolder.RemoveStatistic
        Statistics.Remove(statisticType)
    End Sub

    Public Function HasStatistic(statisticType As String) As Boolean Implements IStatisticsHolder.HasStatistic
        Return Statistics.ContainsKey(statisticType)
    End Function

    Public Function GetStatistic(statisticType As String, Optional defaultValue As Integer = 0) As Integer Implements IStatisticsHolder.GetStatistic
        Return If(HasStatistic(statisticType), Statistics(statisticType), defaultValue)
    End Function

    Public Function HasMetadata(identifier As String) As Boolean Implements IMetadataHolder.HasMetadata
        Return Metadatas.ContainsKey(identifier)
    End Function

    Public Sub SetMetadata(identifier As String, value As String) Implements IMetadataHolder.SetMetadata
        Metadatas(identifier) = value
    End Sub

    Public Function GetMetadata(identifier As String) As String Implements IMetadataHolder.GetMetadata
        Return If(HasMetadata(identifier), Metadatas(identifier), Nothing)
    End Function

    Public Sub RemoveMetadata(identifier As String) Implements IMetadataHolder.RemoveMetadata
        Metadatas.Remove(identifier)
    End Sub
End Class
