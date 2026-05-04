using Godot;
using FarmGame.Scripts.Environment;

namespace FarmGame.Scripts.UI;

public partial class Grid : Node3D
{
    [Export] public float GridLineWidth = 0.02f;
    [Export] public float GridHeight = 0.11f;

    public Color GridColor = new(1, 1, 1, 0.3f);
    private MeshInstance3D meshInstance;
    private Field field;

    public override void _Ready()
    {
        field = GetParent<Field>();
        if (field == null)
        {
            GD.PrintErr("Grid must be a child of a Field node");
            return;
        }

        // Create mesh instance
        meshInstance = new MeshInstance3D();
        AddChild(meshInstance);

        DrawGrid();
    }

    public void DrawGrid()
    {
        if (field == null) return;

        var immediateMesh = new ImmediateMesh();
        var material = new StandardMaterial3D
        {
            AlbedoColor = GridColor,
            DisableReceiveShadows = true,
            NoDepthTest = false,
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha
        };

        immediateMesh.SurfaceBegin(Mesh.PrimitiveType.Lines, material);

        float tileSize = Field.TILE_SIZE;
        int width = field.Width;
        int height = field.Height;

        // Draw vertical lines (parallel to Z-axis)
        for (int x = 0; x <= width; x++)
        {
            Vector3 start = new(x * tileSize, GridHeight, 0);
            Vector3 end = new(x * tileSize, GridHeight, height * tileSize);

            immediateMesh.SurfaceAddVertex(start);
            immediateMesh.SurfaceAddVertex(end);
        }

        // Draw horizontal lines (parallel to X-axis)
        for (int z = 0; z <= height; z++)
        {
            Vector3 start = new(0, GridHeight, z * tileSize);
            Vector3 end = new(width * tileSize, GridHeight, z * tileSize);

            immediateMesh.SurfaceAddVertex(start);
            immediateMesh.SurfaceAddVertex(end);
        }

        immediateMesh.SurfaceEnd();

        meshInstance.Mesh = immediateMesh;
    }
}