Shader "Custom/Wm3Sky" {
	Properties {
		_Color ("Ambient Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_ScaleX("Scale X", Range(0.1,10)) = 1.0
		_ScaleY("Scale Y", Range(0.1,10)) = 1.7
		_ScrollSpeed("Scroll Speed", Range(0,10)) = 4
		_OffsetY("Offset Y", Range(-1,1)) = 0.1
	}

	SubShader{
			Tags{ "Queue" = "Background" "RenderType" = "Opaque" "IgnoreProjector" = "True" }
			Fog{ Mode off }
			
			Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			fixed4 _Color;
			uniform float _ScaleX;
			uniform float _ScaleY;
			uniform float _OffsetY;
			uniform float _ScrollSpeed;

			struct vs_in
			{
				float4 pos : POSITION;
				float2 tex : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct vs_out
			{
				float4 pos : POSITION;
				float4 tex : TEXCOORD0;
				float3 dir : TEXCOORD1;
				float3 normal : NORMAL;
			};

			vs_out vert(vs_in IN)
			{
				vs_out OUT;
				OUT.pos = UnityObjectToClipPos(IN.pos);
				OUT.normal = normalize(mul(UNITY_MATRIX_M, IN.normal));
				OUT.tex = OUT.pos;// mul(UNITY_MATRIX_M, IN.pos);
				OUT.dir = mul(unity_CameraToWorld, float3(0, 0, 1));
				return OUT;
			}

			#define PI 3.141592653589793238462
			float4 frag(vs_out IN) : COLOR
			{
				//calculate screen UVs
				float2 screenUV = IN.tex.xy / IN.tex.w; // -1..1

				//calcualte screen size and aspect correction
				float2 corr = float2(_ScaleX * _ScreenParams.x / _ScreenParams.y, _ScaleY);

				//horizontal shifting
				float angle = atan2(IN.dir.z, IN.dir.x) / PI; /* -1..1*/
//				screenUV.x -= 2*angle;

				//vertical shifting
				screenUV.y *= -1;
				screenUV.y += _OffsetY + IN.dir.y * 2;

				//apply screen size and aspect correction
				screenUV *= corr;
				screenUV.x -= _ScrollSpeed * angle;

				float4 colortex = tex2D(_MainTex, screenUV);
				float4 color;
				color = saturate((colortex) * _Color);
				return color;
			}
			ENDCG
		}
	}
}
