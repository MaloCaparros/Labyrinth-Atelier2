using Labyrinth.Crawl;
using Labyrinth.Tiles;

namespace Labyrinth.Explore;

public class Explorer(ICrawler crawler, Random? random = null)
{
    private readonly Random _random = random ?? new Random();

    public event EventHandler<CrawlingEventArgs>? PositionChanged;

    public event EventHandler<CrawlingEventArgs>? DirectionChanged;

    public bool GetOut(int n)
    {
        for (var i = 0; i < n && crawler.FacingTile is not Outside; i++)
        {
            // 0: turn left, 1: turn right, 2 to 9: walk (or turn left if even, right if odd, when blocked)
            var move = _random.Next(10);

            if (move >= 2 && crawler.FacingTile.IsTraversable)
            {
                crawler.Walk();
                Raise(PositionChanged);
            }
            else if (move % 2 == 0)
            {
                crawler.Direction.TurnLeft();
                Raise(DirectionChanged);
            }
            else
            {
                crawler.Direction.TurnRight();
                Raise(DirectionChanged);
            }
        }
        return crawler.FacingTile is Outside;
    }

    // The direction is cloned because the crawler keeps turning the same instance after the event.
    private void Raise(EventHandler<CrawlingEventArgs>? handler) =>
        handler?.Invoke(this, new CrawlingEventArgs(crawler.X, crawler.Y, (Direction)crawler.Direction.Clone()));
}
