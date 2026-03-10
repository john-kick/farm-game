using Godot;
using Godot.Collections;

namespace Scripts.Tests
{
    public partial class RayTest : Node3D
    {
        private Camera3D camera;

        public override void _Ready()
        {
            camera = GetNode<Camera3D>("Camera");
        }

        public override void _PhysicsProcess(double delta)
        {
            var spaceState = GetWorld3D().DirectSpaceState;

            Vector2 center = GetViewport().GetVisibleRect().Size / 2;

            Vector3 from = camera.ProjectRayOrigin(center);
            Vector3 dir = camera.ProjectRayNormal(center);
            Vector3 to = from + dir * 1000f;

            var query = PhysicsRayQueryParameters3D.Create(from, to);
            Dictionary result = spaceState.IntersectRay(query);

            if (result.Count > 0)
            {
                GD.Print("Hit at point: ", result["position"]);
            }
        }
    }
}