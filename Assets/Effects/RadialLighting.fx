sampler uImage0 : register(s0);

float2 uOffset; // The center of the shade/spotlight
float uIntensity; // The intensity of the shade (if positive) / spotlight (if negative)
float2 uShadeResolution; // The resolution of the shade/spotlight   
float uRadius; // The radius of the shade (0 to 1)
float4 uSourceRect; // The source rectangle (x, y, width, height) in normalized texture coordinates
float4 uColor = float4(1.0, 1.0, 1.0, 1.0); // Color for colorization

static const float PI = 3.14159265f;
 
float4 RadialLighting(float2 texCoord : TEXCOORD) : COLOR
{
    float2 sampleCoord = texCoord;
    sampleCoord *= uShadeResolution;
    sampleCoord = ceil(sampleCoord);
    sampleCoord /= uShadeResolution;
	
    float4 color = tex2D(uImage0, texCoord);
    float2 center = (uSourceRect.xy + 0.5 * uSourceRect.zw) - uOffset.xy;
    float distance = length(sampleCoord - center);
	 
    // spotlight
    if (uIntensity < 0)
    {
        float2 range = float2(0.4, 0.7) * uRadius;
        color.rgb *= clamp((smoothstep(range.x, range.y, distance) + (1 + uIntensity)), 0, 1);
    }
    else // shade
    {
        float2 range = float2(0.25, 0.5) * uRadius;
        color.rgb -= color.rgb * smoothstep(range.x, range.y, distance) * uIntensity;
    }

    color *= uColor;
    return color;
}

technique
{
    pass RadialLighting
    {
        PixelShader = compile ps_3_0 RadialLighting();
    }
}
