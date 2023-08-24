Imports System.IO
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
            commandTable,
            LoadSfx(),
            LoadMux)
            host.Run()
        End Using
    End Sub

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
    Private ReadOnly commandTable As IReadOnlyDictionary(Of String, Func(Of KeyboardState, GamePadState, Boolean)) =
    New Dictionary(Of String, Func(Of KeyboardState, GamePadState, Boolean)) From
    {
        {AOS.UI.Command.A, Function(keyboard, gamePad) keyboard.IsKeyDown(Keys.Space) OrElse keyboard.IsKeyDown(Keys.Enter) OrElse gamePad.IsButtonDown(Buttons.A)},
        {AOS.UI.Command.B, Function(keyboard, gamePad) keyboard.IsKeyDown(Keys.Escape) OrElse keyboard.IsKeyDown(Keys.NumPad0) OrElse gamePad.IsButtonDown(Buttons.B)},
        {AOS.UI.Command.Up, Function(keyboard, gamePad) keyboard.IsKeyDown(Keys.Up) OrElse keyboard.IsKeyDown(Keys.W) OrElse keyboard.IsKeyDown(Keys.Z) OrElse keyboard.IsKeyDown(Keys.NumPad8) OrElse gamePad.DPad.Up = ButtonState.Pressed},
        {AOS.UI.Command.Down, Function(keyboard, gamePad) keyboard.IsKeyDown(Keys.Down) OrElse keyboard.IsKeyDown(Keys.S) OrElse keyboard.IsKeyDown(Keys.NumPad2) OrElse gamePad.DPad.Down = ButtonState.Pressed},
        {AOS.UI.Command.Left, Function(keyboard, gamePad) keyboard.IsKeyDown(Keys.Left) OrElse keyboard.IsKeyDown(Keys.A) OrElse keyboard.IsKeyDown(Keys.Q) OrElse keyboard.IsKeyDown(Keys.NumPad4) OrElse gamePad.DPad.Left = ButtonState.Pressed},
        {AOS.UI.Command.Right, Function(keyboard, gamePad) keyboard.IsKeyDown(Keys.Right) OrElse keyboard.IsKeyDown(Keys.D) OrElse keyboard.IsKeyDown(Keys.NumPad6) OrElse gamePad.DPad.Right = ButtonState.Pressed}
    }
End Module
