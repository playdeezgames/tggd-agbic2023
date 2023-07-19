Imports System.Runtime.CompilerServices
Imports SPLORR.Game

Friend Module CharacterTypes
    Friend Const Schmeara = "Schmeara"
    Friend Const Glop = "Glop"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, CharacterTypeDescriptor) =
        New Dictionary(Of String, CharacterTypeDescriptor) From
        {
            {
                Schmeara,
                New CharacterTypeDescriptor(
                    "Schmeara",
                    ChrW(2),
                    Pink,
                    ChrW(1),
                    Black)
            },
            {
                Glop,
                New CharacterTypeDescriptor(
                    "Glop",
                    ChrW(&H1B),
                    Hue.LightGreen,
                    ChrW(&H1A),
                    Hue.Black)
            }
        }

    <Extension>
    Friend Function ToCharacterTypeDescriptor(characterType As String) As CharacterTypeDescriptor
        Return descriptors(characterType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
