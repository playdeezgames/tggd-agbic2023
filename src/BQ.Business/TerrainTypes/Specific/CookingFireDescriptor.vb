﻿Friend Class CookingFireDescriptor
    Inherits TerrainTypeDescriptor

    Public Sub New()
        MyBase.New(
            "Cooking Fire",
            ChrW(&H33),
            Business.Hue.Red,
            True,
            hasFire:=True,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {
                    "PutOutFire",
                    New EffectData With
                    {
                        .Metadatas = New Dictionary(Of String, String) From
                        {
                            {Metadatas.TerrainType, "Empty"},
                            {Metadatas.ItemType, "Charcoal"}
                        }
                    }
                },
                {"MakeTorch", New EffectData}
            })
    End Sub
End Class
