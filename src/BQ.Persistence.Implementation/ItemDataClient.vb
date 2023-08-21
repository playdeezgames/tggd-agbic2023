Imports BQ.Data

Friend Class ItemDataClient
    Inherits WorldDataClient
    Protected ReadOnly ItemId As Integer
    Protected ReadOnly Property ItemData As ItemData
        Get
            Return WorldData.Items(ItemId)
        End Get
    End Property
    Public Sub New(worldData As Data.WorldData, itemId As Integer)
        MyBase.New(worldData)
        Me.ItemId = itemId
    End Sub
End Class
