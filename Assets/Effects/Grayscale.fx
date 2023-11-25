sampler2D uTexture : register(s0); // The colored texture, typically passed by the SpriteBatch

float4 Grayscale(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uTexture, texCoord);
    float gray = dot(color.rgb, float3(0.299, 0.587, 0.114));
    return float4(gray, gray, gray, color.a);
}

technique Technique1
{
    pass Grayscale
    {
        PixelShader = compile ps_2_0 Grayscale();
    }
}