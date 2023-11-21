using System;
using Godot;

namespace SpaceEngineer
{
    public enum EnemyState
    {
        SelectingWeapon,
        Firing,
        WaitingForImpact,
    }


    public class EnemyController
    {
        public readonly EnemyData Data;

        public string DisplayName => Data.DisplayName;
        public Texture2D Icon => Data.Icon;
        public int MaxLaserShields => Data.LaserShields;
        public int MaxKineticShields => Data.KineticShields;
        public int MaxMissileShields => Data.MissileShields;

        public EnemyState State { get; private set; }
        public ShipController Target { get; private set; }

        public int LaserShields { get; private set; }
        public int KineticShields { get; private set; }
        public int MissileShields { get; private set; }

        public bool HasLaserShields => LaserShields != 0;
        public bool HasKineticShields => KineticShields != 0;
        public bool HasMissileShields => MissileShields != 0;

        public bool IsDestroyed => !HasLaserShields && !HasKineticShields && !HasMissileShields;

        public AmmoType ActiveWeaponType { get; private set; }

        public float WeaponFireCounter { get; private set; }
        public float WeaponImpactCounter { get; private set; }

        public event Action ShieldValuesChanged;
        public event Action Destroyed;

        public EnemyController(EnemyData data, ShipController target)
        {
            State = EnemyState.Firing;
            Target = target;

            Data = data;
            LaserShields = MaxLaserShields;
            KineticShields = MaxKineticShields;
            MissileShields = MaxMissileShields;
        }

        /// <summary>
        /// Deal damage of the given type to the enemy.
        /// </summary>
        public void Damage(AmmoType ammoType)
        {
            if (IsDestroyed)
            {
                return;
            }

            if (ammoType == AmmoType.Kinetic && HasKineticShields)
            {
                KineticShields = Mathf.Max(KineticShields - 1, 0);
                ShieldValuesChanged?.Invoke();
            }
            else if (ammoType == AmmoType.Missile && HasMissileShields)
            {
                MissileShields = Mathf.Max(MissileShields - 1, 0);
                ShieldValuesChanged?.Invoke();
            }
            else if (ammoType == AmmoType.Laser && HasLaserShields)
            {
                LaserShields = Mathf.Max(LaserShields - 1, 0);
                ShieldValuesChanged?.Invoke();
            }

            if (IsDestroyed)
            {
                Destroyed?.Invoke();
            }
        }

        /// <summary>
        /// Called when the enemy is spawned into the game.
        /// </summary>
        public void OnSpawned()
        {

        }

        /// <summary>
        /// Called each process of the Game Manager.
        /// </summary>
        public void Process(double delta)
        {
            if (State == EnemyState.SelectingWeapon)
            {
                if (Data.TryGetRandomWeapon(out AmmoType ammoType))
                {
                    GD.Print($"Enemy Selected {ActiveWeaponType}");
                    ActiveWeaponType = ammoType;
                    State = EnemyState.Firing;
                }
            }
            else if (State == EnemyState.Firing)
            {
                WeaponFireCounter += (float)delta;
                if (WeaponFireCounter >= Data.GetWeaponFireRate(ActiveWeaponType))
                {
                    GD.Print($"Enemy Firing Weapon {ActiveWeaponType}");
                    State = EnemyState.WaitingForImpact;
                    WeaponFireCounter = 0f;
                }
            }
            else if (State == EnemyState.WaitingForImpact)
            {
                WeaponImpactCounter += (float)delta;
                if (WeaponImpactCounter >= Data.GetWeaponImpactDelay(ActiveWeaponType))
                {
                    GD.Print($"Enemy {ActiveWeaponType} Impacting");
                    Target.Damage(ActiveWeaponType);
                    State = EnemyState.SelectingWeapon;
                    WeaponImpactCounter = 0f;
                }
            }
        }


        /// <summary>
        /// Called when the enemy is destroyed, before being removed.
        /// </summary>
        public void OnDestroyed()
        {

        }

        public float GetWeaponFirePercent()
        {
            float fireRate = Data.GetWeaponFireRate(ActiveWeaponType);
            return Mathf.Clamp(WeaponFireCounter / fireRate, 0f, 1f);
        }

        public float GetWeaponImpactPercent()
        {
            float impactDelay = Data.GetWeaponImpactDelay(ActiveWeaponType);
            return Mathf.Clamp(WeaponImpactCounter / impactDelay, 0f, 1f);
        }
    }
}