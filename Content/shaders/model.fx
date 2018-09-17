#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D Texture;
matrix WVP;

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

// Shaders
VS2PS MainVS(App2VS input) {
	VS2PS output = (VS2PS)0;
	output.Position = mul(input.Position, WVP);
	output.TexCoord = input.TexCoord;
	output.Depth = output.Position.zw;
	return output;
}

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

technique ModelRender {
	pass Pass2 {
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}
