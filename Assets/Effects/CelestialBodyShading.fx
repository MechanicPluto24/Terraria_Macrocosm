sampler uImage0 : register(s0);

float2 uOffset;   // The user-defined center of the shade/spotlight
float uIntensity; // The intensity of the shade (if positive) / spotlight (if negative)

static const float PI = 3.14159265f;
 
float4 CelestialBodyShading(float2 texCoord : TEXCOORD) : COLOR
{
    float4 color = tex2D(uImage0, texCoord);

    // Adjust the shade center based on the texture center and user offset
    float2 center = float2(0.5, 0.5) - uOffset.xy;

    float2 d = texCoord - center;
    float dist = length(d);
 
    if (uIntensity < 0) 
        color.rgb *= clamp((smoothstep(0.4, 0.7, dist) + (1 + uIntensity)), 0, 1); // spotlight
    else
        color.rgb -= color.rgb * smoothstep(0.25, 0.5, dist) * uIntensity; // shade
  
    return color;
}

technique Technique1 
{
    pass CelestialBodyShading 
    {
        PixelShader = compile ps_2_0 CelestialBodyShading();
    }
}

