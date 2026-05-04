using FarmGame.Scripts.Controls.Interactions;
using FarmGame.Scripts.Environment;
using Godot;

namespace FarmGame.Scripts.Tiles
{
	public abstract partial class Tile(Field field = null) : Node3D, IInteractable
	{
		/// <summary>
		/// The type of this tile
		/// </summary>
		public abstract TileType TileType { get; }

		/// <summary>
		/// The height of this tile
		/// </summary>
		public abstract float Height { get; }

		/// <summary>
		/// Reference to the Field this tile is on, if any
		/// </summary>
		public Field Field = field;

		/// <summary>
		/// Grid position of this tile
		/// </summary>
		[Export] public Vector2I GridPosition { get; set; }

		protected ReplaceTileInteraction CreateReplaceTileInteraction(TileType newTileType)
		{
			return new ReplaceTileInteraction(newTileType, Field, GridPosition);
		}

		public virtual Interaction PrimaryInteraction() => new NoInteraction();
		public virtual Interaction SecondaryInteraction() => new NoInteraction();
		public virtual Interaction TertiaryInteraction() => new NoInteraction();
	}
}
