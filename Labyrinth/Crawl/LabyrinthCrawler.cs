using Labyrinth.Items;
using Labyrinth.Tiles;

namespace Labyrinth.Crawl;

public class LabyrinthCrawler : ICrawler
{
    public int X { get; } = 2;
    public int Y { get; } = 1;
    public Direction Direction { get; } = Direction.North;
    public Tile FacingTile { get; } = Wall.Singleton;
    public Inventory Walk()
    {
        throw new NotImplementedException();
    }
}