using Godot;
using System;
using System.Collections.Generic;

namespace SpaceEngineer
{
	public enum PlayerState
	{
		Normal,
		Dash,
		Knockback,
		Frozen,
	}

	public partial class PlayerController : CharacterBody3D
	{
		public const string PLAYER_VISUAL_NODE_PATH = "Hamster";
		public const string BALL_VISUAL_NODE_PATH = "HamsterBall";
		public const string INTERACT_NODE_PATH = "InteractArea";
		public const string ITEM_VISUAL_NODE_PATH = "ItemVisualParent";

		[Export] float moveSpeed = 5f;

		[ExportGroup("Dash")]
		[Export] float dashTurnRate = 5f;
		[Export] float dashSpeed = 10f;
		[Export] bool dropItemOnDash = false;

		[ExportGroup("Knockback")]
		[Export] float knockbackDuration = 2f;
		[Export] float knockbackDirChangeTime = 0.25f;
		[Export] float knockbackSpeed = 15f;
		[Export] float knockbackTurnRate = 10f;
		[Export] bool dropItemOnKnockback = true;

		[ExportGroup("Inputs")]
		[Export] private string moveForwardAction = "P1MoveForward";
		[Export] private string moveBackAction = "P1MoveBack";
		[Export] private string moveLeftAction = "P1MoveLeft";
		[Export] private string moveRightAction = "P1MoveRight";
		[Export] private string interactAction = "P1Interact";
		[Export] private string dashAction = "P1Dash";

		// Scene references
		private Node3D playerVisual;
		private Node3D ballVisual;
		private Area3D interactArea;
		private Node3D itemVisualParent;
		private Node3D heldItemVisual;
		private AnimationPlayer playerAnimationPlayer;
		private AnimationPlayer ballAnimationPlayer;

		// Interactions
		private bool isInteracting = false;
		private List<Interactable> interactables;
		private Interactable targetInteractable;

		// Knockback
		private bool isOnWall = false;
		private Random rand = new Random();
		private float knockbackCounter;
		private float knockbackDirChangeCounter;

		public PlayerState State { get; private set; }
		public Item HeldItem { get; private set; }
		public Vector3 DesiredVelocity { get; private set; }
		public Vector3 DesiredDirection { get; private set; }

		public PlayerController()
		{
			State = PlayerState.Normal;
			interactables = new List<Interactable>();
		}

		public override void _Ready()
		{
			FetchAndValidateSceneNodes();

			playerVisual.Show();
			ballVisual.Hide();

			playerAnimationPlayer?.Play("Idle");
		}

		private void FetchAndValidateSceneNodes()
		{
			playerVisual = GetNode<Node3D>(PLAYER_VISUAL_NODE_PATH);
			if (playerVisual is not null)
			{
				playerAnimationPlayer = playerVisual.GetNode<AnimationPlayer>("AnimationPlayer");
				if (playerAnimationPlayer is not null)
				{
					playerAnimationPlayer.AnimationFinished += OnAnimationFinished;
				}
				else
				{
					playerVisual.PrintMissingChildError("AnimationPlayer", nameof(AnimationPlayer));
				}
			}
			else
			{
				this.PrintMissingChildError(PLAYER_VISUAL_NODE_PATH, nameof(Node3D));
			}

			ballVisual = GetNode<Node3D>(BALL_VISUAL_NODE_PATH);
			if (ballVisual is not null)
			{
				ballAnimationPlayer = ballVisual.GetNode<AnimationPlayer>("AnimationPlayer");
				if (ballAnimationPlayer is null)
				{
					ballVisual.PrintMissingChildError("AnimationPlayer", nameof(AnimationPlayer));
				}
			}
			else
			{
				this.PrintMissingChildError(BALL_VISUAL_NODE_PATH, nameof(Node3D));
			}

			interactArea = GetNode<Area3D>(INTERACT_NODE_PATH);
			if (interactArea is not null)
			{
				interactArea.AreaEntered += OnInteractAreaEntered;
				interactArea.AreaExited += OnInteractAreaExited;
			}
			else
			{
				this.PrintMissingChildError(INTERACT_NODE_PATH, nameof(Area3D));
			}

			itemVisualParent = GetNode<Node3D>(ITEM_VISUAL_NODE_PATH);
			if (itemVisualParent is null)
			{
				this.PrintMissingChildError(ITEM_VISUAL_NODE_PATH, nameof(Node3D));
			}
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

		private void OnAnimationFinished(StringName animName)
		{
			if (animName == "Interact")
			{
				isInteracting = false;
			}
			else if (animName == "Tuck" && State == PlayerState.Dash)
			{
				playerVisual.Hide();
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

		private void SetTargetInteractable(Interactable interactable)
		{
			if (targetInteractable == interactable)
			{
				return;
			}

			targetInteractable = interactable;
			GameEvents.PlayerTargetChanged.Emit(targetInteractable);
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

		private void SetState(PlayerState state)
		{
			if (State == state)
			{
				return;
			}

			State = state;
		}

		public override void _Process(double delta)
		{
			SetTargetInteractable(GetTargetInteractable());

			switch (State)
			{
				case PlayerState.Normal:
					ProcessNormalState(delta);
					break;
				case PlayerState.Dash:
					ProcessDashState(delta);
					break;
				case PlayerState.Knockback:
					ProcessKnockbackState(delta);
					break;
			}
		}

		private void ProcessNormalState(double delta)
		{
			if (targetInteractable is not null)
			{
				if (Input.IsActionJustPressed(interactAction))
				{
					StartInteraction();
				}

				if (Input.IsActionJustReleased(interactAction))
				{
					StopInteraction();
				}
			}

			if (Input.IsActionJustPressed(dashAction))
			{
				StartDash();
				return;
			}

			DesiredVelocity = GetInputDirection() * moveSpeed;
			if (DesiredVelocity != Vector3.Zero)
			{
				// DesiredVelocity = GlobalTransform.Basis.Z * GetInputDirection().Length();
				DesiredVelocity = new Vector3(DesiredVelocity.X, Velocity.Y, DesiredVelocity.Z);
			}
			else
			{
				DesiredVelocity = new Vector3(
					Mathf.MoveToward(DesiredVelocity.X, 0f, moveSpeed),
					0f,
					Mathf.MoveToward(DesiredVelocity.Z, 0f, moveSpeed)
				);
			}

			// Rotate the player in the direction of the input.
			if (DesiredVelocity.Length() > 0)
			{
				GlobalTransform = GlobalTransform.LookingAt(GlobalPosition - DesiredVelocity, Vector3.Up);
			}

			Velocity = DesiredVelocity;

			MoveAndSlide();

			if (!isInteracting)
			{
				if (Velocity.Length() > 0.1f)
				{
					playerAnimationPlayer?.Play("Walk");

				}
				else
				{
					playerAnimationPlayer?.Play("Idle");
				}
			}
		}

		private void ProcessDashState(double delta)
		{
			if (Input.IsActionJustReleased(dashAction))
			{
				StopDash();
				return;
			}

			var input = GetInputDirection();
			if (input != Vector3.Zero)
			{
				DesiredVelocity = GlobalTransform.Basis.Z * input.Length() * dashSpeed;
			}
			else
			{
				DesiredVelocity = new Vector3(
					Mathf.MoveToward(DesiredVelocity.X, 0f, moveSpeed),
					-8f,
					Mathf.MoveToward(DesiredVelocity.Z, 0f, moveSpeed)
				);
			}

			// Rotate the player in the direction of the input.
			if (DesiredVelocity.Length() > 0 && input != Vector3.Zero)
			{
				Vector3 forwardDirection;
				var desiredDirection = input.Normalized();

				/// There's a known issue with the Slerp function in Godot, where it can throw an error when the two input vectors are 
				/// collinear (i.e., they point in the same direction or opposite directions). This is because the Slerp function uses 
				/// the cross product of the two vectors to determine the rotation axis, and the cross product of two collinear vectors 
				/// is a zero vector, which cannot be normalized 
				if (desiredDirection.IsEqualApprox(GlobalTransform.Basis.Z))
				{
					forwardDirection = desiredDirection;
				}
				else if (desiredDirection.IsEqualApprox(-GlobalTransform.Basis.Z))
				{
					forwardDirection = (desiredDirection.Cross(Vector3.Up) * 0.01f + desiredDirection).Normalized();
				}
				else
				{
					forwardDirection = GlobalTransform.Basis.Z.Slerp(desiredDirection, dashTurnRate * (float)delta);
				}

				GlobalTransform = GlobalTransform.LookingAt(GlobalPosition - forwardDirection, Vector3.Up);
			}

			Velocity = DesiredVelocity;

			if (MoveAndSlide())
			{
				// if (IsOnWall() && !isOnWall)
				// {
				// 	isOnWall = true;
				// 	var forward = CalculateForwardVectorAfterCollision();
				// 	GlobalTransform = GlobalTransform.LookingAt(GlobalPosition + forward, Vector3.Up);
				// }
			}

			// if (!IsOnWall() && isOnWall)
			// {
			// 	isOnWall = false;
			// }

			if (GetInputDirection().Length() >= 0.2f)
			{
				ballAnimationPlayer?.Play("Move");
			}
			else
			{
				ballAnimationPlayer?.Play("Idle");
			}
		}


		private void ProcessKnockbackState(double delta)
		{
			knockbackCounter += (float)delta;
			if (knockbackCounter >= knockbackDuration)
			{
				knockbackCounter = 0f;
				SetState(PlayerState.Normal);
				return;
			}

			knockbackDirChangeCounter += (float)delta;

			DesiredVelocity = GlobalTransform.Basis.Z * knockbackSpeed;

			// Rotate the player in the direction of the input.
			if (DesiredVelocity.Length() > 0)
			{
				Vector3 forwardDirection;

				if (DesiredDirection == Vector3.Zero)
				{
					DesiredDirection = GlobalTransform.Basis.Z;
				}

				if (knockbackDirChangeCounter >= 0.5f)
				{
					knockbackDirChangeCounter = 0f;
					DesiredDirection = new Vector3(
						(rand.NextSingle() * 2f) - 1f,
						0f,
						(rand.NextSingle() * 2f) - 1f
					).Normalized();
				}


				/// There's a known issue with the Slerp function in Godot, where it can throw an error when the two input vectors are 
				/// collinear (i.e., they point in the same direction or opposite directions). This is because the Slerp function uses 
				/// the cross product of the two vectors to determine the rotation axis, and the cross product of two collinear vectors 
				/// is a zero vector, which cannot be normalized 
				if (DesiredDirection.IsEqualApprox(GlobalTransform.Basis.Z))
				{
					forwardDirection = DesiredDirection;
				}
				else if (DesiredDirection.IsEqualApprox(-GlobalTransform.Basis.Z))
				{
					forwardDirection = (DesiredDirection.Cross(Vector3.Up) * 0.01f + DesiredDirection).Normalized();
				}
				else
				{
					forwardDirection = GlobalTransform.Basis.Z.Slerp(DesiredDirection, knockbackTurnRate * (float)delta);
				}

				GlobalTransform = GlobalTransform.LookingAt(GlobalPosition - forwardDirection, Vector3.Up);
			}

			Velocity = DesiredVelocity;

			if (MoveAndSlide())
			{
				if (IsOnWall() && !isOnWall)
				{
					isOnWall = true;
					var forward = CalculateForwardVectorAfterCollision();
					GlobalTransform = GlobalTransform.LookingAt(GlobalPosition + forward, Vector3.Up);
				}
			}

			if (!IsOnWall() && isOnWall)
			{
				isOnWall = false;
			}

			ballAnimationPlayer?.Play("Move");
		}

		private Vector3 CalculateForwardVectorAfterCollision()
		{
			Vector3 result = GlobalTransform.Basis.Z;
			result.Y = 0;
			result = result.Normalized();

			for (int i = 0; i < GetSlideCollisionCount(); i++)
			{
				var normal = GetSlideCollision(i).GetNormal();

				// Only check for wall normals.
				if (Mathf.Abs(Vector3.Up.Dot(normal)) > 0.3f)
				{
					continue;
				}

				normal.Y = 0f;
				normal = normal.Normalized();

				result = result.Reflect(normal);
			}

			return result;
		}

		private void StartInteraction()
		{
			targetInteractable?.StartInteract(this);

			isInteracting = true;
			playerAnimationPlayer?.Play("Interact");
		}

		private void StopInteraction()
		{
			targetInteractable?.StopInteract(this);
			targetInteractable = null;
		}

		private void StartDash()
		{
			if (isInteracting)
			{
				StopInteraction();
				isInteracting = false;
			}

			SetState(PlayerState.Dash);
			playerAnimationPlayer.Play("Tuck");
			ballVisual.Show();

			if (dropItemOnDash)
			{
				DropItem();
			}
		}

		private void StopDash()
		{
			ballVisual.Hide();
			playerVisual.Show();
			playerAnimationPlayer.PlayBackwards("Tuck");
			SetState(PlayerState.Normal);
		}

		public void Knockback()
		{
			if (isInteracting)
			{
				StopInteraction();
				isInteracting = false;
			}

			SetState(PlayerState.Knockback);
			playerAnimationPlayer.Play("Tuck");
			ballVisual.Show();

			if (dropItemOnKnockback)
			{
				DropItem();
			}
		}

		public void DropItem()
		{
			if (HeldItem is null)
			{
				return;
			}

			var item = HeldItem.InstantiateWorld();
			GetParent().AddChild(item);
			item.GlobalPosition = itemVisualParent.GlobalPosition;
			item.GlobalRotation = itemVisualParent.GlobalRotation;

			SetHeldItem(null);
		}

		private Vector3 GetInputDirection()
		{
			var input = Input.GetVector(moveLeftAction, moveRightAction, moveForwardAction, moveBackAction);
			return new Vector3(input.X, 0f, input.Y);
		}
	}
}