using Godot;

namespace SpaceEngineer
{
    public partial class PlayerCamera : Node3D
    {
        [Export] private Node3D followTarget;

        public CameraShaker Shaker { get; private set; }

        public override void _Ready()
        {
            Shaker = GetNode<CameraShaker>("CameraPivot/Camera3D");
        }

        public override void _Process(double delta)
        {
            if (followTarget is null)
            {
                return;
            }
            
            GlobalPosition = followTarget.GlobalPosition;
        }
    }
}