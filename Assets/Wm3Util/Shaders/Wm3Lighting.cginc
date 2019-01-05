inline float4 Wm3Lighting(float4 tex, float3 normal, float3 worldPosition, float diffuseFac, float4 ambient)
{
	float4 diffuse = diffuseFac * saturate(dot(_WorldSpaceLightPos0, normal));
	diffuse.a = 0;

	float viewDistance = distance(_WorldSpaceCameraPos, worldPosition);
	float fogFactor = saturate(viewDistance * unity_FogParams.z + unity_FogParams.w);
	float4 light = ambient + diffuse + unity_AmbientSky;
	float4 color = saturate(tex * light);
	color.a = ambient.a;
	color = lerp(unity_FogColor, color, fogFactor);

	return color;
}