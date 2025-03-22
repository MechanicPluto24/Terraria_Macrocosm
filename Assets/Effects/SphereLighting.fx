sampler2D uImage0 : register(s0);

float3 uLightSource;
float2 uEntityPosition;
float2 uTextureSize;
float2 uEntitySize;
float uRadius = 0.138;
int uPixelSize;
float4 uColor = float4(1.0, 1.0, 1.0, 1.0);

float4 SphereLighting(float2 uv : TEXCOORD0) : COLOR0
{
    uv = floor((uv * uTextureSize) / uPixelSize) / uTextureSize * uPixelSize;

    float sphereRadius = 0.5 - uRadius;
    float2 uv2D = (uv - float2(0.5, 0.5)) / sphereRadius;
    if (dot(uv2D, uv2D) > 1)
    {
        return float4(0, 0, 0, 0);
    }
    float3 uv3D = float3(uv2D, sqrt(1 - dot(uv2D, uv2D)));
    float3 normal = normalize(uv3D);
    
    float3 uvWorld = float3(uv * uEntitySize + uEntityPosition, 0);
    float diffuse = max(0.1, dot(normal, normalize(uLightSource - uvWorld)));
    diffuse = pow(diffuse, 2);
    float4 color = tex2D(uImage0, uv);
    
    float4 finalColor = float4(color.rgb * max(diffuse, 0.05), color.a) * color.a * uColor;
    return finalColor;
}

technique
{
    pass SphereLighting
    {
        PixelShader = compile ps_3_0 SphereLighting();
    }
}
