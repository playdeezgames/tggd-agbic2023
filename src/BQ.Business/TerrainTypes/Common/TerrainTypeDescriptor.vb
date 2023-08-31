Imports BQ.Persistence
Imports SPLORR.Game

Friend Class TerrainTypeDescriptor
    Inherits VisibleEntityDescriptor
    Friend ReadOnly Property CanBuildFurnace As Boolean
    Friend ReadOnly Property CanSleep As Boolean
    Friend ReadOnly Property IsFurnace As Boolean
    Friend ReadOnly Property DepletedTerrainType As String
    Friend ReadOnly Property HasFire As Boolean
    Friend ReadOnly Property Peril As Integer
    Friend ReadOnly Property IsWaterSource As Boolean
    Private ReadOnly Property CreatureTypeGenerator As IReadOnlyDictionary(Of String, Integer)
    Private ReadOnly Property Effects As IReadOnlyDictionary(Of String, EffectData)
    Friend ReadOnly Property Tenable As Boolean
    Friend ReadOnly Property CanInteract As Boolean
        Get
            Return Effects.Any
        End Get
    End Property
    Friend ReadOnly Property Foragables As IReadOnlyDictionary(Of String, Integer)
    Friend ReadOnly Property AllEffectTypes As IEnumerable(Of String)
        Get
            Return Effects.Keys
        End Get
    End Property
    Private ReadOnly Property CellInitializer As Action(Of ICell)
    Friend Sub Initialize(cell As ICell)
        CellInitializer.Invoke(cell)
    End Sub
    Sub New(
           name As String,
           glyph As Char,
           hue As Integer,
           Optional tenable As Boolean = True,
           Optional peril As Integer = 0,
           Optional depletedTerrainType As String = Nothing,
           Optional cellInitializer As Action(Of ICell) = Nothing,
           Optional foragables As IReadOnlyDictionary(Of String, Integer) = Nothing,
           Optional effects As IReadOnlyDictionary(Of String, EffectData) = Nothing,
           Optional hasFire As Boolean = False,
           Optional creatureTypeGenerator As IReadOnlyDictionary(Of String, Integer) = Nothing,
           Optional isWaterSource As Boolean = False,
           Optional canBuildFurnace As Boolean = False,
           Optional canSleep As Boolean = True,
           Optional isFurnace As Boolean = False)
        MyBase.New(name, glyph, hue)
        Me.IsFurnace = isFurnace
        Me.CanBuildFurnace = canBuildFurnace
        Me.IsWaterSource = isWaterSource
        Me.HasFire = hasFire
        Me.Tenable = tenable
        Me.DepletedTerrainType = depletedTerrainType
        Me.CellInitializer = If(cellInitializer, AddressOf DoNothing)
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
    Private Sub DoNothing(cell As ICell)
        'as ordered!
    End Sub
End Class
