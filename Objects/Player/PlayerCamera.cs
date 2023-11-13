using Godot;

namespace SpaceEngineer
{
    public partial class PlayerCamera : Node3D
    {
        [Export] private Node3D followTarget;

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