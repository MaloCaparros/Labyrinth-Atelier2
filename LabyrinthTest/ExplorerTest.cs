using Labyrinth.Build;
using Labyrinth.Crawl;
using Labyrinth.Explore;

namespace LabyrinthTest.Explore;

[TestFixture(Description = "Explorer tests with a mocked random generator")]
public class ExplorerTest
{
    /// <summary>
    /// Fake random generator returning the given values in order (0 = walk, 1 = turn left, 2 = turn right).
    /// </summary>
    private class FakeRandom(params int[] values) : Random
    {
        private readonly Queue<int> _values = new(values);

        public override int Next(int maxValue) => _values.Dequeue();
    }

    private static ICrawler NewCrawlerFor(string asciiMap) =>
        new Labyrinth.Labyrinth(new AsciiParser(asciiMap)).NewCrawler();

    [Test]
    public void GetOutWhenAlreadyFacingOutsideReturnsTrueWithoutMoving()
    {
        var crawler = NewCrawlerFor("""
                +x+
                | |
                +-+
                """);
        var test = new Explorer(crawler, new FakeRandom());

        var found = test.GetOut(10);

        using var all = Assert.EnterMultipleScope();

        Assert.That(found, Is.True);
        Assert.That(crawler.X, Is.EqualTo(1));
        Assert.That(crawler.Y, Is.EqualTo(0));
    }

    [Test]
    public void GetOutWithZeroMoveReturnsFalse()
    {
        var crawler = NewCrawlerFor("""
                +--+
                | x|
                +--+
                """);
        var test = new Explorer(crawler, new FakeRandom());

        var found = test.GetOut(0);

        using var all = Assert.EnterMultipleScope();

        Assert.That(found, Is.False);
        Assert.That(crawler.X, Is.EqualTo(2));
        Assert.That(crawler.Y, Is.EqualTo(1));
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
        var test = new Explorer(crawler, new FakeRandom(0, 0));

        var found = test.GetOut(10);

        using var all = Assert.EnterMultipleScope();

        Assert.That(found, Is.True);
        Assert.That(crawler.X, Is.EqualTo(1));
        Assert.That(crawler.Y, Is.EqualTo(0));
        Assert.That(crawler.Direction, Is.EqualTo(Direction.North));
    }

    [Test]
    public void GetOutTurnsLeftThenWalksToTheExit()
    {
        var crawler = NewCrawlerFor("""
                +--+
                 x |
                +--+
                """);
        var test = new Explorer(crawler, new FakeRandom(1, 0));

        var found = test.GetOut(10);

        using var all = Assert.EnterMultipleScope();

        Assert.That(found, Is.True);
        Assert.That(crawler.X, Is.EqualTo(0));
        Assert.That(crawler.Y, Is.EqualTo(1));
        Assert.That(crawler.Direction, Is.EqualTo(Direction.West));
    }

    [Test]
    public void GetOutTurnsRightInsteadOfWalkingIntoAWall()
    {
        var crawler = NewCrawlerFor("""
                +--+
                | x|
                +--+
                """);
        var test = new Explorer(crawler, new FakeRandom(0));

        var found = test.GetOut(1);

        using var all = Assert.EnterMultipleScope();

        Assert.That(found, Is.False);
        Assert.That(crawler.X, Is.EqualTo(2));
        Assert.That(crawler.Y, Is.EqualTo(1));
        Assert.That(crawler.Direction, Is.EqualTo(Direction.East));
    }

    [Test]
    public void GetOutStopsAfterNMoves()
    {
        var crawler = NewCrawlerFor("""
                +--+
                | x|
                +--+
                """);
        var test = new Explorer(crawler, new FakeRandom(2, 2, 2, 2, 2));

        var found = test.GetOut(3);

        using var all = Assert.EnterMultipleScope();

        Assert.That(found, Is.False);
        Assert.That(crawler.Direction, Is.EqualTo(Direction.West));
    }
}
