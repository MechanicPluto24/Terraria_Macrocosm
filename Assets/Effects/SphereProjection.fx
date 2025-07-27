#include "common.fxh"

sampler2D uImage0 : register(s0);

float3 uLightSource;
float4 uLightColor;
float4 uShadowColor;
float4 uLightAtmosphereColor;
float4 uShadowAtmosphereColor;

float2 uEntityPosition;
float2 uEntitySize;
float3x3 uOrientation;
float uRadius;
float4 uColor = float4(1.0, 1.0, 1.0, 1.0);

float3 sphere(float2 uv, float dist, float radius)
{
    float z = radius * sin(acos(dist / radius));
    
        // Calculate sphere normals.
    return float3(uv, z);
}

float shadow(float2 coords, float3 sp)
{
    float3 uvWorld = float3(coords * uEntitySize + uEntityPosition, 0);
    float shad = 1 - pow(1 - dot(sp, normalize(uLightSource - uvWorld)), 3);
    
    shad = saturate(shad);
    
    return shad;
}

float4 atmo(float dist, float shad)
{
    if (dist < uRadius)
        return float4(0, 0, 0, 0);
    
        // Hacky solution for a faux atmosphere.
    float atmo = pow(1 - map(dist, uRadius, 1, 0, 1), 3);
		
    float4 atmoColor = lerp(uShadowAtmosphereColor, uLightAtmosphereColor, shad);
		
    return atmoColor * atmo;
}

float4 planet(float2 uv, float dist, float3 sp, float shad)
{
    if (dist > uRadius)
        return float4(0, 0, 0, 0);
    
        // Rotate the texture accordingly.
    sp = mul(sp, uOrientation);
    
    float2 pt = lonlat(sp);
    
    return lerp(uShadowColor, tex2D(uImage0, pt) * uLightColor, shad);
}

float4 SphereProjection(float2 coords : TEXCOORD0, float2 screenPos : SV_POSITION) : COLOR0
{
    float2 uv = (coords - .5) * 2;
    
    float dist = length(uv);
    
    float3 sp = sphere(uv, dist, uRadius);
    
    float shad = shadow(coords, sp);
    
    float4 inner = planet(uv, dist, sp, shad);
    
    float4 outer = atmo(dist, shad);
    
    float4 color = (inner + outer) * uColor;
    
    return color * color.a;
}

technique
{
    pass SphereProjection
    {
        PixelShader = compile ps_3_0 SphereProjection();
    }
}
