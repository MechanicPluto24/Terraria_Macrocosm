sampler uImage0 : register(s0);

float2 uOffset;   // The user-defined center of the shade/spotlight
float uIntensity; // The intensity of the shade (if positive) / spotlight (if negative)
float2 uTextureResolution; 
float uRadius; 

static const float PI = 3.14159265f;
 
float4 CelestialBodyShading(float2 texCoord : TEXCOORD) : COLOR
{
    float2 sampleCoord = texCoord;
    sampleCoord *= uTextureResolution;
    sampleCoord = ceil(sampleCoord);
    sampleCoord /= uTextureResolution;
	
    float4 color = tex2D(uImage0, texCoord);
    float2 center = float2(0.5, 0.5) - uOffset.xy;
    float dist = length(sampleCoord - center);
	 
    if (uIntensity < 0)
    {
        float2 range = float2(0.4, 0.7) * uRadius;
        color.rgb *= clamp((smoothstep(range.x, range.y, dist) + (1 + uIntensity)), 0, 1); // spotlight
    }
    else
    {
        float2 range = float2(0.25, 0.5) * uRadius;
        color.rgb -= color.rgb * smoothstep(range.x, range.y, dist) * uIntensity; // shade
    }

    return color;
}

technique Technique1 
{
    pass CelestialBodyShading 
    {
        PixelShader = compile ps_3_0 CelestialBodyShading();
    }
}

