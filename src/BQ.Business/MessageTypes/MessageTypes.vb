Imports System.Data
Imports System.Runtime.CompilerServices

Friend Module MessageTypes
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
    Friend Const NothingHappens = "NothingHappens"
    Private Function MakeLines(ParamArray lines() As (hue As Integer, text As String)) As IEnumerable(Of (hue As Integer, text As String))
        Return lines
    End Function
    Private Function MakeChoices(ParamArray choices() As (text As String, command As String)) As IEnumerable(Of (text As String, command As String))
        Return choices
    End Function
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, MessageTypeDescriptor) =
        New Dictionary(Of String, MessageTypeDescriptor) From
        {
            {NothingHappens, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "Nothing happens.")))},
            {TownSign1, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "This is sign #1")))},
            {TownSign2, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "This is sign #2")))},
            {TownSign3, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "Health Training")))},
            {TownSign4, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "This is sign #4")))},
            {TownSign5, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "This is sign #5")))},
            {TownSign6, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "This is sign #6")))},
            {TownSign7, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "Hippy Druid Lives Here")))},
            {TownSign8, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "This is sign #8")))},
            {TownSign9, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "House of Nihilistic Healing")))},
            {TownSign10, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "This is sign #10")))},
            {
                HealerTalk,
                New MessageTypeDescriptor(
                    lines:=MakeLines(
                        (LightGray, "Welcome to the Nihilistic House of Healing."),
                        (LightGray, "If you go to the basin And wash,"),
                        (LightGray, "you will be healed,"),
                        (LightGray, "but it will cost you half of yer jools."),
                        (LightGray, "Not that I care or anything,"),
                        (LightGray, "because I'm a nihilist.")),
                    choices:=MakeChoices(
                        ("Cool story, bro!", TriggerTypes.ExitDialog),
                        ("What's for sale?", TriggerTypes.NihilistPrices)))
            },
            {
                HealthTrainerTalk,
                New MessageTypeDescriptor(
                    lines:=MakeLines(
                        (LightGray, "I am the health trainer!"),
                        (LightGray, "I can help you increase yer health."),
                        (LightGray, "The cost is 5 AP times yer current health.")),
                    choices:=MakeChoices(
                        ("Cool story, bro!", TriggerTypes.ExitDialog),
                        ("Train Me!", TriggerTypes.TrainHealth)))
            },
            {
                DruidTalk,
                New MessageTypeDescriptor(
                    lines:=MakeLines(
                        (LightGray, "Greetings! I am a druid."),
                        (LightGray, "I can help you learn nature's way.")),
                    choices:=MakeChoices(
                        ("Cool story, bro!", TriggerTypes.ExitDialog),
                        ("Don't druids live in the woods?", TriggerTypes.DruidAllergies),
                        ("Teach me!", TriggerTypes.DruidTeachMenu)))
            }
        }
    <Extension>
    Friend Function ToMessageTypeDescriptor(messageType As String) As MessageTypeDescriptor
        Return descriptors(messageType)
    End Function
End Module
