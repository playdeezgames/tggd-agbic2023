Imports System
Imports Shouldly
Imports Xunit

Public Class World_should
    <Fact>
    Sub hold_statistics()
        Const statisticType = "StatisticType"
        Const statisticValue = 10
        Dim subject As IWorld = New World(New Data.WorldData)
        subject.SetStatistic(statisticType, statisticValue)
        subject.Statistic(statisticType).ShouldBe(statisticValue)
    End Sub
End Class


