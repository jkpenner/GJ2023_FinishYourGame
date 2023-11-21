using Godot;
using System;
using System.Collections.Generic;
using SpaceEngineer;

public partial class EnemyCard : Control {

	private Texture[] shieldTextureResources = {
		GD.Load<Texture>("res://Assets/Sprites/Shields/RedShield.png"),
		GD.Load<Texture>("res://Assets/Sprites/Shields/YellowShield.png"),
		GD.Load<Texture>("res://Assets/Sprites/Shields/BlueShield.png")
	};

	private Texture[] weaponTextureResources = {
		GD.Load<Texture>("res://Assets/Sprites/Weapons/Bullet.png"),
		GD.Load<Texture>("res://Assets/Sprites/Weapons/Missile.png"),
		GD.Load<Texture>("res://Assets/Sprites/Weapons/Laser.png")
	};
	EnemyController enemy;
	EnemyData enemyData;
	private TextureRect shipImage;
	
	public override void _Ready() {
		//CreateRandomEnemyCard();
	}

	public void CreateRandomEnemyCard() {
		enemyData = new EnemyData();
		//enemyData.CreateEnemyData();
		shipImage = GetNode<TextureRect>("ShipImage");
		shipImage.Texture = (Texture2D)enemyData.enemyIcon;
		foreach (int ammoType in enemyData.enemyShields) {
			SetShields(ammoType);
		}
		SetWeapon(enemyData.enemyWeapon);
	}

	//Needs to be a signal
	public void CreateEnemyCard(int[] shields, int weapons) {
		enemyData = new EnemyData();
		enemyData.SetEnemyShieldsAndWeapons(shields,weapons);
	}
	
	public void SetEnemy(EnemyController enemy) {
		this.enemy = enemy;
		this.enemyData = enemy.Data;
		
		shipImage = GetNode<TextureRect>("ShipImage");
		shipImage.Texture = (Texture2D)enemyData.enemyIcon;
		foreach (int ammoType in enemyData.enemyShields) {
			SetShields(ammoType);
		}
		SetWeapon(enemyData.enemyWeapon);
		GD.Print("SetEnemyData");
 	}

	public void SetShields(int ammoType) {
		TextureRect textureRect = new TextureRect();
		textureRect.Texture = (Texture2D)shieldTextureResources[ammoType];
		textureRect.ExpandMode = TextureRect.ExpandModeEnum.FitWidth;
		GetNode<HBoxContainer>("ArmorContainer").AddChild(textureRect);
	}

	public void SetWeapon(int ammoType) {
		TextureRect textureRect = new TextureRect();
		textureRect.Texture = (Texture2D)weaponTextureResources[ammoType];
		textureRect.ExpandMode = TextureRect.ExpandModeEnum.FitWidthProportional;
		GetNode<HBoxContainer>("WeaponContainer").AddChild(textureRect);
	}
}
