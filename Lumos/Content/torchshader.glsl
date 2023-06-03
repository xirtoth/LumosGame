uniform sampler2D sceneTexture;

uniform vec4 TorchPosition;
uniform vec2 TorchRadius;
uniform float TorchIntensity;

void main()
{
    // Calculate the distance from the current pixel to the torch position
    vec2 distance = gl_TexCoord[0].xy - TorchPosition.xy;

    // Calculate the distance-based attenuation factor
    float attenuation = 1.0 - clamp(length(distance) / length(TorchRadius), 0.0, 1.0);

    // Calculate the final pixel color with the torch intensity
    vec4 color = texture2D(sceneTexture, gl_TexCoord[0].xy);
    color.rgb += TorchIntensity * attenuation;

    gl_FragColor = color;
}