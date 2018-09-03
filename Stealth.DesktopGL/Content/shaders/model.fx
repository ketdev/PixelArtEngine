#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D tex;
matrix wvp;
float3 id; // mesh identifier for separating borders

// Properties
sampler2D textureSampler = sampler_state {
	Texture = <tex>;
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

// Structures
struct App2VS {
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VS2PS {
	float4 Position : POSITION0;
	float4 Dist : COLOR0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct PSOut {
	float4 color0 : COLOR0;
	float4 color1 : COLOR1;
	float4 color2 : COLOR2;
};

// Shaders
VS2PS MainVS(App2VS input) {
	VS2PS output = (VS2PS)0;
	output.Position = mul(input.Position, wvp);
	output.Dist.x = output.Position.z / output.Position.w;
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

PSOut MainPS(VS2PS input) {
	PSOut output = (PSOut)0;
	output.color0 = tex2D(textureSampler, input.TextureCoordinate);	
	output.color1.rgb = id;
	output.color1.a = 1;

	// depth
	output.color2.r = input.Dist.x;
	output.color2.a = 1;
	return output;
}

technique ModelRender {
	pass Pass2 {
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}