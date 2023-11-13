using Godot;
using System.Collections.Generic;

namespace SpaceEngineer
{
	public partial class PlayerController : CharacterBody3D
	{
		[Export] private Area3D interactArea;


		[ExportGroup("Inputs")]
		[Export] private string moveForwardAction = "P1MoveForward";
		[Export] private string moveBackAction = "P1MoveBack";
		[Export] private string moveLeftAction = "P1MoveLeft";
		[Export] private string moveRightAction = "P1MoveRight";
		[Export] private string interactAction = "P1Interact";
		[Export] private string dashAction = "P1Dash";

		public const float Speed = 5.0f;
		public const float JumpVelocity = 4.5f;

		public Item HeldItem { get; private set; }
		private List<Interactable> interactables;

		public PlayerController()
		{
			interactables = new List<Interactable>();
		}

		public override void _Ready()
		{
			interactArea.AreaEntered += OnInteractAreaEntered;
			interactArea.AreaExited += OnInteractAreaExited;
		}

		private void OnInteractAreaEntered(Area3D area)
		{
			if (area is Interactable interactable)
			{
				interactables.Add(interactable);
			}
		}


		private void OnInteractAreaExited(Area3D area)
		{
			if (area is Interactable interactable)
			{
				interactables.Remove(interactable);
			}
		}

		public void SetHeldItem(Item item)
		{
			HeldItem = item;
		}

		public Interactable GetTargetInteractable()
		{
			Interactable target = null;
			float targetDot = -1;

			foreach (var interactable in interactables)
			{
				if (!interactable.IsInteractable)
				{
					continue;
				}

				var playerToTarget = (interactable.GlobalPosition - GlobalPosition).Normalized();
				var playerToTargetDot = GlobalTransform.Basis.Z.Dot(playerToTarget);

				if (playerToTargetDot > targetDot)
				{
					target = interactable;
					targetDot = playerToTargetDot;
				}
			}

			return target;
		}

		public override void _PhysicsProcess(double delta)
		{
			Vector3 velocity = Velocity;

			// Handle player interaction.
			if (Input.IsActionJustPressed(interactAction) && interactables.Count > 0)
			{
				var interactable = GetTargetInteractable();
				if (interactable?.IsInteractable ?? false)
				{
					interactable.Interact(this);
				}
			}

			// Get the input direction and handle the movement/deceleration.
			// As good practice, you should replace UI actions with custom gameplay actions.
			Vector2 inputDir = Input.GetVector(moveLeftAction, moveRightAction, moveForwardAction, moveBackAction);
			if (inputDir != Vector2.Zero)
			{
				var velY = velocity.Y;
				velocity = Basis.Z * Speed * inputDir.Length();
				velocity.Y = velY;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			}

			// Rotate the player in the direction of the input.
			if (inputDir.Length() > 0)
			{
				GlobalTransform = GlobalTransform.LookingAt(GlobalPosition - new Vector3(inputDir.X, 0, inputDir.Y), Vector3.Up);
			}

			Velocity = velocity;
			MoveAndSlide();
		}
	}
}