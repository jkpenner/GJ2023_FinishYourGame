using System;
using System.Linq;
using Godot;
using SpaceEngineer;

public partial class MainMenu : Node
{
    [Export] UIFade fade;

    [Export] Node3D[] hallwayObjects;
    [Export] float hallwayMoveSpeed = 1.84f;
    [Export] Node3D hallwayStart;
    [Export] Node3D hallwayEnd;

    [Export] Node3D[] conveyorObjects;
    [Export] float conveyorMoveSpeed = 3.14f;
    [Export] Node3D conveyorStart;
    [Export] Node3D conveyorEnd;

    private int hallwayObjectIndex;
    private Node3D hallwayObject;
    private int conveyorObjectIndex;
    private Node3D conveyorObject;
    private Random random = new Random();

    public override void _Ready()
    {
        fade.FadeOutCompleted += OnFadeOutComplete;
    }

    private void OnFadeOutComplete()
    {
        GetTree().ChangeSceneToFile("res://Scenes/Levels/ShipLevel.tscn");
    }


    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Exit"))
        {
            GetTree().Quit();
        }

        if (Input.IsActionJustPressed("P1Dash"))
        {
            fade.FadeOut();
        }

        if (hallwayObject is null && hallwayObjects.Length > 0)
        {
            hallwayObjectIndex = random.Next(hallwayObjects.Length);
            hallwayObject = hallwayObjects[hallwayObjectIndex];
            hallwayObject.GlobalPosition = hallwayStart.GlobalPosition;

            if (hallwayObject.Name == "Hamster")
            {
                hallwayObject.GetNode<AnimationPlayer>("AnimationPlayer").Play("Walk");
            }
            else if (hallwayObject.Name == "HamsterBall")
            {
                hallwayObject.GetNode<AnimationPlayer>("AnimationPlayer").Play("Move");
            }
        }

        if (hallwayObject is not null)
        {
            hallwayObject.GlobalPosition = hallwayObject.GlobalPosition.MoveToward(hallwayEnd.GlobalPosition, hallwayMoveSpeed * (float)delta);
            if (hallwayObject.GlobalPosition.DistanceTo(hallwayEnd.GlobalPosition) < 0.2f)
            {
                hallwayObject = null;
            }
        }

        if (conveyorObject is null && conveyorObjects.Length > 0)
        {
            conveyorObjectIndex = random.Next(conveyorObjects.Length);
            conveyorObject = conveyorObjects[conveyorObjectIndex];
            conveyorObject.GlobalPosition = conveyorStart.GlobalPosition;
        }

        if (conveyorObject is not null)
        {
            conveyorObject.GlobalPosition = conveyorObject.GlobalPosition.MoveToward(conveyorEnd.GlobalPosition, conveyorMoveSpeed * (float)delta);
            if (conveyorObject.GlobalPosition.DistanceTo(conveyorEnd.GlobalPosition) < 0.2f)
            {
                conveyorObject = null;
            }
        }
    }
}
