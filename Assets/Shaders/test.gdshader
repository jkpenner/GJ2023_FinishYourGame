shader_type spatial;

void fragment() {
	ALBEDO = vec3(.1,.2,1.0);
	RIM = 1.0;
	ROUGHNESS = 1.0;
	
	/*if(UV.x > 0.5){
		ALBEDO =vec3(0,.2,1);		
		RIM = 1.0;
		ROUGHNESS = .9;
	} else {
		ALBEDO = vec3(1,.2,0);
		RIM = 1.0;
		ROUGHNESS = .2;
	}*/
	
	vec3 color;
	//color.r = sin(TIME *5.0 + VERTEX.x) +1.0;
	//color.g = sin(TIME *6.0 + VERTEX.x) +1.0;
	//color.b = sin(TIME *7.0 + VERTEX.x) + 1.0;
	//ALBEDO = color;
	//ROUGHNESS = 0.2;
	//RIM = 1.0;
	
	
	
	// Place fragment code here.
}

void vertex() {
	//VERTEX.y += sin(TIME * 5.0 + VERTEX.x * 5.0) * .2;
	//VERTEX.x += VERTEX.x *20.0;
	
	
	
}
