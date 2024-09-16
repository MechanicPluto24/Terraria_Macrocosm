sampler2D uImage0 : register(s0);
float uRotation;
float uScale;

float4 Skew(float2 uv : TEXCOORD0, float4 sampleColor : COLOR) : COLOR0
{ 
    float2 dir = uv - float2(0.5, 0.5);
    dir.y /= uScale;
    float rot = atan2(dir.y, dir.x) + uRotation;
    float len = length(dir);
    uv = float2(0.5, 0.5) + float2(sin(rot), cos(rot)) * len;
    if(uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
    {
        return float4(0, 0, 0, 0);
    }
    return tex2D(uImage0, uv) * sampleColor;
}

Technique
{
    pass Skew
    {
        PixelShader = compile ps_3_0 Skew();
    }
}