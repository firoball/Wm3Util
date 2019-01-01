Shader "Custom/Wm3Overlay" {
	Properties{
		_Color("Ambient Color", Color) = (1,1,1,1)
		_Overlay("Activate Overlay", Int) = 1
		_TransColor("Overlay Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}

		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "TransparentCutout" "IgnoreProjector" = "True"}
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
				float fog : TEXCOORD2;
			};

			vs_out vert(vs_in IN)
			{
				vs_out OUT;
				OUT.pos = UnityObjectToClipPos(IN.pos);
				OUT.worldPos = mul(unity_ObjectToWorld, IN.pos);
				OUT.normal = normalize(mul(unity_ObjectToWorld, IN.normal));
				OUT.tex = IN.tex;
				OUT.fog = OUT.pos.z;
				return OUT;
			}

			float4 frag(vs_out IN) : COLOR
			{
				float4 colortex = tex2D(_MainTex, IN.tex);
				float trans = (colortex == _TransColor) && (_Overlay > 0);
				clip(-trans);
				float diffuse = 1;// 0.7 + 1.1*saturate(dot(_WorldSpaceLightPos0, IN.normal) * 4); //TODO albedo

				float viewDistance = distance( _WorldSpaceCameraPos, IN.worldPos);
				float fogFactor =  saturate((viewDistance - 50) / (100 - 50));

				float4 color;
				color = saturate(colortex * diffuse * _Color);// *unity_AmbientSky);
				color = lerp(color, unity_FogColor, fogFactor);
				color.a = _Color.a;
				return color;
			}
			ENDCG
		}
	}
}
