Imports BQ.Data

Public Class WorldDataClient
    Protected ReadOnly WorldData As WorldData
    Protected Shared ReadOnly LuaState As New Lua
    Sub New(worldData As WorldData)
        Me.WorldData = worldData
    End Sub
End Class
