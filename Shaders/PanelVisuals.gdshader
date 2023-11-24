shader_type spatial;

uniform vec4 main_color : source_color = vec4( 1.0, 0.75, 0.3, 1.0 );
uniform vec4 base_color : source_color = vec4( 1.0, 0.03, 0.001, 1.0 );

uniform float fade = 0.5;
uniform float fade_size = 0.1;

uniform bool pulse_main = false;
uniform vec3 main_pulse_color: source_color = vec3(1.0, 1.0, 1.0);

uniform bool pulse_base = false;
uniform vec3 base_pulse_color: source_color = vec3(1.0, 1.0, 1.0);

uniform float pulse_rate = 3.0;

void fragment() {
	ALBEDO = main_color.rgb * (UV.y) + base_color.rgb * (1.0 - UV.y);
	
	vec3 main = main_color.rgb;
	vec3 base = base_color.rgb;
	
	float pulse_t = ((sin(TIME * pulse_rate) + 1.0) * 0.5);
	
	if (pulse_main)
	{
		main = main * pulse_t + main_pulse_color * (1.0 - pulse_t);
	}
	
	if (pulse_base)
	{
		base = base * pulse_t + base_pulse_color * (1.0 - pulse_t);
	}
	
	if (fade_size == 0.0)
	{
		if (fade < UV.y)
		{
			ALBEDO = main;
		}
		else
		{
			ALBEDO = base;
		}
	}
	else
	{
		// Adjust the fade value to make it so only one color is visible when
		// fade is set to 0.0 or 1.0.
		float fade_t = (fade * (1.0 + fade_size)) - (fade_size * 0.5);
		float min_fade = fade_t - (fade_size * 0.5);
		float max_fade = fade_t + (fade_size * 0.5);
		
		float t = clamp((UV.y - min_fade) / (max_fade - min_fade), 0.0, 1.0);
		ALBEDO = main * t + base * (1.0 - t);
	}
}
