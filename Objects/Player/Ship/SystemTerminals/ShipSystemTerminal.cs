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

        [ExportGroup("Screens")]
        [Export] MeshInstance3D[] mainScreens;
        [Export] MeshInstance3D[] progressScreens;

        [ExportGroup("Materials")]
        [Export] Color poweredColor = new Color("#00FF00");
        [Export] Color disabledColor = new Color("#FFAA00");
        [Export] Color overclockedColor = new Color("#0000FF");
        [Export] Color damagedColor = new Color("#FF0000");
        [Export] Color destroyedColor = new Color("#AA0000");

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
                OnSystemStateChanged();
            }

            toggle.Interacted += OnToggleInteraction;
            toggle.ValidateInteraction = OnValidateToggle;
            overclock.Interacted += OnOverclockInteraction;
            overclock.SetActionText($"Overclock {systemType}");
            repair.Interacted += OnRepairInteraction;
            repair.SetActionText($"Repair {systemType}");

            var visual = GetNode<Node3D>("SystemTerminal");
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
                    SetScreenColor(damagedColor);
                    break;
                case ShipSystemState.Destroyed:
                    toggle.IsInteractable = false;
                    overclock.IsInteractable = false;
                    repair.IsInteractable = true;
                    SetScreenColor(destroyedColor);
                    break;
                case ShipSystemState.Disabled:
                    toggle.IsInteractable = true;
                    overclock.IsInteractable = true;
                    repair.IsInteractable = false;
                    SetScreenColor(disabledColor);
                    break;
                case ShipSystemState.Overclocked:
                    toggle.IsInteractable = false;
                    overclock.IsInteractable = false;
                    repair.IsInteractable = false;
                    SetScreenColor(overclockedColor);
                    break;
                case ShipSystemState.Powered:
                    toggle.IsInteractable = true;
                    overclock.IsInteractable = true;
                    repair.IsInteractable = false;
                    SetScreenColor(poweredColor);
                    break;
            }
        }

        private void SetScreenColor(Color color)
        {
            foreach (var screen in mainScreens)
            {
                if (screen.GetSurfaceOverrideMaterial(0) is ShaderMaterial shader)
                {
                    shader.SetShaderParameter("fade", 0.0f);
                    shader.SetShaderParameter("main_color", color);
                }
                else
                {
                    GD.Print("No ShaderMaterial assigned to MaterialOverride");
                }
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