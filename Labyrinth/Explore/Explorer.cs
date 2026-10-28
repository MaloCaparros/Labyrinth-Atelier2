using Labyrinth.Crawl;
using Labyrinth.Tiles;

namespace Labyrinth.Explore;

public class Explorer(ICrawler crawler, Random? random = null)
{
    private readonly Random _random = random ?? new Random();
    
    public bool GetOut(int n)
    {
        for (var i = 0; i < n && crawler.FacingTile is not Outside; i++)
        {
            switch (_random.Next(3))
            {
                case 0 when crawler.FacingTile.IsTraversable:
                    crawler.Walk();
                    break;
                case 1:
                    crawler.Direction.TurnLeft();
                    break;
                default:
                    crawler.Direction.TurnRight();
                    break;
            }
        }
        return crawler.FacingTile is Outside;
    }
}
