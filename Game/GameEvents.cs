namespace SpaceEngineer
{
    public static class GameEvents
    {
        /// <summary>
        /// Invoked when an inpact ship event occurs.
        /// </summary>
        public static readonly GameEvent Impact = new GameEvent();

        /// <summary>
        /// Occurs when the energy usage for all active system exceeds the ship's energy capacity
        /// and the ship is about to be overloaded.
        /// </summary>
        public static readonly GameEvent ShipEnergyOverloading = new GameEvent();
        
        /// <summary>
        /// Occurs when the energy usage for all active system exceeds the ship's energy capacity
        /// for an extended period of time.
        /// </summary>
        public static readonly GameEvent ShipEnergyOverloaded = new GameEvent();
        
        /// <summary>
        /// Occurs when the energy usage falls back below the ship's capacity after entering
        /// an overloading state.
        /// </summary>
        public static readonly GameEvent ShipEnergyNormalized = new GameEvent();

        /// <summary>
        /// Occurs when a system is damaged.
        /// </summary>
        public static readonly GameEvent<ShipSystem> SystemDamaged = new GameEvent<ShipSystem>();

        /// <summary>
        /// Occurs when a system is destoyed.
        /// </summary>
        public static readonly GameEvent<ShipSystem> SystemDestroyed = new GameEvent<ShipSystem>();
    }
}