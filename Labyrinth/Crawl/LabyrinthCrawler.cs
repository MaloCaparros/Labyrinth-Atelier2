using Labyrinth.Items;
using Labyrinth.Tiles;

namespace Labyrinth.Crawl;

public class LabyrinthCrawler(int x, int y, Tile[,] tiles) : ICrawler
{
    public int X { get; } = x;
    public int Y { get; } = y;
    public Direction Direction { get; } = Direction.North;
    public Tile FacingTile => tiles[X + Direction.DeltaX, Y + Direction.DeltaY];
    public Inventory Walk()
    {
        throw new NotImplementedException();
    }
}
