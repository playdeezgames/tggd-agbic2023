Imports SPLORR.Game

Friend Class CombatModel
    Implements ICombatModel

    Private world As IWorld

    Public Sub New(world As IWorld)
        Me.world = world
    End Sub

    Public ReadOnly Property Exists As Boolean Implements ICombatModel.Exists
        Get
            Return world.Avatar.Cell.HasOtherCharacters(world.Avatar)
        End Get
    End Property

    Public ReadOnly Property Enemies As IEnumerable(Of (Glyph As Char, Hue As Integer, MaskGlyph As Char, MaskHue As Integer)) Implements ICombatModel.Enemies
        Get
            Return world.Avatar.Cell.
                OtherCharacters(world.Avatar).
                Select(Function(x) (CharacterExtensions.Descriptor(x).Glyph, CharacterExtensions.Descriptor(x).Hue, CharacterExtensions.Descriptor(x).MaskGlyph, CharacterExtensions.Descriptor(x).MaskHue))
        End Get
    End Property

    Public ReadOnly Property Enemy(index As Integer) As (Name As String, Health As Integer, MaximumHealth As Integer, HealthDisplay As String) Implements ICombatModel.Enemy
        Get
            Dim character = world.Avatar.Cell.
                OtherCharacters(world.Avatar).ElementAt(index)
            Return (CharacterExtensions.Name(character), CharacterExtensions.Health(character), CharacterExtensions.MaximumHealth(character), CharacterExtensions.HealthDisplay(character))
        End Get
    End Property

    Public ReadOnly Property Count As Integer Implements ICombatModel.Count
        Get
            Return world.Avatar.Cell.
                OtherCharacters(world.Avatar).
                Count
        End Get
    End Property

    Private ReadOnly runDeltas As IReadOnlyList(Of (Integer, Integer)) =
        New List(Of (Integer, Integer)) From
        {
            (0, -1),
            (0, 1),
            (-1, 0),
            (1, 0)
        }
    Public Sub Run() Implements ICombatModel.Run
        Retaliation("Opportunity Attack")
        If CharacterExtensions.IsDead(world.Avatar) Then
            Return
        End If
        Dim delta = RNG.FromEnumerable(runDeltas)
        Dim avatar = world.Avatar
        If CharacterExtensions.Move(avatar, delta) Then
            Dim msg = world.CreateMessage().AddLine(7, $"{CharacterExtensions.Name(avatar)} runs away!")
            If Exists Then
                msg.AddLine(4, "An ambush awaits!")
            End If
            Return
        End If
        world.CreateMessage().AddLine(7, $"{CharacterExtensions.Name(avatar)} cannot run away!")
    End Sub

    Public Sub Attack(enemyIndex As Integer) Implements ICombatModel.Attack
        Const EnergyCost = 1
        Dim avatar = world.Avatar
        If CharacterExtensions.Energy(avatar) < EnergyCost Then
            world.CreateMessage().AddLine(4, $"{CharacterExtensions.Name(avatar)} doesn't have the energy to fight!")
            Return
        End If
        CharacterExtensions.AddEnergy(avatar, -EnergyCost)
        Dim target = avatar.Cell.
                OtherCharacters(world.Avatar).ElementAt(enemyIndex)
        Dim damageDone = False
        Do
            While world.HasMessages
                world.DismissMessage()
            End While
            damageDone = damageDone Or CharacterExtensions.Attack(world.Avatar, target)
            damageDone = Retaliation("Counter Attack", damageDone)
        Loop Until damageDone
    End Sub

    Private Function Retaliation(text As String, Optional damageDone As Boolean = False) As Boolean
        Dim index = 1
        Dim counterAttackers = world.Avatar.Cell.OtherCharacters(world.Avatar)
        For Each counterAttacker In counterAttackers
            damageDone = damageDone Or CharacterExtensions.Attack(counterAttacker, world.Avatar, $"{text} {index}/{counterAttackers.Count}")
            index += 1
        Next
        Return damageDone
    End Function
End Class
