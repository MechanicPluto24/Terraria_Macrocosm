sampler uImage0 : register(s0);

float2 uCenter;   // Center of the fire effect
float uRadius;    // Radius of the fire effect
float uIntensity; // Intensity of the fire effect
 
float4 RadialSaturation(float2 texCoord : TEXCOORD) : COLOR
{
    float4 color = tex2D(uImage0, texCoord);

    // Calculate the distance from the center of the fire effect
    float dist = distance(texCoord, uCenter);
    
    // Calculate the alpha value based on the distance and radius
    float alpha = 1.0 - saturate((dist - uRadius) / uRadius);
    
    // Multiply the color by the time factor and alpha
    color.rgb += uIntensity * alpha * color.a;
    
    return color;
}

technique Technique1
{
    pass RadialSaturation
    {
        PixelShader = compile ps_3_0 RadialSaturation();
    }
}