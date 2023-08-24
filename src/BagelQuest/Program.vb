Imports System.IO
Imports System.Security.Cryptography
Imports System.Text.Json
Imports AOS.Presentation
Imports BQ.Presentation
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Input
Module Program
    Sub Main(args As String())
        Using host As New Host(
            GameTitle,
            New GameController(
                New BagelQuestSettings(),
                New BagelQuestContext(LoadFonts(), (ViewWidth, ViewHeight))),
            (ViewWidth, ViewHeight),
            LoadHues(),
            LoadCommands(),
            LoadSfx(),
            LoadMux)
            host.Run()
        End Using
    End Sub
    Private Function LoadCommands() As IReadOnlyDictionary(Of String, Func(Of KeyboardState, GamePadState, Boolean))
        Dim keysForCommands = keysTable.
            GroupBy(Function(x) x.Value).
            ToDictionary(
                Function(x) x.Key,
                Function(x) x.Select(Function(y) y.Key).ToList())
        Dim result = New Dictionary(Of String, Func(Of KeyboardState, GamePadState, Boolean))
        For Each cmd In gamePadCommandTable.Keys
            result.Add(cmd, MakeCommandHandler(keysForCommands(cmd), gamePadCommandTable(cmd)))
        Next
        Return result
    End Function

    Private Function MakeCommandHandler(keys As List(Of Keys), func As Func(Of GamePadState, Boolean)) As Func(Of KeyboardState, GamePadState, Boolean)
        Return Function(k, g)
                   Return func(g) OrElse keys.Any(Function(x) k.IsKeyDown(x))
               End Function
    End Function

    Private Const HueFilename As String = "Content/hue.json"
    Private Const SfxFilename As String = "Content/sfx.json"
    Private Const FontFilename As String = "Content/font.json"
    Private Const MuxFilename As String = "Content/mux.json"
    Private Function LoadHues() As IReadOnlyDictionary(Of Integer, Color)
        Return JsonSerializer.Deserialize(Of Dictionary(Of Integer, Color))(File.ReadAllText(HueFilename))
    End Function
    Private Function LoadSfx() As IReadOnlyDictionary(Of String, String)
        Return JsonSerializer.Deserialize(Of Dictionary(Of String, String))(File.ReadAllText(SfxFilename))
    End Function
    Private Function LoadFonts() As IReadOnlyDictionary(Of String, String)
        Return JsonSerializer.Deserialize(Of Dictionary(Of String, String))(File.ReadAllText(FontFilename))
    End Function
    Private Function LoadMux() As IReadOnlyDictionary(Of String, String)
        Return JsonSerializer.Deserialize(Of Dictionary(Of String, String))(File.ReadAllText(MuxFilename))
    End Function
    Private ReadOnly gamePadCommandTable As IReadOnlyDictionary(Of String, Func(Of GamePadState, Boolean)) =
    New Dictionary(Of String, Func(Of GamePadState, Boolean)) From
    {
        {AOS.UI.Command.A, Function(gamePad) gamePad.IsButtonDown(Buttons.A)},
        {AOS.UI.Command.B, Function(gamePad) gamePad.IsButtonDown(Buttons.B)},
        {AOS.UI.Command.Up, Function(gamePad) gamePad.DPad.Up = ButtonState.Pressed},
        {AOS.UI.Command.Down, Function(gamePad) gamePad.DPad.Down = ButtonState.Pressed},
        {AOS.UI.Command.Left, Function(gamePad) gamePad.DPad.Left = ButtonState.Pressed},
        {AOS.UI.Command.Right, Function(gamePad) gamePad.DPad.Right = ButtonState.Pressed}
    }
    Private ReadOnly keysTable As IReadOnlyDictionary(Of Keys, String) =
        New Dictionary(Of Keys, String) From
        {
            {Keys.Space, AOS.UI.Command.A},
            {Keys.Enter, AOS.UI.Command.A},
            {Keys.Escape, AOS.UI.Command.B},
            {Keys.NumPad0, AOS.UI.Command.B},
            {Keys.Up, AOS.UI.Command.Up},
            {Keys.W, AOS.UI.Command.Up},
            {Keys.Z, AOS.UI.Command.Up},
            {Keys.NumPad8, AOS.UI.Command.Up},
            {Keys.Down, AOS.UI.Command.Down},
            {Keys.S, AOS.UI.Command.Down},
            {Keys.NumPad2, AOS.UI.Command.Down},
            {Keys.Left, AOS.UI.Command.Left},
            {Keys.A, AOS.UI.Command.Left},
            {Keys.Q, AOS.UI.Command.Left},
            {Keys.NumPad4, AOS.UI.Command.Left},
            {Keys.Right, AOS.UI.Command.Right},
            {Keys.D, AOS.UI.Command.Right},
            {Keys.NumPad6, AOS.UI.Command.Right}
        }
End Module
