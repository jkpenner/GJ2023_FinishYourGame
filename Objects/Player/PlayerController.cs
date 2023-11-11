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

		private Item heldItem;
		private List<ItemSlot> interactables;

		public PlayerController()
		{
			interactables = new List<ItemSlot>();
		}

		public override void _Ready()
		{
			interactArea.AreaEntered += OnInteractAreaEntered;
			interactArea.AreaExited += OnInteractAreaExited;
		}

		private void OnInteractAreaEntered(Area3D area)
		{
			if (area is ItemSlot interactable)
			{
				GD.Print($"{area.Name} entered the interact area.");
				interactables.Add(interactable);
			}
		}


		private void OnInteractAreaExited(Area3D area)
		{
			if (area is ItemSlot interactable)
			{
				GD.Print($"{area.Name} exited the interact area.");
				interactables.Remove(interactable);
			}
		}

		public ItemSlot GetTargetInteractable()
		{
			ItemSlot target = null;
			float targetDot = -1;

			foreach (var interactable in interactables)
			{
				if (!interactable.IsInteractable)
				{
					GD.Print($"Skipping {interactable.Name} as it is not interactable.");
					continue;
				}

				var playerToTarget = (interactable.GlobalPosition - GlobalPosition).Normalized();
				var playerToTargetDot = (GlobalTransform.Basis.Z).Dot(playerToTarget);

				GD.Print($"{playerToTargetDot} > {targetDot}?");
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
					if (heldItem is not null)
					{
						if (interactable.TryPlaceObject(heldItem))
						{
							heldItem = null;
						}
						else
						{
							GD.Print("Failed to place item");
						}
					}
					else
					{
						if (interactable.TryTakeObject(out var item))
						{
							heldItem = item;
						}
						else
						{
							GD.Print("Failed to take item");
						}
					}
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