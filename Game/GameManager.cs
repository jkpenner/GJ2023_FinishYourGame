using System;
using Godot;

namespace SpaceEngineer
{
    public enum GameState
    {
        Initializing,
        Starting,
        Active,
        Paused,
        Victory,
        GameOver,
        Exiting,
    }

    public partial class GameManager : Node
    {
        [Export] Resource enemyResource;
        [Export] ShipController playerShip;

        [ExportGroup("Life Support")]
        [Export] float lifeSupportDuration = 20f;

        [ExportGroup("Debug")]
        [Export] bool debugSimulateHullBreach = false;
        [Export] bool debugDisableLifeSupport = false;

        public GameState State { get; private set; }
        public bool IsGameActive => State == GameState.Active;
        public ShipController PlayerShip => playerShip;

        /// <summary>
        /// Check if the life support is currently failing and the timer is running.
        /// </summary>
        public bool IsLifeSupportDepleting { get; private set; }

        /// <summary>
        /// The maximum amount of time the life support can remain active.
        /// </summary>
        public float LifeSupportMaxDuration => lifeSupportDuration;

        /// <summary>
        /// The current remaining amount of time till life support is depleted and game over.
        /// </summary>
        public float LifeSupportRemainingTime => lifeSupportDuration - lifeSupportCounter;

        

        private float lifeSupportCounter;

        public GameManager()
        {
            State = GameState.Initializing;
        }

        public override void _Ready()
        {
            RegisterGlobalEvents();

            // Inform game that everything is setup and ready to go.
            SetGameState(GameState.Starting);
        }

        public override void _ExitTree()
        {
            UnregisterGlobalEvents();
        }

        public override void _Process(double delta)
        {
            switch (State)
            {
                case GameState.Active:
                    ActiveStateProcess(delta);
                    break;
            }
        }

        private void ActiveStateProcess(double delta)
        {
            ProcessLifeSupport(delta);
        }

        private void ProcessLifeSupport(double delta) {
            if (debugDisableLifeSupport)
            {
                if (IsLifeSupportDepleting)
                {
                    IsLifeSupportDepleting = false;
                    lifeSupportCounter = 0f;
                    GameEvents.LifeSupportRestored.Emit();
                }

                return;
            }

            // Life Support starts to deplete if there are any breaches in the ship's hull. When the
            // ship's shields are overclocked the life support remains stable and does not deplete.
            if ((PlayerShip.CheckForHullBreach() && PlayerShip.ShieldSystem.State != ShipSystemState.Overclocked) || debugSimulateHullBreach)
            {
                if (!IsLifeSupportDepleting)
                {
                    IsLifeSupportDepleting = true;
                    GameEvents.LifeSupportDepleting.Emit();
                }
            }
            else if (IsLifeSupportDepleting)
            {
                IsLifeSupportDepleting = false;
                lifeSupportCounter = 0f;
                GameEvents.LifeSupportRestored.Emit();
            }

            if (IsLifeSupportDepleting)
            {
                lifeSupportCounter += (float)delta;
                if (lifeSupportCounter >= lifeSupportDuration)
                {
                    GD.Print("Life support ran out.");
                    TriggerGameOver();
                }
            }
        }

        private void SetGameState(GameState state)
        {
            if (State == state)
            {
                return;
            }

            GameEvents.GameStateExited.Emit(State);
            State = state;
            GD.Print($"Entering Game State: {State}");
            GameEvents.GameStateEntered.Emit(State);
            OnGameStateEntered();
        }

        private void OnGameStateEntered()
        {
            switch (State)
            {
                case GameState.Starting:
                    // Todo: Maybe do a count down or something in future, for now just start the game
                    SetGameState(GameState.Active);
                    break;
                case GameState.Active:
                    GD.Print(enemyResource);
                    SpawnEnemy(enemyResource as EnemyData);
                    break;
            }
        }

        public void TogglePause()
        {
            if (State == GameState.Active)
            {
                SetGameState(GameState.Paused);
            }
            else if (State == GameState.Paused)
            {
                SetGameState(GameState.Active);
            }
        }

        /// <summary>
        /// Trigger the game to display the game over state.
        /// </summary>
        public void TriggerGameOver()
        {
            SetGameState(GameState.GameOver);
        }

        /// <summary>
        /// Trigger the game to display the victory state.
        /// </summary>
        public void TriggerVictory()
        {
            SetGameState(GameState.Victory);
        }

        /// <summary>
        /// Will place the game in the exiting state. This should
        /// return the game back to the main menu.
        /// </summary>
        public void Exit()
        {
            SetGameState(GameState.Exiting);
        }

        public void SpawnEnemy(EnemyData enemyData){
            GameEvents.EnemySpawned.Emit(enemyData);
            GameEvents.EnemyDestroy.Emit(enemyData);
        }

        private void RegisterGlobalEvents()
        {
            if (PlayerShip is not null)
            {
                PlayerShip.SystemStateChanged += GameEvents.ShipSystemStateChanged.Emit;
                PlayerShip.EnergyUsageChanged += GameEvents.ShipEnergyUsageChanged.Emit;
                PlayerShip.EnergyCapacityChanged += GameEvents.ShipEnergyCapacityChanged.Emit;

                PlayerShip.Overloading += GameEvents.ShipEnergyOverloading.Emit;
                PlayerShip.OverloadEventStarted += GameEvents.ShipEnergyOverloaded.Emit;
                PlayerShip.EnergyNormalized += GameEvents.ShipEnergyNormalized.Emit;
            }
        }

        private void UnregisterGlobalEvents()
        {
            if (PlayerShip is not null)
            {
                PlayerShip.SystemStateChanged -= GameEvents.ShipSystemStateChanged.Emit;
                PlayerShip.EnergyUsageChanged -= GameEvents.ShipEnergyUsageChanged.Emit;
                PlayerShip.EnergyCapacityChanged -= GameEvents.ShipEnergyCapacityChanged.Emit;

                PlayerShip.Overloading -= GameEvents.ShipEnergyOverloading.Emit;
                PlayerShip.OverloadEventStarted -= GameEvents.ShipEnergyOverloaded.Emit;
                PlayerShip.EnergyNormalized -= GameEvents.ShipEnergyNormalized.Emit;
            }
        }
    }
}