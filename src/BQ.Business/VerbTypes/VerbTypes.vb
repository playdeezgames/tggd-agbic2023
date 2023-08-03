Imports System.Runtime.CompilerServices

Friend Module VerbTypes
    Friend Const Forage = "Forage"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, VerbDescriptor) =
        New Dictionary(Of String, VerbDescriptor) From
        {
            {Forage, New VerbDescriptor(Forage)}
        }
    <Extension>
    Friend Function ToVerbTypeDescriptor(verbType As String) As VerbDescriptor
        Return descriptors(verbType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
