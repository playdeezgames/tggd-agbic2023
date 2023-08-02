Public Class MapData
    Inherits BaseData
    Public Property Columns As Integer
    Public Property Rows As Integer
    Public Property MapType As String
    Public Property Cells As New List(Of CellData)
    Public Property Effects As New List(Of EffectData)
End Class
