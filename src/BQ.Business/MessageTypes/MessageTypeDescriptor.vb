Friend Class MessageTypeDescriptor
    ReadOnly Property Sfx As String
    ReadOnly Property Lines As IReadOnlyList(Of (hue As Integer, text As String))
    Sub New(sfx As String, lines As IEnumerable(Of (hue As Integer, text As String)))
        Me.Sfx = sfx
        Me.Lines = New List(Of (hue As Integer, text As String))(lines)
    End Sub
End Class
