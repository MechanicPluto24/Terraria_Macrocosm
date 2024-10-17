sampler uImage0 : register(s0);

bool uHorizontal;
float4 uColorStart;
float4 uColorEnd;

float4 Gradient(float2 texCoord : TEXCOORD0) : COLOR
{
    float4 texColor = tex2D(uImage0, texCoord);

    float gradientFactor = uHorizontal ? texCoord.x : texCoord.y;

    float4 gradientColor = lerp(uColorStart, uColorEnd, gradientFactor);

    float4 finalColor = texColor * gradientColor;
    return finalColor;
}

technique
{
    pass Gradient
    {
        PixelShader = compile ps_3_0 Gradient();
    }
}
