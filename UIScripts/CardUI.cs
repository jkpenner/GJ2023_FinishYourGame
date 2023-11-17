using Godot;
using System;

public partial class CardUI : Control {
	[Export] private int numberOfEnemies = 3;
	PackedScene packedScene = (PackedScene)ResourceLoader.Load("res://Scenes/EnemyCard.tscn"); 
	public override void _Ready() {
		base._Ready();
		for (int i = 0; i < numberOfEnemies; i++) {
			AddEnemy();
		}
	}
	
	public void AddEnemy() {
		Node enemyCard = packedScene.Instantiate();
		GetNode<HBoxContainer>("EnemyCardContainer").AddChild(enemyCard);
	}
	
}
