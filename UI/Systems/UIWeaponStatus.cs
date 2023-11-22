using System;
using Godot;

namespace SpaceEngineer
{
    public partial class UIWeaponStatus : Control
    {
        private const string SYSTEM_NAME_NODE_PATH = "WeaponState/MarginContainer/HBoxContainer/WeaponName";
        private const string STATE_CONTAINER_PATH = "SystemPower/MarginContainer/HBoxContainer/PanelContainer";
        private const string STATE_NODE_PATH = "WeaponState/MarginContainer/HBoxContainer/PanelContainer/MarginContainer/WeapnState";
        private const string STATUS_CONTAINER_PATH = "Status";
        private const string STATUS_LABEL_PATH = "Status/MarginContainer/HBoxContainer/Status";

        [Export] Color loaded = new Color("#00FF00");
        [Export] Color empty = new Color("#FF0000");

        [Export] Color firing = new Color("#AA0000");

        private GameManager gameManager;
        private Weapon weapon;

        private Label weaponName;
        private Control stateContainer;
        private Label state;
        private Control statusContainer;
        private Label status;

        public override void _Ready()
        {
            this.TryGetGameManager(out gameManager);

            weaponName = GetNode<Label>(SYSTEM_NAME_NODE_PATH);
            stateContainer = GetNode<Control>(STATE_CONTAINER_PATH);
            state = GetNode<Label>(STATE_NODE_PATH);
            statusContainer = GetNode<Control>(STATUS_CONTAINER_PATH);
            status = GetNode<Label>(STATUS_LABEL_PATH);
        }

        public override void _Process(double delta)
        {
            if (gameManager.PlayerShip.ActiveWeapon == weapon)
            {
                statusContainer.Visible = true;
                statusContainer.SelfModulate = firing;
                status.Text = "Firing";
            }
            else
            {
                statusContainer.Visible = false;
            }
        }

        public void Setup(Weapon weapon)
        {
            this.weapon = weapon;
            this.weapon.ItemChanged += OnItemChanged;

            weaponName.Text = $"{weapon.AmmoType}";
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