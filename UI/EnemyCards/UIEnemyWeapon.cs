using System;
using Godot;

namespace SpaceEngineer
{
    public partial class UIEnemyWeapon : Control
    {
        private const string LASER_ICON_PATH = "LaserAmmo";
        private const string KINETIC_ICON_PATH = "KineticAmmo";
        private const string MISSILE_ICON_PATH = "MissleAmmo";
        private const string ACTIVE_INDICATOR_PATH = "ActiveBorder";

        private Control activeIndicator;
        private Control laserIcon;
        private Control kineticIcon;
        private Control missileIcon;
        public override void _Ready()
        {
            activeIndicator = GetNode<Control>(ACTIVE_INDICATOR_PATH);
            laserIcon = GetNode<Control>(LASER_ICON_PATH);
            kineticIcon = GetNode<Control>(KINETIC_ICON_PATH);
            missileIcon = GetNode<Control>(MISSILE_ICON_PATH);
        }

        public void SetWeaponType(AmmoType ammoType)
        {
            laserIcon.Visible = ammoType == AmmoType.Laser;
            kineticIcon.Visible = ammoType == AmmoType.Kinetic;
            missileIcon.Visible = ammoType == AmmoType.Missile;
        }

        public void SetActive(bool isActive)
        {
            activeIndicator.Visible = isActive;
        }
    }
}