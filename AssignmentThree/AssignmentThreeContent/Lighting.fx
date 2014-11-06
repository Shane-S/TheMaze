float4x4 World;
float4x4 ViewProjection;

// The ambient lighthing intensity. Must be a value between -1 and 1 (inclusive).
float AmbientLightingFactor;

// The wall texture and a sampler to sample its values

Texture2D BoxTexture;
sampler AmbientSampler = sampler_state { 
											texture = <BoxTexture>; 
											magfilter = LINEAR; 
											minfilter = LINEAR; 
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
    float4 Position  : POSITION0;
	float4 Normal    : TEXCOORD0;
	float2 TexCoords : TEXCOORD1;
};

// The output from the ambient lighting pixel shader.
struct AMBIENT_PS_OUT
{
	float4 Colour : COLOR0;
};

/**
 * The vertex shader for the AmbientLighthing technique.
 *
 * Gets the ambient lighthing intensity and adds a given amount of
 * white to each vertex's colour value to simulate "night" or "day".
 * May be moved to the pixel shader later for more interesting effects.
 */
AMBIENT_VS_OUT AmbientVertexShader(AMBIENT_VS_IN input)
{
    AMBIENT_VS_OUT output = (AMBIENT_VS_OUT)0;

	// Computed in the pre-shader (i.e., on the CPU)
	float4x4 preWorldViewProjection = mul(World, ViewProjection);

	output.Position = mul(input.Position, preWorldViewProjection);
	output.Normal = input.Normal;
	output.TexCoords = input.TexCoords;

    return output;
}

/**
 * The pixel shader for the AmbientLighting technique.
 *
 * Currently, this just assigns the colour that was calculated by the
 * interpolator, since ambient lighting is basically just raising/darkening
 * brightness.
 */
AMBIENT_PS_OUT AmbientPixelShader(AMBIENT_VS_OUT input)
{
    // Might customise this later, but for now, vertex shading should
	// be fine for ambient lighting
	AMBIENT_PS_OUT Output = (AMBIENT_PS_OUT)0;
	float4 baseColour = tex2D(AmbientSampler, input.TexCoords);
	Output.Colour.rgb = baseColour.rgb + (baseColour.rgb * AmbientLightingFactor);

	return Output;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 AmbientVertexShader();
        PixelShader = compile ps_2_0 AmbientPixelShader();
    }
}
