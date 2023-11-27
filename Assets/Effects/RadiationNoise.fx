sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

// some pseudo random noise generation 
float RandomNoise(float2 coords, float seed)
{
    return frac(sin(dot(coords, float2(12.9898, 78.233))) * 43758.5453 * seed);
}

float4 RadiationNoise(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    
    color.rgb -= uIntensity * RandomNoise(coords, sin(uTime));
       
    return color;
}

technique Technique1
{
    pass RadiationNoise
    {
        PixelShader = compile ps_3_0 RadiationNoise();
    }
}