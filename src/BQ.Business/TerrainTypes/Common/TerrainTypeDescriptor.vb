Imports System.Threading

Friend Class TerrainTypeDescriptor
    Inherits VisibleEntityDescriptor
    Public ReadOnly Property CanBuildFurnace As Boolean
    Public ReadOnly Property CanSleep As Boolean
    Public ReadOnly Property IsFurnace As Boolean
    Public ReadOnly Property DepletedTerrainType As String
    Public ReadOnly Property HasFire As Boolean
    Public ReadOnly Property Peril As Integer
    Public ReadOnly Property IsWaterSource As Boolean
    Public ReadOnly Property CreatureTypeGenerator As IReadOnlyDictionary(Of String, Integer)
    Public ReadOnly Property Effects As IReadOnlyDictionary(Of String, EffectData)
    Public ReadOnly Property Tenable As Boolean
    Friend ReadOnly Property CanInteract As Boolean
        Get
            Return Effects.Any
        End Get
    End Property
    Public ReadOnly Property Foragables As IReadOnlyDictionary(Of String, Integer)
    Friend ReadOnly Property AllEffectTypes As IEnumerable(Of String)
        Get
            Return Effects.Keys
        End Get
    End Property
    Public ReadOnly Property InitializerScript As String
    Friend Sub Initialize(luaState As Lua, cell As ICell)
        If Not String.IsNullOrEmpty(InitializerScript) Then
            Dim oldCell = luaState("cell")
            luaState("cell") = cell
            luaState.DoString(InitializerScript)
            luaState("cell") = oldCell
            Return
        End If
    End Sub
    Sub New(
           name As String,
           glyph As Char,
           hue As Integer,
           Optional tenable As Boolean = True,
           Optional peril As Integer = 0,
           Optional depletedTerrainType As String = Nothing,
           Optional foragables As IReadOnlyDictionary(Of String, Integer) = Nothing,
           Optional effects As IReadOnlyDictionary(Of String, EffectData) = Nothing,
           Optional hasFire As Boolean = False,
           Optional creatureTypeGenerator As IReadOnlyDictionary(Of String, Integer) = Nothing,
           Optional isWaterSource As Boolean = False,
           Optional canBuildFurnace As Boolean = False,
           Optional canSleep As Boolean = True,
           Optional isFurnace As Boolean = False,
           Optional initializerScript As String = Nothing)
        MyBase.New(name, glyph, hue)
        Me.IsFurnace = isFurnace
        Me.CanBuildFurnace = canBuildFurnace
        Me.IsWaterSource = isWaterSource
        Me.HasFire = hasFire
        Me.Tenable = tenable
        Me.DepletedTerrainType = depletedTerrainType
        Me.InitializerScript = initializerScript
        Me.Foragables = If(foragables, New Dictionary(Of String, Integer) From {{"", 1}})
        Me.Effects = If(effects, New Dictionary(Of String, EffectData))
        Me.Peril = peril
        Me.CreatureTypeGenerator = If(creatureTypeGenerator, New Dictionary(Of String, Integer))
        Me.CanSleep = canSleep
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
End Class
