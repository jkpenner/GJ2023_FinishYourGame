using Godot;
using System;
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
	EnemyType enemyType;
	private TextureRect shipImage;
	//private 
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		CreateEnemyCard();
		
	}

	public void CreateEnemyCard() {
		enemyType = new EnemyType();
		shipImage = GetNode<TextureRect>("ShipImage");
		shipImage.Texture = (Texture2D)enemyType.enemyIcon;
		foreach (AmmoType ammoType in enemyType.enemyShields) {
			AddShields(ammoType);
		}
		AddWeapon(enemyType.enemyWeapon);
	}

	private void AddShields(AmmoType ammoType) {
		TextureRect textureRect = new TextureRect();
		textureRect.Texture = (Texture2D)shieldTextureResources[(int)ammoType];
		textureRect.ExpandMode = TextureRect.ExpandModeEnum.FitWidth;
		GetNode<HBoxContainer>("ArmorContainer").AddChild(textureRect);
	}

	private void AddWeapon(AmmoType ammoType) {
		TextureRect textureRect = new TextureRect();
		textureRect.Texture = (Texture2D)weaponTextureResources[(int)ammoType];
		textureRect.ExpandMode = TextureRect.ExpandModeEnum.FitWidthProportional;
		GetNode<HBoxContainer>("WeaponContainer").AddChild(textureRect);
	}
	
	
	
	
}
