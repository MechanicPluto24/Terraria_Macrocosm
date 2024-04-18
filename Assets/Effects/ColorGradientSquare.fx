sampler uImage0 : register(s0);

float2 uImageSize0 = float2(1, 1);
float4 uSourceRect = float4(0, 0, 1, 1);

float2 uOffset;
float4 uColorIntensity;

float uSize = 1;

float4 ColorGradientSquare(float2 texCoord : TEXCOORD) : COLOR
{
    float4 color = tex2D(uImage0, texCoord);
    
    float frameX = (texCoord.x * uImageSize0.x - uSourceRect.x) / uSourceRect.z;
    float frameY = (texCoord.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w;

    float2 frameCoord = float2(frameX, frameY);
    frameCoord.x *= uSourceRect.z / uSourceRect.w;
        
    float dist = distance(frameCoord, uOffset);

    float2 distXY = abs(frameCoord - uOffset);
    dist = saturate(max(distXY.x, distXY.y) - uSize);

    float3 newColor = color.rgb + lerp(float3(0, 0, 0), uColorIntensity.rgb, dist) * color.a;
    float alpha = lerp(color.a, uColorIntensity.a, dist) * color.a;
    
    return float4(newColor, alpha);
}

technique
{
    pass ColorGradientSquare
    {
        PixelShader = compile ps_3_0 ColorGradientSquare();
    }
}

