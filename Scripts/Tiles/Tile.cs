using System;
using System.Collections.Generic;
using Godot;

namespace FarmGame.Scripts.Tiles
{
	public abstract partial class Tile : StaticBody3D
	{
		[Export] public Vector2I GridPosition;

		protected MeshInstance3D meshInstance;
		protected CollisionShape3D collisionShape;

		public Tile()
		{ }

		public Tile(Vector2I gridPosition)
		{
			GridPosition = gridPosition;
		}

        public override void _EnterTree()
        {
        }

		public override void _Ready()
		{
			meshInstance = GetNode<MeshInstance3D>("Mesh");
			collisionShape = GetNode<CollisionShape3D>("CollisionShape");
		}

		private void ApplyMaterial()
		{
			meshInstance.MaterialOverride = GetMaterial();
		}

		public void Render(Neighbor<Tile>[] neighbors, Mesh.PrimitiveType primitiveType = Mesh.PrimitiveType.Triangles)
		{

			ArrayMesh mesh = new();

			List<Vector3> vertices = [];
			List<Vector3> normals = [];
			List<Vector2> uvs = [];
			List<int> indices = [];

			AddTopQuad(vertices, normals, uvs, indices);
			FillGapsBetweenTiles(vertices, normals, uvs, indices, neighbors);

			Godot.Collections.Array arrays = [];
			arrays.Resize((int)Mesh.ArrayType.Max);
			arrays[(int)Mesh.ArrayType.Vertex] = vertices.ToArray();
			arrays[(int)Mesh.ArrayType.Normal] = normals.ToArray();
			arrays[(int)Mesh.ArrayType.TexUV] = uvs.ToArray();
			arrays[(int)Mesh.ArrayType.Index] = indices.ToArray();

			mesh.AddSurfaceFromArrays(primitiveType, arrays);
			meshInstance.Mesh = mesh;
			GenerateCollisionShape();
			ApplyMaterial();
		}

		private void AddTopQuad(
			List<Vector3> vertices,
			List<Vector3> normals,
			List<Vector2> uvs,
			List<int> indices
		)
		{
			float height = GetHeight();

			int start = vertices.Count;
			
			vertices.Add(new Vector3(0 + GridPosition.X, height, 0 + GridPosition.Y));
			vertices.Add(new Vector3(0 + GridPosition.X, height, 1 + GridPosition.Y));
			vertices.Add(new Vector3(1 + GridPosition.X, height, 0 + GridPosition.Y));
			vertices.Add(new Vector3(1 + GridPosition.X, height, 1 + GridPosition.Y));

			normals.Add(Vector3.Up);			
			normals.Add(Vector3.Up);			
			normals.Add(Vector3.Up);			
			normals.Add(Vector3.Up);

			uvs.Add(Vector2.Zero);			
			uvs.Add(Vector2.Zero);			
			uvs.Add(Vector2.Zero);			
			uvs.Add(Vector2.Zero);

			indices.Add(start + 0);
			indices.Add(start + 3);
			indices.Add(start + 1);

			indices.Add(start + 0);
			indices.Add(start + 2);
			indices.Add(start + 3);
		}

		/// <summary>
		/// Fills the gaps between tiles with quads. The new quad will have the same color as the higher tile.
		/// </summary>
		private void FillGapsBetweenTiles(
			List<Vector3> vertices,
			List<Vector3> normals,
			List<Vector2> uvs,
			List<int> indices,
			Neighbor<Tile>[] neighbors
		)
		{

			foreach (Neighbor<Tile> neighbor in neighbors)
			{
				if (neighbor.Element == null)
					continue;

				float h = GetHeight();
				float nh = neighbor.Element.GetHeight();

				if (h <= nh)
					continue;

				int ox = neighbor.Offset.X;
				int oz = neighbor.Offset.Y;

				Vector3 v0, v1, v2, v3, normal;

				if (ox == 1) // right
				{
					v0 = new Vector3(GridPosition.X + 1, h, GridPosition.Y);
					v1 = new Vector3(GridPosition.X + 1, h, GridPosition.Y + 1);
					v2 = new Vector3(GridPosition.X + 1, nh, GridPosition.Y);
					v3 = new Vector3(GridPosition.X + 1, nh, GridPosition.Y + 1);
					normal = new Vector3(1, 0, 0);
				}
				else if (ox == -1) // left
				{
					v0 = new Vector3(GridPosition.X, h, GridPosition.Y + 1);
					v1 = new Vector3(GridPosition.X, h, GridPosition.Y);
					v2 = new Vector3(GridPosition.X, nh, GridPosition.Y + 1);
					v3 = new Vector3(GridPosition.X, nh, GridPosition.Y);
					normal = new Vector3(-1, 0, 0);
				}
				else if (oz == 1) // bottom
				{
					v0 = new Vector3(GridPosition.X + 1, h, GridPosition.Y + 1);
					v1 = new Vector3(GridPosition.X, h, GridPosition.Y + 1);
					v2 = new Vector3(GridPosition.X + 1, nh, GridPosition.Y + 1);
					v3 = new Vector3(GridPosition.X, nh, GridPosition.Y + 1);
					normal = new Vector3(0, 0, 1);
				}
				else // top (oz == -1)
				{
					v0 = new Vector3(GridPosition.X, h, GridPosition.Y);
					v1 = new Vector3(GridPosition.X + 1, h, GridPosition.Y);
					v2 = new Vector3(GridPosition.X, nh, GridPosition.Y);
					v3 = new Vector3(GridPosition.X + 1, nh, GridPosition.Y);
					normal = new Vector3(0, 0, -1);
				}

				int start = vertices.Count;

				vertices.Add(v0);
				vertices.Add(v1);
				vertices.Add(v2);
				vertices.Add(v3);

				normals.Add(normal);
				normals.Add(normal);
				normals.Add(normal);
				normals.Add(normal);

				uvs.Add(Vector2.Zero);
				uvs.Add(Vector2.Zero);
				uvs.Add(Vector2.Zero);
				uvs.Add(Vector2.Zero);

				indices.Add(start + 0);
				indices.Add(start + 3);
				indices.Add(start + 1);

				indices.Add(start + 0);
				indices.Add(start + 2);
				indices.Add(start + 3);
			}
		}

		private void GenerateCollisionShape()
		{
			collisionShape.Shape = meshInstance.Mesh.CreateTrimeshShape();
		}

		public static PackedScene GetScene()
		{
			throw new Exception("Called from base class");
		}

		public virtual void HandleClick()
		{
			// Do nothing
		}

		protected virtual float GetHeight()
		{
			return 1;
		}

		public abstract Material GetMaterial();
		public abstract TileType GetTileType();
	}
}
