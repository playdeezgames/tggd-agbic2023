Public Interface IWorld
    Sub Save(filename As String)
    Function CreateMap(mapType As String, size As (Integer, Integer), terrainType As String) As IMap
    Function CreateCharacter(characterType As String, cell As ICell) As ICharacter
    Sub DismissMessage()
    ReadOnly Property CurrentMessage As IMessage
    Property Avatar As ICharacter
    ReadOnly Property Characters As IEnumerable(Of ICharacter)
    ReadOnly Property Character(id As Integer) As ICharacter
    ReadOnly Property HasMessages As Boolean
    Function CreateMessage() As IMessage
    Function CreateItem(itemType As String) As IItem
    ReadOnly Property Item(id As Integer) As IItem
    ReadOnly Property Map(id As Integer) As IMap
    ReadOnly Property Maps As IEnumerable(Of IMap)
End Interface
