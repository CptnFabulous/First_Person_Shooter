Shader "Custom/NightVisionEffect"
{
    Properties
    {
        _MainTex("-", 2D) = ""{}
    }
	CGINCLUDE
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	float2 _MainTex_TexelSize;

	half4 _ColourTint;
	float _FuzzOffset;

	float nrand(float x, float y)
	{
		return frac(sin(dot(float2(x,y),float2(12.9898,78.233)))*43758.5453);
	}

	half4 frag(v2f_img i): SV_Target
	{
		float u = i.uv.x;
		float v = i.uv.y;

		float fuzz = nrand(v, _Time.x) * _FuzzOffset;
		half4 fuzzResult = tex2D(_MainTex, frac(float2(u + fuzz, v + fuzz)));
		return fuzzResult * _ColourTint;


	}
	ENDCG

    SubShader
    {
        Pass
		{
			ZTEST Always Cull Off ZWrite Off
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 3.0
			ENDCG
		}
    }
    FallBack "Diffuse"
}
