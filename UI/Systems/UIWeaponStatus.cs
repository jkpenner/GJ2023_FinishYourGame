using System;
using Godot;

namespace SpaceEngineer
{
    public partial class UIWeaponStatus : Control
    {
        private const string SYSTEM_NAME_NODE_PATH = "WeaponState/MarginContainer/HBoxContainer/WeaponName";
        private const string STATE_CONTAINER_PATH = "WeaponState/MarginContainer/HBoxContainer/PanelContainer";
        private const string STATE_NODE_PATH = "WeaponState/MarginContainer/HBoxContainer/PanelContainer/MarginContainer/WeapnState";
        private const string STATUS_PROGRESS_PATH = "Status/ProgressBar";
        private const string STATUS_LABEL_PATH = "Status/Status";

        [Export] Color loaded = new Color("#00FF00");
        [Export] Color empty = new Color("#FF0000");

        [Export] Color firing = new Color("#ff4800");

        private GameManager gameManager;
        private Weapon weapon;

        private Label weaponName;
        private PanelContainer stateContainer;
        private Label state;

        private ProgressBar statusProgress;
        private Label statusLabel;

        public override void _Ready()
        {
            this.TryGetGameManager(out gameManager);

            weaponName = GetNode<Label>(SYSTEM_NAME_NODE_PATH);
            stateContainer = GetNode<PanelContainer>(STATE_CONTAINER_PATH);
            state = GetNode<Label>(STATE_NODE_PATH);
            statusProgress = GetNode<ProgressBar>(STATUS_PROGRESS_PATH);
            statusLabel = GetNode<Label>(STATUS_LABEL_PATH);

            OnItemChanged(null);
        }

        public override void _Process(double delta)
        {
            if (gameManager.PlayerShip.ActiveWeapon == weapon)
            {
                if (gameManager.PlayerShip.CombatState == ShipCombatState.Targeting)
                {
                    statusProgress.Visible = true;
                    statusProgress.SelfModulate = firing;
                    statusProgress.Value = gameManager.PlayerShip.WeaponTargetingPercent;
                    
                    statusLabel.Visible = true;
                    statusLabel.Text = "Firing";
                }
                else
                {
                    statusProgress.Visible = false;
                    statusLabel.Visible = false;
                }
            }
            else
            {
                statusProgress.Visible = false;
                statusLabel.Visible = false;
            }
        }

        public void Setup(Weapon weapon)
        {
            this.weapon = weapon;
            this.weapon.ItemChanged += OnItemChanged;

            weaponName.Text = string.IsNullOrEmpty(weapon.DisplayName) ? $"{weapon.AmmoType}" : weapon.DisplayName;
        }

        private void OnItemChanged(Item item)
        {
            if (item is not null)
            {
                state.Text = "Loaded";
                stateContainer.SelfModulate = loaded;
            }
            else
            {
                state.Text = "Empty";
                stateContainer.SelfModulate = empty;
            }
        }
    }
}