using System;
using Godot;

namespace SpaceEngineer
{
    public partial class WorldItem : RigidBody3D
    {
        [Export] private Item item;
        [Export] private Interactable interactable;

        public override void _Ready()
        {
            var visual = item.InstantiateVisual();
            if (visual is not null)
            {
                AddChild(visual);
            }

            interactable.IsInteractable = true;
            interactable.Interacted += OnInteraction;
        }

        private void OnInteraction(PlayerController interactor)
        {
            if (interactor.HeldItem is null)
            {
                interactor.SetHeldItem(item);
                QueueFree();
            }
        }
    }
}