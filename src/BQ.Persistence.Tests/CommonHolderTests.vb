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

        subject.RemoveStatistic(statisticType)
        subject.HasStatistic(statisticType).ShouldBeFalse
        subject.TryGetStatistic(statisticType).ShouldBe(0)
    End Sub

    Friend Sub DoMetadataHolderTests(subject As IMetadataHolder)
        Const metadataIdentifier = "MetadataIdentifier"
        Const metadataValue = "MetadataValue"
        subject.HasMetadata(metadataIdentifier).ShouldBeFalse
        subject.SetMetadata(metadataIdentifier, metadataValue)
        subject.HasMetadata(metadataIdentifier).ShouldBeTrue
        subject.Metadata(metadataIdentifier).ShouldBe(metadataValue)
        subject.RemoveMetadata(metadataIdentifier)
        subject.HasMetadata(metadataIdentifier).ShouldBeFalse
    End Sub

    Friend Sub DoFlagHolderTests(subject As IFlagHolder)
        Const flagType = "FlagType"
        subject.Flag(flagType).ShouldBeFalse
        subject.Flag(flagType) = True
        subject.Flag(flagType).ShouldBeTrue
        subject.Flag(flagType) = False
        subject.Flag(flagType).ShouldBeFalse
    End Sub
End Module
