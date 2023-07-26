Imports SPLORR.Game

Friend Class TreeDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
                    "Tree",
                    ChrW(&HA),
                    Business.Hue.Green,
                    True,
                    cellInitializer:=AddressOf Initialize,
                    verbTypes:=New Dictionary(Of String, Action(Of ICharacter, ICell)) From
                    {
                        {Business.VerbTypes.Forage, AddressOf DoForage}
                    },
                    foragables:=New Dictionary(Of String, Integer) From
                    {
                        {String.Empty, 1},
                        {ItemTypes.Stick, 1}
                    })
    End Sub

    Private Shared Sub Initialize(cell As ICell)
        cell.Statistic(StatisticTypes.Peril) = 1
    End Sub
End Class
