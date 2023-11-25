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
        private ShipSystemTerminalVisual visual;
        private Label3D status;

        public override void _Ready()
        {
            if (!this.TryGetGameManager(out gameManager) || gameManager.PlayerShip is null)
            {
                return;
            }

            visual = GetNode<ShipSystemTerminalVisual>("ShipSystemTerminalVisual");
            if (visual is null)
            {
                GD.PrintErr("Failed to find terminal visual in children");
            }

            status = GetNode<Label3D>("SystemStatus");

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

        public override void _Process(double delta)
        {
            if (system is not null && system.State == ShipSystemState.Overclocked)
            {
                visual.SetProgressFade(1.0f - (system.OverclockRemainder / system.OverclockDuration));
            }
        }

        private void OnSystemStateChanged()
        {
            status.Text = system.State.ToString();

            switch (system.State)
            {
                case ShipSystemState.Damaged:
                    

                    toggle.IsInteractable = false;
                    overclock.IsInteractable = false;
                    repair.IsInteractable = true;
                    
                    visual.SetNormalScreenColor(damagedColor);
                    visual.SetNormalScreenPulseMode(true, 8f);
                    visual.SetNormalScreenPulseColor(damagedColor * 1.2f);
                    visual.SetProgressColors(damagedColor, damagedColor);
                    visual.SetProgressPulse(true, true, 8f);
                    visual.SetProgressFade(0.0f);
                    visual.SetProgressPulseColors(damagedColor * 1.6f, damagedColor * 1.6f);
                    visual.SetOverloadBodyVisible(false);
                    break;
                case ShipSystemState.Destroyed:
                    toggle.IsInteractable = false;
                    overclock.IsInteractable = false;
                    repair.IsInteractable = true;
                    visual.SetNormalScreenColor(destroyedColor);
                    visual.SetNormalScreenPulseMode(true, 8f);
                    visual.SetNormalScreenPulseColor(destroyedColor * 1.2f);
                    visual.SetProgressColors(destroyedColor, destroyedColor);
                    visual.SetProgressPulse(true, true, 8f);
                    visual.SetProgressFade(0.0f);
                    visual.SetProgressPulseColors(destroyedColor * 1.6f, destroyedColor * 1.6f);
                    visual.SetOverloadBodyVisible(false);
                    break;
                case ShipSystemState.Disabled:
                    toggle.IsInteractable = true;
                    overclock.IsInteractable = true;
                    repair.IsInteractable = false;
                    visual.SetNormalScreenColor(disabledColor);
                    visual.SetNormalScreenPulseMode(false);
                    visual.SetProgressColors(disabledColor, disabledColor);
                    visual.SetProgressPulse(false, false);
                    visual.SetOverloadBodyVisible(false);
                    break;
                case ShipSystemState.Overclocked:
                    toggle.IsInteractable = false;
                    overclock.IsInteractable = false;
                    repair.IsInteractable = false;
                    visual.SetNormalScreenColor(overclockedColor);
                    visual.SetNormalScreenPulseMode(false);
                    visual.SetProgressColors(overclockedColor * 0.8f, overclockedColor * 0.2f);
                    visual.SetProgressPulse(true, true, 10f);
                    visual.SetProgressFade(0.0f);
                    visual.SetProgressPulseColors(overclockedColor * 2f, overclockedColor * 0.4f);
                    visual.SetOverloadBodyVisible(true);
                    break;
                case ShipSystemState.Powered:
                    toggle.IsInteractable = true;
                    overclock.IsInteractable = true;
                    repair.IsInteractable = false;
                    visual.SetNormalScreenColor(poweredColor);
                    visual.SetNormalScreenPulseMode(false);
                    visual.SetProgressColors(poweredColor, poweredColor);
                    visual.SetProgressPulse(false, false);
                    visual.SetOverloadBodyVisible(false);
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