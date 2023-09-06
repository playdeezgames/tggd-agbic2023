Public Class TerrainTypeDescriptor
    Inherits VisibleEntityDescriptor
    Implements IFlagHolder
    Public Property Flags As New HashSet(Of String)
    Public Property CanBuildFurnace As Boolean
    Public Property CanSleep As Boolean
    Public Property IsFurnace As Boolean
    Public Property DepletedTerrainType As String
    Public Property HasFire As Boolean
    Public Property Peril As Integer
    Public Property IsWaterSource As Boolean
    Public Property CreatureTypeGenerator As IReadOnlyDictionary(Of String, Integer)
    Public Property Effects As IReadOnlyDictionary(Of String, EffectData)
    Public Property Tenable As Boolean
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
End Class
