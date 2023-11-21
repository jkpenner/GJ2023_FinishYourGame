using System.Collections.Generic;
using Godot;

namespace SpaceEngineer
{
    public partial class UIEnemyCardController : Control
    {
        private const string ENEMY_CARD_PARENT_PATH = "EnemyCardContainer";

        [Export] PackedScene enemyCardScene;

        private Control enemyCardParent;
        private Dictionary<EnemyController, UIEnemyCard> enemyCards = new Dictionary<EnemyController, UIEnemyCard>();

        public override void _Ready()
        {
            enemyCardParent = GetNode<Control>(ENEMY_CARD_PARENT_PATH);
            if (enemyCardParent is null)
            {
                this.PrintMissingChildError(ENEMY_CARD_PARENT_PATH, nameof(Control));
            }
        }

        public override void _EnterTree()
        {
            GameEvents.EnemySpawned.Connect(AddEnemy);
            GameEvents.EnemyDestroy.Connect(RemoveEnemy);
        }

        public override void _ExitTree()
        {
            GameEvents.EnemySpawned.Disconnect(AddEnemy);
            GameEvents.EnemyDestroy.Disconnect(RemoveEnemy);
        }

        public void AddEnemy(EnemyController enemy)
        {
            var enemyCard = enemyCardScene.Instantiate<UIEnemyCard>();
            if (enemyCard is null)
            {
                GD.PrintErr($"Assigned enemy card scene is not of type {nameof(UIEnemyCard)}");
                return;
            }

            enemyCards.Add(enemy, enemyCard);
            enemyCardParent.AddChild(enemyCard);

            enemyCard.SetEnemy(enemy);
        }

        public void RemoveEnemy(EnemyController enemy)
        {
            if (enemyCards.TryGetValue(enemy, out UIEnemyCard enemyCard))
            {
                enemyCard.QueueFree();
                enemyCards.Remove(enemy);
            }
        }
    }
}