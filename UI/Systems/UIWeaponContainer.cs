using System;
using System.Collections.Generic;
using Godot;

namespace SpaceEngineer
{
    public partial class UIWeaponContainer : Control
    {
        private const string WEAPON_STATUS_SCENE_PATH = "res://UI/Systems/UIWeaponStatus.tscn";

        private GameManager gameManager;
        private PackedScene weaponStatusScene;
        private Dictionary<Weapon, UIWeaponStatus> statusMap = new Dictionary<Weapon, UIWeaponStatus>();

        public override void _Ready()
        {
            this.TryGetGameManager(out gameManager);

            weaponStatusScene = ResourceLoader.Load<PackedScene>(WEAPON_STATUS_SCENE_PATH);
            if (weaponStatusScene is null)
            {
                GD.PrintErr($"Failed to load UIWeaponStatus from {WEAPON_STATUS_SCENE_PATH}");
            }

            // Add any weapons that existed before the ui element was created.
            foreach(var weapon in gameManager.PlayerShip.Weapons)
            {
                AddWeapon(weapon);
            }

            gameManager.PlayerShip.WeaponRegistered += AddWeapon;
            gameManager.PlayerShip.WeaponUnegistered += RemoveWeapon;
            gameManager.PlayerShip.WeaponLoading += ShowLoadingState;
            gameManager.PlayerShip.WeaponFired += HideLoadingState;
        }

        public override void _ExitTree()
        {
            gameManager.PlayerShip.WeaponRegistered -= AddWeapon;
            gameManager.PlayerShip.WeaponUnegistered -= RemoveWeapon;
            gameManager.PlayerShip.WeaponLoading -= ShowLoadingState;
            gameManager.PlayerShip.WeaponFired -= HideLoadingState;
        }

        private void HideLoadingState(Weapon weapon)
        {
            
        }

        private void ShowLoadingState(Weapon weapon)
        {
            
        }

        private void RemoveWeapon(Weapon weapon)
        {
            if (!statusMap.TryGetValue(weapon, out var instance))
            {
                return;
            }

            statusMap.Remove(weapon);
            RemoveChild(instance);
            instance.QueueFree();
        }

        private void AddWeapon(Weapon weapon)
        {
            if (statusMap.ContainsKey(weapon))
            {
                return;
            }

            var instance = weaponStatusScene.Instantiate<UIWeaponStatus>();
            if (instance is null)
            {
                GD.PrintErr("Weapon status scene is null");
                return;
            }

            statusMap.Add(weapon, instance);
            AddChild(instance);
            instance.Setup(weapon);
        }
    }
}