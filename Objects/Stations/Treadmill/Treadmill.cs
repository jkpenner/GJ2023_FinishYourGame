using Godot;
using SpaceEngineer;
using System;
using System.Collections.Generic;

public partial class Treadmill : Node3D
{
	[Export] Area3D area;

	private ShipController ship;
	private List<PlayerController> players = new List<PlayerController>();

	public Action EnergyGenerated;

	public override void _Ready()
	{
		ship = this.FindParentOfType<ShipController>();
		if (ship is null)
		{
			GD.PrintErr($"[{nameof(Treadmill)}]: Treadmill must be a child of a {nameof(ShipController)} node.");
			QueueFree();
			return;
		}

		ship.RegisterTreadmill(this);

		area.BodyEntered += OnBodyEntered;
		area.BodyExited += OnBodyExited;
	}

    public override void _ExitTree()
    {
        if (ship is not null)
		{
			ship.UnregisterTreadmill(this);
		}
    }

    public override void _Process(double delta)
    {
        if (players.Count == 0)
		{
			return;
		}

		foreach(var player in players)
		{
			// if almost stopped ignore.
			if (player.DesiredVelocity.Length() <= 0.2f)
			{
				continue;
			}

			var direction = player.DesiredVelocity.Normalized();
			var dot = GlobalTransform.Basis.Z.Dot(direction);
			if (Mathf.Abs(dot) > 0.75)
			{
				EnergyGenerated?.Invoke();

				// only gerate additional power once.
				break;
			}
		}
    }

    private void OnBodyEntered(Node3D body)
	{
		if (body is PlayerController player)
		{
			players.Add(player);
		}
	}


	private void OnBodyExited(Node3D body)
	{
		if (body is PlayerController player)
		{
			players.Remove(player);
		}
	}

}
