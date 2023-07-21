Public Interface IAvatarModel
    Sub Move(delta As (x As Integer, y As Integer))
    ReadOnly Property IsDead As Boolean
End Interface
