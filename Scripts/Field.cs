using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FarmGame.Scripts.Tiles;
using Godot;

namespace FarmGame.Scripts
{
	public partial class Field(Vector2I Size, Mesh.PrimitiveType PrimitiveType): Node3D
	{
		private Dictionary<int, Tile> tiles = [];

		public override void _Ready()
		{
			GenerateUniformField(TileType.GRASS);
			InitRender();   

			Tile t = GetTile(40, 0, true);
			GD.Print($"{t.GridPosition}: {t.GetTileType()}");
		}

		public void GenerateRandomField(Vector2I Size)
		{
			for (int z = 0; z < Size.Y; z++)
			{
				for (int x = 0; x < Size.X; x++)
				{
					Tile tile = TileFactory.GetRandomTile();
					tile.GridPosition = new Vector2I(x, z);
					tiles[z * Size.X + x] = tile;
				}
			}
		}

		public void GenerateUniformField(TileType type)
		{
			for (int z = 0; z < Size.Y; z++)
			{
				for (int x = 0; x < Size.X; x++)
				{
					Tile tile = TileFactory.GetTile(type);
					tile.GridPosition = new Vector2I(x, z);
					tiles[z * Size.X + x] = tile;
				}
			}
		}

		/// <summary>
		/// Adds the tiles contained in the `Tiles` array to the Scene
		/// </summary>
		public void InitRender()
		{
			for (int z = 0; z < Size.Y; z++)
			{
				for (int x = 0; x < Size.X; x++)
				{
					Tile tile = GetTile(x, z);
					Neighbor<Tile>[] neighbors = GetNeighbors(tile);
					AddChild(tile);
					tile.Render(neighbors, PrimitiveType);
				}
			}
		}

		public void ReplaceTile(Tile oldTile, TileType newType)
		{
			Vector2I gridPosition = oldTile.GridPosition;

			// Generate the new tile
			Tile newTile = TileFactory.GetTile(newType);
			newTile.GridPosition = gridPosition;

			// Add the new tile to the tiles list
			tiles[gridPosition.Y * Size.X + gridPosition.X] = newTile;

			// Remove the old tile from the field
			RemoveChild(oldTile);
			oldTile.QueueFree();

			// Add the new tile to the field
			AddChild(newTile);
			Neighbor<Tile>[] neighbors = GetNeighbors(newTile);
			newTile.Render(neighbors);

			// Re-render the neighbors
			foreach (Neighbor<Tile> n in neighbors)
			{
				n.Element.Render(GetNeighbors(n.Element));
			}
		}

		private Neighbor<Tile>[] GetNeighbors(Tile tile)
		{
			int x = tile.GridPosition.X;
			int z = tile.GridPosition.Y;

			return [
				new Neighbor<Tile>()
				{
					// Right
					Element = GetTile(x + 1, z),
					Offset = new Vector2I(1, 0)
				},
				new Neighbor<Tile>()
				{
					// Left
					Element = GetTile(x - 1, z),
					Offset = new Vector2I(-1, 0)
				},
				new Neighbor<Tile>()
				{
					// Top
					Element = GetTile(x, z - 1),
					Offset = new Vector2I(0, -1)
				},
				new Neighbor<Tile>()
				{
					// Bottom
					Element = GetTile(x,z + 1),
					Offset = new Vector2I(0, 1)
				}
			];
		}

		public Tile GetTile(int x, int z, bool print = false)
		{
			if (x >= Size.X || x < 0 || z >= Size.Y || z < 0)
				return new BaseTile()
				{
					GridPosition = new Vector2I(x,z)
				};

			if (print)
				GD.Print(new Vector2(x,z));

			return tiles[z * Size.X + x];
		}
	}
}
