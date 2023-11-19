using Godot;
using System;

namespace SpaceEngineer
{
    public partial class UIShipEnergy : Control
    {
        [Export] PackedScene cellScene;

        private GameManager gameManager;
        private Control cellParent;
        private Label overloadNotification;

        public override void _Ready()
        {
            this.TryGetGameManager(out gameManager);

            cellParent = GetNode<Control>("MarginContainer/GridContainer");
            if (cellParent is null)
            {
                this.PrintMissingChildError("MarginContainer/GridContainer", nameof(Control));
            }

            overloadNotification = GetNode<Label>("MarginContainer/PanelContainer/OverloadTimer");
            if (overloadNotification is null)
            {
                this.PrintMissingChildError("MarginContainer/PanelContainer/OverloadTimer", nameof(Label));
            }

            // Trigger initial value assignments
            SetupEnergyCells();
            UpdateEnergyCellStates();
        }

        public override void _Process(double delta)
        {
            if (gameManager.PlayerShip.OverloadState == ShipOverloadState.Overloading)
            {
                overloadNotification.Text = $"{(int)gameManager.PlayerShip.GetRemainingTimeTillOverload()}s till Overload!";
            }
        }

        public override void _EnterTree()
        {
            GameEvents.ShipEnergyCapacityChanged.Connect(OnEnergyEvent);
            GameEvents.ShipEnergyNormalized.Connect(UpdateEnergyCellStates);
            GameEvents.ShipEnergyOverloading.Connect(UpdateEnergyCellStates);
            GameEvents.ShipEnergyUsageChanged.Connect(OnEnergyEvent);
        }

        public override void _ExitTree()
        {
            GameEvents.ShipEnergyCapacityChanged.Disconnect(OnEnergyEvent);
            GameEvents.ShipEnergyNormalized.Disconnect(UpdateEnergyCellStates);
            GameEvents.ShipEnergyOverloading.Disconnect(UpdateEnergyCellStates);
            GameEvents.ShipEnergyUsageChanged.Disconnect(OnEnergyEvent);
        }

        private void OnEnergyEvent(int _)
        {
            UpdateEnergyCellStates();
        }

        private void SetupEnergyCells()
        {
            // Remove all previous cells
            for (int i = 0; i < cellParent.GetChildCount(); i++)
            {
                cellParent.GetChild(i).QueueFree();
            }

            // Add cells up to the total maximum
            for (int i = 0; i < gameManager.PlayerShip.MaximumEnergy; i++)
            {
                cellParent.AddChild(cellScene.Instantiate<UIShipEnergyCell>());
            }
        }

        private void UpdateEnergyCellStates()
        {
            if (overloadNotification.GetParent() is Control overloadParent)
            {
                overloadParent.Visible = gameManager.PlayerShip.OverloadState == ShipOverloadState.Overloading;
            }

            for (int i = 0; i < cellParent.GetChildCount(); i++)
            {
                var cell = cellParent.GetChild<UIShipEnergyCell>(i);
                if (gameManager.PlayerShip.OverloadState == ShipOverloadState.NotOverloaded)
                {
                    if (i < gameManager.PlayerShip.EnergyUsage)
                    {
                        cell.SetState(UIShipEnergyCellState.Active);
                    }
                    else if (i < gameManager.PlayerShip.EnergyCapacity)
                    {
                        cell.SetState(UIShipEnergyCellState.Enactive);
                    }
                    else
                    {
                        cell.SetState(UIShipEnergyCellState.Depleted);
                    }
                }
                else
                {
                    cell.SetState(UIShipEnergyCellState.Overloaded);
                }
            }
        }
    }
}
