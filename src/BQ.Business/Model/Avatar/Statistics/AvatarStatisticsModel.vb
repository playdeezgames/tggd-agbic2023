Friend Class AvatarStatisticsModel
    Implements IAvatarStatisticsModel

    Private avatar As ICharacter

    Public Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub

    Public ReadOnly Property AdvancementPoints As String Implements IAvatarStatisticsModel.AdvancementPoints
        Get
            Return avatar.APDisplay
        End Get
    End Property

    Public ReadOnly Property Attack As String Implements IAvatarStatisticsModel.Attack
        Get
            Return avatar.ATKDisplay
        End Get
    End Property

    Public ReadOnly Property Defend As String Implements IAvatarStatisticsModel.Defend
        Get
            Return avatar.DEFDisplay
        End Get
    End Property

    Public ReadOnly Property Health As String Implements IAvatarStatisticsModel.Health
        Get
            Return avatar.HealthDisplay
        End Get
    End Property

    Public ReadOnly Property Energy As String Implements IAvatarStatisticsModel.Energy
        Get
            Return avatar.EnergyDisplay
        End Get
    End Property

    Public ReadOnly Property XPLevel As String Implements IAvatarStatisticsModel.XPLevel
        Get
            Return avatar.XPLevelDisplay
        End Get
    End Property

    Public ReadOnly Property XP As String Implements IAvatarStatisticsModel.XP
        Get
            Return avatar.XPDisplay
        End Get
    End Property

    Public ReadOnly Property Jools As String Implements IAvatarStatisticsModel.Jools
        Get
            Return avatar.JoolsDisplay
        End Get
    End Property
End Class
