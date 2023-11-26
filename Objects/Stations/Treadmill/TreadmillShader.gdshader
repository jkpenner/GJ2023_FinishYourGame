shader_type spatial;

uniform vec4 main_color : source_color = vec4(0.0, 1.0, 1.0, 1.0 );
uniform float speed = 0.0;
uniform float size = 80.0;

void fragment() {
	float delta = TIME * speed;
	float shift = UV.y * size;
	ALBEDO = main_color.rgb * (sin(shift + delta) + 1.0) * 0.5;
}
