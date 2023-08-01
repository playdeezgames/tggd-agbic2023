Friend Class EnergyHerbDescriptor
    Inherits ItemTypeDescriptor

    Friend Sub New()
        MyBase.New(
            "Energy Herb",
            ChrW(&H28),
            Cyan,
            verbTypes:=New Dictionary(Of String, Action(Of ICharacter, IItem)) From
            {
                {Business.VerbTypes.Use, AddressOf UseEnergyHerb}
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
