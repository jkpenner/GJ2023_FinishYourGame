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
        public string INTERACTABLE_NODE_PATH = "Interactable";
        public string ARMORED_VISUAL_NODE_PATH = "ArmoredVisual";
        public string DAMAGED_VISUAL_NODE_PATH = "DamagedVisual";
        public string BREACHED_VISUAL_NODE_PATH = "BreachedVisual";

        [Export] HullState initialState;
        [Export] Item itemGainedAfterScrapped;
        [Export] Item requiredItemToRepair;

        private ShipController ship;
        private Node3D armoredVisual;
        private Node3D damagedVisual;
        private Node3D breachedVisual;
        private Interactable interactable;

        public HullState State { get; private set; }

        public delegate void HullEvent(DamagableHull hull);
        public event HullEvent HullBreached;
        public event HullEvent BreachContained;


        public override void _Ready()
        {
            ship = this.FindParentOfType<ShipController>();
            if (ship is null)
            {
                GD.PrintErr($"[{nameof(DamagableHull)}]: Node must be a child of a {nameof(ShipController)} node.");
                QueueFree();
                return;
            }

            ship.RegisterHull(this);

            FetchAndValidateSceneNodes();

            UpdateVisibility();
            UpdateInteractablity();
        }

        public override void _ExitTree()
        {
            if (ship is not null)
            {
                ship.UnregisterHull(this);
            }
        }

        private void FetchAndValidateSceneNodes()
        {
            armoredVisual = GetNode<Node3D>(ARMORED_VISUAL_NODE_PATH);
            if (armoredVisual is null)
            {
                this.PrintMissingChildError(ARMORED_VISUAL_NODE_PATH, nameof(Node3D));
            }

            damagedVisual = GetNode<Node3D>(DAMAGED_VISUAL_NODE_PATH);
            if (damagedVisual is null)
            {
                this.PrintMissingChildError(DAMAGED_VISUAL_NODE_PATH, nameof(Node3D));
            }

            breachedVisual = GetNode<Node3D>(BREACHED_VISUAL_NODE_PATH);
            if (breachedVisual is null)
            {
                this.PrintMissingChildError(BREACHED_VISUAL_NODE_PATH, nameof(Node3D));
            }

            interactable = GetNode<Interactable>("Interactable");
            if (interactable is not null)
            {
                interactable.Interacted += OnInteraction;
                interactable.ValidateInteraction = OnValidateInteraction;
            }
            else
            {
                this.PrintMissingChildError(INTERACTABLE_NODE_PATH, nameof(Interactable));
            }
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
            State = State switch
            {
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
            State = State switch
            {
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