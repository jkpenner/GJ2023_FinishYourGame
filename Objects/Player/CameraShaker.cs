using Godot;
using System;

public partial class CameraShaker : Camera3D
{
	[Export] float decay = 0.6f;
	[Export] Vector2 maxOffset = new Vector2(0.1f, 0.05f);
	[Export] float maxRoll = 0.05f;
	
	[Export] float trauma = 0f;
	[Export] float traumaPower = 2.5f;

	[Export] bool debugTest = false;
	[Export] float debugTrauma = 10f;

    public override void _Ready()
    {
		GD.Randomize();
    }

    public override void _Process(double delta)
    {
		if (debugTest)
		{
			debugTest = false;
			AddTrauma(debugTrauma);
		}

        if (trauma > 0.0f)
		{
			trauma = Mathf.Max(trauma - (decay * (float)delta), 0f);
			Shake();
		}
    }

    private void Shake()
    {
        float amount = Mathf.Pow(trauma, traumaPower);

		var rot = Rotation;
		rot.Z = maxRoll * amount * (float)GD.RandRange(-1f, 1f);
		Rotation = rot;

		var pos = Position;
		pos.X = maxOffset.X * amount * (float)GD.RandRange(-1f, 1f);
		pos.Y = maxOffset.Y * amount * (float)GD.RandRange(-1f, 1f);
		Position = pos;
    }

    public void AddTrauma(float amount)
	{
		trauma = Mathf.Min(trauma + amount, 1.0f);
	}
}
