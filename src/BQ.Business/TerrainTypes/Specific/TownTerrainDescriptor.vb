Friend Class TownTerrainDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New("Town", ChrW(7), 4, True, canSleep:=False)
    End Sub
End Class
