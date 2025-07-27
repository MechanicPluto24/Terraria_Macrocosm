#include "common.fxh"

sampler textureSampler : register(s0);  // The texture, typically passed by the SpriteBatch
sampler patternSampler : register(s1);  // The pattern, must be provided by the user 

// Number of actual colors
int uColorCount;

// The color keys table
float3 uColorKey[64];

// The user defined color values, per each existing key
float4 uColor[64];

// The max accepted color distance between the 
// provided color mask and the provided known color keys
float uColorDistance = 0.1f;

float ColorDistance(float3 color1, float3 color2)
{
    float3 dist = color1 - color2;
    return length(dist);
}

float4 ColorMaskShading(float2 texCoord : TEXCOORD, float4 inputColor : COLOR0) : COLOR0
{
    float4 texColor = tex2D(textureSampler, texCoord);
    float4 patternColor = tex2D(patternSampler, texCoord);
    
    //float3 brightness = RGBToLuminance(texColor.rgb);
    float4 outputColor = texColor * inputColor;
    
    if (patternColor.a == 0.0f)
        return outputColor;
        
    for (int i = 0; i < uColorCount; i++)
    {
        if (ColorDistance(patternColor.rgb, uColorKey[i]) < uColorDistance)
        {
            outputColor = float4(lerp(outputColor.rgb, outputColor.rgb * uColor[i].rgb, patternColor.a), texColor.a);
            break;
        }
    }
     
    return outputColor;
}

technique 
{
    pass ColorMaskShading
    {
        PixelShader = compile ps_3_0 ColorMaskShading();
    }
}

