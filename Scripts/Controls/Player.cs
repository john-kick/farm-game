using Godot;
using Godot.Collections;
using FarmGame.Scripts.Controls.Interactions;
using FarmGame.Scripts.Tiles;

namespace FarmGame.Scripts.Controls
{
	public partial class Player : CharacterBody3D
	{
		[Export] public float Speed = 1.0f;
		[Export] public float FastMultiplier = 2.0f;
		[Export] public float JumpStrength = 10.0f;
		[Export] public float MouseSensitivity = .15f;
		[Export] public float Gravity = 0.3f;
		[Export] public float DoubleTapTime = 0.2f;
		[Export] public float MaxInteractDistance = 5.0f;

		private float jumpLastPressed = 0.0f;
		private Vector3 targetVelocity;
		private Camera3D camera;

		public override void _Ready()
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
			camera = GetNode<Camera3D>("Camera");
		}

		public override void _Input(InputEvent @event)
		{
			if (@event.IsActionPressed("ui_cancel"))
			{
				Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured
					? Input.MouseModeEnum.Visible
					: Input.MouseModeEnum.Captured;
			}

			if (Input.MouseMode != Input.MouseModeEnum.Captured)
				return;

			if (@event.IsActionPressed("ui_primary_action"))
				HandleInteraction();
			if (@event.IsActionPressed("ui_secondary_action"))
				HandleInteraction(InteractionType.Secondary);
			if (@event.IsActionPressed("ui_tertiary_action"))
				HandleInteraction(InteractionType.Tertiary);

			if (@event is InputEventMouseMotion mouseMotion)
			{
				RotationDegrees -= new Vector3(0, mouseMotion.Relative.X, 0) * MouseSensitivity;
				camera.RotationDegrees -= new Vector3(mouseMotion.Relative.Y, 0, 0) * MouseSensitivity;
				camera.RotationDegrees = new Vector3(Mathf.Clamp(camera.RotationDegrees.X, -89, 89), 0, 0);
			}
		}

		public override void _PhysicsProcess(double delta)
		{
			targetVelocity = Velocity;
			ApplyMovementInput();
			ApplyGravity(delta);
			Velocity = targetVelocity;
			Move();
		}

		private void ApplyMovementInput()
		{
			float speed = Speed;

			if (Input.IsActionPressed("ui_shift"))
				speed *= FastMultiplier;

			Vector3 direction = Vector3.Zero;
			if (Input.IsActionPressed("ui_up")) direction -= Transform.Basis.Z;
			if (Input.IsActionPressed("ui_down")) direction += Transform.Basis.Z;
			if (Input.IsActionPressed("ui_left")) direction -= Transform.Basis.X;
			if (Input.IsActionPressed("ui_right")) direction += Transform.Basis.X;

			if (direction != Vector3.Zero)
			{
				direction.Y = 0;
				direction = direction.Normalized();
			}

			targetVelocity.X = direction.X * speed;
			targetVelocity.Z = direction.Z * speed;

			if (Input.IsActionJustPressed("ui_jump") && IsOnFloor())
			{
				Jump();
			}
		}

		private void Jump()
		{
			targetVelocity.Y = JumpStrength;
		}

		private void ApplyGravity(double delta)
		{
			if (IsOnFloor())
			{
				if (targetVelocity.Y < 0)
					targetVelocity.Y = 0;

				return;
			}

			targetVelocity.Y -= Gravity * (float)delta;
		}

		private void Move()
		{
			MoveAndSlide();
		}

		private void HandleInteraction(InteractionType interactionType = InteractionType.Primary)
		{
			if (camera == null)
				return;

			Vector2 center = GetViewport().GetVisibleRect().Size / 2;
			Vector3 rayOrigin = camera.ProjectRayOrigin(center);
			Vector3 rayDirection = camera.ProjectRayNormal(center);
			Vector3 rayEnd = rayOrigin + rayDirection * MaxInteractDistance;

			PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
			PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd);
			query.CollideWithAreas = false;
			query.CollideWithBodies = true;

			// Exclude the player itself from the raycast
			query.Exclude = [GetRid()];
			Dictionary result = spaceState.IntersectRay(query);

			if (result.Count == 0 ||
				!result.TryGetValue("position", out Variant hitPositionVariant) ||
				!result.TryGetValue("collider", out Variant colliderVariant))
				return;

			if (colliderVariant.AsGodotObject() is not Node hitNode)
				return;

			Vector3 hitPosition = hitPositionVariant.AsVector3();
			IInteractable interactable = ResolveInteractable(hitNode, hitPosition);


			Interaction interaction = interactionType switch
			{
				InteractionType.Primary => interactable?.PrimaryInteraction(),
				InteractionType.Secondary => interactable?.SecondaryInteraction(),
				InteractionType.Tertiary => interactable?.TertiaryInteraction(),
				_ => null
			};

			interaction.Process();

			if (interaction is ReplaceTileInteraction replaceTileInteraction)
			{
				// Vector3 localHitPosition = field.ToLocal(hitPosition);
				// Vector2I gridPosition = field.WorldToGridPosition(localHitPosition);
				// Tile newTile = TileFactory.CreateTile(replaceTileInteraction.NewTileType);
				// field.AddTile(gridPosition, newTile);
				// field.Refresh();
			}
		}

		private IInteractable ResolveInteractable(Node hitNode, Vector3 hitPosition)
		{
			IInteractable interactable = FindInteractableInHierarchy(hitNode);
			if (interactable != null)
				return interactable;

			return null;

			// if (field == null || !IsFieldCollision(hitNode))
			// 	return null;

			// Vector3 localHitPosition = field.ToLocal(hitPosition);
			// Vector2I gridPosition = field.WorldToGridPosition(localHitPosition);
			// return field.GetTile(gridPosition);
		}

		private static IInteractable FindInteractableInHierarchy(Node node)
		{
			for (Node current = node; current != null; current = current.GetParent())
			{
				if (current is IInteractable interactable)
					return interactable;
			}

			return null;
		}

		private bool IsFieldCollision(Node node)
		{
			return false;
			// return field != null && (node == field || field.IsAncestorOf(node));
		}
	}
}
