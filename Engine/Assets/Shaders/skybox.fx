#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#define PI 3.14159265358979323844

matrix wvp;
matrix world;
float3 cameraPosition;
Texture2D texure;

sampler2D skyboxSampler = sampler_state {
	texture = <texure>;
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

// Structures
struct App2VS {
	float4 position : POSITION0;
};

struct VS2PS {
	float4 position : SV_POSITION;
	float4 direction : TEXCOORD0;
};

// Shaders
VS2PS MainVS(App2VS input) {
	VS2PS output = (VS2PS)0;
	float4 vertexPosition = mul(input.position, world);
	output.position = mul(input.position, wvp);
	output.direction = vertexPosition - float4(cameraPosition,0);
	return output;
}

float4 MainPS(VS2PS input) : COLOR {
	float3 v = normalize(input.direction.xyz);
	// convert direction to azimuth and elevation
	float azimuth = atan2(v.y, v.x);
	float elevation = atan2(v.z, sqrt(v.x * v.x + v.y * v.y));
		
	// map azimuth from [-2*PI,2*PI] to [0,1]
	// map elevation from [-PI,PI] to [0,1]
	float tu = clamp(azimuth * 0.5 / PI + 0.5, 0, 1);
	float tv = 1 - clamp(elevation / PI + 0.5,0,1);

	return tex2D(skyboxSampler, float2(tu,tv));
}

technique ModelRender {
	pass Pass2 {
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}
