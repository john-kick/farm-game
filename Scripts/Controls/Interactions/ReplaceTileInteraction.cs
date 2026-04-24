using FarmGame.Scripts.Tiles;
using Godot;

namespace FarmGame.Scripts.Controls.Interactions
{
    public partial class ReplaceTileInteraction(TileType newTileType, Vector2I gridPosition) : Interaction
    {
        public TileType NewTileType = newTileType;
        public Vector2I GridPosition = gridPosition;

        public override void Process()
        {

        }
    }
}