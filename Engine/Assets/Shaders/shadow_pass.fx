#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Structures
struct App2VS {
	float4 Position : POSITION0;
};

struct VS2PS {
	float4 Position : SV_POSITION;
};

// Shaders
VS2PS MainVS(App2VS input) {
	VS2PS output;
	output.Position = input.Position;
	return output;
}

float4 MainPS(VS2PS input) : COLOR {
	// shadow color
	return float4(0, 0, 0, 0.2);
}

technique BorderEffect {
	pass P0 {
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};