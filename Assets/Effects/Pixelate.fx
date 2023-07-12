sampler uImage0 : register(S0);

// Number of output blocks for each axis
float2 uPixelCount;

// Brick effect offset
float uBrickOffset = 0.0f;

float4 PixelateEffect(float2 texCoord : TEXCOORD) : COLOR
{
    float2 brickSize = 1.0 / uPixelCount;

	float2 offsetuv = texCoord;
	bool oddRow = floor(offsetuv.y / brickSize.y) % 2.0 >= 1.0;
	
	if (oddRow)
        offsetuv.x += uBrickOffset * brickSize.x / 2.0;
 
	float2 brickNum = floor(offsetuv / brickSize);
	float2 centerOfBrick = brickNum * brickSize + brickSize / 2;
	float4 color = tex2D(uImage0, centerOfBrick);

	return color;
}

technique Technique1
{
	pass PixelateEffect
	{
		PixelShader = compile ps_2_0 PixelateEffect();
	}
}