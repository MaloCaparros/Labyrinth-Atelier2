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
            return IsOutside(facingX, facingY) ? Outside.Singleton : tiles[facingX, facingY];
        }
    }
    private bool IsOutside(int tileX, int tileY) =>
        tileX < 0 || tileY < 0 || tileX >= tiles.GetLength(0) || tileY >= tiles.GetLength(1);
    public Inventory Walk()
    {
        throw new NotImplementedException();
    }
}
