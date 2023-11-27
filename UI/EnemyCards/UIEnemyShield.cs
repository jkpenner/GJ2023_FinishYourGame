using System;
using Godot;

namespace SpaceEngineer
{
    public partial class UIEnemyShield : Control
    {
        private const string LASER_ICON_PATH = "LaserShield";
        private const string KINETIC_ICON_PATH = "KineticShield";
        private const string MISSILE_ICON_PATH = "MissileShield";

        private Control laserIcon;
        private Control kineticIcon;
        private Control missileIcon;

        public override void _Ready()
        {
            laserIcon = GetNode<Control>(LASER_ICON_PATH);
            kineticIcon = GetNode<Control>(KINETIC_ICON_PATH);
            missileIcon = GetNode<Control>(MISSILE_ICON_PATH);
        }

        public void SetHidden(bool isHidden)
        {
            Visible = !isHidden;
        }

        public void SetActive(bool isActive)
        {
            if (isActive)
            {
                laserIcon.SelfModulate = new Color("#FFFFFF");
                kineticIcon.SelfModulate = new Color("#FFFFFF");
                missileIcon.SelfModulate = new Color("#FFFFFF");
            }
            else
            {
                laserIcon.SelfModulate = new Color("#999999");
                kineticIcon.SelfModulate = new Color("#999999");
                missileIcon.SelfModulate = new Color("#999999");
            }
        }

        public void SetShieldType(AmmoType ammoType)
        {
            laserIcon.Visible = ammoType == AmmoType.Laser;
            kineticIcon.Visible = ammoType == AmmoType.Kinetic;
            missileIcon.Visible = ammoType == AmmoType.Missile;
        }
    }
}