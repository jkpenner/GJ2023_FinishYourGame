using System;
using Godot;

namespace SpaceEngineer
{
    public enum HullState
    {
        Armored,
        Damaged,
        Breached
    }

    public partial class DamagableHull : Node3D
    {
        [Export] HullState initialState;
        [Export] Node3D armoredVisual;
        [Export] Node3D damagedVisual;
        [Export] Node3D breachedVisual;

        [Export] Interactable interactable;
        [Export] Item itemGainedAfterScrapped;
        [Export] Item requiredItemToRepair;

        public HullState State { get; private set; }

        public delegate void HullEvent(DamagableHull hull);
        public event HullEvent HullBreached;
        public event HullEvent BreachContained;
        

        public override void _Ready()
        {
            interactable.Interacted += OnInteraction;
            interactable.ValidateInteraction = OnValidateInteraction;

            UpdateVisibility();
            UpdateInteractablity();
        }

        private bool OnValidateInteraction(PlayerController interactor)
        {
            if (CanBeScrapped() && interactor.HeldItem is null)
            {
                return true;
            }

            if (State != HullState.Armored && interactor.HeldItem == requiredItemToRepair)
            {
                return true;
            }

            return false;
        }

        private void OnInteraction(PlayerController interactor)
        {
            // Player is scrapping the hull, give scrap.
            if (CanBeScrapped() && interactor.HeldItem is null)
            {
                interactor.SetHeldItem(itemGainedAfterScrapped);
                Damage();
                return;
            }

            if (CanBeRepaired() && interactor.HeldItem == requiredItemToRepair)
            {
                interactor.SetHeldItem(null);
                Repair();
                return;
            }
        }

        public bool CanBeScrapped()
        {
            return State == HullState.Armored;
        }

        public bool CanBeDamaged()
        {
            return State != HullState.Breached;
        }

        public bool CanBeRepaired()
        {
            return State != HullState.Armored;
        }

        public void Damage()
        {
            if (!CanBeDamaged())
            {
                return;
            }

            // Update to the next damage state
            State = State switch {
                HullState.Armored => HullState.Damaged,
                _ => HullState.Breached,
            };

            UpdateInteractablity();
            UpdateVisibility();

            // Play any effects here...

            if (State == HullState.Breached)
            {
                HullBreached?.Invoke(this);
            }
        }

        private void Repair()
        {
            if (!CanBeRepaired())
            {
                return;
            }

            var wasBreached = State == HullState.Breached;

            // Update to the next repaired state
            State = State switch {
                HullState.Breached => HullState.Damaged,
                _ => HullState.Armored
            };

            UpdateInteractablity();
            UpdateVisibility();

            // Play any effects here...

            if (wasBreached)
            {
                BreachContained?.Invoke(this);
            }
        }

        private void UpdateInteractablity()
        {
            interactable.IsInteractable = CanBeScrapped() || CanBeRepaired();
        }

        private void UpdateVisibility()
        {
            armoredVisual.Visible = State == HullState.Armored;
            damagedVisual.Visible = State == HullState.Damaged;
            breachedVisual.Visible = State == HullState.Breached;
        }
    }
}