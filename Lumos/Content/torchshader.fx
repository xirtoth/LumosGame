sampler2D sceneTexture;

float4 TorchPosition;
float2 TorchRadius = float2(32, 32);
float TorchIntensity = 1.0;

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    // Calculate the distance from the current pixel to the torch position
    float2 distance = texCoord - TorchPosition.xy;

    // Calculate the distance-based attenuation factor
    float attenuation = 1.0 - saturate(length(distance) / length(TorchRadius));

    // Calculate the final pixel color with the torch intensity
    float4 color = tex2D(sceneTexture, texCoord);
    color.rgb += TorchIntensity * attenuation;

    return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}