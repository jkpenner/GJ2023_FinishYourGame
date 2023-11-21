using System;
using System.Collections.Generic;
using Godot;

namespace SpaceEngineer
{
    public partial class UIEnemyCard : Control
    {
        private const string ENEMY_WEAPON_SCENE_PATH = "res://UI/EnemyCards/UIEnemyWeapon.tscn";
        private const string ENEMY_SHIELD_SCENE_PATH = "res://UI/EnemyCards/UIEnemyShield.tscn";

        private const string SHIP_NAME_NODE_PATH = "MarginContainer/VBoxContainer/ShipName";
        private const string SHIP_ICON_NODE_PATH = "MarginContainer/VBoxContainer/ShipAndWeapons/ShipIcon/ShipImage";
        private const string WEAPON_PARENT_NODE_PATH = "MarginContainer/VBoxContainer/ShipAndWeapons";
        private const string SHIELD_PARENT_NODE_PATH = "MarginContainer/VBoxContainer/Shields";

        private PackedScene enemyWeaponScene;
        private PackedScene enemyShieldScene;

        private Label shipName;
        private TextureRect shipIcon;

        private Control weaponParent;
        private Control shieldParent;

        private EnemyController enemy;
        private UIEnemyWeapon laserWeapon;
        private UIEnemyWeapon kineticWeapon;
        private UIEnemyWeapon missileWeapon;
        private List<UIEnemyShield> laserShields = new List<UIEnemyShield>();
        private List<UIEnemyShield> kineticShields = new List<UIEnemyShield>();
        private List<UIEnemyShield> missileShields = new List<UIEnemyShield>();

        public override void _Ready()
        {
            enemyWeaponScene = ResourceLoader.Load<PackedScene>(ENEMY_WEAPON_SCENE_PATH);
            enemyShieldScene = ResourceLoader.Load<PackedScene>(ENEMY_SHIELD_SCENE_PATH);

            shipName = GetNode<Label>(SHIP_NAME_NODE_PATH);
            shipIcon = GetNode<TextureRect>(SHIP_ICON_NODE_PATH);
            weaponParent = GetNode<Control>(WEAPON_PARENT_NODE_PATH);
            shieldParent = GetNode<Control>(SHIELD_PARENT_NODE_PATH);
        }

        public override void _Process(double delta)
        {
            UpdateEnemyWeapons();

            if (enemy.State == EnemyState.Firing)
            {

                if (enemy.ActiveWeaponType == AmmoType.Kinetic)
                {
                    kineticWeapon.SetTimeTillFire(enemy.Data.GetWeaponFireRate(AmmoType.Kinetic) - enemy.WeaponFireCounter);
                }

                if (enemy.ActiveWeaponType == AmmoType.Laser)
                {
                    kineticWeapon.SetTimeTillFire(enemy.Data.GetWeaponFireRate(AmmoType.Laser) - enemy.WeaponFireCounter);
                }

                if (enemy.ActiveWeaponType == AmmoType.Missile)
                {
                    missileWeapon.SetTimeTillFire(enemy.Data.GetWeaponFireRate(AmmoType.Missile) - enemy.WeaponFireCounter);
                }
            }
        }


        public void SetEnemy(EnemyController enemy)
        {
            this.enemy = enemy;

            if (this.enemy is not null)
            {
                shipName.Text = enemy.DisplayName;
                shipIcon.Texture = enemy.Icon;


                enemy.ShieldValuesChanged += OnShieldValuesChanged;

                CreateEnemyWeapons();
                UpdateEnemyWeapons();
                CreateEnemyShields();
                UpdateEnemyShields();
            }
            else
            {
                GD.Print("Assigning null enemy to ui enemy card");
            }

        }

        private void OnShieldValuesChanged()
        {
            UpdateEnemyShields();
        }

        private void CreateEnemyWeapons()
        {
            if (enemy.Data.HasLaserWeapon)
            {
                laserWeapon = enemyWeaponScene.Instantiate<UIEnemyWeapon>();
                if (laserWeapon is null)
                {
                    GD.PrintErr($"Enemy weapon scene does not have a {nameof(UIEnemyWeapon)} script attached");
                }

                weaponParent.AddChild(laserWeapon);
                laserWeapon.SetWeaponType(AmmoType.Laser);
            }

            if (enemy.Data.HasKineticWeapon)
            {
                kineticWeapon = enemyWeaponScene.Instantiate<UIEnemyWeapon>();
                if (kineticWeapon is null)
                {
                    GD.PrintErr($"Enemy weapon scene does not have a {nameof(UIEnemyWeapon)} script attached");
                }

                weaponParent.AddChild(kineticWeapon);
                kineticWeapon.SetWeaponType(AmmoType.Kinetic);
            }

            if (enemy.Data.HasMissileWeapon)
            {
                missileWeapon = enemyWeaponScene.Instantiate<UIEnemyWeapon>();
                if (missileWeapon is null)
                {
                    GD.PrintErr($"Enemy weapon scene does not have a {nameof(UIEnemyWeapon)} script attached");
                }

                weaponParent.AddChild(missileWeapon);
                missileWeapon.SetWeaponType(AmmoType.Missile);
            }
        }

        private void UpdateEnemyWeapons()
        {
            if (enemy.State != EnemyState.SelectingWeapon)
            {
                laserWeapon?.SetActive(enemy.ActiveWeaponType == AmmoType.Laser);
                kineticWeapon?.SetActive(enemy.ActiveWeaponType == AmmoType.Kinetic);
                missileWeapon?.SetActive(enemy.ActiveWeaponType == AmmoType.Missile);
            }
            else
            {
                laserWeapon?.SetActive(false);
                kineticWeapon?.SetActive(false);
                missileWeapon?.SetActive(false);
            }
        }

        private void CreateEnemyShields()
        {
            for (int i = 0; i < enemy.MaxLaserShields; i++)
            {
                var instance = enemyShieldScene.Instantiate<UIEnemyShield>();
                if (instance is null)
                {
                    GD.PrintErr("Enemy Shield scene is not of type UIEnemeyShield");
                    return;
                }

                laserShields.Add(instance);
                shieldParent.AddChild(instance);
                instance.SetShieldType(AmmoType.Laser);
            }

            for (int i = 0; i < enemy.MaxKineticShields; i++)
            {
                var instance = enemyShieldScene.Instantiate<UIEnemyShield>();
                if (instance is null)
                {
                    GD.PrintErr("Enemy Shield scene is not of type UIEnemeyShield");
                    return;
                }

                laserShields.Add(instance);
                shieldParent.AddChild(instance);
                instance.SetShieldType(AmmoType.Laser);
            }

            for (int i = 0; i < enemy.MaxMissileShields; i++)
            {
                var instance = enemyShieldScene.Instantiate<UIEnemyShield>();
                if (instance is null)
                {
                    GD.PrintErr("Enemy Shield scene is not of type UIEnemeyShield");
                    return;
                }

                laserShields.Add(instance);
                shieldParent.AddChild(instance);
                instance.SetShieldType(AmmoType.Laser);
            }
        }

        private void UpdateEnemyShields()
        {
            for (int i = 0; i < laserShields.Count; i++)
            {
                laserShields[i].SetHidden(!enemy.HasLaserShields);
                laserShields[i].SetActive(i < enemy.LaserShields);
            }

            for (int i = 0; i < kineticShields.Count; i++)
            {
                // Todo: If Sensor system is overclocked, show kinetic shields
                kineticShields[i].SetHidden(enemy.HasLaserShields);
                kineticShields[i].SetActive(i < enemy.KineticShields);
            }

            for (int i = 0; i < missileShields.Count; i++)
            {
                // Todo: If Sensor system is overclocked, show missiles shields
                missileShields[i].SetHidden(enemy.HasLaserShields);
                missileShields[i].SetActive(i < enemy.MissileShields);
            }
        }
    }
}