Friend Class DepletedTreeDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
            "Tree",
            ChrW(&HA),
            Business.Hue.Brown,
            True,
            peril:=1,
            creatureTypeGenerator:=New Dictionary(Of String, Integer) From
            {
                {
                    CharacterTypes.OliveGlop,
                    75
                },
                {
                    CharacterTypes.CherryGlop,
                    25
                }
            })
    End Sub
End Class
