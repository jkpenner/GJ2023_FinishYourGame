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

        private GameManager gameManager;
        private ShipSystem system;

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
                OnSystemStateChanged(system);
            }

            toggle.Interacted += OnToggleInteraction;
            overclock.Interacted += OnOverclockInteraction;
            repair.Interacted += OnRepairInteraction;
        }

        public override void _ExitTree()
        {
            if (system is not null)
            {
                system.StateChanged -= OnSystemStateChanged;
            }
        }

        private void OnSystemStateChanged(ShipSystem system)
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