Friend Class SeasonedRatDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Seasoned Rat",
            ChrW(&H2F),
            11,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {"EatSeasonedRat", New EffectData}
            })
    End Sub
End Class
