// The purpose of this shader is to draw a simple texture onto a list of vertices

// The texture to apply
sampler Texture : register(s0); 

// Matrix for the vertices. Usually we don't need this unless we're working with primitives
matrix uTransformMatrix;
float4 uSourceRect; // (x, y, width, height)

// Simple structs to make referencing data easier, as it's passed into our functions
struct VertexShaderInput
{
	float4 Position : POSITION;
	float2 TexCoords : TEXCOORD0;
	float4 Color : COLOR0;
};

struct VertexShaderOutput // PixelShaderInput
{
	float4 Position : POSITION;
	float2 TexCoords : TEXCOORD0;
	float4 Color : COLOR0;
};

// VertexShaderInput - which MUST represent the layout of our C# IVertexTypes - is consumed here
VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
	// We instantiate our output
	VertexShaderOutput output;
    
	// ALWAYS have to assign every variable inside it, like a C# constructor that wants everything
    output.Position = mul(input.Position, uTransformMatrix);
	
    float2 textureSize = float2(uSourceRect.z, uSourceRect.w);
    float2 sourceStart = float2(uSourceRect.x, uSourceRect.y);
    output.TexCoords = sourceStart + input.TexCoords * textureSize;
	
	output.Color = input.Color;

	// And output it
	return output;
}

// Then our VertexShaderOutput is consumed here, the layout of which is only represented in this shader code
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	// We sample our texture using our input data
	float4 color = tex2D(Texture, input.TexCoords);
	
	// Then return the resulting color
	return color * input.Color;
}

// Techniques just encapsulate a bunch of passes
technique DefaultTechnique
{
	// A pass contains a certain set of functions to compile and apply
	pass DefaultPass
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}