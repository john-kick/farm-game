using FarmGame.Scripts.Tiles;

namespace FarmGame.Scripts.Controls.Interactions
{
    public partial class ReplaceTileInteraction(TileType newTileType) : Interaction
    {
        public TileType NewTileType = newTileType;
    }
}