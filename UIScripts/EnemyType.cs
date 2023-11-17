using Godot;
using System;
using System.Collections.Generic;
using SpaceEngineer;

public class EnemyType {
	
	public string enemyName;
	public Texture enemyIcon;
	public List<AmmoType> enemyShields;
	public AmmoType enemyWeapon;
	
	private string[] names = {"Fighter","Interceptor","Bomber","Advanced","Defender","Phantom","Reaper","Striker","Shuttle","Boarding Craft"};
	
	private Texture[] shipTextureResources = {
		GD.Load<Texture>("res://Assets/Sprites/Ships/Demon.png"),
		GD.Load<Texture>("res://Assets/Sprites/Ships/DieFighter.png"),
		GD.Load<Texture>("res://Assets/Sprites/Ships/F-222.png"),
		GD.Load<Texture>("res://Assets/Sprites/Ships/Thor.png"),
		GD.Load<Texture>("res://Assets/Sprites/Ships/Tracer.png")
		
	};

	public EnemyType() {
		enemyShields = new List<AmmoType>();
		Random random = new Random();
		enemyName = names[random.Next(names.Length)];
		enemyIcon = shipTextureResources[random.Next(shipTextureResources.Length)];
		Array values = Enum.GetValues(typeof(AmmoType));
		
		for (int i = 0; i < 3; i++) {
			enemyShields.Add((AmmoType)values.GetValue(random.Next(values.Length))!);
		}
		enemyWeapon = (AmmoType)values.GetValue(random.Next(values.Length))!;
	}
}
