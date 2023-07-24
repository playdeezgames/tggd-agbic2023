Friend Class MessageTypeDescriptor
    ReadOnly Property Sfx As String
    ReadOnly Property Lines As IReadOnlyList(Of (hue As Integer, text As String))
    Sub New(Optional sfx As String = Nothing, Optional lines As IEnumerable(Of (hue As Integer, text As String)) = Nothing)
        Me.Sfx = sfx
        Me.Lines = If(
            lines IsNot Nothing,
            New List(Of (hue As Integer, text As String))(lines),
            New List(Of (hue As Integer, Text As String)))
    End Sub
End Class
