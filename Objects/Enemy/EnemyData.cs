using Godot;
using System;
using SpaceEngineer;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

[GlobalClass]
public partial class EnemyData : Resource
{
	private static readonly string[] Names = new string[] {
		"Fighter",
		"Interceptor",
		"Bomber",
		"Advanced",
		"Defender",
		"Phantom",
		"Reaper",
		"Striker",
		"Shuttle",
		"Boarding Craft"
	};

	private static readonly string[] TexturePaths = new string[] {
		"res://Assets/Sprites/Ships/Demon.png",
		"res://Assets/Sprites/Ships/DieFighter.png",
		"res://Assets/Sprites/Ships/F-222.png",
		"res://Assets/Sprites/Ships/Thor.png",
		"res://Assets/Sprites/Ships/Tracer.png"
	};

	[Export] string displayName;
	[Export] Texture2D icon;

	[ExportGroup("Shields")]
	[Export] int laserShields;
	[Export] int kineticShields;
	[Export] int missileShields;

	[ExportGroup("Kinetic Weapons")]
	[Export] bool hasKineticWeapon;
	[Export] float kineticWeaponFireRate;
	[Export] float kineticWeaponImpactDelay;

	[ExportGroup("Laser Weapons")]
	[Export] bool hasLaserWeapon;
	[Export] float laserWeaponFireRate;
	[Export] float laserWeaponImpactDelay;

	[ExportGroup("Missle Weapons")]
	[Export] bool hasMissileWeapon;
	[Export] float missileWeaponFireRate;
	[Export] float missileWeaponImpactDelay;

	public string DisplayName
	{
		get
		{
			// Assign a random name when none is assigned.
			if (string.IsNullOrEmpty(displayName))
			{
				Random rand = Random.Shared;
				displayName = Names[rand.Next(Names.Length)];
			}
			return displayName;
		}
		set => displayName = value;
	}

	public Texture2D Icon
	{
		get
		{
			// Assign a random texture when none is assigned.
			if (icon is null)
			{
				Random rand = Random.Shared;
				string path = TexturePaths[rand.Next(TexturePaths.Length)];
				icon = ResourceLoader.Load<Texture2D>(path);
				if (icon is null)
				{
					GD.PrintErr($"Failed to load icon at path: {path}");
				}
			}
			return icon;
		}
		set => icon = value;
	}

	public int LaserShields
	{
		get => laserShields;
		set => laserShields = Mathf.Max(value, 0);
	}

	public int KineticShields
	{
		get => kineticShields;
		set => kineticShields = Mathf.Max(value, 0);
	}

	public int MissileShields
	{
		get => missileShields;
		set => missileShields = Mathf.Max(value, 0);
	}

	public bool HasLaserWeapon
	{
		get => hasLaserWeapon;
		set => hasLaserWeapon = value;
	}

	public bool HasKineticWeapon
	{
		get => hasKineticWeapon;
		set => hasKineticWeapon = value;
	}

	public bool HasMissileWeapon
	{
		get => hasMissileWeapon;
		set => hasMissileWeapon = value;
	}

	public float LaserWeaponFireRate
	{
		get => laserWeaponFireRate;
		set => laserWeaponFireRate = Mathf.Max(value, 0f);
	}
	public float KineticWeaponFireRate
	{
		get => kineticWeaponFireRate;
		set => kineticWeaponFireRate = Mathf.Max(value, 0f);
	}
	public float MissileWeaponFireRate
	{
		get => missileWeaponFireRate;
		set => missileWeaponFireRate = Mathf.Max(value, 0f);
	}

	public float LaserWeaponImpactDelay
	{
		get => laserWeaponImpactDelay;
		set => laserWeaponImpactDelay = Mathf.Max(value, 0f);
	}
	public float KineticWeaponImpactDelay
	{
		get => kineticWeaponImpactDelay;
		set => kineticWeaponImpactDelay = Mathf.Max(value, 0f);
	}
	public float MissileWeaponImpactDelay
	{
		get => missileWeaponImpactDelay;
		set => missileWeaponImpactDelay = Mathf.Max(value, 0f);
	}

	public static EnemyData CreateRandomEnemy()
	{
		EnemyData enemy = new EnemyData();

		Random rand = Random.Shared;

		enemy.displayName = Names[rand.Next(Names.Length)];

		string path = TexturePaths[rand.Next(TexturePaths.Length)];
		enemy.icon = ResourceLoader.Load<Texture2D>(path);
		if (enemy.icon is null)
		{
			GD.PrintErr($"Failed to load icon at path: {path}");
		}

		enemy.laserShields = rand.Next(0, 4);
		enemy.kineticShields = rand.Next(0, 4);
		enemy.missileShields = rand.Next(0, 4);

		enemy.hasLaserWeapon = rand.Next(0, 2) > 0;
		enemy.hasKineticWeapon = rand.Next(0, 2) > 0;
		enemy.hasMissileWeapon = rand.Next(0, 2) > 0;

		// Random fire rate between 5s and 15s
		enemy.laserWeaponFireRate = (rand.NextSingle() * 10f) + 5f;
		enemy.kineticWeaponFireRate = (rand.NextSingle() * 10f) + 5f;
		enemy.missileWeaponFireRate = (rand.NextSingle() * 10f) + 5f;

		enemy.laserWeaponImpactDelay = rand.NextSingle() * 2f;
		enemy.kineticWeaponImpactDelay = (rand.NextSingle() * 3f) + 2f;
		enemy.missileWeaponImpactDelay = (rand.NextSingle() * 5f) + 5f;

		return enemy;
	}

	public bool TryGetRandomWeapon(out AmmoType ammoType)
	{
		Random rand = Random.Shared;

		if (hasKineticWeapon && hasMissileWeapon && hasLaserWeapon)
		{
			ammoType = (AmmoType)rand.Next(0, 3);
			return true;
		}
		else if (hasKineticWeapon && hasMissileWeapon)
		{
			ammoType = rand.Next(0, 2) > 0 ? AmmoType.Kinetic : AmmoType.Missile;
			return true;
		}
		else if (hasKineticWeapon && hasLaserWeapon)
		{
			ammoType = rand.Next(0, 2) > 0 ? AmmoType.Kinetic : AmmoType.Laser;
			return true;
		}
		else if (hasMissileWeapon && hasLaserWeapon)
		{
			ammoType = rand.Next(0, 2) > 0 ? AmmoType.Missile : AmmoType.Laser;
			return true;
		}
		else if (hasKineticWeapon)
		{
			ammoType = AmmoType.Kinetic;
			return true;
		}
		else if (hasMissileWeapon)
		{
			ammoType = AmmoType.Missile;
			return true;
		}
		else if (hasLaserWeapon)
		{
			ammoType = AmmoType.Laser;
			return true;
		}
		else
		{
			ammoType = AmmoType.Kinetic;
			return false;
		}
	}

	/// <summary>
	/// Check if the enemy has a weapon for the specific type.
	/// </summary>
	public bool HasWeaponType(AmmoType ammoType)
	{
		return ammoType switch
		{
			AmmoType.Laser => HasLaserWeapon,
			AmmoType.Kinetic => HasKineticWeapon,
			AmmoType.Missile => HasMissileWeapon,
			_ => false
		};
	}

	/// <summary>
	/// Get the fire rate for the specific ammo type.
	/// </summary>
	public float GetWeaponFireRate(AmmoType ammoType)
	{
		return ammoType switch
		{
			AmmoType.Laser => LaserWeaponFireRate,
			AmmoType.Kinetic => KineticWeaponFireRate,
			AmmoType.Missile => MissileWeaponFireRate,
			_ => 10f
		};
	}

	/// <summary>
	/// Get the impact delay for the specific ammo type.
	/// </summary>
	public float GetWeaponImpactDelay(AmmoType ammoType)
	{
		return ammoType switch
		{
			AmmoType.Laser => LaserWeaponImpactDelay,
			AmmoType.Kinetic => KineticWeaponImpactDelay,
			AmmoType.Missile => MissileWeaponImpactDelay,
			_ => 1f
		};
	}
}
