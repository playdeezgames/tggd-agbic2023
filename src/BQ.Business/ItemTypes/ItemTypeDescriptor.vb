Public Class ItemTypeDescriptor
    Inherits VisibleEntityDescriptor
    Public ReadOnly Property Effects As IReadOnlyDictionary(Of String, EffectData)
    Public ReadOnly Property Statistics As IReadOnlyDictionary(Of String, Integer)
    Public ReadOnly Property Flags As IReadOnlyList(Of String)
    Public ReadOnly Property EquipSlotType As String
    Friend ReadOnly Property LegacyFullName As Func(Of IItem, String)
    Public Function FullName(luaState As Lua, item As IItem) As String
        If Not String.IsNullOrEmpty(FullNameScript) Then
            luaState("item") = item
            Dim result = luaState.DoString(FullNameScript)
            luaState("item") = Nothing
            Return result.ToString()
        End If
        Return LegacyFullName(item)
    End Function
    Public ReadOnly FullNameScript As String
    Friend ReadOnly Property CanEquip As Boolean
        Get
            Return EquipSlotType IsNot Nothing
        End Get
    End Property
    Friend Sub New(
                  name As String,
                  glyph As Char,
                  hue As Integer,
                  Optional statistics As IReadOnlyDictionary(Of String, Integer) = Nothing,
                  Optional equipSlotType As String = Nothing,
                  Optional fullName As Func(Of IItem, String) = Nothing,
                  Optional canTake As Boolean = True,
                  Optional flags As IReadOnlyList(Of String) = Nothing,
                  Optional effects As IReadOnlyDictionary(Of String, EffectData) = Nothing,
                  Optional fullNameScript As String = Nothing)
        MyBase.New(name, glyph, hue)
        Me.EquipSlotType = equipSlotType
        Me.Statistics = If(statistics, New Dictionary(Of String, Integer))
        Me.LegacyFullName = If(fullName, AddressOf DefaultFullName)
        Me.Flags = If(flags, New List(Of String))
        Me.Effects = If(effects, New Dictionary(Of String, EffectData))
        Me.FullNameScript = fullNameScript
    End Sub

    Private Function DefaultFullName(item As IItem) As String
        Return ItemExtensions.Name(item)
    End Function
    Friend ReadOnly Property AllEffectTypes As IEnumerable(Of String)
        Get
            Return Effects.Keys
        End Get
    End Property
    Friend ReadOnly Property IsCuttingTool As Boolean
        Get
            Return Flags.Contains(FlagTypes.IsCuttingTool)
        End Get
    End Property
    Friend Function ToItemEffect(effectType As String, item As IItem) As IItemEffect
        Return New ItemEffect(effectType, Effects(effectType), item)
    End Function
End Class
