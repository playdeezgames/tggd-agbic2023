Friend Class SeasonedRatDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Seasoned Rat",
            ChrW(&H2F),
            Orange,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {"EatSeasonedRat", New EffectData}
            })
    End Sub
End Class
