sampler uTexture : register(s0); // The colored texture, typically passed by the SpriteBatch
sampler uColorMask : register(s1); // The color mask, must be provided by the user 
sampler uDetailTexture : register(s2);

// The max accepted color distance between the 
// provided color mask and the provided known color keys
const float MAX_COLOR_DIST = 0.1f;

int uColorCount;

// The color keys table
float3 uColorKey[64];

// The user defined color values, per each existing key
float4 uColor[64];

// Whether this detail displays on non-customizable areas of the rocket
bool uDetailUseMask = false;

// The ambience light color 
float3 uAmbientColor = float3(1, 1, 1);

// Returns the grayscale of a RGB color, using the NTSC formula
float3 RGBToLuminance(float3 color)
{
    float3 weights = float3(0.299, 0.587, 0.114);
    return dot(color, weights);
}

float ColorDistance(float3 color1, float3 color2)
{
    float3 dist = color1 - color2;
    return length(dist);
}

float4 ColorMaskShading(float2 texCoord : TEXCOORD) : COLOR0
{
    // Sample the input texture and the color mask (pattern)
    float4 color = tex2D(uTexture, texCoord);  
    float4 mask = tex2D(uColorMask, texCoord);  
    float4 detail = tex2D(uDetailTexture, texCoord);  
    
    // Get the brightness of the local pixel
    float3 texelBrightness = RGBToLuminance(color.rgb);
    
    float4 newColor = color;
    
    if (detail.a > 0.0f)
    { 
        float tranparency = uDetailUseMask ? mask.a : detail.a;
        newColor = float4(detail.rgb * texelBrightness.rgb, 1.0f);
        return float4(newColor.rgb * uAmbientColor, tranparency);
    }
    
    // Ignore pixel if mask is transparent!
    if (mask.a < 1.0f)
        return float4(color.rgb * uAmbientColor, color.a);
    
    for (int i = 0; i < uColorCount; i++)
    {
        if (ColorDistance(mask.rgb, uColorKey[i]) < MAX_COLOR_DIST)
        {
            newColor = uColor[i];  
            break;
        }
    }
        
    // Blend the original color with the new found, accounting for the ambient brightness 
    // (will return the original if found unrecognized key in the mask) 
    color.rgb = lerp(color.rgb, newColor.rgb * texelBrightness, newColor.a);
    return float4(color.rgb * uAmbientColor, color.a);
}

technique Technique1 
{
    pass ColorMaskShading
    {
        PixelShader = compile ps_3_0 ColorMaskShading();
    }
}

