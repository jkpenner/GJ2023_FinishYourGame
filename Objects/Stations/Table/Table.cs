using System;
using Godot;

namespace SpaceEngineer
{
    public partial class Table : Station
    {
        public const string INTERACTABLE_NODE_PATH = "Interactable";

        [Export] Item initialItem;
        
        private Interactable interactable;

        public override void _Ready()
        {
            base._Ready();

            FetchAndValidateSceneNodes();

            TryPlaceObject(initialItem);
        }

        private void FetchAndValidateSceneNodes()
        {
            interactable = GetNode<Interactable>(INTERACTABLE_NODE_PATH);
            if (interactable is not null)
            {
                interactable.IsInteractable = true;
                interactable.Interacted += OnInteraction;
            }
            else
            {
                this.PrintMissingChildError(INTERACTABLE_NODE_PATH, nameof(Interactable));
            }
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