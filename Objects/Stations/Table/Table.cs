using System;
using Godot;

namespace SpaceEngineer
{
    public partial class Table : Station
    {
        [Export] Item initialItem;
        [Export] Interactable interactable;

        public override void _Ready()
        {
            base._Ready();

            interactable.IsInteractable = true;
            interactable.Interacted += OnInteraction;

            TryPlaceObject(initialItem);
        }

        private void OnInteraction(PlayerController interactor)
        {
            if (interactor.HeldItem is not null)
            {
                if (TryPlaceObject(interactor.HeldItem))
                {
                    interactor.SetHeldItem(null);
                }
                else
                {
                    GD.Print("Failed to place item");
                }
            }
            else
            {
                if (TryTakeObject(out var item))
                {
                    interactor.SetHeldItem(item);
                }
                else
                {
                    GD.Print("Failed to take item");
                }
            }
        }

    }
}