using System;
using System.Collections;
using System.Collections.Generic;
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
        Restarting,

    }

    public partial class GameManager : Node
    {
        private const string MAIN_MENU_SCENE = "res://Scenes/Main.tscn";

        [Export] UIFade fade;
        [Export] PlayerController player;
        [Export] PlayerCamera camera;
        [Export] ShipController playerShip;
        [Export] GameEncounter encoutner;


        public GameState State { get; private set; }
        public bool IsGameActive => State == GameState.Active;

        public PlayerController Player => player;
        public PlayerCamera Camera => camera;
        public ShipController PlayerShip => playerShip;

        public List<EnemyController> Enemies { get; private set; } = new List<EnemyController>();

        public int SpawnInfoIndex { get; private set; }
        public EnemySpawnInfo SpawnInfo { get; private set; }
        public float SpawnCounter { get; private set; }


        public GameManager()
        {
            State = GameState.Initializing;
        }

        public override void _Ready()
        {
            RegisterGlobalEvents();

            PlayerShip.HullDamaged += OnHullDamaged;
            PlayerShip.ShieldBroken += OnShieldBroken;

            // Inform game that everything is setup and ready to go.
            SetGameState(GameState.Starting);

            fade.FadeInCompleted += OnFadeInCompleted;
            fade.FadeOutCompleted += OnFadeOutCompleted;
        }

        private void OnShieldBroken()
        {
            Camera.Shaker.AddTrauma(3f);
        }


        private void OnHullDamaged(DamagableHull hull)
        {
            Camera.Shaker.AddTrauma(hull.State switch
            {
                HullState.Damaged => 6f,
                HullState.Breached => 12f,
                _ => throw new NotImplementedException(),
            });
        }


        private void OnFadeInCompleted()
        {
            SetGameState(GameState.Active);
        }


        private void OnFadeOutCompleted()
        {
            if (State == GameState.Exiting)
            {
                GetTree().ChangeSceneToFile(MAIN_MENU_SCENE);
            }
            else if (State == GameState.Restarting)
            {
                GetTree().ReloadCurrentScene();
            }
        }


        public override void _ExitTree()
        {
            PlayerShip.HullDamaged -= OnHullDamaged;
            UnregisterGlobalEvents();
        }

        public override void _Process(double delta)
        {
            if (Input.IsActionJustPressed("Exit"))
            {
                Exit();
            }

            switch (State)
            {
                case GameState.Active:
                    ActiveStateProcess(delta);
                    break;
            }
        }

        private void ActiveStateProcess(double delta)
        {
            // Get the next enemy to spawn
            if (SpawnInfo is null && SpawnInfoIndex < encoutner.EnemySpawns.Count)
            {
                SpawnInfo = encoutner.EnemySpawns[SpawnInfoIndex];
                if (SpawnInfo.WaitForAllEnemiesDefeated && Enemies.Count > 0)
                {
                    // Skip if other enemies are still active.
                    SpawnInfo = null;
                }

                if (SpawnInfo is not null)
                {
                    SpawnCounter = 0f;
                    SpawnInfoIndex += 1;
                }
            }

            // Wait for the spawn delay then spawn the enemy
            if (SpawnInfo is not null)
            {
                SpawnCounter += (float)delta;
                if (SpawnCounter >= SpawnInfo.SpawnDelay)
                {
                    SpawnEnemy(SpawnInfo.Data);
                    SpawnInfo = null;
                    SpawnCounter = 0f;
                }
            }

            // Process all non destroyed enemies.
            foreach (var enemy in Enemies)
            {
                if (!enemy.IsDestroyed)
                {
                    enemy.Process(delta);
                }
            }

            // Remove all destroyed enemies.
            for (int i = Enemies.Count - 1; i >= 0; i--)
            {
                if (Enemies[i].IsDestroyed)
                {
                    Enemies[i].OnDestroyed();
                    GameEvents.EnemyDestroy.Emit(Enemies[i]);
                    Enemies.RemoveAt(i);
                }
            }

            // Check if all enemies in the encounter are destroyed.
            if (Enemies.Count == 0 && SpawnInfoIndex == encoutner.EnemySpawns.Count && SpawnInfo is null)
            {
                TriggerVictory();
            }
        }

        public void SpawnEnemy(EnemyData enemyData)
        {
            var enemy = new EnemyController(enemyData, PlayerShip);
            Enemies.Add(enemy);
            enemy.OnSpawned();
            GameEvents.EnemySpawned.Emit(enemy);
        }

        private void SetGameState(GameState state)
        {
            if (State == state)
            {
                return;
            }

            GameEvents.GameStateExited.Emit(State);
            State = state;
            GameEvents.GameStateEntered.Emit(State);
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
            fade.FadeOut();
        }

        public void Restart()
        {
            SetGameState(GameState.Restarting);
            fade.FadeOut();
        }

        private void RegisterGlobalEvents()
        {
            if (PlayerShip is not null)
            {
                PlayerShip.LifeSupportDepleted += OnLifeSupportDepleted;
                PlayerShip.LifeSupportRestored += GameEvents.LifeSupportRestored.Emit;
                PlayerShip.LifeSupportDepleting += GameEvents.LifeSupportDepleting.Emit;

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
                PlayerShip.LifeSupportDepleted -= OnLifeSupportDepleted;
                PlayerShip.LifeSupportRestored -= GameEvents.LifeSupportRestored.Emit;
                PlayerShip.LifeSupportDepleting -= GameEvents.LifeSupportDepleting.Emit;

                PlayerShip.SystemStateChanged -= GameEvents.ShipSystemStateChanged.Emit;
                PlayerShip.EnergyUsageChanged -= GameEvents.ShipEnergyUsageChanged.Emit;
                PlayerShip.EnergyCapacityChanged -= GameEvents.ShipEnergyCapacityChanged.Emit;

                PlayerShip.Overloading -= GameEvents.ShipEnergyOverloading.Emit;
                PlayerShip.OverloadEventStarted -= GameEvents.ShipEnergyOverloaded.Emit;
                PlayerShip.EnergyNormalized -= GameEvents.ShipEnergyNormalized.Emit;
            }
        }

        private void OnLifeSupportDepleted()
        {
            TriggerGameOver();
        }
    }
}