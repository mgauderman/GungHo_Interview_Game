﻿Shader "Custom/SpriteOutline"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_OutlineAlpha("Outline Alpha Threshold", Float) = 0
		_OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
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

		Pass
		{
			CGPROGRAM
			#pragma shader_feature OUTLINE_ON
			#pragma shader_feature DIFFUSE_ON
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
			};

			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
						OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			float _OutlineAlpha;
			fixed4 _OutlineColor;
			float4 _MainTex_TexelSize;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
				c.rgb *= c.a;
				fixed a = c.a;
				#if !DIFFUSE_ON
					c = (0, 0, 0, 0);
				#endif

				#if OUTLINE_ON
					if (a < _OutlineAlpha) {
						// check pixels adjacent to current pixel to see if they are above threshold alpha
						fixed rightAlpha = tex2D(_MainTex, IN.texcoord + fixed2(0, _MainTex_TexelSize.x)).a;
						fixed leftAlpha = tex2D(_MainTex, IN.texcoord - fixed2(0, _MainTex_TexelSize.x)).a;
						fixed upAlpha = tex2D(_MainTex, IN.texcoord + fixed2(0, _MainTex_TexelSize.y)).a;
						fixed downAlpha = tex2D(_MainTex, IN.texcoord - fixed2(0, _MainTex_TexelSize.y)).a;

						if (_OutlineAlpha < rightAlpha || _OutlineAlpha < leftAlpha || _OutlineAlpha < upAlpha || _OutlineAlpha < downAlpha)
						{
							c = _OutlineColor;
						}
					}
					// if there is a non-transparent pixel on edge of sprite, make it outlineColor so there isn't a hole in outline
					else if (IN.texcoord.x < _MainTex_TexelSize.x || IN.texcoord.x + _MainTex_TexelSize.x > 1 || 
								IN.texcoord.y < _MainTex_TexelSize.y || IN.texcoord.y + _MainTex_TexelSize.y > 1)
					{
						c = _OutlineColor;
					}
				#endif

				return c;					
			}
			ENDCG
		}
	}
	CustomEditor "CustomShaderInspector"
}