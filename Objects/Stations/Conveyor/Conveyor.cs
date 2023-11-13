using Godot;

namespace SpaceEngineer
{
    public partial class Conveyor : Table
    {
        [Export] private Station moveTarget;

        public override void _Process(double delta)
        {
            base._Process(delta);
            
            // Moves item to the next station.
            MoveTo(moveTarget, ItemMoveMode.Slide);
        }
    }
}