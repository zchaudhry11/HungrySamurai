Shader "Custom/NormalSprites" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)

		_MainTex ("Albedo (RGB)", 2D) = "white" {}

		_NormalTex ("Normal Map", 2D) = "bump" {}
		_NormalIntensity ("Normal Intensity", Float) = 1

		_Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
	}
	SubShader 
		{
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Off
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha vertex:vert alphatest:_Cutoff 
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NormalTex;
		half _NormalIntensity;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_NormalTex;
			fixed4 color;
		};

		fixed4 _Color;

		void vert(inout appdata_full v, out Input o)
		{
			v.normal = float3(0, 0, -1);
			v.tangent = float4(1, 0, 0, 1);
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = _Color;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;

			o.Albedo = c.rgb;
			o.Alpha = c.a;

			o.Normal = UnpackNormal(tex2D(_NormalTex, IN.uv_NormalTex));

			_NormalIntensity = 1 / _NormalIntensity;
			o.Normal.z = o.Normal.z * _NormalIntensity;
			o.Normal = normalize((half3)o.Normal);
		}
		ENDCG
	}
	FallBack "Diffuse"
}