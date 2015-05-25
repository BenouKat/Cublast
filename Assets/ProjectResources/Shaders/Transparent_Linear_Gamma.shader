// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/Transparent_Linear_Gamma" 
{
	Properties 
	{
		_Color ("Main Color", COLOR) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}

	SubShader 
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 
		
		Pass 
		{  
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile __ UNITY_COLORSPACE_GAMMA

				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					half4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					half4 color : COLOR;
					half2 texcoord : TEXCOORD0;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed4 _Color;

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.color = v.color;
					return o;
				}

				fixed4 frag (v2f i) : COLOR
				{
					half4 col = i.color;
                    col.a *= tex2D(_MainTex, i.texcoord).a;
                    col = col * _Color;

					#if !defined(UNITY_COLORSPACE_GAMMA)
						col.a = pow(col.a, 2.2);
					#endif
					return col;
				}
			ENDCG
		}
	}
}
