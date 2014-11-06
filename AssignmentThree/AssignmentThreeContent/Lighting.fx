float4x4 World;
float4x4 View;
float4x4 Projection;

// TODO: add effect parameters here.

// Input to the ambient lighthing vertex shader
struct AMBIENT_VS_IN
{
    float4 Position : POSITION0;
};

// Output from the ambient lighthing vertex shader
struct AMBIENT_VS_OUT
{
    float4 Position : POSITION0;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

// Ouput from the ambient lighthing pixel shader
struct AMBIENT_PS_OUT
{
	float4 Color : COLOR0;
};

// Simply translates everything to the correct position
AMBIENT_VS_OUT VSAmbientLighting(AMBIENT_VS_IN input)
{
    AMBIENT_VS_OUT output;

	// Translate the vertices to their correct world positions
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    // TODO: add your vertex shader code here.

    return output;
}

// Applies ambient lighthing conditions to each pixel
AMBIENT_PS_OUT PSAmbienLighting(AMBIENT_VS_OUT input)
{
    // TODO: add your pixel shader code here.
	AMBIENT_PS_OUT Ouput;
	Output.Color = input.Color;
    return float4(1, 0, 0, 1);
}

technique Ambient
{
	// Do all ambient lighting calculations
    pass Pass0
    {
        VertexShader = compile vs_2_0 VSAmbientLighting();
        PixelShader = compile ps_2_0 PSAmbientLighting();
	};
}

technique Flashlight
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 VSFlashlight();
		PixelShader = compile ps_2_0 PSFlashlight();
	};
};