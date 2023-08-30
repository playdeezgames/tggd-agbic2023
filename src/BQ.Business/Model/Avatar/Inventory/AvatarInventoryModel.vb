Friend Class AvatarInventoryModel
    Implements IAvatarInventoryModel

    Private ReadOnly avatar As ICharacter

    Public Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub

    Public ReadOnly Property GridSize As (columns As Integer, rows As Integer) Implements IAvatarInventoryModel.GridSize
        Get
            If Not avatar.HasItems Then
                Return (0, 0)
            End If
            Dim itemCountsByName = CharacterExtensions.ItemCountsByName(avatar).Keys.Count
            Dim rows As Integer = CInt(Math.Sqrt(itemCountsByName))
            Dim columns As Integer = (itemCountsByName + rows - 1) \ rows
            Return (columns, rows)
        End Get
    End Property

    Public ReadOnly Property Display As IEnumerable(Of (glyph As Char, hue As Integer, name As String, count As Integer)) Implements IAvatarInventoryModel.Display
        Get
            Return CharacterExtensions.ItemCountsByName(avatar).Select(Function(x) (ItemExtensions.Descriptor(x.Value.First).Glyph, ItemExtensions.Descriptor(x.Value.First).Hue, x.Key, x.Value.Count))
        End Get
    End Property

    Public ReadOnly Property ItemCount(name As String) As Integer Implements IAvatarInventoryModel.ItemCount
        Get
            Return avatar.Items.Count(Function(x) ItemExtensions.Name(x) = name)
        End Get
    End Property

    Public ReadOnly Property Exists As Boolean Implements IAvatarInventoryModel.Exists
        Get
            Return avatar.HasItems
        End Get
    End Property

    Public Function FormatItemCount(name As String) As String Implements IAvatarInventoryModel.FormatItemCount
        Return $"{name}(x{ItemCount(name)})"
    End Function
End Class
