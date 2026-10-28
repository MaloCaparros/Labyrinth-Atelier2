using Labyrinth.Build;
using Labyrinth.Crawl;
using Labyrinth.Tiles;

namespace LabyrinthTest.Crawl;

[TestFixture(Description = "Integration test for the crawler implementation in the labyrinth")]
public class LabyrinthCrawlerTest
{
    private static ICrawler NewCrawlerFor(string asciiMap) =>
        new Labyrinth.Labyrinth(new AsciiParser(asciiMap)).NewCrawler();

    #region Initialization
    [Test]
    public void InitWithCenteredX()
    {
        var test = NewCrawlerFor("""
                +--+
                | x|
                +--+
                """);

        using var all = Assert.EnterMultipleScope();

        Assert.That(test.X, Is.EqualTo(2));
        Assert.That(test.Y, Is.EqualTo(1));
        Assert.That(test.Direction, Is.EqualTo(Direction.North));
        Assert.That(test.FacingTile, Is.TypeOf<Wall>());
    }

    [Test]
    public void InitWithMultipleXUsesLastOne()
    {
        var test = NewCrawlerFor("""
                +--+
                |x |
                | x|
                +--+
                """);

        using var all = Assert.EnterMultipleScope();

        Assert.That(test.X, Is.EqualTo(2));
        Assert.That(test.Y, Is.EqualTo(2));
        Assert.That(test.Direction, Is.EqualTo(Direction.North));
        Assert.That(test.FacingTile, Is.TypeOf<Room>());
    }

    [Test]
    public void InitWithNoXThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Labyrinth.Labyrinth(new AsciiParser("""
                +--+
                |  |
                +--+
                """)));
    }
    #endregion

    #region Labyrinth borders
    [Test]
    public void FacingNorthOnUpperTileReturnsOutside()
    {
        var test = NewCrawlerFor("""
                +x+
                | |
                +-+
                """);

        Assert.That(test.FacingTile, Is.TypeOf<Outside>());
    }

    [Test]
    public void FacingWestOnFarLeftTileReturnsOutside()
    {
        var test = NewCrawlerFor("""
                +-+
                x |
                +-+
                """);

        test.Direction.TurnLeft();

        Assert.That(test.FacingTile, Is.TypeOf<Outside>());
    }

    [Test]
    public void FacingEastOnFarRightTileReturnsOutside()
    {
        var test = NewCrawlerFor("""
                +-+
                | x
                +-+
                """);

        test.Direction.TurnRight();

        Assert.That(test.FacingTile, Is.TypeOf<Outside>());
    }

    [Test]
    public void FacingSouthOnBottomTileReturnsOutside()
    {
        var test = NewCrawlerFor("""
                +-+
                | |
                +x+
                """);

        test.Direction.TurnRight();
        test.Direction.TurnRight();

        Assert.That(test.FacingTile, Is.TypeOf<Outside>());
    }
    #endregion

    #region Moves
    [Test]
    public void TurnLeftFacesWestTile()
    {
        var test = NewCrawlerFor("""
                +--+
                |  |
                |x |
                +--+
                """);

        test.Direction.TurnLeft();

        using var all = Assert.EnterMultipleScope();

        Assert.That(test.Direction, Is.EqualTo(Direction.West));
        Assert.That(test.FacingTile, Is.TypeOf<Wall>());
    }

    [Test]
    public void WalkReturnsInventoryAndChangesPositionAndFacingTile()
    {
        var test = NewCrawlerFor("""
                +--+
                |  |
                |x |
                +--+
                """);

        var inventory = test.Walk();

        using var all = Assert.EnterMultipleScope();

        Assert.That(inventory.HasItem, Is.False);
        Assert.That(test.X, Is.EqualTo(1));
        Assert.That(test.Y, Is.EqualTo(1));
        Assert.That(test.FacingTile, Is.TypeOf<Wall>());
    }

    [Test]
    public void TurnAndWalkReturnsInventoryChangesPositionAndFacingTile()
    {
        var test = NewCrawlerFor("""
                +--+
                |  |
                |x |
                +--+
                """);

        test.Direction.TurnRight();

        var inventory = test.Walk();

        using var all = Assert.EnterMultipleScope();

        Assert.That(inventory.HasItem, Is.False);
        Assert.That(test.X, Is.EqualTo(2));
        Assert.That(test.Y, Is.EqualTo(2));
        Assert.That(test.Direction, Is.EqualTo(Direction.East));
        Assert.That(test.FacingTile, Is.TypeOf<Wall>());
    }

    [Test]
    public void WalkOnNonTraversableTileThrowsInvalidOperationException()
    {
        var test = NewCrawlerFor("""
                +--+
                | x|
                +--+
                """);

        using var all = Assert.EnterMultipleScope();

        Assert.Throws<InvalidOperationException>(() => test.Walk());
        Assert.That(test.X, Is.EqualTo(2));
        Assert.That(test.Y, Is.EqualTo(1));
    }
    #endregion

    #region Items and doors
    [Test]
    public void WalkInARoomWithAnItem()
    {
        Assert.That(false);
    }

    [Test]
    public void WalkUseAWrongKeyToOpenADoor()
    {
        Assert.That(false);
    }

    [Test]
    public void WalkUseKeyToOpenADoorAndPass()
    {
        var test = NewCrawlerFor("""
                +--+
                |xk|
                +-/|
                """);

        test.Direction.TurnRight();

        var inventory = test.Walk();

        test.Direction.TurnRight();
        ((Door)test.FacingTile).Open(inventory);

        test.Walk();

        using var all = Assert.EnterMultipleScope();

        Assert.That(test.X, Is.EqualTo(2));
        Assert.That(test.Y, Is.EqualTo(2));
        Assert.That(test.Direction, Is.EqualTo(Direction.South));
        Assert.That(test.FacingTile, Is.TypeOf<Outside>());
    }
    #endregion
}