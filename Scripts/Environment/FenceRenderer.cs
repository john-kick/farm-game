using FarmGame.Scripts.Tiles;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace FarmGame.Scripts.Environment
{
	public class FenceRenderer
	{
		private const string FenceCollisionName = "FenceCollision";

		private const float FencePostWidth = 0.15f;
		private const float FencePostHeight = 0.8f;
		private const float FencePostDepth = 0.15f;

		private readonly Node3D fieldNode;
		private MultiMeshInstance3D fenceInstance;

		public FenceRenderer(Node3D fieldNode)
		{
			this.fieldNode = fieldNode;
		}

		public void RenderFences(IEnumerable<Tile> tiles, float tileSize)
		{
			List<Tile> edgeTiles = [.. tiles.Where(t => t.TileType == TileType.Edge)];
			if (edgeTiles.Count == 0)
				return;

			float tileTopY = edgeTiles[0].Height;
			ArrayMesh fenceMesh = CreateFenceMesh();

			MultiMesh multiMesh = new()
			{
				Mesh = fenceMesh,
				TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
				InstanceCount = edgeTiles.Count
			};

			StaticBody3D fenceBody = new() { Name = FenceCollisionName };

			for (int i = 0; i < edgeTiles.Count; i++)
			{
				Tile tile = edgeTiles[i];
				Vector3 worldPos = new(tile.GridPosition.X * tileSize, tileTopY, tile.GridPosition.Y * tileSize);
				multiMesh.SetInstanceTransform(i, new Transform3D(Basis.Identity, worldPos));
				AddFenceCollision(fenceBody, worldPos, tileSize);
			}

			fenceInstance = new MultiMeshInstance3D()
			{
				Multimesh = multiMesh,
				Name = "FenceGroup",
				Visible = true,
				CastShadow = GeometryInstance3D.ShadowCastingSetting.Off
			};

			fieldNode.AddChild(fenceInstance);
			fieldNode.AddChild(fenceBody);
		}

		public void Clear()
		{
			fenceInstance?.QueueFree();
			fenceInstance = null;
			fieldNode.GetNodeOrNull<StaticBody3D>(FenceCollisionName)?.QueueFree();
		}

		private static ArrayMesh CreateFenceMesh()
		{
			float halfW = FencePostWidth / 2f;
			float halfD = FencePostDepth / 2f;

			SurfaceTool surfaceTool = new();
			surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
			surfaceTool.SetMaterial(new StandardMaterial3D() { AlbedoColor = new Color(0.55f, 0.35f, 0.1f) });

			Vector3 blf = new(-halfW, 0, -halfD);
			Vector3 brf = new(halfW, 0, -halfD);
			Vector3 brb = new(halfW, 0, halfD);
			Vector3 blb = new(-halfW, 0, halfD);
			Vector3 tlf = new(-halfW, FencePostHeight, -halfD);
			Vector3 trf = new(halfW, FencePostHeight, -halfD);
			Vector3 trb = new(halfW, FencePostHeight, halfD);
			Vector3 tlb = new(-halfW, FencePostHeight, halfD);

			AddQuad(surfaceTool, tlf, trf, trb, tlb, Vector3.Up);
			AddQuad(surfaceTool, blf, brf, trf, tlf, Vector3.Forward);
			AddQuad(surfaceTool, brb, blb, tlb, trb, Vector3.Back);
			AddQuad(surfaceTool, blb, blf, tlf, tlb, Vector3.Left);
			AddQuad(surfaceTool, brf, brb, trb, trf, Vector3.Right);

			return surfaceTool.Commit();
		}

		private static void AddFenceCollision(StaticBody3D body, Vector3 position, float tileSize)
		{
			BoxShape3D box = new()
			{
				Size = new Vector3(tileSize, FencePostHeight, tileSize)
			};

			CollisionShape3D shape = new()
			{
				Shape = box,
				Position = position + new Vector3(0, FencePostHeight / 2f, 0)
			};

			body.AddChild(shape);
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
