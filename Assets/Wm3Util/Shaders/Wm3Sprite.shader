Shader "Custom/Wm3Sprite" {
	Properties{
		_Color("Ambient Color", Color) = (1,1,1,1)
		_TransColor("Overlay Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}

		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "TransparentCutout" "IgnoreProjector" = "True" "DisableBatching" = "True"}
		Fog { Mode off }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite on

		Pass{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			fixed4 _Color;
			fixed4 _TransColor;
			fixed _Overlay;

			struct vs_in
			{
				float4 pos : POSITION;
				float2 tex : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct vs_out
			{
				float4 pos : POSITION;
				float2 tex : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float3 normal : NORMAL;
			};

			float3 BillboardRotation(float3 vertex)
			{
				float3 dir = normalize(mul(unity_CameraToWorld, float3(1, 0, 0)));
				float2x2 m = float2x2(dir.x, -dir.z, dir.z, dir.x);
				return float3(mul(m, vertex.xz), vertex.y).xzy;
			}

			float4 BillboardTRS(float4 vertex)
			{	//strip off translation
				float3 pos = mul((float3x3)unity_ObjectToWorld, vertex.xyz);
				//store translation separately
				float4 translation = float4(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13, unity_ObjectToWorld._m23, 1);
				//apply billboard rotation
				pos = BillboardRotation(pos);
				//now apply translation
				pos += translation.xyz;
				return float4(pos, 1);
			}

			vs_out vert(vs_in IN)
			{
				vs_out OUT;
				float4 pos = BillboardTRS(IN.pos);
				OUT.pos = mul(UNITY_MATRIX_VP, pos);

				OUT.worldPos = mul(unity_ObjectToWorld, IN.pos);
				OUT.normal = normalize(mul(unity_ObjectToWorld, IN.normal));
				OUT.tex = IN.tex;
				return OUT;
			}

			float4 frag(vs_out IN) : COLOR
			{
				float4 colortex = tex2D(_MainTex, IN.tex);
				float trans = (colortex == _TransColor);
				clip(-trans);
				float diffuse = 1;// 0.7 + 1.1*saturate(dot(_WorldSpaceLightPos0, IN.normal) * 4); //TODO albedo

				float viewDistance = distance( _WorldSpaceCameraPos, IN.worldPos);
				float fogFactor =  saturate((viewDistance - 50) / (100 - 50));

				float4 color;
				color = saturate(colortex * diffuse * _Color);// *unity_AmbientSky);
				color = lerp(color, unity_FogColor, fogFactor);
				return color;
			}
			ENDCG
		}
	}
}
