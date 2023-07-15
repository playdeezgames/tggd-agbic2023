Public Interface ITargetCharacterModel
    ReadOnly Property Verbs As IEnumerable(Of (String, String))
    Sub DoVerb(verbType As String)
    ReadOnly Property Exists As Boolean
    ReadOnly Property Name As String
    ReadOnly Property Statistic(statisticType As String) As Integer
    ReadOnly Property Health As Integer
    ReadOnly Property MaximumHealth As Integer
End Interface
