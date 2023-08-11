Public Module Constants
    Public Const DefaultScreenWidth = ViewWidth * 3
    Public Const DefaultScreenHeight = ViewHeight * 3
    Public Const BagelQuestFont = "BagelQuestFont"
    Public Const ViewHeight = 216
    Public Const ViewWidth = 384

    Friend Const AttackText = "Attack!"
    Friend Const BuildFireText = "Build Fire"
    Friend Const BuildFurnaceText = "Build Furnace"
    Friend Const DropAllText = "Drop All"
    Friend Const DropHalfText = "Drop Half"
    Friend Const DropOneText = "Drop One"
    Friend Const EquipText = "Equip"
    Friend Const EquipmentText = "Equipment"
    Friend Const ForageText = "Forage..."
    Friend Const GroundText = "Ground..."
    Friend Const InventoryText = "Inventory"
    Friend Const KnapText = "Knap"
    Friend Const MakeHatchetText = "Make Hatchet"
    Friend Const MakeTorchText = "Make Torch"
    Friend Const MakeTwineText = "Make Twine"
    Friend Const PutOutFireText = "Put Out Fire"
    Friend Const RunText = "Run!"
    Friend Const SleepText = "Sleep"
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
End Module
