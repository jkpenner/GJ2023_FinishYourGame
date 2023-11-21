using Godot;
using System;

namespace SpaceEngineer
{
    public partial class UIShipEnergy : Control
    {
        private const string CELL_PARENT_PATH = "Energy/MarginContainer/Container/Margins/GridContainer";
        private const string OVERLOAD_TIMER_PATH = "Energy/MarginContainer/Container/Margins/PanelContainer/OverloadTimer";
        private const string OVERLOAD_PROGRESS_PATH = "Energy/OverloadProgress";


        [Export] PackedScene cellScene;

        private GameManager gameManager;
        private Control cellParent;
        private Label overloadNotification;
        private ProgressBar overloadProgress;

        public override void _Ready()
        {
            this.TryGetGameManager(out gameManager);

            cellParent = GetNode<Control>(CELL_PARENT_PATH);
            if (cellParent is null)
            {
                this.PrintMissingChildError(CELL_PARENT_PATH, nameof(Control));
            }

            overloadNotification = GetNode<Label>(OVERLOAD_TIMER_PATH);
            if (overloadNotification is null)
            {
                this.PrintMissingChildError(OVERLOAD_TIMER_PATH, nameof(Label));
            }

            overloadProgress = GetNode<ProgressBar>(OVERLOAD_PROGRESS_PATH);
            if (overloadProgress is null)
            {
                this.PrintMissingChildError(OVERLOAD_PROGRESS_PATH, nameof(ProgressBar));
            }

            // Trigger initial value assignments
            SetupEnergyCells();
            UpdateEnergyCellStates();
        }

        public override void _Process(double delta)
        {
            overloadProgress.Value = gameManager.PlayerShip.GetOverloadPercent();

            if (gameManager.PlayerShip.OverloadState == ShipOverloadState.Overloading)
            {
                overloadNotification.Text = $"{(int)gameManager.PlayerShip.GetRemainingTimeTillOverload()}s till Overload!";
            }

            if (gameManager.PlayerShip.EnergyCapacity < gameManager.PlayerShip.MaximumEnergy)
            {
                var childIndex = gameManager.PlayerShip.EnergyCapacity;
                var child = cellParent.GetChild<UIShipEnergyCell>(childIndex);
                if (child is not null)
                {
                    child.SetRechargePercent(gameManager.PlayerShip.GetEnergyRechargePercent());
                }
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
