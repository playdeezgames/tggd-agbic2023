Public Module Constants
    Public Const DefaultScreenWidth = ViewWidth * 3
    Public Const DefaultScreenHeight = ViewHeight * 3
    Public Const BagelQuestFont = "BagelQuestFont"
    Public Const ViewHeight = 216
    Public Const ViewWidth = 384

    Friend Const AttackText = "Attack!"
    Friend Const GroundText = "Ground..."
    Friend Const RunText = "Run!"
    Friend Const StatisticsText = "Statistics"
    Friend Const TakeAllText = "Take All"
    Friend Const TakeHalfText = "Take Half"
    Friend Const TakeOneText = "Take One"

    Friend Const CellHeight = 16
    Friend Const CellWidth = 16
    Private Const CenterX = ViewWidth \ 2
    Private Const CenterY = ViewHeight \ 2
    Friend Const CenterCellX = CenterX - CellWidth \ 2
    Friend Const CenterCellY = CenterY - CellHeight \ 2
    Friend Const LeftColumn = -((CenterCellX + CellWidth - 1) \ CellWidth)
    Private Const RightColumn = (ViewWidth - CenterCellX + CellWidth - 1) \ CellWidth
    Private Const BottomRow = (ViewHeight - CenterCellY + CellHeight - 1) \ CellHeight
    Friend Const TopRow = -((CenterCellY + CellHeight - 1) \ CellHeight)
    Friend Const MapRenderX = CenterCellX + LeftColumn * CellWidth
    Friend Const MapRenderY = CenterCellY + TopRow * CellHeight
    Friend Const MapRenderColumns = RightColumn - LeftColumn
    Friend Const MapRenderRows = BottomRow - TopRow
    'Friend Const DropText = "Drop"
    'Friend Const EquipText = "Equip"
    'Friend Const EquipmentText = "Equipment"
    'Friend Const GroundText = "Ground..."
    'Friend Const InventoryText = "Inventory"
    'Friend Const TakeText = "Take"
    'Friend Const BagelQuestDelaySeconds = 1.0
    'Friend Const UnequipText = "Unequip"
End Module
