using Godot;

namespace SpaceEngineer
{
    public partial class Conveyor : Node3D
    {
        [Export] private ItemSlot slot;
        [Export] private ItemSlot nextSlot;

        public override void _Ready()
        {
            slot.IsInteractable = true;
        }

        public override void _Process(double delta)
        {
            slot.MoveTo(nextSlot, MoveMode.Slide);
        }
    }
}