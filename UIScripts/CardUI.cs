using Godot;
using System;
using System.Collections.Generic;
using SpaceEngineer;

public partial class CardUI : Control {
	
	[Export] private int numberOfEnemies = 3;
	PackedScene packedScene = (PackedScene)ResourceLoader.Load("res://Scenes/EnemyCard.tscn");

	public override void _EnterTree() {
		base._EnterTree();
		GameEvents.EnemySpawned.Connect(AddEnemy);
		GameEvents.EnemyDestroy.Connect(AddEnemy);
	}
	
	public override void _ExitTree() {
		base._ExitTree();
		GameEvents.EnemySpawned.Disconnect(AddEnemy);
		GameEvents.EnemyDestroy.Disconnect(AddEnemy);
	}

	public override void _Ready() {
        //AddEnemy();
	}
	
	public void AddEnemy() {
		Node enemyCardNode = packedScene.Instantiate();
		if (enemyCardNode is EnemyCard enemyCard) {
			enemyCard.CreateRandomEnemyCard();
			GetNode<HBoxContainer>("EnemyCardContainer").AddChild(enemyCardNode);
		}
	}
	
	public void AddEnemy(EnemyData enemyData) {
		Node enemyNode = packedScene.Instantiate();
		if (enemyNode is EnemyCard enemyCard) {
			enemyCard.SetEnemyData(enemyData);
			GetNode<HBoxContainer>("EnemyCardContainer").AddChild(enemyNode);
		}
	}
	
	public void RemoveEnemy(EnemyData enemyData) {
		//TODO
		//GetNode<HBoxContainer>("EnemyCardContainer").AddChild(enemyNode);
	}
}
