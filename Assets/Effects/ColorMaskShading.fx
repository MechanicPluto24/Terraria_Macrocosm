sampler2D uTexture : register(s0);   // The colored texture, typically passed by the SpriteBatch
sampler2D uColorMask : register(s1); // The color mask, must be provided by the user 

// The max accepted color distance between the 
// provided color mask and the provided known color keys
const float MAX_COLOR_DIST = 0.1f;
int uColorNum = 8;

// The color keys tables; separated into multiple arrays for faster logic
float3 uColorKeys0[4];
float3 uColorKeys1[4];

// The user defined color values, per each existing key
float4 uColors0[4];
float4 uColors1[4];

// The ambience light brightness onto the object, expressed as a scalar
float uAmbientBrightness;

// Returns the grayscale of a RGB color, using the NTSC formula
float3 RGBToLuminance(float3 color)
{
    float3 weights = float3(0.299, 0.587, 0.114);
    return dot(color, weights);
}

float ColorDistance(float3 color1, float3 color2)
{
    float redDistance = abs(color1.r - color2.r);
    float greenDistance = abs(color1.g - color2.g);
    float blueDistance = abs(color1.b - color2.b);

    float totalDistance = sqrt((redDistance * redDistance) +
                               (greenDistance * greenDistance) +
                               (blueDistance * blueDistance));
    
    return totalDistance;
}

float4 ColorMaskShading(float2 texCoord : TEXCOORD) : COLOR0
{
    float4 color = tex2D(uTexture, texCoord);  
    float4 mask = tex2D(uColorMask, texCoord);  
        
    // Get the brightness of the local pixel
    float3 texelBrightness = RGBToLuminance(color.rgb);
        
    for (int i = 0; i < uColorNum; i++)
    {
        float3 key = (i < 4) ? uColorKeys0[i].rgb : uColorKeys1[i-4].rgb;
        float4 newColor = (i < 4) ? uColors0[i] : uColors1[i-4];
                
        if (ColorDistance(key, mask.rgb) < 0.1)
        {
            // Blend the original color and the user color adjusted to the pixel's original brightness, based on the alpha
            color.rgb = lerp(color.rgb, texelBrightness * newColor.rgb, newColor.a);
            break;
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

