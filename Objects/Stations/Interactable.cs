using System;
using Godot;

namespace SpaceEngineer
{
    public partial class Interactable : Area3D
    {
        [Export] private bool isInteractable = true;

        public delegate void InteractionEvent(PlayerController interactor);
        public event InteractionEvent Interacted;

        public bool IsInteractable
        {
            get => isInteractable;
            set => isInteractable = value;
        }

        public void Interact(PlayerController interactor)
        {
            if (!IsInteractable || interactor is null)
            {
                return;
            }

            GD.Print($"{interactor.Name} interacted with {this.Name}");
            Interacted?.Invoke(interactor);
        }
    }
}