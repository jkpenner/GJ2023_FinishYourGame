shader_type spatial;
render_mode cull_disabled;

uniform sampler2D noise;
uniform sampler2D gradient;
uniform float speed = 2.0;
uniform float wrapCompress = 10.0;

void fragment() {
	vec4 grad = texture(gradient, vec2(UV.y, 0.0));
	ALBEDO = texture(noise, vec2(UV.x * wrapCompress, UV.y) + vec2(0.0, TIME * speed)).rgb * grad.rgb;
	ALPHA = ALBEDO.x * grad.a;
}
