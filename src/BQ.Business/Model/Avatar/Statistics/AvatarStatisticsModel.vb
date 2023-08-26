Friend Class AvatarStatisticsModel
    Implements IAvatarStatisticsModel

    Private avatar As ICharacter

    Public Sub New(avatar As ICharacter)
        Me.avatar = avatar
    End Sub

    Public ReadOnly Property AdvancementPoints As String Implements IAvatarStatisticsModel.AdvancementPoints
        Get
            Return CharacterExtensions.APDisplay(avatar)
        End Get
    End Property

    Public ReadOnly Property Attack As String Implements IAvatarStatisticsModel.Attack
        Get
            Return CharacterExtensions.ATKDisplay(avatar)
        End Get
    End Property

    Public ReadOnly Property Defend As String Implements IAvatarStatisticsModel.Defend
        Get
            Return CharacterExtensions.DEFDisplay(avatar)
        End Get
    End Property

    Public ReadOnly Property Health As String Implements IAvatarStatisticsModel.Health
        Get
            Return CharacterExtensions.HealthDisplay(avatar)
        End Get
    End Property

    Public ReadOnly Property Energy As String Implements IAvatarStatisticsModel.Energy
        Get
            Return CharacterExtensions.EnergyDisplay(avatar)
        End Get
    End Property

    Public ReadOnly Property XPLevel As String Implements IAvatarStatisticsModel.XPLevel
        Get
            Return CharacterExtensions.XPLevelDisplay(avatar)
        End Get
    End Property

    Public ReadOnly Property XP As String Implements IAvatarStatisticsModel.XP
        Get
            Return CharacterExtensions.XPDisplay(avatar)
        End Get
    End Property

    Public ReadOnly Property Jools As String Implements IAvatarStatisticsModel.Jools
        Get
            Return CharacterExtensions.JoolsDisplay(avatar)
        End Get
    End Property
End Class
