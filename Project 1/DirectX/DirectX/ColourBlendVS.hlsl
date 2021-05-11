cbuffer CBuf
{
	matrix transform;
};

struct VSOut
{
	float4 colour : Colour;
	float4 pos : SV_Position;
};

VSOut main(float3 pos : Position, float4 colour : Colour)
{
	VSOut vso;
	vso.pos = mul(float4(pos, 1.0f), transform);
	vso.colour = colour;
	return vso;
}