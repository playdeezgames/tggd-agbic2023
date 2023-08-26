Public Interface IWorld
    Inherits IStatisticsHolder
    Inherits IFlagHolder
    Inherits IMetadataHolder
    Function GetCharacter(id As Integer) As ICharacter
    Function GetItem(id As Integer) As IItem
    ReadOnly Property SerializedData As String
    Function CreateMap(mapType As String, size As (Integer, Integer), terrainType As String) As IMap
    Function CreateCharacter(characterType As String, cell As ICell) As ICharacter
    Sub DismissMessage()
    ReadOnly Property CurrentMessage As IMessage
    Property Avatar As ICharacter
    ReadOnly Property Characters As IEnumerable(Of ICharacter)
    ReadOnly Property HasMessages As Boolean
    Function CreateMessage() As IMessage
    Function CreateItem(itemType As String) As IItem
    Function GetMap(id As Integer) As IMap
    ReadOnly Property Maps As IEnumerable(Of IMap)
End Interface
