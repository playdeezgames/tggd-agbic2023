Imports System.Runtime.CompilerServices

Friend Module EquipSlotTypes
    Friend Const Weapon = "Weapon"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, EquipSlotTypeDescriptor) =
        New Dictionary(Of String, EquipSlotTypeDescriptor) From
        {
            {Weapon, New EquipSlotTypeDescriptor("Weapon")}
        }
    <Extension>
    Friend Function ToEquipSlotTypeDescriptor(equipSlotType As String) As EquipSlotTypeDescriptor
        Return descriptors(equipSlotType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
