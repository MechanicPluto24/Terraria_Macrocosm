sampler uImage0 : register(s0);

float2 uOffset;
float uIntensity;

static const float PI = 3.14159265f;
 
float4 CelestialBodyShading(float2 texCoord : TEXCOORD) : COLOR
{
    // Sample the texture
    float4 color = tex2D(uImage0, texCoord);

    float2 center = float2(0.5, 0.5) - uOffset.xy;

    float2 d = texCoord - center;
    float dist = length(d);
 
    if (uIntensity < 0) 
        color.rgb *= clamp((smoothstep(0.4, 0.7, dist) + (1 + uIntensity)), 0, 1);
    else
        color.rgb -= color.rgb * smoothstep(0.25, 0.5, dist) * uIntensity;
  
    return color;
}

technique Technique1 
{
    pass CelestialBodyShading 
    {
        PixelShader = compile ps_2_0 CelestialBodyShading();
    }
}

