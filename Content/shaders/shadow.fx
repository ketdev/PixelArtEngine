#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix wvp;

// Structures
struct App2VS {
	float4 Position : POSITION0;
};

struct VS2PS {
	float4 Position : SV_POSITION;
	float2 Depth : TEXCOORD2;
};

// Shaders
VS2PS MainVS(App2VS input) {
	VS2PS output = (VS2PS)0;
	output.Position = mul(input.Position, wvp);
	output.Depth = output.Position.zw;
	return output;
}

float4 MainPS(VS2PS input) : COLOR {
	float4 output;
	output.rgb = input.Depth.x / input.Depth.y;
	output.a = 1;
	return output;
}

technique ModelRender {
	pass Pass2 {
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}
