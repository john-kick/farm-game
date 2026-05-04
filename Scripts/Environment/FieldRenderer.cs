using System.Collections.Generic;
using FarmGame.Scripts.Tiles;
using Godot;

namespace FarmGame.Scripts.Environment;

public class FieldRenderer
{
	private readonly Dictionary<byte, (PackedScene, float)> _tileMap = [];
	private readonly Dictionary<Vector2I, Node3D> _rTiles = [];
	private readonly Field _field;

	public FieldRenderer(Field field)
	{
		_field = field;
		LoadTileScenes();
	}

	/// <summary>
	/// (Re-)loads all render tiles of the field
	/// </summary>
	public void Update()
	{
		int width = _field.Width + 1;
		int height = _field.Height + 1;

		for (int x = 0; x < width; x++)
			for (int z = 0; z < height; z++)
				UpdateRenderTile(new Vector2I(x, z));
	}

	public void UpdateSurroundingTiles(Vector2I GridPosition)
	{
		// Update has been requested for the logical tile at GridPosition.
		// Update the surrounding render tiles.

		UpdateRenderTile(GridPosition);
		UpdateRenderTile(GridPosition + new Vector2I(0, 1));
		UpdateRenderTile(GridPosition + new Vector2I(1, 1));
		UpdateRenderTile(GridPosition + new Vector2I(1, 0));
	}

	public void UpdateRenderTile(Vector2I GridPosition)
	{
		byte mask = GetNeighborMask(GridPosition);
		var (scene, rotation) = _tileMap[mask];

		Node3D tile = scene.Instantiate<Node3D>();
		tile.Rotation = new Vector3(0, rotation, 0);
		tile.Position = new Vector3(GridPosition.X, 0, GridPosition.Y);

		if (_rTiles.TryGetValue(GridPosition, out Node3D oldTile))
			oldTile.QueueFree();

		_field.AddChild(tile);
		_rTiles[GridPosition] = tile;
	}

	private byte GetNeighborMask(Vector2I GridPosition)
	{
		byte mask = 0;

		Tile ne = _field.GetTile(GridPosition);
		Tile se = _field.GetTile(GridPosition + new Vector2I(0, -1));
		Tile sw = _field.GetTile(GridPosition + new Vector2I(-1, -1));
		Tile nw = _field.GetTile(GridPosition + new Vector2I(-1, 0));

		if (ne == null || ne.TileType == TileType.Grass)
			mask |= 0b0001;

		if (se == null || se.TileType == TileType.Grass)
			mask |= 0b0010;

		if (sw == null || sw.TileType == TileType.Grass)
			mask |= 0b0100;

		if (nw == null || nw.TileType == TileType.Grass)
			mask |= 0b1000;

		return mask;
	}

	private void LoadTileScenes()
	{
		PackedScene emtpy = GD.Load<PackedScene>("res://Scenes/Tiles/empty.tscn");
		PackedScene convex = GD.Load<PackedScene>("res://Scenes/Tiles/convex.tscn");
		PackedScene half = GD.Load<PackedScene>("res://Scenes/Tiles/half.tscn");
		PackedScene bridge = GD.Load<PackedScene>("res://Scenes/Tiles/bridge.tscn");
		PackedScene concave = GD.Load<PackedScene>("res://Scenes/Tiles/concave.tscn");
		PackedScene full = GD.Load<PackedScene>("res://Scenes/Tiles/full.tscn");

		float R0 = 0f;                   //   0°
		float R90 = Mathf.Pi / 2f;       //  90°
		float R180 = Mathf.Pi;           // 180°
		float R270 = 3f * Mathf.Pi / 2f; // 270°

		// LSB -> MSB: NE -> SE -> SW -> NW
		_tileMap[0b0000] = (emtpy, R0);
		_tileMap[0b0001] = (convex, R270);
		_tileMap[0b0010] = (convex, R0);
		_tileMap[0b0011] = (half, R270);

		_tileMap[0b0100] = (convex, R90);
		_tileMap[0b0101] = (bridge, R90);
		_tileMap[0b0110] = (half, R0);
		_tileMap[0b0111] = (concave, R0);

		_tileMap[0b1000] = (convex, R180);
		_tileMap[0b1001] = (half, R180);
		_tileMap[0b1010] = (bridge, R0);
		_tileMap[0b1011] = (concave, R270);

		_tileMap[0b1100] = (half, R90);
		_tileMap[0b1101] = (concave, R180);
		_tileMap[0b1110] = (concave, R90);
		_tileMap[0b1111] = (full, R0);
	}
}
