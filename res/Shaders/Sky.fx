uniform extern float4x4 View;
uniform extern float4x4 Projection;

void SkyboxVertexShader( float3 pos : POSITION0,
                         out float4 SkyPos : POSITION0,
                         out float3 SkyCoord : TEXCOORD0 )
{
    // Calculate rotation. Using a float3 result, so translation is ignored
    float3 rotatedPosition = mul(pos, View);           
    // Calculate projection, moving all vertices to the far clip plane 
    // (w and z both 1.0)
    SkyPos = mul(float4(rotatedPosition, 1), Projection).xyww;    

    SkyCoord = pos;
};
uniform extern texture SkyTexture;
sampler SkyboxS = sampler_state
{
    Texture = <SkyTexture>;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = CLAMP;
    AddressV = CLAMP;
};
float4 SkyboxPixelShader( float3 SkyCoord : TEXCOORD0 ) : COLOR
{
    // grab the pixel color value from the skybox cube map
    return texCUBE(SkyboxS, SkyCoord);
};
technique SkyboxTechnique
{
    pass P0
    {
        vertexShader = compile vs_2_0 SkyboxVertexShader();
        pixelShader = compile ps_2_0 SkyboxPixelShader();

        // We're drawing the inside of a model
        CullMode = None;  
        // We don't want it to obscure objects with a Z < 1
        ZWriteEnable = false; 
    }
}
