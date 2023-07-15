Imports System.Runtime.CompilerServices

Friend Module VerbTypes
    Friend Const Disengage = "Disengage"
    Friend Const Fight = "Fight"
    Friend Const Eat = "Eat"
    Friend Const Use = "Use"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, VerbDescriptor) =
        New Dictionary(Of String, VerbDescriptor) From
        {
            {Disengage, New VerbDescriptor("Disengage")},
            {Fight, New VerbDescriptor("Fight!")},
            {Eat, New VerbDescriptor("Eat")},
            {Use, New VerbDescriptor("Use")}
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
