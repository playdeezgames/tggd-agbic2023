Imports System.ComponentModel
Imports BQ.Persistence

Public Class ItemTypeDescriptor
    Inherits VisibleEntityDescriptor
    Private ReadOnly Property Effects As IReadOnlyDictionary(Of String, EffectData)
    Friend ReadOnly Property Statistics As IReadOnlyDictionary(Of String, Integer)
    Friend ReadOnly Property Flags As IReadOnlyList(Of String)
    Friend ReadOnly Property EquipSlotType As String
    Friend ReadOnly Property FullName As Func(Of IItem, String)
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
                  Optional effects As IReadOnlyDictionary(Of String, EffectData) = Nothing)
        MyBase.New(name, glyph, hue)
        Me.EquipSlotType = equipSlotType
        Me.Statistics = If(statistics, New Dictionary(Of String, Integer))
        Me.FullName = If(fullName, AddressOf DefaultFullName)
        Me.Flags = If(flags, New List(Of String))
        Me.Effects = If(effects, New Dictionary(Of String, EffectData))
    End Sub

    Private Function DefaultFullName(item As IItem) As String
        Return item.Name
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
