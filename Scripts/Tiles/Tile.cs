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
		/// The material for this tile
		/// </summary>
		public abstract Material Material { get; }

		/// <summary>
		/// Reference to the Field this tile is on, if any
		/// </summary>
		public Field Field = field;

		/// <summary>
		/// Grid position of this tile
		/// </summary>
		[Export] public Vector2I GridPosition { get; set; }

		public ArrayMesh CreateMesh(float TileSize)
		{
			SurfaceTool surfaceTool = new();
			surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
			surfaceTool.SetMaterial(Material);

			Vector3 bottomFrontLeft = new(0, 0, 0);
			Vector3 bottomFrontRight = new(TileSize, 0, 0);
			Vector3 bottomBackRight = new(TileSize, 0, TileSize);
			Vector3 bottomBackLeft = new(0, 0, TileSize);
			Vector3 topFrontLeft = new(0, Height, 0);
			Vector3 topFrontRight = new(TileSize, Height, 0);
			Vector3 topBackRight = new(TileSize, Height, TileSize);
			Vector3 topBackLeft = new(0, Height, TileSize);

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

		protected ReplaceTileInteraction CreateReplaceTileInteraction(TileType newTileType)
		{
			return new ReplaceTileInteraction(newTileType, Field, GridPosition);
		}

		public virtual Interaction PrimaryInteraction() => new NoInteraction();
		public virtual Interaction SecondaryInteraction() => new NoInteraction();
		public virtual Interaction TertiaryInteraction() => new NoInteraction();
	}
}
