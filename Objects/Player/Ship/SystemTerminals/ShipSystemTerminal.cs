using System;
using Godot;

namespace SpaceEngineer
{
    public partial class ShipSystemTerminal : Node3D
    {
        [Export] ShipSystemType systemType;
        [Export] Interactable toggle;
        [Export] Interactable overclock;
        [Export] Interactable repair;

        [ExportGroup("Materials")]
        public Material active;
        public Material inactive;
        public Material overclocked;

        private GameManager gameManager;
        private ShipSystem system;

        private Node3D[] displays;

        public override void _Ready()
        {
            if (!this.TryGetGameManager(out gameManager) || gameManager.PlayerShip is null)
            {
                return;
            }

            system = gameManager.PlayerShip.GetSystem(systemType);
            if (system is not null)
            {
                system.StateChanged += OnSystemStateChanged;
                // Initial value setup
                OnSystemStateChanged();
            }

            toggle.Interacted += OnToggleInteraction;
            toggle.ValidateInteraction = OnValidateToggle;
            overclock.Interacted += OnOverclockInteraction;
            overclock.SetActionText($"Overclock {systemType}");
            repair.Interacted += OnRepairInteraction;
            repair.SetActionText($"Repair {systemType}");

            var visual = GetNode<Node3D>("SystemTerminal");
            foreach(var display in displays)
            {
                // display
            }
        }

        private bool OnValidateToggle(PlayerController interator)
        {
            if (system.State == ShipSystemState.Powered)
            {
                toggle.SetActionText($"Power Off {systemType}");
            }
            else
            {
                toggle.SetActionText($"Power On {systemType}");
            }
            return true;
        }


        public override void _ExitTree()
        {
            if (system is not null)
            {
                system.StateChanged -= OnSystemStateChanged;
            }
        }

        private void OnSystemStateChanged()
        {
            switch (system.State)
            {
                case ShipSystemState.Damaged:
                    toggle.IsInteractable = false;
                    overclock.IsInteractable = false;
                    repair.IsInteractable = true;
                    break;
                case ShipSystemState.Destroyed:
                    toggle.IsInteractable = false;
                    overclock.IsInteractable = false;
                    repair.IsInteractable = true;
                    break;
                case ShipSystemState.Disabled:
                    toggle.IsInteractable = true;
                    overclock.IsInteractable = true;
                    repair.IsInteractable = false;
                    break;
                case ShipSystemState.Overclocked:
                    toggle.IsInteractable = false;
                    overclock.IsInteractable = false;
                    repair.IsInteractable = false;
                    break;
                case ShipSystemState.Powered:
                    toggle.IsInteractable = true;
                    overclock.IsInteractable = true;
                    repair.IsInteractable = false;
                    break;
            }
        }

        private void OnRepairInteraction(PlayerController interactor)
        {
            GD.Print($"{systemType}: Repair Interaction");
            system?.Repair();
        }

        private void OnOverclockInteraction(PlayerController interactor)
        {
            GD.Print($"{systemType}: Overclock Interaction");
            system?.Overclock();
        }

        private void OnToggleInteraction(PlayerController interactor)
        {
            GD.Print($"{systemType}: Toggle Power Interaction");
            system?.TogglePower();
        }
    }
}