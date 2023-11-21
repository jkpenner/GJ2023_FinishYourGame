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

            UpdateActionText();
        }

        private void FetchAndValidateSceneNodes()
        {
            interactable = GetNode<Interactable>(INTERACTABLE_NODE_PATH);
            if (interactable is not null)
            {
                interactable.IsInteractable = true;
                interactable.ValidateInteraction = OnValidateInteration;
                interactable.Interacted += OnInteraction;
            }
            else
            {
                this.PrintMissingChildError(INTERACTABLE_NODE_PATH, nameof(Interactable));
            }
        }

        private bool OnValidateInteration(PlayerController interactor)
        {
            UpdateActionText();

            if (HeldItem is null && interactor.HeldItem is not null)
            {
                return true;
            }
            else if (HeldItem is not null && interactor.HeldItem is null)
            {
                return true;
            }
            return false;
        }


        private void OnInteraction(PlayerController interactor)
        {
            if (interactor.HeldItem is not null)
            {
                if (TryPlaceObject(interactor.HeldItem))
                {
                    UpdateActionText();
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
                    UpdateActionText();
                    interactor.SetHeldItem(item);
                }
                else
                {
                    GD.Print("Failed to take item");
                }
            }
        }

        private void UpdateActionText()
        {
            if (HeldItem is not null)
            {
                interactable.SetActionText($"Pick up {HeldItem.DisplayName}");
            }
            else
            {
                interactable.SetActionText($"Place item");
            }
        }

    }
}