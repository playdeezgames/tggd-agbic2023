Imports System.IO
Imports System.Text.Json
Imports AOS.UI

Public Class BagelQuestSettings
    Implements ISettings
    Sub New()
        Dim cfg = ReadConfig()
        WindowSize = (cfg.WindowWidth, cfg.WindowHeight)
        FullScreen = cfg.FullScreen
        Volume = cfg.SfxVolume
    End Sub
    Public Property WindowSize As (Integer, Integer) Implements ISettings.WindowSize
    Public Property FullScreen As Boolean Implements ISettings.FullScreen
    Public Property Volume As Single Implements ISettings.Volume
    Private Const ConfigFileName = "config.json"
    Private Shared Function ReadConfig() As BagelQuestConfig
        Try
            Return JsonSerializer.Deserialize(Of BagelQuestConfig)(File.ReadAllText(ConfigFileName))
        Catch ex As Exception
            Return New BagelQuestConfig() With
            {
                .FullScreen = False,
                .SfxVolume = 0.5,
                .WindowHeight = DefaultScreenHeight,
                .WindowWidth = DefaultScreenWidth
            }
        End Try
    End Function
    Public Sub Save() Implements ISettings.Save
        File.WriteAllText(ConfigFileName, JsonSerializer.Serialize(New BagelQuestConfig With {.SfxVolume = Volume, .WindowHeight = WindowSize.Item2, .WindowWidth = WindowSize.Item1, .FullScreen = FullScreen}))
    End Sub
End Class
