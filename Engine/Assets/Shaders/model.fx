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
	float3 Normal : NORMAL;
	float2 TexCoord : TEXCOORD0;
	int4   Indices  : BLENDINDICES0;
	float4 Weights  : BLENDWEIGHT0;
};

struct VS2PS {
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
	float2 Depth : TEXCOORD1;
	float3 Normal : TEXCOORD2;
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
	output.Normal = normalize(input.Normal);
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
	float4 tex = tex2D(TextureSampler, input.TexCoord);
	float3 border = tex2D(TextureSampler, float2(0, 0)).rgb;

	float mult = input.Normal.z * 0.3 + 0.7;

	output.Color = tex;	
	//output.Color.rgb *= mult;
	
	// border color is first pixel in texture
	output.Border.rgb = border; // tex * 0.2;
	output.Border.a = 1;
	
	output.Depth.r = input.Depth.x / input.Depth.y;
	output.Depth.a = 1;
	
	return output;
}

PSOut ShadowPS(VS2PS input) {
	// do nothing, write to stencil only
	return (PSOut)0;
}

// Techniques

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

technique StaticShadow {
	pass P0 {
		VertexShader = compile VS_SHADERMODEL StaticVS();
		PixelShader = compile PS_SHADERMODEL ShadowPS();
	}
}
technique SkinnedShadow1 {
	pass P0 {
		VertexShader = compile VS_SHADERMODEL Skinned1VS();
		PixelShader = compile PS_SHADERMODEL ShadowPS();
	}
}
technique SkinnedShadow2 {
	pass P0 {
		VertexShader = compile VS_SHADERMODEL Skinned2VS();
		PixelShader = compile PS_SHADERMODEL ShadowPS();
	}
}
technique SkinnedShadow4 {
	pass P0 {
		VertexShader = compile VS_SHADERMODEL Skinned4VS();
		PixelShader = compile PS_SHADERMODEL ShadowPS();
	}
}

