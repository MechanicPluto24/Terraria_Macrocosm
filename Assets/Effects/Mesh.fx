// The purpose of this shader is to draw a simple texture onto a list of vertices

// Matrix for the vertices. Usually we don't need this unless we're working with primitives
// Shader.Parameters["TransformMatrix"].SetValue(...);
matrix TransformMatrix;

// The texture to apply
sampler Texture : register(s0); 

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
	output.Color = input.Color;
	output.TexCoords = input.TexCoords;
	output.Position = mul(input.Position, TransformMatrix);

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