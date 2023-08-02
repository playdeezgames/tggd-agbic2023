Imports System.Data
Imports System.Runtime.CompilerServices

Friend Module MessageTypes
    Friend Const InnSign = "InnSign"
    Friend Const TownSign2 = "TownSign2"
    Friend Const HealthTrainerSign = "HealthTrainerSign"
    Friend Const EnergyTrainerSign = "TownSign4"
    Friend Const TownSign5 = "TownSign5"
    Friend Const TownSign6 = "TownSign6"
    Friend Const DruidSign = "DruidSign"
    Friend Const TownSign8 = "TownSign8"
    Friend Const HealerSign = "HealerSign"
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
            {InnSign, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "Jusdatip Inn")))},
            {TownSign2, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "This is sign #2")))},
            {HealthTrainerSign, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "Health Training")))},
            {EnergyTrainerSign, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "Endurance Training")))},
            {TownSign5, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "This is sign #5")))},
            {TownSign6, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "This is sign #6")))},
            {DruidSign, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "Hippy Druid Lives Here")))},
            {TownSign8, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "This is sign #8")))},
            {HealerSign, New MessageTypeDescriptor(Nothing, MakeLines((LightGray, "House of Nihilistic Healing")))},
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
                        ("Cool story, bro!", EffectTypes.ExitDialog),
                        ("What's for sale?", EffectTypes.HealerPrices)))
            },
            {
                HealthTrainerTalk,
                New MessageTypeDescriptor(
                    lines:=MakeLines(
                        (LightGray, "I am the health trainer!"),
                        (LightGray, "I can help you increase yer health."),
                        (LightGray, "The cost is 5 AP times yer current health.")),
                    choices:=MakeChoices(
                        ("Cool story, bro!", EffectTypes.ExitDialog),
                        ("Train Me!", EffectTypes.TrainHealth)))
            },
            {
                DruidTalk,
                New MessageTypeDescriptor(
                    lines:=MakeLines(
                        (LightGray, "Greetings! I am a druid."),
                        (LightGray, "I can help you learn nature's way.")),
                    choices:=MakeChoices(
                        ("Cool story, bro!", EffectTypes.ExitDialog),
                        ("Don't druids live in the woods?", EffectTypes.DruidAllergies),
                        ("Teach me!", EffectTypes.DruidTeachMenu)))
            }
        }
    <Extension>
    Friend Function ToMessageTypeDescriptor(messageType As String) As MessageTypeDescriptor
        Return descriptors(messageType)
    End Function
End Module
