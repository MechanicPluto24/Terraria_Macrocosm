sampler2D uTexture : register(s0);
sampler2D uColorMask : register(s1);

float3 uColor1Key;
float3 uColor2Key;
float3 uColor3Key;
float3 uColor4Key;

float4 uColor1;
float4 uColor2;
float4 uColor3;
float4 uColor4;

float3 uAmbientColor;

const int COLOR_MAP_SIZE = 4;
const float MAX_COLOR_DIST = 0.1f;

float3 RGBToLuminance(float3 color)
{
    float3 weights = float3(0.299, 0.587, 0.114);
    return dot(color, weights);
}

float CalculateColorDistance(float3 color1, float3 color2)
{
    float3 diff = color1 - color2;
    return length(diff);
}

float4 ColorMaskShading(float2 texCoord : TEXCOORD) : COLOR
{
    float4 color = tex2D(uTexture, texCoord);
    float4 mask = tex2D(uColorMask, texCoord);
    
    float3 ambientBrightness = RGBToLuminance(uAmbientColor);
    float3 texelBrightness = RGBToLuminance(color.rgb);
    
    float colorDistance = CalculateColorDistance(mask.rgb, uColor1Key.rgb);
    
    if (CalculateColorDistance(mask.rgb, uColor1Key.rgb) < MAX_COLOR_DIST)
    {
        color.rgb = lerp(color.rgb, texelBrightness * uColor1.rgb, uColor1.a);
    }
    
    if (CalculateColorDistance(mask.rgb, uColor2Key.rgb) < MAX_COLOR_DIST)
    {
        color.rgb = lerp(color.rgb, texelBrightness * uColor2.rgb, uColor2.a);
    }
    
    if (CalculateColorDistance(mask.rgb, uColor3Key.rgb) < MAX_COLOR_DIST)
    {
        color.rgb = lerp(color.rgb, texelBrightness * uColor3.rgb, uColor3.a);
    }
    
    if (CalculateColorDistance(mask.rgb, uColor4Key.rgb) < MAX_COLOR_DIST)
    {
        color.rgb = lerp(color.rgb, texelBrightness * uColor4.rgb, uColor4.a);
    }
    
    return float4(color.rgb * ambientBrightness, color.a);
}

technique Technique1 
{
    pass ColorMaskShading
    {
        PixelShader = compile ps_2_0 ColorMaskShading();
    }
}

