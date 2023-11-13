using Godot;

namespace SpaceEngineer
{
    public partial class Converter : Station
    {
        [Export] private Item inputItem;
        [Export] private Item outputItem;
        [Export] private Station outputTarget;
        [Export] private float processTime = 5.0f;

        private float counter = 0.0f;

        public override bool ValidateItem(Item item)
        {
            if (inputItem is null)
            {
                return true;
            }

            return inputItem == item;
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            if (State == StationState.Idle && HeldItem is not null)
            {
                counter += (float)delta;
                if (counter >= processTime)
                {
                    counter = 0f;

                    // Switch out the old item for the new
                    DestroyItem();
                    SpawnItem(outputItem);
                    MoveTo(outputTarget, ItemMoveMode.Slide);
                }
            }
        }
    }
}