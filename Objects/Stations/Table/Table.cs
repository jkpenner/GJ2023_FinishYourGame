using Godot;

namespace SpaceEngineer
{
    public partial class Table : Node3D
    {
        [Export] Item initialItem;
        [Export] ItemSlot slot;

        public override void _Ready()
        {
            slot.IsInteractable = true;

            if (initialItem is not null)
            {
                slot.TryPlaceObject(initialItem);
            }            
        }
    }
}