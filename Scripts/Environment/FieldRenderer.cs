using FarmGame.Scripts.Tiles;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace FarmGame.Scripts.Environment
{
	public class FieldRenderer
	{
		private const string TerrainCollisionName = "TerrainCollision";

		private readonly Node3D fieldNode;
		private readonly Dictionary<TileType, MultiMeshInstance3D> tileRenderers = [];

		public FieldRenderer(Node3D fieldNode)
		{
			this.fieldNode = fieldNode;
		}

		public void RenderTiles(IEnumerable<Tile> tiles, float tileSize)
		{
			StaticBody3D body = new() { Name = TerrainCollisionName };
			List<IGrouping<TileType, Tile>> groupedTiles = [.. tiles.GroupBy(t => t.TileType)];

			foreach (IGrouping<TileType, Tile> group in groupedTiles)
				RenderTileGroup(group, body, tileSize);

			fieldNode.AddChild(body);
		}

		public void Clear()
		{
			foreach (MultiMeshInstance3D renderer in tileRenderers.Values)
				renderer.QueueFree();

			tileRenderers.Clear();
			fieldNode.GetNodeOrNull<StaticBody3D>(TerrainCollisionName)?.QueueFree();
		}

		private void RenderTileGroup(IGrouping<TileType, Tile> group, StaticBody3D body, float tileSize)
		{
			TileType tileType = group.Key;
			List<Tile> tilesOfType = [.. group];

			ArrayMesh mesh = tilesOfType[0].CreateMesh(tileSize);
			MultiMesh multiMesh = new()
			{
				Mesh = mesh,
				TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
				InstanceCount = tilesOfType.Count
			};

			for (int i = 0; i < tilesOfType.Count; i++)
			{
				Tile tile = tilesOfType[i];
				Vector3 worldPos = new(tile.GridPosition.X * tileSize, 0, tile.GridPosition.Y * tileSize);
				SetTileTransform(multiMesh, worldPos, i);
				AddCollisionShape(body, worldPos, mesh);
			}

			MultiMeshInstance3D meshInstance = new()
			{
				Multimesh = multiMesh,
				Name = $"{tileType}Group",
				Visible = true,
				CastShadow = GeometryInstance3D.ShadowCastingSetting.Off
			};
			fieldNode.AddChild(meshInstance);

			tileRenderers[tileType] = meshInstance;
		}

		private static void SetTileTransform(MultiMesh multiMesh, Vector3 position, int index)
		{
			multiMesh.SetInstanceTransform(index, new Transform3D(Basis.Identity, position));
		}

		private static void AddCollisionShape(StaticBody3D body, Vector3 position, ArrayMesh mesh)
		{
			CollisionShape3D collisionShape = new()
			{
				Shape = mesh.CreateTrimeshShape(),
				Position = position
			};
			body.AddChild(collisionShape);
		}
	}
}
