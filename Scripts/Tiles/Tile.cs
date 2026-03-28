using Godot;

namespace FarmGame.Scripts.Tiles
{
	public abstract partial class Tile : Node3D
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
		/// Grid position of this tile
		/// </summary>
		[Export] public Vector2I GridPosition { get; set; }

		public override void _Ready()
		{
			// Rendering is handled by TileField using MultiMesh
		}
	}
}
