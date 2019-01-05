Shader "Custom/Wm3Overlay" {
	Properties{
		_Color("Ambient Color", Color) = (1,1,1,1)
		_Overlay("Activate Overlay", Int) = 1
		_TransColor("Overlay Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Diffuse("Diffuse", Range(0,1)) = 0
	}

		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "TransparentCutout" "IgnoreProjector" = "True"}
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
			fixed4 _TransColor;
			fixed _Overlay;
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
			};

			vs_out vert(vs_in IN)
			{
				vs_out OUT;
				OUT.pos = UnityObjectToClipPos(IN.pos);
				OUT.worldPos = mul(unity_ObjectToWorld, IN.pos);
				OUT.normal = normalize(mul(unity_ObjectToWorld, IN.normal));
				OUT.tex = IN.tex;
				return OUT;
			}

			float4 frag(vs_out IN) : COLOR
			{
				float4 colortex = tex2D(_MainTex, IN.tex);
				float trans = all(colortex.rgb == _TransColor.rgb) & (_Overlay > 0);
				clip(-trans);
				float4 color = Wm3Lighting(colortex, IN.normal, IN.worldPos, _Diffuse, _Color);
				return color;
			}
			ENDCG
		}
	}
}
