Shader "Custom/BurnSprite"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0

		// Clip
		_SliceGuide("Slice Guide", 2D) = "white" {}
		_SliceAmount("Slice Amount", Range(0, 1)) = 0.5

		// Burn
		_BurnSize("Burn Size", Range(0, 1)) = 0.15
		_BurnRamp("Burn Ramp", 2D) = "white" {}

		//Distortion
		_DistPow("Distortion Power", Float) = 1.0
		_DistTex("Distortion (Inner)", 2D) = "bump" {}

		//Distortion Vertex Movement
		_DistSpeed("Distortion Speed (Vertex Movement)", Float) = 1
		_DistAmplitude("Distortion Amplitude (Vertex Movement)", Float) = 1
		_DistHeight("Distortion Height (Vertex Movement)", Float) = 1
	}

		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
	}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		CGPROGRAM
			#pragma surface surf Lambert vertex:vert nofog keepalpha
			#pragma target 4.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

			sampler2D _MainTex;
			fixed4 _Color;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

			sampler2D _SliceGuide;
			sampler2D _BurnRamp;
			sampler2D _DistTex;

			// Clip
			half _SliceAmount;

			// Burn
			half _BurnSize;

			//Distortion
			half _DistPow;

			//Distortion Vertex Movement
			half _DistSpeed;
			half _DistAmplitude;
			half _DistHeight;


			struct Input
			{
				float2 uv_MainTex;
				fixed4 color;
				half2 uv_SliceGuide;
				half2 uv_DistTex;
				half _SliceAmount;
			};

			void vert(inout appdata_full v, out Input o)
			{
				#if defined(PIXELSNAP_ON)
						v.vertex = UnityPixelSnap(v.vertex);
				#endif

				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.color = v.color * _Color;
			}

			fixed4 SampleSpriteTexture(float2 uv)
			{
				fixed4 color = tex2D(_MainTex, uv);

				#if ETC1_EXTERNAL_ALPHA
						color.a = tex2D(_AlphaTex, uv).r;
				#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}

			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 c = SampleSpriteTexture(IN.uv_MainTex) * IN.color;
				o.Albedo = c.rgb * c.a;
				o.Alpha = c.a;

				// Slice dark pixels
				clip(tex2D(_SliceGuide, IN.uv_SliceGuide).rgb - _SliceAmount);

				// Edge distortion
				float fader = smoothstep(1, 0, _Time.x * 7);
				half4 distortionflow = tex2D(_DistTex, float2(IN.uv_DistTex.x + (_Time.x * 2), IN.uv_DistTex.y + (_Time.x * 2)));

				// Add burned edge to sliced pixels
				half test = tex2D(_SliceGuide, IN.uv_MainTex).rgb - _SliceAmount;

				float4 burnTex = tex2D(_BurnRamp, float2(test * (1 / _BurnSize), 0));

				if (test < _BurnSize && _SliceAmount > 0 && _SliceAmount < 1)
				{
					o.Emission = burnTex - distortionflow * _DistPow;
					o.Albedo *= o.Emission;
				}

				o.Alpha = c.a;
			}
		ENDCG
	}
	Fallback "Transparent/VertexLit"
}