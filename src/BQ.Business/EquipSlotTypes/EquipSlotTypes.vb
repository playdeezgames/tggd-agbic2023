Friend Module EquipSlotTypes
    Private descriptors As IReadOnlyDictionary(Of String, EquipSlotTypeDescriptor)
    <Extension>
    Friend Function ToEquipSlotTypeDescriptor(equipSlotType As String) As EquipSlotTypeDescriptor
        Return descriptors(equipSlotType)
    End Function

    Friend Sub Load(filename As String)
        descriptors = JsonSerializer.Deserialize(Of Dictionary(Of String, EquipSlotTypeDescriptor))(File.ReadAllText(filename))
    End Sub

    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
