Imports System.Runtime.CompilerServices

Friend Module MessageTypes
    Friend Const HealerIntroduction = "HealerIntroduction"
    Friend Const TownSign1 = "TownSign1"
    Friend Const TownSign2 = "TownSign2"
    Friend Const TownSign3 = "TownSign3"
    Friend Const TownSign4 = "TownSign4"
    Friend Const TownSign5 = "TownSign5"
    Friend Const TownSign6 = "TownSign6"
    Friend Const TownSign7 = "TownSign7"
    Friend Const TownSign8 = "TownSign8"
    Friend Const TownSign9 = "TownSign9"
    Friend Const TownSign10 = "TownSign10"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, MessageTypeDescriptor) =
        New Dictionary(Of String, MessageTypeDescriptor) From
        {
            {TownSign1, New MessageTypeDescriptor(Nothing, (LightGray, "This is sign #1"))},
            {TownSign2, New MessageTypeDescriptor(Nothing, (LightGray, "This is sign #2"))},
            {TownSign3, New MessageTypeDescriptor(Nothing, (LightGray, "This is sign #3"))},
            {TownSign4, New MessageTypeDescriptor(Nothing, (LightGray, "This is sign #4"))},
            {TownSign5, New MessageTypeDescriptor(Nothing, (LightGray, "This is sign #5"))},
            {TownSign6, New MessageTypeDescriptor(Nothing, (LightGray, "This is sign #6"))},
            {TownSign7, New MessageTypeDescriptor(Nothing, (LightGray, "This is sign #7"))},
            {TownSign8, New MessageTypeDescriptor(Nothing, (LightGray, "This is sign #8"))},
            {TownSign9, New MessageTypeDescriptor(Nothing, (LightGray, "House of Nihilistic Healing"))},
            {TownSign10, New MessageTypeDescriptor(Nothing, (LightGray, "This is sign #10"))},
            {
                HealerIntroduction,
                New MessageTypeDescriptor(
                    Nothing,
                    (LightGray, "Welcome to the Nihilistic House of Healing."),
                    (LightGray, "If you go to the basin And wash,"),
                    (LightGray, "you will be healed."),
                    (LightGray, "Not that I care Or anything,"),
                    (LightGray, "because I'm a nihilist."))
            }
        }
    <Extension>
    Friend Function ToMessageTypeDescriptor(messageType As String) As MessageTypeDescriptor
        Return descriptors(messageType)
    End Function
End Module
