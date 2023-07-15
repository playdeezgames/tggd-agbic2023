Public Interface IAvatarModel
    Sub Move(delta As (Integer, Integer))
    ReadOnly Property IsBagelQuest As Boolean
    ReadOnly Property Ground As IAvatarGroundModel
    ReadOnly Property SelectedItem As IAvatarSelectedItemModel
    ReadOnly Property Combat As IAvatarCombatModel

    'TODO: refactor into selected item
    Sub TakeSelectedItem()
    Property SelectedItemId As Integer?
    Property SelectedItemName As String

    'TODO: refactor into combat
    ReadOnly Property MaximumAttack As Integer
    ReadOnly Property MaximumDefend As Integer
    ReadOnly Property AverageAttack As Double
    ReadOnly Property AverageDefend As Double
    ReadOnly Property Health As Integer
    ReadOnly Property MaximumHealth As Integer
    Property Epitaph As String

    ReadOnly Property Dignity As Integer
    ReadOnly Property Inventory As IAvatarInventoryModel
    ReadOnly Property Encumbrance As IAvatarEncumbranceModel
    ReadOnly Property Equipment As IAvatarEquipmentModel
    ReadOnly Property SelectedItemFullName As String
End Interface
