Imports Shouldly

Friend Module CommonHolderTests

    Friend Sub DoStatisticHolderTests(subject As IStatisticsHolder)
        Const statisticType = "StatisticType"
        Const statisticValue = 10
        subject.HasStatistic(statisticType).ShouldBeFalse
        subject.TryGetStatistic(statisticType).ShouldBe(0)

        subject.SetStatistic(statisticType, statisticValue)
        subject.HasStatistic(statisticType).ShouldBeTrue
        subject.Statistic(statisticType).ShouldBe(statisticValue)
        subject.TryGetStatistic(statisticType).ShouldBe(statisticValue)

        subject.AddStatistic(statisticType, statisticValue)
        subject.Statistic(statisticType).ShouldBe(statisticValue * 2)
        subject.TryGetStatistic(statisticType).ShouldBe(statisticValue * 2)

        subject.RemoveStatistic(statisticType)
        subject.HasStatistic(statisticType).ShouldBeFalse
        subject.TryGetStatistic(statisticType).ShouldBe(0)
    End Sub

    Friend Sub DoMetadataHolderTests(subject As IMetadataHolder)
        Const metadataIdentifier = "MetadataIdentifier"
        Const metadataValue = "MetadataValue"
        subject.HasMetadata(metadataIdentifier).ShouldBeFalse
        subject.ChangeMetadataTo(metadataIdentifier, metadataValue)
        subject.HasMetadata(metadataIdentifier).ShouldBeTrue
        subject.Metadata(metadataIdentifier).ShouldBe(metadataValue)
        subject.RemoveMetadata(metadataIdentifier)
        subject.HasMetadata(metadataIdentifier).ShouldBeFalse
    End Sub

    Friend Sub DoFlagHolderTests(subject As IFlagHolder)
        Const flagType = "FlagType"
        subject.GetFlag(flagType).ShouldBeFalse
        subject.SetFlag(flagType, True)
        subject.GetFlag(flagType).ShouldBeTrue
        subject.SetFlag(flagType, False)
        subject.GetFlag(flagType).ShouldBeFalse
    End Sub
End Module
