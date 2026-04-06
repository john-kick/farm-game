using FarmGame.Scripts.Environment;
using FarmGame.Scripts.Tiles;
using Godot;

namespace FarmGame.Scripts.Controls.Interactions
{
    public partial class ReplaceTileInteraction(TileType newTileType, Field field, Vector2I gridPosition) : Interaction
    {
        public TileType NewTileType = newTileType;
        public Field Field = field;
        public Vector2I GridPosition = gridPosition;

        public override void Process()
        {
            Tile tile = TileFactory.CreateTile(NewTileType);   
            Field.AddTile(GridPosition, tile);
            Field.Refresh();
        }
    }
}