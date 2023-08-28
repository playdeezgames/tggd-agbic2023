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
    Friend Const Furnace = "Furnace"
    Friend Const Bagel = "Bagel"
    Friend Descriptors As IReadOnlyDictionary(Of String, RecipeDescriptor)
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
