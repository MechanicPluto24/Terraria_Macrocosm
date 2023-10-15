sampler uImage0 : register(s0);

float2 uImageSize0 = float2(1, 1);
float4 uSourceRect = float4(0, 0, 1, 1);

float2 uOffset;
float4 uColorIntensity;

float uSDF = 0;
float uSize = 1;

float4 ColorGradient(float2 texCoord : TEXCOORD) : COLOR
{
    float4 color = tex2D(uImage0, texCoord);
    
    float frameX = (texCoord.x * uImageSize0.x - uSourceRect.x) / uSourceRect.z;
    float frameY = (texCoord.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w;
        
    float2 frameCoord = float2(frameX, frameY);
    frameCoord.x *= uSourceRect.z / uSourceRect.w;
    
    float dist = distance(frameCoord, uOffset);
    
    // square
    if (uSDF == 1)
    {
        float2 distXY = abs(frameCoord - uOffset);
        dist = saturate(max(distXY.x, distXY.y) - uSize);
    }
    else // disc
    {
        dist = saturate(dist - uSize);
    }
    
    float4 newColor = color;
    newColor.rgb += lerp(float3(0, 0, 0), uColorIntensity.rgb, dist) * color.a;
    newColor.a = lerp(color.a, uColorIntensity.a, dist) * color.a;
    
    return newColor;
}

technique Technique1 
{
    pass ColorGradient 
    {
        PixelShader = compile ps_2_0 ColorGradient();
    }
}

