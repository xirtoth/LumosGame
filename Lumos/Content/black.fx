Texture2D SpriteTexture;
float4 _Time; // Declare the _Time variable
float4 newColor;

sampler SpriteTextureSampler : register(s0);

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float rand(float2 uv, float time)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233)) + time) * 43758.5453 + time);
}
float2 subtract(float2 a, float2 b)
{
    return float2(a.x - b.x, a.y - b.y);
}

float length(float2 v)
{
    return sqrt(v.x * v.x + v.y * v.y);
}

float4 MainPS(VertexShaderOutput input) : SV_Target
{
    // Sample the color from the sprite texture
    float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;

    // Calculate the distance from the center of the globe
    float2 center = float2(0.5, 0.5);
    float2 distanceVec = subtract(input.TextureCoordinates, center);
    float distance = length(distanceVec);

    // Define the brightness factor based on the distance from the center
    float brightnessFactor = 1.0 - distance;

    // Adjust the color based on the brightness factor
    color.rgb += brightnessFactor;

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
