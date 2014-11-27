float4x4 World;
float4x4 View;
float4x4 Projection;

// The ambient lighthing intensity. Must be a value between -1 and 1 (inclusive).
float AmbientLightingFactor;

// The position of the camera and the direction in which the camera is looking
float3 LightPos;
float3 CameraLookAt;

float LightDecayExponent;
float LightDistance;
float4 LightColour;
float DiffuseIntensity;

// Fog variables
float FarPlane;
float4 FogColour;
bool FogOn;

bool FlashlightOn;
bool IsChicken;

// The wall texture and a sampler to sample its values
Texture2D BoxTexture;
sampler AmbientSampler = sampler_state { 
											texture = <BoxTexture>; 
											MAGFILTER = ANISOTROPIC;
											MINFILTER = ANISOTROPIC;
											mipfilter = LINEAR; 
											AddressU = clamp; 
											AddressV = clamp;
									   };

// Stores the scene's depth values
Texture2D DepthTexture;
sampler DepthSampler = sampler_state {
											texture = <DepthTexture>;
											MAGFILTER = ANISOTROPIC;
											MINFILTER = ANISOTROPIC;
											mipfilter = LINEAR;
											AddressU = clamp;
											AddressV = clamp;
									 };

// The input to the ambient lighting vertex shader.
struct AMBIENT_VS_IN
{
    float4 Position  : POSITION0;
	float4 Normal    : NORMAL0;
	float2 TexCoords : TEXCOORD0;
};

// The output from the ambient lighting vertex shader.
struct AMBIENT_VS_OUT
{
    float4 Position   : POSITION0;
	float3 Position3D : TEXCOORD0;
	float ViewSpaceZ    : TEXCOORD1;
	float3 Normal     : TEXCOORD2;
	float2 TexCoords  : TEXCOORD3;
};

// The output from the ambient lighting pixel shader.
struct AMBIENT_PS_OUT
{
	float4 Colour : COLOR0;
};

/**
 * The vertex shader for the AmbientLighthing technique.
 */
AMBIENT_VS_OUT AmbientVertexShader(AMBIENT_VS_IN input)
{
    AMBIENT_VS_OUT output = (AMBIENT_VS_OUT)0;

	float4 pos = mul(input.Position, World);
	float4 viewPos = mul(pos, View);

	output.Position = mul(viewPos, Projection);

	output.TexCoords = input.TexCoords;
	output.Normal = normalize(mul(input.Normal, (float3x3)World));
	output.Position3D = pos;
	output.ViewSpaceZ = -viewPos.z / FarPlane;
	
	if (IsChicken)
		output.Normal *= -1;

    return output;
}

/**
 * The pixel shader for the AmbientLighting technique.
 *
 * Darkens or brightens the pixels by a 
 */
AMBIENT_PS_OUT AmbientPixelShader(AMBIENT_VS_OUT input)
{
	const float FOG_MIN = 0.5;
	const float FOG_MAX = 0.99;

	AMBIENT_PS_OUT Output = (AMBIENT_PS_OUT)0;
	
	// The colour of the texture at this pixel
	float4 baseColour = tex2D(AmbientSampler, input.TexCoords);

	// Determines the brightness of the scene by adding/subtracting a fraction
	// of the base colour to/from itself
	float4 ambientColour = baseColour;

	// The final lighting colour.
	float4 finalColour;

	// The direction from the vertex to the light
	float3 lightDir = LightPos - input.Position3D;

	// The attenuation value of the light hitting this pixel. The further the
	// light is from the centre of the cone, the more attenuated it will be.
	float attenuation = saturate(1.0f - length(lightDir) / LightDistance);

	// Project's the light's actual direction onto the direction from the light to the
	// vertex.
	float lightProjectionAngle;

	// Normalise direction from vertex to light and dot with light direction
	lightDir = normalize(lightDir);
	lightProjectionAngle = dot(CameraLookAt, -lightDir);

	// Determine the brightness
	ambientColour.rgb += ambientColour.rgb * AmbientLightingFactor;
	finalColour = ambientColour;

	if (FlashlightOn)
	{
		float spotIntensity = pow(lightProjectionAngle, LightDecayExponent);
		float3 normal = normalize(input.Normal);
		// The light direction and the camera view are the same in this instance, so we can use lightDir instead
		// of calculating the direction from the camera separately
		float diff = saturate(dot(normal, -lightDir));
		float3 reflect = normalize(2 * diff * normal + lightDir);
		finalColour += spotIntensity * attenuation  * LightColour * DiffuseIntensity * baseColour * diff;
	}

	Output.Colour = finalColour;

	if (FogOn)
	{
		float space = saturate(input.ViewSpaceZ);
		float4 fogged = lerp(finalColour, FogColour, lerp(FOG_MIN, FOG_MAX, space));
		Output.Colour = fogged;
	}

	return Output;
}

technique Technique1
{
	pass P0
	{
		VertexShader = compile vs_2_0 AmbientVertexShader();
		PixelShader = compile ps_2_0 AmbientPixelShader();
	}
}
