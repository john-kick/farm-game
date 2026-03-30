using Godot;
using System.Collections.Generic;

namespace FarmGame.Scripts.Tiles
{
	public partial class Field : Node3D
	{
		[Export] public int Width = 20;
		[Export] public int Height = 20;
		[Export] public float TileSize = 1.0f;

		private readonly Dictionary<Vector2I, Tile> tiles = [];
		private FieldRenderer fieldRenderer;

		public override void _Ready()
		{
			fieldRenderer = new FieldRenderer(this);
			InitializeField(TileSize);
		}

		public void InitializeField(float TileSize)
		{
			this.TileSize = TileSize;
			ClearField();
			CreateRandomField();
			RenderTiles();
		}

		private void CreateUniformField(TileType tileType)
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					Vector2I gridPos = new(x, y);
					Tile tile = TileFactory.CreateTile(tileType);
					tile.GridPosition = gridPos;
					AddTile(gridPos, tile);
				}
			}
		}

		private void CreateRandomField()
		{
			var random = new RandomNumberGenerator();
			random.Randomize();

			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					Vector2I gridPos = new(x, y);
					TileType tileType = (TileType)(random.Randi() % 3);
					Tile tile = TileFactory.CreateTile(tileType);
					tile.GridPosition = gridPos;
					AddTile(gridPos, tile);
				}
			}
		}

		/// <summary>
		/// Add a tile to the field at the specified grid position
		/// </summary>
		public void AddTile(Vector2I gridPos, Tile tile)
		{
			if (tiles.ContainsKey(gridPos))
				RemoveTile(gridPos);
			
			tile.GridPosition = gridPos;
			tiles[gridPos] = tile;
		}

		/// <summary>
		/// Remove a tile from the field
		/// </summary>
		public void RemoveTile(Vector2I gridPos)
		{
			tiles.Remove(gridPos);
		}

		/// <summary>
		/// Get a tile at the specified grid position
		/// </summary>
		public Tile GetTile(Vector2I gridPos)
		{
			tiles.TryGetValue(gridPos, out Tile tile);
			return tile;
		}

		/// <summary>
		/// Convert grid coordinates to world position
		/// </summary>
		public Vector3 GridToWorldPosition(Vector2I gridPos)
		{
			return new Vector3(gridPos.X * TileSize, 0, gridPos.Y * TileSize);
		}

		/// <summary>
		/// Convert world position to grid coordinates
		/// </summary>
		public Vector2I WorldToGridPosition(Vector3 worldPos)
		{
			return new Vector2I(
				Mathf.RoundToInt(worldPos.X / TileSize),
				Mathf.RoundToInt(worldPos.Z / TileSize)
			);
		}

		/// <summary>
		/// Clear all tiles from the field
		/// </summary>
		public void ClearField()
		{
			tiles.Clear();
			fieldRenderer?.Clear();
		}

		/// <summary>
		/// Check if a grid position is within field bounds
		/// </summary>
		public bool IsWithinBounds(Vector2I gridPos)
		{
			return gridPos.X >= 0 && gridPos.X < Width &&
				   gridPos.Y >= 0 && gridPos.Y < Height;
		}

		private void RenderTiles() => fieldRenderer?.RenderTiles(tiles.Values, TileSize);
	}
}
