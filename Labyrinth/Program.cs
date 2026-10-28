using Labyrinth.Build;
using Labyrinth.Crawl;
using Labyrinth.Explore;

var labyrinth = new Labyrinth.Labyrinth(new AsciiParser("""
    +--+--------+
    |  /        |
    |  +--+--+  |
    |     |k    |
    +--+  |  +--+
       |k  x    |
    +  +-------/|
    |           |
    +-----------+
    """));
var crawler = labyrinth.NewCrawler();
var explorer = new Explorer(crawler);

Console.Clear();
Console.Write(labyrinth);
var bottom = Console.CursorTop;

var (lastX, lastY) = (crawler.X, crawler.Y);
DrawExplorer(crawler.X, crawler.Y, crawler.Direction);

explorer.PositionChanged += OnCrawling;
explorer.DirectionChanged += OnCrawling;

var found = explorer.GetOut(500);

Console.SetCursorPosition(0, bottom);
Console.WriteLine(found ? "Sortie trouvée !" : "Pas de sortie trouvée.");

void OnCrawling(object? sender, CrawlingEventArgs e)
{
    Console.SetCursorPosition(lastX, lastY);
    Console.Write(' ');
    DrawExplorer(e.X, e.Y, e.Direction);
    (lastX, lastY) = (e.X, e.Y);
    Thread.Sleep(50);
}

static void DrawExplorer(int x, int y, Direction direction)
{
    Console.SetCursorPosition(x, y);
    Console.Write(direction switch
    {
        (0, -1) => '^',
        (1, 0) => '>',
        (0, 1) => 'v',
        _ => '<'
    });
}
