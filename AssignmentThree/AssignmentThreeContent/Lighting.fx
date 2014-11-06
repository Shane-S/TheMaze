float4x4 World;
float4x4 View;
float4x4 Projection;

// TODO: add effect parameters here.

// The input to the ambient lighting vertex shader.
struct AMBIENT_VS_IN
{
    float4 Position : POSITION0;
	float4 Texture  : TEXTURE0;
	//float4 
};

// The output from the ambient lighting vertex shader.
struct AMBIENT_VS_OUT
{
    float4 Position : POSITION0;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

// The output from the ambient lighting pixel shader.
struct AMBIENT_PS_OUT
{
	float4 Color : COLOR0;
};

AMBIENT_VS_OUT AmbientVertexShader(AMBIENT_VS_IN input)
{
    AMBIENT_VS_OUT output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    // TODO: add your vertex shader code here.

    return output;
}

AMBIENT_PS_OUT AmbientPixelShader(AMBIENT_VS_OUT input)
{
    // TODO: add your pixel shader code here.
	AMBIENT_PS_OUT Output;
	Output.Color = 1;
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
