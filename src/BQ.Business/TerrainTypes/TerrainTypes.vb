Public Module TerrainTypes
    Private descriptors As IReadOnlyDictionary(Of String, TerrainTypeDescriptor)
    Private Sub Save(filename As String)
        File.WriteAllText(filename, JsonSerializer.Serialize(descriptors))
    End Sub
    Friend Sub Load(filename As String)
        descriptors = JsonSerializer.Deserialize(Of Dictionary(Of String, TerrainTypeDescriptor))(File.ReadAllText(filename))
        For Each entry In descriptors
            entry.Value.SetFlag("IsFurnace", entry.Value.IsFurnace)
            entry.Value.SetFlag("IsWaterSource", entry.Value.IsWaterSource)
            entry.Value.SetFlag("CanBuildFurnace", entry.Value.CanBuildFurnace)
            entry.Value.SetFlag("CanSleep", entry.Value.CanSleep)
            entry.Value.SetFlag("HasFire", entry.Value.HasFire)
            entry.Value.SetFlag("Tenable", entry.Value.Tenable)
            If entry.Value.Peril <> 0 Then
                entry.Value.SetStatistic("Peril", entry.Value.Peril)
            End If
            If Not String.IsNullOrEmpty(entry.Value.DepletedTerrainType) Then
                entry.Value.SetMetadata("DepletedTerrainType", entry.Value.DepletedTerrainType)
            End If
        Next
        Save(filename)
    End Sub
    Public Function Descriptor(cell As ICell) As TerrainTypeDescriptor
        Return descriptors(cell.TerrainType)
    End Function
End Module
