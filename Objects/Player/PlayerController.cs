using Godot;
using System;
using System.Collections.Generic;

namespace SpaceEngineer
{
	public partial class PlayerController : CharacterBody3D
	{
		[Export] private Area3D interactArea;
		[Export] private Node3D visual;
		[Export] private Node3D itemVisualParent;


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
		private Node3D heldItemVisual;

		private List<Interactable> interactables;
		private Interactable targetInteractable;

		private bool isInteracting = false;
		private AnimationPlayer animationPlayer;

		public Vector3 DesiredVelocity { get; private set; }

		public PlayerController()
		{
			interactables = new List<Interactable>();
		}

		public override void _Ready()
		{
			interactArea.AreaEntered += OnInteractAreaEntered;
			interactArea.AreaExited += OnInteractAreaExited;

			if (visual is not null)
			{
				animationPlayer = visual.GetNode<AnimationPlayer>("AnimationPlayer");
				if (animationPlayer is not null)
				{
					animationPlayer.AnimationFinished += OnAnimationFinished;
				}
				else
				{
					GD.Print("failed to find an animation player on the player's visual");
				}
			}
			else
			{
				GD.Print("No character visual assigned to the player controller");
			}

			animationPlayer?.Play("Idle");
		}

		private void OnAnimationFinished(StringName animName)
		{
			GD.Print("Animation Finished " + animName);
			isInteracting = false;
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
			if (heldItemVisual is not null)
			{
				heldItemVisual.QueueFree();
				heldItemVisual = null;
			}

			HeldItem = item;

			heldItemVisual = HeldItem?.InstantiateVisual();
			if (heldItemVisual is not null)
			{
				itemVisualParent.AddChild(heldItemVisual);
				heldItemVisual.Position = Vector3.Zero;
				heldItemVisual.Rotation = Vector3.Zero;
			}
		}

		public Interactable GetTargetInteractable()
		{
			Interactable target = null;
			float targetWeight = 0f;

			foreach (var interactable in interactables)
			{
				if (!interactable.CanInteract(this))
				{
					continue;
				}

				var playerToTarget = interactable.GlobalPosition - GlobalPosition;
				var playerToTargetDirection = playerToTarget.Normalized();

				var playerToTargetDot = GlobalTransform.Basis.Z.Dot(playerToTargetDirection);
				// Convert dot value to a 0 to 1 range where closer to 1 is better.
				playerToTargetDot = (playerToTargetDot + 1f) / 2f;

				var playerToTargetDistance = playerToTarget.Length();
				// Convert distance to a 0 to 1 range where closer to 1 is better.
				playerToTargetDistance = (2f - Mathf.Clamp(playerToTargetDistance, 0f, 2f)) / 2f;

				var weight = playerToTargetDot + playerToTargetDistance;

				if (weight > targetWeight)
				{
					target = interactable;
					targetWeight = weight;
				}
			}

			return target;
		}

		public override void _PhysicsProcess(double delta)
		{
			Vector3 velocity = Velocity;

			if (Input.IsActionJustPressed(interactAction))
			{
				targetInteractable = GetTargetInteractable();
				targetInteractable?.StartInteract(this);

				isInteracting = true;
				animationPlayer?.Play("Interact");
			}

			if (Input.IsActionJustReleased(interactAction))
			{
				targetInteractable?.StopInteract(this);
				targetInteractable = null;

				// isInteracting = false;
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
			DesiredVelocity = velocity;
			
			MoveAndSlide();

			if (!isInteracting)
			{
				if (velocity.Length() > 0.1f)
				{
					animationPlayer?.Play("Walk");

				}
				else
				{
					animationPlayer?.Play("Idle");
				}
			}
		}
	}
}