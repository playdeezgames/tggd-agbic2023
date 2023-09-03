Friend Class DepletedTreeDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
            "Tree",
            ChrW(&HA),
            6,
            True,
            peril:=1,
            creatureTypeGenerator:=New Dictionary(Of String, Integer) From
            {
                {
                    "OliveGlop",
                    75
                },
                {
                    "CherryGlop",
                    25
                }
            })
    End Sub
End Class
