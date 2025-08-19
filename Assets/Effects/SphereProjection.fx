#include "common.fxh"

    // Light texture.
sampler2D uImage0 : register(s0);
    // Shadow texture.
sampler2D uImage1 : register(s1);
    // Overlay texture.
sampler2D uImage2 : register(s2);

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

float shadow(float2 coords, float3 sp, float expo = 12)
{
    float3 uvWorld = float3(coords * uEntitySize + uEntityPosition, 0);
    
    float shad = 1 - pow(1 - saturate(dot(sp, normalize(uLightSource - uvWorld))), expo);
    
    return shad;
}

float4 atmo(float dist, float shad, float radius)
{
        // Hacky solution for a faux atmosphere.
    float atmo = map(dist, radius, 1, 1, 0) * step(radius, dist);
	
    float4 atmoColor = lerp(uShadowAtmosphereColor, uLightAtmosphereColor == 0 ? tex2D(uImage0, .5) : uLightAtmosphereColor, shad);
	
    return atmoColor * atmo;
}

float4 planet(float2 uv, float dist, float3 sp, float shad)
{
        // Rotate the texture accordingly.
    sp = mul(sp, uOrientation);
    
    float2 pt = lonlat(sp);
    
    float4 overlay = tex2D(uImage2, pt);
    
    float4 light = lerp(tex2D(uImage0, pt), overlay, overlay.a);
    
    float4 col = lerp(tex2D(uImage1, pt) * uShadowColor, light * uLightColor, shad);
    
    col.a = 1;
    
    return col;
}

float4 SphereProjection(float2 coords : TEXCOORD0, float2 screenPos : SV_POSITION) : COLOR0
{
    float2 uv = (coords - .5) * 2;
    
    float dist = length(uv);
    
    if (dist > uRadius)
    {
        float3 sp = sphere(uv, dist, 1);
        float shad = shadow(coords, sp, 4);
		
        float4 color = atmo(dist, shad, uRadius);
        
        color *= color.a;
        
        color.a = 0;
        
        return color * uColor;
    }
    else
    {
        float3 sp = sphere(uv, dist, uRadius);
        float shad = shadow(coords, sp);
		
        float4 color = planet(uv, dist, sp, shad);
		
        return color * color.a * uColor;
    }
}

technique
{
    pass SphereProjection
    {
        PixelShader = compile ps_3_0 SphereProjection();
    }
}
