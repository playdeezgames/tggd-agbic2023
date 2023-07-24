Imports System.Threading

Friend Class AvatarModel
    Implements IAvatarModel

    Private ReadOnly avatar As ICharacter

    Public Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub

    Public ReadOnly Property IsDead As Boolean Implements IAvatarModel.IsDead
        Get
            Return avatar.IsDead
        End Get
    End Property

    Public ReadOnly Property Character As (Glyph As Char, Hue As Integer, MaskGlyph As Char, MaskHue As Integer) Implements IAvatarModel.Character
        Get
            Dim descriptor = avatar.CharacterType.ToCharacterTypeDescriptor
            Return (descriptor.Glyph, descriptor.Hue, descriptor.MaskGlyph, descriptor.MaskHue)
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IAvatarModel.Name
        Get
            Return avatar.Name
        End Get
    End Property

    Public ReadOnly Property Health As Integer Implements IAvatarModel.Health
        Get
            Return avatar.Health
        End Get
    End Property

    Public ReadOnly Property MaximumHealth As Integer Implements IAvatarModel.MaximumHealth
        Get
            Return avatar.MaximumHealth
        End Get
    End Property

    Public ReadOnly Property XP As Integer Implements IAvatarModel.XP
        Get
            Return avatar.XP
        End Get
    End Property

    Public ReadOnly Property XPGoal As Integer Implements IAvatarModel.XPGoal
        Get
            Return avatar.XPGoal
        End Get
    End Property

    Public ReadOnly Property XPLevel As Integer Implements IAvatarModel.XPLevel
        Get
            Return avatar.XPLevel
        End Get
    End Property

    Public ReadOnly Property AverageAttack As Double Implements IAvatarModel.AverageAttack
        Get
            Return avatar.Statistic(StatisticTypes.AttackDice) / 6
        End Get
    End Property

    Public ReadOnly Property AverageDefend As Double Implements IAvatarModel.AverageDefend
        Get
            Return avatar.Statistic(StatisticTypes.DefendDice) / 6
        End Get
    End Property

    Public ReadOnly Property MaximumAttack As Integer Implements IAvatarModel.MaximumAttack
        Get
            Return avatar.Statistic(StatisticTypes.MaximumAttack)
        End Get
    End Property

    Public ReadOnly Property MaximumDefend As Integer Implements IAvatarModel.MaximumDefend
        Get
            Return avatar.Statistic(StatisticTypes.MaximumDefend)
        End Get
    End Property

    Public ReadOnly Property HasItems As Boolean Implements IAvatarModel.HasItems
        Get
            Return avatar.HasItems
        End Get
    End Property

    Public ReadOnly Property Items As IEnumerable(Of (String, String)) Implements IAvatarModel.Items
        Get
            Return avatar.Items.GroupBy(Function(x) x.Name).Select(Function(x) ($"{x.Key}(x{x.Count})", x.Key))
        End Get
    End Property

    Public ReadOnly Property ItemCount(itemName As String) As Integer Implements IAvatarModel.ItemCount
        Get
            Return avatar.Items.Count(Function(x) x.Name = itemName)
        End Get
    End Property

    Public Sub Move(delta As (x As Integer, y As Integer)) Implements IAvatarModel.Move
        avatar.Move(delta)
    End Sub
End Class
