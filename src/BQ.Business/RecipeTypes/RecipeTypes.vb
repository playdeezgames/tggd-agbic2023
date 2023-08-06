Friend Module RecipeTypes
    Friend Const Twine = "Twine"
    Friend Const SharpRock = "SharpRock"
    Friend Const CookingFire = "CookingFire"
    Friend Const Torch = "Torch"
    Private ReadOnly Descriptors As IReadOnlyDictionary(Of String, RecipeDescriptor) =
        New Dictionary(Of String, RecipeDescriptor) From
        {
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
