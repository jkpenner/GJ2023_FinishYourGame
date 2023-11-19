using Godot;
using System;
using SpaceEngineer;

[GlobalClass]
public partial class EnemyData :Resource {
	
	[Export] public string enemyName;
	[Export] public Texture enemyIcon;
	[Export] public int[] enemyShields;
	[Export] public int enemyWeapon;
	
	private string[] names = {"Fighter","Interceptor","Bomber","Advanced","Defender","Phantom","Reaper","Striker","Shuttle","Boarding Craft"};
	
	private Texture[] shipTextureResources = {
		GD.Load<Texture>("res://Assets/Sprites/Ships/Demon.png"),
		GD.Load<Texture>("res://Assets/Sprites/Ships/DieFighter.png"),
		GD.Load<Texture>("res://Assets/Sprites/Ships/F-222.png"),
		GD.Load<Texture>("res://Assets/Sprites/Ships/Thor.png"),
		GD.Load<Texture>("res://Assets/Sprites/Ships/Tracer.png")
	};
	
	public EnemyData() {
		Random random = new Random();
		enemyShields = new int[3];
		GD.Print("bam");
		enemyName = names[random.Next(names.Length)];
		enemyIcon = enemyIcon = shipTextureResources[random.Next(shipTextureResources.Length)];
		for (int i = 0; i < 3; i++) {
			enemyShields[i] = random.Next(0, 2);
		}
		enemyWeapon = random.Next(0, 2);
	}

	public void CreateEnemyData() {
		enemyShields = new int[3];
		Random random = new Random();
		enemyName = names[random.Next(names.Length)];
		GD.Print("bam");
		enemyIcon = shipTextureResources[random.Next(shipTextureResources.Length)];
		for (int i = 0; i < 3; i++) {
			enemyShields[i] = random.Next(0, 2);
		}
		enemyWeapon = random.Next(0, 2);
	}
	
	// public void CreateEnemyData(int[] shields, int weapon) {
	// 	Random random = new Random();
	// 	enemyName = names[random.Next(names.Length)];
	// 	enemyIcon = shipTextureResources[random.Next(shipTextureResources.Length)];
	// 	enemyShields = shields;
	// 	enemyWeapon = weapon;
	// }
	
	public void SetEnemyShieldsAndWeapons(int[] shields, int weapon) {
		enemyShields = shields;
		enemyWeapon = weapon;
	}
}
