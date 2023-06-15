sampler uImage0 : register(s0);

float2 uCenter;   // Center of the fire effect
float uRadius;    // Radius of the fire effect
float uIntensity; // Intensity of the fire effect
float uTime;      // Time for animation
 
float4 SunFire(float2 texCoord : TEXCOORD) : COLOR
{
    float4 color = tex2D(uImage0, texCoord);

    // Calculate the distance from the center of the fire effect
    float dist = distance(texCoord, uCenter);
    
    // Calculate the alpha value based on the distance and radius
    float alpha = 1.0 - saturate((dist - uRadius) / uRadius);
    
    // Apply a sine wave to create the flickering effect
    float timeFactor = sin(uTime + texCoord.x * 0.1 + texCoord.y * 0.05) * uIntensity;

    // Multiply the color by the time factor and alpha
    color.rgb += timeFactor * alpha * color.a;
    
    return color;
}

technique Technique1
{
    pass SunFire
    {
        PixelShader = compile ps_2_0 SunFire();
    }
}