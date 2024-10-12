sampler uImage0 : register(S0);

// Number of output pixels for each axis
float2 uPixelCount;

bool uFilterBilinear = false;

float4 Pixelate(float2 texCoord : TEXCOORD) : COLOR
{
    float2 pixelSize = 1.0 / uPixelCount;
 
    float2 pixelNum = floor(texCoord / pixelSize);
    float2 pixelCenter = pixelNum * pixelSize + pixelSize / 2;
    
    float4 color = tex2D(uImage0, pixelCenter);
    
    return color;
}

technique
{
    pass Pixelate
    {
        PixelShader = compile ps_3_0 Pixelate();
    }
}