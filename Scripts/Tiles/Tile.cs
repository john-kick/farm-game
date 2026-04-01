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
		/// The material for this tile
		/// </summary>
		public abstract Material Material { get; }

		public Material MaterialOverride;

		/// <summary>
		/// Grid position of this tile
		/// </summary>
		[Export] public Vector2I GridPosition { get; set; }

		public ArrayMesh CreateMesh(float TileSize)
		{
			float halfSize = TileSize / 2f;
			SurfaceTool surfaceTool = new();
			surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
			Material material = MaterialOverride ?? Material;
			surfaceTool.SetMaterial(material);

			Vector3 bottomFrontLeft = new(-halfSize, 0, -halfSize);
			Vector3 bottomFrontRight = new(halfSize, 0, -halfSize);
			Vector3 bottomBackRight = new(halfSize, 0, halfSize);
			Vector3 bottomBackLeft = new(-halfSize, 0, halfSize);
			Vector3 topFrontLeft = new(-halfSize, Height, -halfSize);
			Vector3 topFrontRight = new(halfSize, Height, -halfSize);
			Vector3 topBackRight = new(halfSize, Height, halfSize);
			Vector3 topBackLeft = new(-halfSize, Height, halfSize);

			AddQuad(surfaceTool, topFrontLeft, topFrontRight, topBackRight, topBackLeft, Vector3.Up);
			AddQuad(surfaceTool, bottomFrontLeft, bottomFrontRight, topFrontRight, topFrontLeft, Vector3.Forward);
			AddQuad(surfaceTool, bottomBackRight, bottomBackLeft, topBackLeft, topBackRight, Vector3.Back);
			AddQuad(surfaceTool, bottomBackLeft, bottomFrontLeft, topFrontLeft, topBackLeft, Vector3.Left);
			AddQuad(surfaceTool, bottomFrontRight, bottomBackRight, topBackRight, topFrontRight, Vector3.Right);

			return surfaceTool.Commit();
		}

		private static void AddQuad(SurfaceTool surfaceTool, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 normal)
		{
			surfaceTool.SetNormal(normal);
			surfaceTool.SetUV(new Vector2(0, 0));
			surfaceTool.AddVertex(a);
			surfaceTool.SetNormal(normal);
			surfaceTool.SetUV(new Vector2(1, 0));
			surfaceTool.AddVertex(b);
			surfaceTool.SetNormal(normal);
			surfaceTool.SetUV(new Vector2(1, 1));
			surfaceTool.AddVertex(c);

			surfaceTool.SetNormal(normal);
			surfaceTool.SetUV(new Vector2(0, 0));
			surfaceTool.AddVertex(a);
			surfaceTool.SetNormal(normal);
			surfaceTool.SetUV(new Vector2(1, 1));
			surfaceTool.AddVertex(c);
			surfaceTool.SetNormal(normal);
			surfaceTool.SetUV(new Vector2(0, 1));
			surfaceTool.AddVertex(d);
		}
	}
}
