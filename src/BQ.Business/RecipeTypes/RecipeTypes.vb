Friend Module RecipeTypes
    Friend Const Twine = "Twine"
    Friend Const SharpRock = "SharpRock"
    Friend Const CookingFire = "CookingFire"
    Friend Const Torch = "Torch"
    Friend Const Foraging = "Foraging"
    Friend Const CookedRatBody = "CookedRatBody"
    Friend Const CookedRatCorpse = "CookedRatCorpse"
    Friend Const Hatchet = "Hatchet"
    Friend Const UnfiredPot = "UnfiredPot"
    Friend Const ClayPot = "ClayPot"
    Friend Const CrackedPot = "CrackedPot"
    Friend Const WaterPot = "WaterPot"
    Friend Const Flour = "Flour"
    Friend Const Dough = "Dough"
    Friend Const SmokedPepper = "SmokedPepper"
    Friend Const Paprika = "Paprika"
    Friend Const SeasonedRat = "SeasonedRat"
    Private ReadOnly Descriptors As IReadOnlyDictionary(Of String, RecipeDescriptor) =
        New Dictionary(Of String, RecipeDescriptor) From
        {
            {
                SeasonedRat,
                New RecipeDescriptor(
                    "Seasoned Rat",
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.CookedRat, 1},
                        {ItemTypes.Paprika, 1}
                    },
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.SeasonedRat, 1}
                    })
            },
            {
                Paprika,
                New RecipeDescriptor(
                    "Paprika",
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.SmokedPepper, 1},
                        {ItemTypes.Rock, 2}
                    },
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.Paprika, 1},
                        {ItemTypes.Rock, 2}
                    })
            },
            {
                SmokedPepper,
                New RecipeDescriptor(
                    "Smoked Pepper",
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.Pepper, 1},
                        {ItemTypes.Stick, 2}
                    },
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.SmokedPepper, 1},
                        {ItemTypes.Stick, 1}
                    })
            },
            {
                Dough,
                New RecipeDescriptor(
                    "Dough",
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.Flour, 1},
                        {ItemTypes.WaterPot, 1}
                    },
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.Dough, 1},
                        {ItemTypes.ClayPot, 1}
                    })
            },
            {
                Flour,
                New RecipeDescriptor(
                    "Flour",
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.Wheat, 5},
                        {ItemTypes.Rock, 2}
                    },
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.Rock, 2},
                        {ItemTypes.Flour, 1}
                    })
            },
            {
                WaterPot,
                New RecipeDescriptor(
                    "Water-Filled Pot",
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.ClayPot, 1}
                    },
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.WaterPot, 1}
                    })
            },
            {
                ClayPot,
                New RecipeDescriptor(
                    "Clay Pot",
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.UnfiredPot, 1}
                    },
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.ClayPot, 1}
                    })
            },
            {
                CrackedPot,
                New RecipeDescriptor(
                    "Cracked Pot",
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.UnfiredPot, 1}
                    },
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.CrackedPot, 1}
                    })
            },
            {
                UnfiredPot,
                New RecipeDescriptor(
                    "Unfired Pot",
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.PlantFiber, 5},
                        {ItemTypes.Clay, 5}
                    },
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.UnfiredPot, 1}
                    })
            },
            {
                Hatchet,
                New RecipeDescriptor(
                    "Hatchet",
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.Twine, 1},
                        {ItemTypes.SharpRock, 1},
                        {ItemTypes.Stick, 1}
                    },
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.Hatchet, 1}
                    })
            },
            {
                Twine,
                New RecipeDescriptor(
                    "Twine",
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.PlantFiber, 2}
                    },
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.Twine, 1}
                    })
            },
            {
                CookedRatBody,
                New RecipeDescriptor(
                    "Cooked Rat Body",
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.RatBody, 1},
                        {ItemTypes.Stick, 2}
                    },
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.CookedRat, 1},
                        {ItemTypes.Stick, 1}
                    })
            },
            {
                CookedRatCorpse,
                New RecipeDescriptor(
                    "Cooked Rat Corpse",
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.RatCorpse, 1},
                        {ItemTypes.Stick, 2}
                    },
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.CookedRat, 1},
                        {ItemTypes.Stick, 1}
                    })
            },
            {
                Foraging,
                New RecipeDescriptor(
                    "Foraging",
                    New Dictionary(Of String, Integer),
                    New Dictionary(Of String, Integer))
            },
            {
                Torch,
                New RecipeDescriptor(
                    "Torch",
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.Charcoal, 1},
                        {ItemTypes.Stick, 1}
                    },
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.Torch, 1}
                    })
            },
            {
                SharpRock,
                New RecipeDescriptor(
                    "Sharp Rock",
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.Rock, 2}
                    },
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.Rock, 1},
                        {ItemTypes.SharpRock, 1}
                    })
            },
            {
                CookingFire,
                New RecipeDescriptor(
                    "Cooking Fire",
                    New Dictionary(Of String, Integer) From
                    {
                        {ItemTypes.Rock, 5},
                        {ItemTypes.Stick, 5}
                    })
            }
        }
    Friend Function CanCraft(recipeName As String, character As ICharacter) As Boolean
        Return Descriptors(recipeName).CanCraft(character)
    End Function
    Friend Function Craft(recipeName As String, character As ICharacter) As Boolean
        If CanCraft(recipeName, character) Then
            Descriptors(recipeName).Craft(character)
            Return True
        End If
        Return False
    End Function
    Friend Function Inputs(recipeName As String) As IEnumerable(Of (itemType As String, count As Integer))
        Return Descriptors(recipeName).Inputs.Select(Function(x) (x.Key, x.Value))
    End Function
End Module
