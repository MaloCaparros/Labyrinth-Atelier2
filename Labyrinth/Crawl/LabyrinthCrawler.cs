using Labyrinth.Items;
using Labyrinth.Tiles;

namespace Labyrinth.Crawl;

public class LabyrinthCrawler(int x, int y, Tile[,] tiles) : ICrawler
{
    public int X { get; } = x;
    public int Y { get; } = y;
    public Direction Direction { get; } = Direction.North;
    public Tile FacingTile
    {
        get
        {
            var (facingX, facingY) = (X + Direction.DeltaX, Y + Direction.DeltaY);
            return facingX < 0 || facingY < 0 || facingX >= tiles.GetLength(0)
                ? Outside.Singleton
                : tiles[facingX, facingY];
        }
    }
    public Inventory Walk()
    {
        throw new NotImplementedException();
    }
}
