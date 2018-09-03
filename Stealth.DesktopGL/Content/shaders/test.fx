#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D ColorTexture;
Texture2D DepthTexture;
float2 ScreenPixel; // size of a screen pixel

sampler2D ColorSampler = sampler_state {
	Texture = <ColorTexture>;
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};
sampler2D DepthSampler = sampler_state {
	Texture = <DepthTexture>;
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

// Structures
struct VSInput {
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VSOutput {
	float4 Position : SV_POSITION;
	float2 TextureCoordinate : TEXCOORD0;
};

VSOutput MainVS(VSInput input) {
	VSOutput output;
	output.Position = input.Position;
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}

float4 MainPS(VSOutput input) : COLOR {
	float4 color = float4(0,0,0,0);

	float2 dX = float2(ScreenPixel.x, 0);
	float2 dY = float2(0, ScreenPixel.y);

	float4 color_c = tex2D(ColorSampler, input.TextureCoordinate);
	float4 color_t = tex2D(ColorSampler, input.TextureCoordinate + dY);
	float4 color_b = tex2D(ColorSampler, input.TextureCoordinate - dY);
	float4 color_r = tex2D(ColorSampler, input.TextureCoordinate + dX);
	float4 color_l = tex2D(ColorSampler, input.TextureCoordinate - dX);
	
	float depth_c = tex2D(DepthSampler, input.TextureCoordinate).r;
	float depth_t = tex2D(DepthSampler, input.TextureCoordinate + dY).r;
	float depth_b = tex2D(DepthSampler, input.TextureCoordinate - dY).r;
	float depth_r = tex2D(DepthSampler, input.TextureCoordinate + dX).r;
	float depth_l = tex2D(DepthSampler, input.TextureCoordinate - dX).r;

	// depth deltas
	float dt = depth_t - depth_c;
	float db = depth_b - depth_c;
	float dr = depth_r - depth_c;
	float dl = depth_l - depth_c;

	// check if different to neighbor ids
	bool diff_t = (color_c.r != color_t.r) || (color_c.g != color_t.g) || (color_c.b != color_t.b);
	bool diff_b = (color_c.r != color_b.r) || (color_c.g != color_b.g) || (color_c.b != color_b.b);
	bool diff_r = (color_c.r != color_r.r) || (color_c.g != color_r.g) || (color_c.b != color_r.b);
	bool diff_l = (color_c.r != color_l.r) || (color_c.g != color_l.g) || (color_c.b != color_l.b);
	
	// needs to be furthest than at least one neighbor with a different id
	float epsilon = 0.00001f;
	bool furthest = (diff_t && dt < epsilon)
		         || (diff_b && db < epsilon) 
		         || (diff_r && dr < epsilon) 
		         || (diff_l && dl < epsilon);
	
	if (furthest) {
		// take border color of closest
		float lowest = min(min(dt, db), min(dr, dl));
		dt = floor(1 - (dt - lowest));
		db = floor(1 - (db - lowest));
		dr = floor(1 - (dr - lowest));
		dl = floor(1 - (dl - lowest));
		color =   color_t * dt
				+ color_b * db
				+ color_r * dr
				+ color_l * dl;
		color.a = 1;
	}

	return color;
}

technique SpriteDrawing {
	pass P0 {
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};