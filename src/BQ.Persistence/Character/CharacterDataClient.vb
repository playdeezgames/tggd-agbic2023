Imports BQ.Data

Friend Class CharacterDataClient
    Inherits WorldDataClient
    Protected ReadOnly CharacterId As Integer
    Protected ReadOnly Property CharacterData As CharacterData
        Get
            Return WorldData.Characters(CharacterId)
        End Get
    End Property
    Public Sub New(worldData As Data.WorldData, characterId As Integer)
        MyBase.New(worldData)
        Me.CharacterId = characterId
    End Sub
End Class
