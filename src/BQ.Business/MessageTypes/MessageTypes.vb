Imports System.Data

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
    Friend Const PotterSign = "PotterSign"
    Friend Const NothingHappens = "NothingHappens"
    Private descriptors As IReadOnlyDictionary(Of String, MessageTypeDescriptor)
    <Extension>
    Friend Function ToMessageTypeDescriptor(messageType As String) As MessageTypeDescriptor
        Return descriptors(messageType)
    End Function

    Friend Sub Save(filename As String)
        File.WriteAllText(filename, JsonSerializer.Serialize(descriptors))
    End Sub

    Friend Sub Load(filename As String)
        descriptors = JsonSerializer.Deserialize(Of Dictionary(Of String, MessageTypeDescriptor))(File.ReadAllText(filename))
    End Sub
End Module
