Imports System.ComponentModel
Imports BQ.Persistence

Friend Class ItemTypeDescriptor
    Inherits VisibleEntityDescriptor
    Friend ReadOnly Property VerbTypes As IReadOnlyDictionary(Of String, Action(Of ICharacter, IItem))
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
                  Optional verbTypes As IReadOnlyDictionary(Of String, Action(Of ICharacter, IItem)) = Nothing,
                  Optional statistics As IReadOnlyDictionary(Of String, Integer) = Nothing,
                  Optional equipSlotType As String = Nothing,
                  Optional fullName As Func(Of IItem, String) = Nothing,
                  Optional canTake As Boolean = True,
                  Optional flags As IReadOnlyList(Of String) = Nothing)
        MyBase.New(name, glyph, hue)
        Me.VerbTypes = If(verbTypes, New Dictionary(Of String, Action(Of ICharacter, IItem)))
        Me.EquipSlotType = equipSlotType
        Me.Statistics = If(statistics, New Dictionary(Of String, Integer))
        Me.FullName = If(fullName, AddressOf DefaultFullName)
        Me.Flags = If(flags, New List(Of String))
    End Sub

    Private Function DefaultFullName(item As IItem) As String
        Return item.Name
    End Function

    Friend Function HasVerb(verbType As String) As Boolean
        Return VerbTypes.ContainsKey(verbType)
    End Function
    Friend ReadOnly Property AllVerbTypes As IEnumerable(Of String)
        Get
            Return VerbTypes.Keys
        End Get
    End Property
    Friend ReadOnly Property IsCuttingTool As Boolean
        Get
            Return Flags.Contains(FlagTypes.IsCuttingTool)
        End Get
    End Property
End Class
