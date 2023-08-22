Public Interface ICell
    Inherits IStatisticsHolder
    Inherits IFlagHolder
    Inherits IMetadataHolder
    ReadOnly Property Characters As IEnumerable(Of ICharacter)
    ReadOnly Property OtherCharacters(character As ICharacter) As IEnumerable(Of ICharacter)
    ReadOnly Property HasCharacters As Boolean
    Sub AddCharacter(character As ICharacter)
    Sub RemoveCharacter(character As ICharacter)
    ReadOnly Property HasCharacter(character As ICharacter) As Boolean
    ReadOnly Property HasOtherCharacters(character As ICharacter) As Boolean
    ReadOnly Property Id As Integer
    ReadOnly Property Map As IMap
    ReadOnly Property Column As Integer
    ReadOnly Property Row As Integer
    Property TerrainType As String
    Sub AddItem(item As IItem)
    Sub RemoveItem(item As IItem)
    ReadOnly Property TopItem As IItem
    ReadOnly Property HasItems As Boolean
    ReadOnly Property Items As IEnumerable(Of IItem)
    ReadOnly Property HasEffect As Boolean
    Property Effect As IMapEffect
End Interface
