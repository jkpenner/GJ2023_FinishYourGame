using System;
using Godot;

namespace SpaceEngineer
{
    public partial class WorldItem : RigidBody3D
    {
        [Export] private Item item;
        [Export] private Interactable interactable;

        private Node3D visual;

        public override void _Ready()
        {
            SetItem(item);

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

        public void SetItem(Item newItem)
        {
            if (visual is not null)
            {
                RemoveChild(visual);
                visual.QueueFree();
            }

            item = newItem;

            if (item is not null)
            {
                visual = item.InstantiateVisual();
                if (visual is not null)
                {
                    AddChild(visual);
                }
            }
        }
    }
}