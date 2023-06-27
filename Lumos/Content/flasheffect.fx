Texture2D SpriteTexture;


sampler SpriteTextureSampler : register(s0);

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};


float4 MainPS(VertexShaderOutput input) : SV_Target
{
    // Sample the color from the sprite texture
    float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;

    // Calculate the distance from the center of the globe
    color.rgb = 1.0;
    

    return color;
}
#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
