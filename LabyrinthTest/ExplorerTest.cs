using Labyrinth.Build;
using Labyrinth.Crawl;
using Labyrinth.Explore;

namespace LabyrinthTest.Explore;

[TestFixture(Description = "Explorer tests with a mocked random generator")]
public class ExplorerTest
{
    /// <summary>
    /// Fake random generator returning the given values in order (0 = turn left, 1 = turn right, 2 = walk).
    /// </summary>
    private class FakeRandom(params int[] values) : Random
    {
        private readonly Queue<int> _values = new(values);

        public override int Next(int maxValue) => _values.Dequeue();
    }

    private List<(int X, int Y, Direction Direction)> _positions = [];
    private List<(int X, int Y, Direction Direction)> _directions = [];

    [SetUp]
    public void ResetEvents()
    {
        _positions = [];
        _directions = [];
    }

    private static ICrawler NewCrawlerFor(string asciiMap) =>
        new Labyrinth.Labyrinth(new AsciiParser(asciiMap)).NewCrawler();

    /// <summary>
    /// Creates an explorer playing the given moves and recording the raised events.
    /// </summary>
    private Explorer NewExplorer(ICrawler crawler, params int[] moves)
    {
        var explorer = new Explorer(crawler, new FakeRandom(moves));
        explorer.PositionChanged += (_, e) => _positions.Add((e.X, e.Y, e.Direction));
        explorer.DirectionChanged += (_, e) => _directions.Add((e.X, e.Y, e.Direction));
        return explorer;
    }

    [Test]
    public void GetOutWhenAlreadyFacingOutsideReturnsTrueWithoutMoving()
    {
        var crawler = NewCrawlerFor("""
                +x+
                | |
                +-+
                """);
        var test = NewExplorer(crawler);

        var found = test.GetOut(10);

        using var all = Assert.EnterMultipleScope();

        Assert.That(found, Is.True);
        Assert.That(crawler.X, Is.EqualTo(1));
        Assert.That(crawler.Y, Is.EqualTo(0));
        Assert.That(_positions, Is.Empty);
        Assert.That(_directions, Is.Empty);
    }

    [Test]
    public void GetOutWithZeroMoveReturnsFalse()
    {
        var crawler = NewCrawlerFor("""
                +--+
                | x|
                +--+
                """);
        var test = NewExplorer(crawler);

        var found = test.GetOut(0);

        using var all = Assert.EnterMultipleScope();

        Assert.That(found, Is.False);
        Assert.That(crawler.X, Is.EqualTo(2));
        Assert.That(crawler.Y, Is.EqualTo(1));
        Assert.That(_positions, Is.Empty);
        Assert.That(_directions, Is.Empty);
    }

    [Test]
    public void GetOutWalksToTheExit()
    {
        var crawler = NewCrawlerFor("""
                + +
                | |
                |x|
                +-+
                """);
        var test = NewExplorer(crawler, 2, 2);

        var found = test.GetOut(10);

        using var all = Assert.EnterMultipleScope();

        Assert.That(found, Is.True);
        Assert.That(crawler.X, Is.EqualTo(1));
        Assert.That(crawler.Y, Is.EqualTo(0));
        Assert.That(crawler.Direction, Is.EqualTo(Direction.North));
        Assert.That(_positions, Is.EqualTo(new[]
        {
            (1, 1, Direction.North),
            (1, 0, Direction.North)
        }));
        Assert.That(_directions, Is.Empty);
    }

    [Test]
    public void GetOutTurnsLeftThenWalksToTheExit()
    {
        var crawler = NewCrawlerFor("""
                +--+
                 x |
                +--+
                """);
        var test = NewExplorer(crawler, 0, 2);

        var found = test.GetOut(10);

        using var all = Assert.EnterMultipleScope();

        Assert.That(found, Is.True);
        Assert.That(crawler.X, Is.EqualTo(0));
        Assert.That(crawler.Y, Is.EqualTo(1));
        Assert.That(crawler.Direction, Is.EqualTo(Direction.West));
        Assert.That(_directions, Is.EqualTo(new[] { (1, 1, Direction.West) }));
        Assert.That(_positions, Is.EqualTo(new[] { (0, 1, Direction.West) }));
    }

    [Test]
    public void GetOutTurnsInsteadOfWalkingIntoAWall()
    {
        var crawler = NewCrawlerFor("""
                +--+
                | x|
                +--+
                """);
        var test = NewExplorer(crawler, 2);

        var found = test.GetOut(1);

        using var all = Assert.EnterMultipleScope();

        Assert.That(found, Is.False);
        Assert.That(crawler.X, Is.EqualTo(2));
        Assert.That(crawler.Y, Is.EqualTo(1));
        Assert.That(crawler.Direction, Is.EqualTo(Direction.West));
        Assert.That(_positions, Is.Empty);
        Assert.That(_directions, Is.EqualTo(new[] { (2, 1, Direction.West) }));
    }

    [Test]
    public void GetOutStopsAfterNMoves()
    {
        var crawler = NewCrawlerFor("""
                +--+
                | x|
                +--+
                """);
        var test = NewExplorer(crawler, 1, 1, 1, 1, 1);

        var found = test.GetOut(3);

        using var all = Assert.EnterMultipleScope();

        Assert.That(found, Is.False);
        Assert.That(crawler.Direction, Is.EqualTo(Direction.West));
        Assert.That(_positions, Is.Empty);
        Assert.That(_directions, Is.EqualTo(new[]
        {
            (2, 1, Direction.East),
            (2, 1, Direction.South),
            (2, 1, Direction.West)
        }));
    }
}
