#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#define MAX_BONES   72

Texture2D Texture;
matrix WVP;
float4x3 Bones[MAX_BONES];

// Properties
sampler2D TextureSampler = sampler_state {
	Texture = <Texture>;
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

// Structures
struct App2VS {
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	int4   Indices  : BLENDINDICES0;
	float4 Weights  : BLENDWEIGHT0;
};

struct VS2PS {
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
	float2 Depth : TEXCOORD1;
};

struct PSOut {
	float4 Color : COLOR0;
	float4 Border : COLOR1;
	float4 Depth : COLOR2;
};

// Functions
void Skin(inout App2VS input, uniform int boneCount) {
	float4x3 skinning = 0;

	[unroll]
	for (int i = 0; i < boneCount; i++) {
		skinning += Bones[input.Indices[i]] * input.Weights[i];
	}

	input.Position.xyz = mul(input.Position, skinning);
}

VS2PS ApplyTransform(App2VS input) {
	VS2PS output = (VS2PS)0;
	output.Position = mul(input.Position, WVP);
	output.TexCoord = input.TexCoord;
	output.Depth = output.Position.zw;
	return output;
}

// Vertex Shaders

VS2PS StaticVS(App2VS input) {
	return ApplyTransform(input);
}
VS2PS Skinned1VS(App2VS input) {
	Skin(input, 1); // one bone
	return ApplyTransform(input);
}
VS2PS Skinned2VS(App2VS input) {
	Skin(input, 2); // two bones
	return ApplyTransform(input);
}
VS2PS Skinned4VS(App2VS input) {
	Skin(input, 4); // four bones
	return ApplyTransform(input);
}

// Pixel Shaders

PSOut MainPS(VS2PS input) {
	PSOut output = (PSOut)0;
	output.Color = tex2D(TextureSampler, input.TexCoord);
		
	// border color is first pixel in texture
	output.Border.rgb = tex2D(TextureSampler, float2(0,0));
	output.Border.a = 1;
	
	output.Depth.r = input.Depth.x / input.Depth.y;
	output.Depth.a = 1;
	
	return output;
}

technique StaticModel {
	pass P0 {
		VertexShader = compile VS_SHADERMODEL StaticVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}
technique SkinnedModel1 {
	pass P0 {
		VertexShader = compile VS_SHADERMODEL Skinned1VS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}
technique SkinnedModel2 {
	pass P0 {
		VertexShader = compile VS_SHADERMODEL Skinned2VS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}
technique SkinnedModel4 {
	pass P0 {
		VertexShader = compile VS_SHADERMODEL Skinned4VS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}
