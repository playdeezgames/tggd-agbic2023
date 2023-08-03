Friend Class EnergyHerbDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Energy Herb",
            ChrW(&H28),
            Cyan,
            effects:=New Dictionary(Of String, EffectData) From
            {
                {EffectTypes.UseEnergyHerb, New EffectData() With {.EffectType = EffectTypes.UseEnergyHerb}}
            })
    End Sub

    Private Shared Sub UseEnergyHerb(character As ICharacter, item As IItem)
        Const energyBenefit = 10
        character.AddEnergy(energyBenefit)
        character.RemoveItem(item)
        item.Recycle()
        character.World.
            CreateMessage().
            AddLine(LightGray, $"{character.Name} eats the {item.Name}.").
            AddLine(LightGray, $"{character.Name} regains energy!").
            AddLine(LightGray, $"{character.Name} now has {character.Energy}/{character.MaximumEnergy} energy.")
    End Sub
End Class
