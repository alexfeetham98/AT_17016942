cbuffer LightCBuf
{
	float3 lightPos;
	float3 ambient;
	float3 diffuseColor;
	float diffuseIntensity;
	float attConst;
	float attLin;
	float attQuad;
};

cbuffer ObjectCBuf
{
	float3 materialColor;
	float specularIntensity;
	float specularPower;
};

float4 main(float3 worldPos : Position, float3 n : Normal) : SV_Target
{
	// Fragment to light vector data
	const float3 vToL = lightPos - worldPos;
	const float distToL = length(vToL);
	const float3 dirToL = vToL / distToL;
	// Attenuation
	const float att = 1.0f / (attConst + attLin * distToL + attQuad * (distToL * distToL));
	// Diffuse Intensity
	const float3 diffuse = diffuseColor * diffuseIntensity * att * max(0.0f, dot(dirToL, n));
	// Reflected Light Vector
	const float3 w = n * dot(vToL, n);
	const float3 r = w * 2.0f - vToL;
	// Calculate specular intensity based on angle between viewing vector
	// and reflection vector, narrow with power function
	const float3 specular = att * (diffuseColor * diffuseIntensity) * specularIntensity *
	pow(max(0.0f, dot(normalize(-r), normalize(worldPos))), specularPower);
	//Final Colour
	return float4(saturate((diffuse + ambient + specular) * materialColor), 1.0f);
}