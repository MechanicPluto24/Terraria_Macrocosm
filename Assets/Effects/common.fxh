#ifndef COMMON_FX
#define COMMON_FX

static const float TAU = 6.28318530718;
static const float PI = 3.14159265359;
static const float PIOVER2 = 1.57079632679;

float map(float value, float start1, float stop1, float start2, float stop2)
{
    value = clamp(value, start1, stop1);
    return start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
}

float aafi(float2 p)
{
    float fi = atan2(p.y, p.x);
    fi += step(p.y, 0) * TAU;
    return fi;
}

float2 lonlat(float3 p)
{
    float lon = aafi(p.xy) / TAU;
    float lat = aafi(float2(p.z, length(p.xy))) / PI;
    return float2(1 - lon, lat);
}

// Returns the grayscale of a RGB color, using the NTSC formula
float3 RGBToLuminance(float3 color)
{
    float3 weights = float3(0.299, 0.587, 0.114);
    return dot(color, weights);
}

#endif