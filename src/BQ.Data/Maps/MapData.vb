Public Class MapData
    Public Property Columns As Integer
    Public Property Rows As Integer
    Public Property MapType As String
    Public Property Cells As New List(Of CellData)
    Public Property Triggers As New List(Of TriggerData)
    Public Property Statistics As New Dictionary(Of String, Integer)
End Class
