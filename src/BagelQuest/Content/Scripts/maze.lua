function CreateMaze(columns, rows,directions)
  function CreateMazeCell()
    local mazeCell = {}
    mazeCell._neighbors = {}
    mazeCell._doors={}
    function mazeCell:HasNeighbor(direction)
      return self._neighbors[direction]~=nil
    end
    function mazeCell:SetNeighbor(direction,nextCell)
      self._neighbors[direction]=nextCell
    end
    function mazeCell:GetNeighbor(direction)
      return self._neighbors[direction]
    end
    function mazeCell:GetDoor(direction)
      return self._doors[direction]
    end
    function mazeCell:Neighbors()
      local result={}
      for _,v in pairs(self._neighbors) do
        table.insert(result,v)
      end
      return result
    end
    function mazeCell:Directions()
      local result={}
      for k,_ in pairs(self._neighbors) do
        table.insert(result,k)
      end
      return result 
    end
    function mazeCell:SetDoor(direction,door)
      self._doors[direction]=door
    end
    function mazeCell:Reset()
      for _,v in pairs(self._doors) do
        v.Open=false
      end
    end
    function mazeCell:OpenDoorCount()
      local result=0
      for _,v in pairs(self.doors) do
        if v.Open then
          result=result+1
        end
      end
      return result
    end
    return mazeCell
  end
  local maze = {}
  maze.Columns=columns
  maze.Rows=rows
  maze._cells = {}
  while #maze._cells<columns*rows do
    table.insert(maze._cells, CreateMazeCell())
    print(#maze._cells)
  end
  function maze:Reset()
    for _,v in ipairs(self._cells) do
      v:Reset()
    end
  end
  function maze:GetCell(column, row)
    if column<1 or row<1 or column>self.Columns or row>self.Rows then
      return nil
    end
    return self._cells[column+(row-1)*self.Columns]
  end
  function maze:Generate()
    self:Reset()
    local cell = self:GetCell(RNG.FromRange(1,self.Columns),RNG.FromRange(1,self.Rows))
    local inside={cell}
    local frontier = {}
    for _,v in ipairs(cell:Neighbors()) do
      table.insert(frontier,v)
    end
    while #frontier>0 do
      cell=RNG.FromList(frontier)
      local candidates={}
      for _,direction in ipairs(cell:Directions()) do
        local neighbor = cell:GetNeighbor(direction)
        local found=false
        for _,v in ipairs(inside) do
          if v==neighbor then
            found=true
            break
          end
        end
        if found then
          table.insert(candidates,direction)
        end
      end
      local direction=RNG.FromList(candidates)
      cell:GetDoor(direction).Open=true
      table.insert(inside,cell)
      local index=0
      for i,v in ipairs(frontier) do
        if v==cell then
          index=i
          break
        end
      end
      table.remove(frontier,index)
      for _,neighbor in ipairs(cell:Neighbors()) do
        local found=false
        for _,v in ipairs(inside) do
          if v==neighbor then
            found=true
            break
          end
        end
        if not found then
          found=false
          for _,v in ipairs(frontier) do
            if v==neighbor then
              found=true
              break
            end
          end
          if not found then
            table.insert(frontier,neighbor)
          end
        end
      end
    end
  end
  for column=1,maze.Columns do
    for row=1,maze.Rows do
      local cell = maze:GetCell(column,row)
      for k,v in pairs(directions) do
        if not cell:HasNeighbor(k) then
          local nextColumn=column+v.DeltaX
          local nextRow =row+v.DeltaY
          local nextCell = maze:GetCell(nextColumn,nextRow)
          if nextCell ~= nil then
            cell:SetNeighbor(k,nextCell)
            nextCell:SetNeighbor(v.Opposite,cell)
            local door = {Open=false}
            cell:SetDoor(k,door)
            nextCell:SetDoor(v.Opposite,door)
          end
        end
      end
    end
  end
  return maze
end