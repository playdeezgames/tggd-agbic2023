Public Class WorldData
    Inherits BaseData
    Public Property Maps As New List(Of MapData)
    Public Property Characters As New List(Of CharacterData)
    Public Property AvatarCharacterId As Integer?
    Public Property Messages As New List(Of MessageData)
    Public Property Items As New List(Of ItemData)
End Class
