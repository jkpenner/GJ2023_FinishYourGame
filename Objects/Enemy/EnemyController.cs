using Godot;

namespace SpaceEngineer
{
    public class EnemyController
    {
        public readonly EnemyData Data;

        // Expose data variables for quick access...
        public AmmoType AmmoType => (AmmoType)Data.enemyWeapon;

        public int LaserShields { get; private set; }
        public int KineticShields { get; private set; }
        public int MissileShields { get; private set; }

        public bool HasLaserShields => LaserShields != 0;
        public bool HasKineticShields => KineticShields != 0;
        public bool HasMissileShields => MissileShields != 0;

        public bool IsDestroyed => !HasLaserShields && !HasKineticShields && !HasMissileShields;

        public EnemyController(EnemyData data)
        {
            Data = data;

            foreach(var shield in data.enemyShields)
            {
                if ((AmmoType)shield == AmmoType.Kinetic)
                {
                    KineticShields += 1;
                }
                if ((AmmoType)shield == AmmoType.Missile)
                {
                    MissileShields += 1;
                }
                if ((AmmoType)shield == AmmoType.Laser)
                {   
                    LaserShields += 1;
                }   
            }

            // Initialize Enemy shields here...
        }

        /// <summary>
        /// Deal damage of the given type to the enemy.
        /// </summary>
        public void Damage(AmmoType ammoType)
        {
            switch (ammoType)
            {
                case AmmoType.Kinetic:
                    KineticShields = Mathf.Max(KineticShields - 1, 0);
                    break;
                case AmmoType.Missile:
                    MissileShields = Mathf.Max(MissileShields - 1, 0);
                    break;
                case AmmoType.Laser:
                    LaserShields = Mathf.Max(LaserShields - 1, 0);
                    break;
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

        }

        /// <summary>
        /// Called when the enemy is destroyed, before being removed.
        /// </summary>
        public void OnDestroyed()
        {

        }
    }
}