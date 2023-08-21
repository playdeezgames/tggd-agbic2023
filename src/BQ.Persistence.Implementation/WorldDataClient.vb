Imports BQ.Data

Public Class WorldDataClient
    Protected ReadOnly WorldData As WorldData
    Sub New(worldData As WorldData)
        Me.WorldData = worldData
    End Sub
End Class
