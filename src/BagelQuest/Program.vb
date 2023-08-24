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
            hueTable,
            commandTable,
            LoadSfx(),
            LoadMux)
            host.Run()
        End Using
    End Sub
    Private Function LoadSfx() As IReadOnlyDictionary(Of String, String)
        Return JsonSerializer.Deserialize(Of Dictionary(Of String, String))(File.ReadAllText("Content/sfx.json"))
    End Function
    Private Function LoadFonts() As IReadOnlyDictionary(Of String, String)
        Return JsonSerializer.Deserialize(Of Dictionary(Of String, String))(File.ReadAllText("Content/font.json"))
    End Function
    Private Function LoadMux() As IReadOnlyDictionary(Of String, String)
        Return JsonSerializer.Deserialize(Of Dictionary(Of String, String))(File.ReadAllText("Content/mux.json"))
    End Function
    Private ReadOnly hueTable As IReadOnlyDictionary(Of Integer, Color) =
        New Dictionary(Of Integer, Color) From
{
            {Hue.Black, New Color(0, 0, 0)},
            {Hue.Cyan, New Color(41, 208, 208)},
            {Hue.Purple, New Color(129, 38, 192)},
            {Hue.White, New Color(255, 255, 255)},
            {Hue.Orange, New Color(255, 146, 51)},
            {Hue.Brown, New Color(129, 74, 25)},
            {Hue.Red, New Color(173, 35, 35)},
            {Hue.Blue, New Color(42, 75, 215)},
            {Hue.DarkGray, New Color(87, 87, 87)},
            {Hue.LightGray, New Color(160, 160, 160)},
            {Hue.LightGreen, New Color(129, 197, 122)},
            {Hue.LightBlue, New Color(157, 175, 255)},
            {Hue.Yellow, New Color(255, 238, 51)},
            {Hue.Tan, New Color(233, 222, 187)},
            {Hue.Pink, New Color(255, 205, 243)},
            {Hue.Green, New Color(29, 105, 20)}
        }
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
