Public Interface IAvatarGroundModel
    ReadOnly Property GroundItems As IEnumerable(Of (String, String))
    ReadOnly Property GroundItemsByName As IEnumerable(Of (String, Integer))
    ReadOnly Property HasGroundItems As Boolean
End Interface
