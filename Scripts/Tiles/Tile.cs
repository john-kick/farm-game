using System;
using System.Collections.Generic;
using Godot;

namespace FarmGame.Tiles
{
	public abstract partial class Tile : MeshInstance3D
	{
		public Vector2I GridPosition;

		public Tile()
		{ }

		public Tile(Vector2I gridPosition)
		{
			GridPosition = gridPosition;
		}

		public override void _Ready()
		{
			ApplyMaterial();
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
			Mesh = mesh;
		}

		private void ApplyMaterial()
		{
			MaterialOverride = GetMaterial();
		}

		private static void AddQuad(
			List<Vector3> vertices,
			List<Vector3> normals,
			List<Vector2> uvs,
			List<int> indices,
			Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3
		)
		{
			int start = vertices.Count;

			vertices.Add(v0);
			vertices.Add(v1);
			vertices.Add(v2);
			vertices.Add(v3);

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

		private void AddTopQuad(
			List<Vector3> vertices,
			List<Vector3> normals,
			List<Vector2> uvs,
			List<int> indices
		)
		{
			float height = GetHeight();

			AddQuad(
				vertices, normals, uvs, indices,
				new Vector3(0 + GridPosition.X, height, 0 + GridPosition.Y),
				new Vector3(0 + GridPosition.X, height, 1 + GridPosition.Y),
				new Vector3(1 + GridPosition.X, height, 0 + GridPosition.Y),
				new Vector3(1 + GridPosition.X, height, 1 + GridPosition.Y)
			);
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
			GD.Print($"[SELF] Position: {GridPosition}, Type: {GetTileType()}");
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

				GD.Print($"[NEIGHBOR] Position: {neighbor.Element.GridPosition}, Type: {neighbor.Element.GetTileType()}, Offset: {neighbor.Offset}, ox: {ox}, oz: {oz}");

                Vector3 v0, v1, v2, v3;

                if (ox == 1) // right
                {
                    v0 = new Vector3(GridPosition.X + 1, h, GridPosition.Y);
                    v1 = new Vector3(GridPosition.X + 1, h, GridPosition.Y + 1);
                    v2 = new Vector3(GridPosition.X + 1, nh, GridPosition.Y);
                    v3 = new Vector3(GridPosition.X + 1, nh, GridPosition.Y + 1);
                }
                else if (ox == -1) // left
                {
                    v0 = new Vector3(GridPosition.X, h, GridPosition.Y + 1);
                    v1 = new Vector3(GridPosition.X, h, GridPosition.Y);
                    v2 = new Vector3(GridPosition.X, nh, GridPosition.Y + 1);
                    v3 = new Vector3(GridPosition.X, nh, GridPosition.Y);
                }
                else if (oz == 1) // bottom
                {
                    v0 = new Vector3(GridPosition.X + 1, h, GridPosition.Y + 1);
                    v1 = new Vector3(GridPosition.X, h, GridPosition.Y + 1);
                    v2 = new Vector3(GridPosition.X + 1, nh, GridPosition.Y + 1);
                    v3 = new Vector3(GridPosition.X, nh, GridPosition.Y + 1);
                }
                else // top (oz == -1)
                {
                    v0 = new Vector3(GridPosition.X, h, GridPosition.Y);
                    v1 = new Vector3(GridPosition.X + 1, h, GridPosition.Y);
                    v2 = new Vector3(GridPosition.X, nh, GridPosition.Y);
                    v3 = new Vector3(GridPosition.X + 1, nh, GridPosition.Y);
                }

                AddQuad(vertices, normals, uvs, indices, v0, v1, v2, v3);
			}
		}

		public static PackedScene GetScene()
		{
			throw new Exception("Called from base class");
		}

		protected virtual float GetHeight()
		{
			return 1;
		}

		public abstract Material GetMaterial();
		public abstract TileType GetTileType();
	}
}
