sampler2D uTexture : register(s0);
sampler2D uColorMask : register(s1);

const int COLOR_MAP_SIZE = 4;
const float MAX_COLOR_DIST = 0.1f;

float3 uColorKeys[4];
float4 uColors[4];

float3 uAmbientColor;

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
        
    for (int i = 0; i < COLOR_MAP_SIZE; i++)
    {
        float colorDistance = CalculateColorDistance(mask.rgb, uColorKeys[i].rgb);
        
        if (colorDistance < MAX_COLOR_DIST)
        {
            color.rgb = lerp(color.rgb, texelBrightness * uColors[i].rgb, uColors[i].a);
            break;
        }
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

