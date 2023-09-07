Public Class ItemTypeDescriptor
    Inherits VisibleEntityDescriptor
    Public Property Effects As New Dictionary(Of String, EffectData)
    Public Property Statistics As New Dictionary(Of String, Integer)
    Public Property Flags As New HashSet(Of String)
    Public Property EquipSlotType As String
    Public Function FullName(luaState As Lua, item As IItem) As String
        If Not String.IsNullOrEmpty(FullNameScript) Then
            luaState("item") = item
            Dim result = luaState.DoString(FullNameScript)
            luaState("item") = Nothing
            Return result.ToString()
        End If
        Return DefaultFullName(item)
    End Function
    Public Property FullNameScript As String
    Friend Function CanEquip() As Boolean
        Return EquipSlotType IsNot Nothing
    End Function

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
            Return Flags.Contains("IsCuttingTool")
        End Get
    End Property
    Friend Function ToItemEffect(effectType As String, item As IItem) As IItemEffect
        Return New ItemEffect(effectType, Effects(effectType), item)
    End Function
End Class
