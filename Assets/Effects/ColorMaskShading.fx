sampler2D uTexture : register(s0);   // The colored texture, typically passed by the SpriteBatch
sampler2D uColorMask : register(s1); // The color mask, must be provided by the user 

// The max accepted color distance between the 
// provided color mask and the provided known color keys
const float MAX_COLOR_DIST = 0.5f;

// The size of the color table
const int MAX_COLOR_NUM = 8;
int uColorNum;

// The color keys tables; separated into multiple arrays for faster logic
float3 uColorKeys0[4];
float3 uColorKeys1[4];

// The user defined color values, per each existing key
float4 uColors0[4];
float4 uColors1[4];

bool uDebug = false;

// The ambience light brightness onto the object, expressed as a scalar
float uAmbientBrightness;

// Returns the grayscale of a RGB color, using the NTSC formula
float3 RGBToLuminance(float3 color)
{
    float3 weights = float3(0.299, 0.587, 0.114);
    return dot(color, weights);
}

//// Fetch the color key based on the lookup index
//float3 GetKey(int index)
//{
//    if (index < 4)
//        return uColorKeys0[index].rgb;
//    else
//        return uColorKeys1[index].rgb;
//}
//
//// Fetch the user color based on the lookup index
//float4 GetNewColor(int index)
//{
//    if (index < 4)
//        return uColors0[index].rgba;
//    else
//        return uColors1[index].rgba;
//}

float4 ColorMaskShading(float2 texCoord : TEXCOORD) : COLOR0
{
    float4 color = tex2D(uTexture, texCoord);  
    float4 mask = tex2D(uColorMask, texCoord);  
    
    if (uDebug)
        return float4(mask.rgb, color.a);
    
    if (mask.a < 0.9f)
        return color;
    
    // Get the brightness of the local pixel
    float3 texelBrightness = RGBToLuminance(color.rgb);
        
    for (int i = 0; i < uColorNum; i++)
    {
        float3 key = (i < 4) ? uColorKeys0[i].rgb : uColorKeys1[i].rgb;
        float4 newColor = (i < 4) ? uColors0[i] : uColors1[i];
                
        if (!any(mask.rgb - key))
        {
            // Blend the original color and the user color adjusted to the pixel's original brightness, based on the alpha
            color.rgb = lerp(color.rgb, texelBrightness * newColor.rgb, newColor.a);
            //break;
        } else
        {
            return mask.rgba;
        }
    }
    
    return float4(color.rgb * uAmbientBrightness, color.a);
}

technique Technique1 
{
    pass ColorMaskShading
    {
        PixelShader = compile ps_2_0 ColorMaskShading();
    }
}

