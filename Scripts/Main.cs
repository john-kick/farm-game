using Godot;
using FarmGame.Tiles;

public partial class Main : Node3D
{
	[Export] public Vector2I Size;
	[Export] public Node3D Field;
	[Export] public Mesh.PrimitiveType PrimitiveType = Mesh.PrimitiveType.Triangles;

	private Tile[] Tiles;

	public override void _Ready()
	{
		if (Field == null)
		{
			Field = new Node3D();
			AddChild(Field);
		}

		GenerateRandomField();
		// GenerateTestField();
		RenderField();
		// GrassTile grassTile = GetNode<GrassTile>("Tiles/GrassTile");
		// grassTile.Render([]);
	}

	/// <summary>
	/// Generates random tiles and stores them in the `Tiles` array
	/// </summary>
	private void GenerateRandomField()
	{
		Tiles = new Tile[Size.X * Size.Y];

		for (int z = 0; z < Size.Y; z++)
		{
			for (int x = 0; x < Size.X; x++)
			{
				Tile tile = TileFactory.GetRandomTile();
				tile.GridPosition = new Vector2I(x, z);
				Tiles[z * Size.X + x] = tile;
			}
		}
	}

	private void GenerateTestField()
	{
		Size = new Vector2I(2, 2);
		Tiles = [
			new StoneTile() {GridPosition = new Vector2I(0,0)}, // top-left
			new DirtTile()  {GridPosition = new Vector2I(1,0)}, // top-right
			new DirtTile()  {GridPosition = new Vector2I(0,1)}, // bottom-left
			new StoneTile() {GridPosition = new Vector2I(1,1)}  // bottom-right
		];
	}

	/// <summary>
	/// Adds the tiles contained in the `Tiles` array to the Scene
	/// </summary>
	private void RenderField()
	{
		for (int z = 0; z < Size.Y; z++)
		{
			for (int x = 0; x < Size.X; x++)
			{
				Tile tile = GetTile(x, z);

				Neighbor<Tile>[] neighbors = [
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

				tile.Render(neighbors, PrimitiveType);
				Field.AddChild(tile);
			}
		}
	}

	private void GenerateCollisionBody()
	{
		for (int z = 0; z < Size.Y; z++)
		{
			for (int x = 0; x < Size.X; x++)
			{
				StaticBody3D staticBody = new();
				CollisionShape3D collisionShape3D = new();
			}
		}
	}

	private Tile GetTile(int x, int z)
	{
		if (x < 0 || z < 0 || x >= Size.X || z >= Size.Y)
			return null;

		return Tiles[z * Size.X + x];
	}
}
