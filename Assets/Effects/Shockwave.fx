sampler uImage0 : register(S0);

float2 uTargetPosition;
float2 uTextureResolution;

float uProgress;
float uOpacity;

float uRippleCount;       
float uSize;              
float uPropagationSpeed; 

static const float PI = 3.14159265f;

float4 Shockwave(float4 position : SV_POSITION, float2 coords : TEXCOORD0) : COLOR0
{
    float2 targetCoords = uTargetPosition / uTextureResolution;
    float2 centreCoords = (coords - targetCoords) * (uTextureResolution / uTextureResolution.y);
    float dotField = dot(centreCoords, centreCoords);
    float ripple = dotField * uSize * PI - uProgress * uPropagationSpeed;

    if (ripple < 0 && ripple > uRippleCount * -2 * PI)
    {
        ripple = saturate(sin(ripple));
    }
    else
    {
        ripple = 0;
    }

    float2 sampleCoords = coords + ((ripple * uOpacity / uTextureResolution) * centreCoords);

    return tex2D(uImage0, sampleCoords);
}

technique Technique1
{
    pass Shockwave
    {
        PixelShader = compile ps_3_0 Shockwave();
    }
}