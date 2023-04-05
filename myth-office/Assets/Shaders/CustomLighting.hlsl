#ifndef CUSTOM_LIGHTING
#define CUSTOM_LIGHTING

struct CustomLightingData {
	float3 normalWS; // normal of the fragment in world space
	float3 viewDirection; // Direction from the camera to the fragment


	float3 albedo; // Base RGB color with no light influence, eg. Sampled color from texture. 
	float smoothness; // stength of specular highlight

};

// translate 0-1 smoothness to an exponent, reducing the highlight as smothness grows
float GetSmoothnessPower(float rawSmoothness) {
	return exp2(10 * rawSmoothness + 1);
}


// Light class is not avalible in preview window
#ifndef SHADERGRAPH_PREVIEW
	// compute the diffuse light on the fragment
	float3 CustomLightHandling(CustomLightingData d, Light light) {

	// the light has an RGB color
	float3 radiance = light.color; 

	// Get the light strength based on the angle between the normal and the light direction. Clamped between 0-1
	float diffuseStrength = saturate(dot(d.normalWS, light.direction));
	float specularBase = saturate(dot(d.normalWS, normalize(light.direction + d.viewDirection)));
	float specularStrength = diffuseStrength * pow(specularBase, GetSmoothnessPower(d.smoothness));
	// combine the color and strength of the light hitting the fragment.
	float3 lightStrength = radiance * (diffuseStrength + specularStrength); 

	// Use the strength of the light to light up the base texture color sample
	float3 color = d.albedo * lightStrength;

	return color;
	}
#endif

float3 CalculateCustomLighting(CustomLightingData d) {
	// Light class is not avalible in preview window
	#ifdef SHADERGRAPH_PREVIEW
		// assume light direction and calculate based on that.
		float3 lightDir = float3(0.5, 0.5, 0);
		float intensity = saturate(dot(d.normalWS, lightDir)) +
			pow(saturate(dot(d.normalWS, normalize(d.viewDirection + lightDir))), GetSmoothnessPower(d.smoothness);
		return d.albedo * intensity;	
	#else
		// Returns the single main light in the scene. Must be a directional light?
		Light mainLight = GetMainLight();

		// start with a black pixel because no light has hit it yet.
		float3 color = 0;

		// Add the light from the main light to the pixel
		color += CustomLightHandling(d, mainLight);

		return color;  
	#endif
}


// Wrapper function called by shader graph. Output is provided through out variables
// _float suffix specifies precision level on GPU
void CalculateCustomLighting_float(float3 Albedo, float3 Normal, float3 ViewDirection, float Smoothness,
	out float3 Color) {
	CustomLightingData d;
	d.albedo = Albedo;
	d.normalWS = Normal;
	d.viewDirection = ViewDirection;
	d.smoothness = Smoothness;

	Color = CalculateCustomLighting(d);
}

#endif
