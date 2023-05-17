sampler uImage0 : register(s0);

float2 uOffset;
float uIntensity;

float4 OverheatGradient(float2 texCoord : TEXCOORD) : COLOR
{
    float4 color = tex2D(uImage0, texCoord);
    float4 newColor = color;
    newColor.r = newColor.r + lerp(0, 1 * uIntensity, (texCoord + uOffset).x) * color.a;
    return newColor;
}

technique Technique1 
{
    pass OverheatGradient
    {
        PixelShader = compile ps_2_0 OverheatGradient();
    }
}

