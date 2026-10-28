using Labyrinth.Tiles;

namespace Labyrinth.Build
{
    public interface IBuilder
    {
        /// <summary>
        /// Raised each time a starting position is found while building.
        /// </summary>
        event EventHandler<StartEventArgs>? StartPositionFound;

        Tile[,] Build();
    }
}
