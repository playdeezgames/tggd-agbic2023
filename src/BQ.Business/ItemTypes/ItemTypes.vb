Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports BQ.Persistence

Friend Module ItemTypes
    Friend Const PlantFiber = "PlantFiber"
    Friend Const Twine = "Twine"
    Friend Const Stick = "Stick"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, ItemTypeDescriptor) =
        New Dictionary(Of String, ItemTypeDescriptor) From
        {
            {PlantFiber, New ItemTypeDescriptor("Plant Fiber", ChrW(&H23), LightGreen)},
            {Twine, New ItemTypeDescriptor("Twine", ChrW(&H21), Tan)},
            {Stick, New ItemTypeDescriptor("Stick", ChrW(&H25), Brown)}
        }

    <Extension>
    Friend Function ToItemTypeDescriptor(itemType As String) As ItemTypeDescriptor
        Return descriptors(itemType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
