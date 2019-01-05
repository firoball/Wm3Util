Shader "Custom/Wm3Solid" {
	Properties{
		_Color("Ambient Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Diffuse("Diffuse", Range(0,1)) = 0
	}

		SubShader{
		Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" "IgnoreProjector" = "True"}
		Fog { Mode off }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite on

		Pass{
			CGPROGRAM

			#include "Wm3Lighting.cginc"
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			fixed4 _Color;
			uniform float _Diffuse;

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
				//float fog : TEXCOORD2;
			};

			vs_out vert(vs_in IN)
			{
				vs_out OUT;
				OUT.pos = UnityObjectToClipPos(IN.pos);
				OUT.worldPos = mul(unity_ObjectToWorld, IN.pos);
				OUT.normal = normalize(mul(unity_ObjectToWorld, IN.normal));
				OUT.tex = IN.tex;
				//OUT.fog = OUT.pos.z;
				return OUT;
			}

			float4 frag(vs_out IN) : COLOR
			{
				float4 colortex = tex2D(_MainTex, IN.tex);
				float4 color = Wm3Lighting(colortex, IN.normal, IN.worldPos, _Diffuse, _Color);
				/*float4 diffuse = _Diffuse * saturate(dot(_WorldSpaceLightPos0, IN.normal));
				diffuse.a = 0;

				float viewDistance = distance( _WorldSpaceCameraPos, IN.worldPos);
				float fogFactor = saturate(viewDistance * unity_FogParams.z + unity_FogParams.w);
				float4 light = _Color + diffuse + unity_AmbientSky;
				float4 color = saturate(colortex * light);
				color = lerp(unity_FogColor, color, fogFactor);*/
				return color;
			}
			ENDCG
		}
	}
}
