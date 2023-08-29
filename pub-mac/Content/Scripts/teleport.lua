function doTeleport(character, effect)
	local mapId = effect:GetStatistic("MapId")
	local cellColumn = effect:GetStatistic("CellColumn")
	local cellRow = effect:GetStatistic("CellRow")
	local map = world:GetMap(mapId)
	local nextCell = map:GetCell(cellColumn, cellRow)
	nextCell:AddCharacter(character)
	character.Cell:RemoveCharacter(character)
	character.Cell = nextCell
end