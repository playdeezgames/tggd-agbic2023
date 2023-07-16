Imports System.IO
Imports System.Text.Json
Imports AOS.UI

Public Class BagelQuestSettings
    Implements ISettings
    Sub New()
        Dim cfg = ReadConfig()
        WindowSize = (cfg.WindowWidth, cfg.WindowHeight)
        FullScreen = cfg.FullScreen
        SfxVolume = cfg.SfxVolume
        MuxVolume = cfg.MuxVolume
    End Sub
    Public Property WindowSize As (Integer, Integer) Implements ISettings.WindowSize
    Public Property FullScreen As Boolean Implements ISettings.FullScreen
    Public Property SfxVolume As Single Implements ISettings.SfxVolume
    Public Property MuxVolume As Single Implements ISettings.MuxVolume
    Private Const ConfigFileName = "config.json"
    Private Shared Function ReadConfig() As BagelQuestConfig
        Try
            Return JsonSerializer.Deserialize(Of BagelQuestConfig)(File.ReadAllText(ConfigFileName))
        Catch ex As Exception
            Return New BagelQuestConfig() With
            {
                .FullScreen = False,
                .SfxVolume = 0.5,
                .MuxVolume = 0.5,
                .WindowHeight = DefaultScreenHeight,
                .WindowWidth = DefaultScreenWidth
            }
        End Try
    End Function
    Public Sub Save() Implements ISettings.Save
        File.WriteAllText(ConfigFileName, JsonSerializer.Serialize(New BagelQuestConfig With {.SfxVolume = SfxVolume, .WindowHeight = WindowSize.Item2, .WindowWidth = WindowSize.Item1, .FullScreen = FullScreen}))
    End Sub
End Class
