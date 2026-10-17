// Records screens of the original game by replaying docs/reference/scenes.txt against the real VB UI.
// Usage: dotnet Oracle.dll <scenes.txt> <outdir>. The working directory must be src/BagelQuest (the game loads Content/...
// relative to it); tools/oracle.sh does that. The world is random, so scenes stage what they show (see the DSL notes in the
// scenes file); the Odin tests replay the same file against the port.
using System.Reflection;
using AOS.UI;
using BQ.Business;
using BQ.Persistence;
using BQ.Presentation;

const int W = 384, H = 216;
var scenesPath = Path.GetFullPath(args[0]);
var outDir = Path.GetFullPath(args[1]);
Directory.CreateDirectory(outDir);

IGameController controller = null;
BagelQuestContext context = null;
HueBuffer buffer = null;
int shots = 0, failures = 0;
string scene = "";

void NewScene(string name)
{
    scene = name;
    var fonts = new Dictionary<string, string>
    {
        ["UIFont"] = "Content/Fonts/CyFont8x8.json",
        ["BagelQuestFont"] = "Content/Fonts/BagelQuest.json",
    };
    context = new BagelQuestContext(fonts, (W, H));
    controller = new GameController(new Settings(), context);
    controller.SetSfxHook(_ => { });
    controller.SetMuxHook(_ => { });
    controller.SetMuxVolumeHook(_ => { });
    controller.SetSizeHook((_, _) => { });
    buffer = new HueBuffer(W, H);
    controller.Render(buffer);
}

IWorld GetWorld() => (IWorld)context.Model.GetType().GetField("_world", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(context.Model);

ICell Cell(string map, int col, int row) => GetWorld().Maps.Single(m => m.MapType == map).GetCell(col, row);
ICell AvatarCell() => GetWorld().Avatar.Cell;

void Press(string cmd)
{
    controller.HandleCommand(cmd);
    controller.Render(buffer);
}

foreach (var raw in File.ReadAllLines(scenesPath))
{
    var line = raw.Trim();
    if (line.Length == 0 || line[0] == '#') continue;
    var p = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    try
    {
        switch (p[0])
        {
            case "scene": NewScene(p[1]); break;
            case "keys": foreach (var k in p.Skip(1)) Press(k); break;
            case "embark": Press("A"); Press("A"); break;
            case "neutral":
                typeof(BaseGameController<IWorldModel>).GetMethod("SetCurrentState", BindingFlags.NonPublic | BindingFlags.Instance)
                    .Invoke(controller, new object[] { "Neutral", false });
                controller.Render(buffer);
                break;
            case "wait": Thread.Sleep(int.Parse(p[1])); controller.Render(buffer); break;
            case "shot":
                controller.Render(buffer);
                File.WriteAllText(Path.Combine(outDir, p[1] + ".txt"), buffer.ToText());
                shots++;
                break;
            case "avatar":
            {
                var w = GetWorld();
                var next = Cell(p[1], int.Parse(p[2]), int.Parse(p[3]));
                next.AddCharacter(w.Avatar);
                w.Avatar.Cell.RemoveCharacter(w.Avatar);
                w.Avatar.Cell = next;
                break;
            }
            case "terrain": Cell(p[1], int.Parse(p[2]), int.Parse(p[3])).TerrainType = p[4]; break;
            case "fill":
                for (int c = int.Parse(p[2]); c <= int.Parse(p[4]); c++)
                    for (int r = int.Parse(p[3]); r <= int.Parse(p[5]); r++)
                        Cell(p[1], c, r).TerrainType = p[6];
                break;
            case "give":
                for (int i = 0; i < (p.Length > 2 ? int.Parse(p[2]) : 1); i++)
                    GetWorld().Avatar.AddItem(ItemInitializer.CreateItem(GetWorld(), p[1]));
                break;
            case "ground":
                for (int i = 0; i < (p.Length > 2 ? int.Parse(p[2]) : 1); i++)
                    AvatarCell().AddItem(ItemInitializer.CreateItem(GetWorld(), p[1]));
                break;
            case "equip": GetWorld().Avatar.Equip(p[1], ItemInitializer.CreateItem(GetWorld(), p[2])); break;
            case "stat": GetWorld().Avatar.SetStatistic(p[1], int.Parse(p[2])); break;
            case "flag": GetWorld().Avatar.SetFlag(p[1], p[2] == "1"); break;
            case "forage": AvatarCell().SetStatistic("ForageRemaining", int.Parse(p[1])); break;
            case "enemy":
            {
                var create = typeof(ItemInitializer).Assembly.GetTypes().Single(t => t.Name == "MapInitializer")
                    .GetMethod("CreateCharacter", BindingFlags.NonPublic | BindingFlags.Static);
                var cell = AvatarCell();
                var enemy = (ICharacter)create.Invoke(null, new object[] { p[1], cell });
                cell.AddCharacter(enemy);
                break;
            }
            default: throw new Exception("unknown op " + p[0]);
        }
    }
    catch (Exception ex)
    {
        var inner = ex is TargetInvocationException t ? t.InnerException : ex;
        failures++;
        Console.WriteLine($"FAILED scene {scene}, line '{line}': {inner.GetType().Name}: {inner.Message}");
        // the scene's remaining lines run against a possibly broken controller: skip to the next scene
        while (false) { }
    }
}
Console.WriteLine($"{shots} shots recorded, {failures} failures");

class Settings : ISettings
{
    public (int width, int height) WindowSize { get; set; } = (384 * 3, 216 * 3);
    public bool FullScreen { get; set; }
    public float SfxVolume { get; set; } = 0.5f;
    public float MuxVolume { get; set; } = 0.3f;
    public void Save() { } // the real one writes config.json; not wanted here
}

// Same bounds rule as the Host's DisplayBuffer: pixels outside the view are ignored.
class HueBuffer : BasePixelSink
{
    readonly int w, h;
    readonly int[] hues;
    public HueBuffer(int w, int h) { this.w = w; this.h = h; hues = new int[w * h]; }
    public override void SetPixel(int x, int y, int hue)
    {
        if (x < 0 || y < 0 || x >= w || y >= h) return;
        hues[x + y * w] = hue;
    }
    public override void Fill(int hue) => Fill((0, 0), (w, h), hue);
    public string ToText()
    {
        var sb = new System.Text.StringBuilder();
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++) sb.Append("0123456789ABCDEF"[hues[x + y * w]]);
            sb.Append('\n');
        }
        return sb.ToString();
    }
}
