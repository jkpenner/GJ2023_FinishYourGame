using System;
using System.Collections.Generic;
using Godot;

namespace SpaceEngineer
{
    public partial class UIShipShields : Control
    {
        private const string SHIELD_NODE_PATH = "res://UI/UIShipShieldNode.tscn";

        private GameManager gameManager;
        private PackedScene nodeScene;

        private List<UIShipShieldNode> shieldNodes = new List<UIShipShieldNode>();

        public override void _Ready()
        {
            this.TryGetGameManager(out gameManager);
            nodeScene = ResourceLoader.Load<PackedScene>(SHIELD_NODE_PATH);

            gameManager.PlayerShip.ShieldBroken += UpdateShields;
            gameManager.PlayerShip.ShieldRestored += UpdateShields;
            gameManager.PlayerShip.ShieldChargesChanged += UpdateShields;

            UpdateShields();
        }

        public override void _ExitTree()
        {
            gameManager.PlayerShip.ShieldBroken -= UpdateShields;
            gameManager.PlayerShip.ShieldRestored -= UpdateShields;
            gameManager.PlayerShip.ShieldChargesChanged -= UpdateShields;
        }

        public override void _Process(double delta)
        {
            var ship = gameManager.PlayerShip;
            if (ship.ShieldCharges < ship.MaxShieldCharges)
            {
                shieldNodes[ship.ShieldCharges].SetValue(
                    ship.ShieldRechargePercent
                );
            }
        }

        private void UpdateShields()
        {
            var ship = gameManager.PlayerShip;

            if (shieldNodes.Count != ship.MaxShieldCharges)
            {
                foreach (var shield in shieldNodes)
                {
                    RemoveChild(shield);
                }

                while (shieldNodes.Count < ship.MaxShieldCharges)
                {
                    shieldNodes.Add(nodeScene.Instantiate<UIShipShieldNode>());
                }

                foreach (var shield in shieldNodes)
                {
                    AddChild(shield);
                }
            }

            for (int i = 0; i < shieldNodes.Count; i++)
            {
                if (i < ship.ShieldCharges)
                {
                    shieldNodes[i].Visible = true;
                    shieldNodes[i].SetValue(1f);
                }
                else if (i < ship.MaxShieldCharges)
                {
                    shieldNodes[i].Visible = true;
                    shieldNodes[i].SetValue(0f);
                }
                else
                {
                    shieldNodes[i].Visible = false;
                }
            }
        }
    }
}