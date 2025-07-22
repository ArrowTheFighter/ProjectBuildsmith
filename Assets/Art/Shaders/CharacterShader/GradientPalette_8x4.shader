Shader "Custom/GradientPalette_8x4"
{
    Properties
    {
        _MainTex ("Palette Texture", 2D) = "white" {}

        // 32 color slots
        _Color0 ("Color 0", Color) = (1,1,1,1)
        _Color1 ("Color 1", Color) = (1,1,1,1)
        _Color2 ("Color 2", Color) = (1,1,1,1)
        _Color3 ("Color 3", Color) = (1,1,1,1)
        _Color4 ("Color 4", Color) = (1,1,1,1)
        _Color5 ("Color 5", Color) = (1,1,1,1)
        _Color6 ("Color 6", Color) = (1,1,1,1)
        _Color7 ("Color 7", Color) = (1,1,1,1)
        _Color8 ("Color 8", Color) = (1,1,1,1)
        _Color9 ("Color 9", Color) = (1,1,1,1)
        _Color10 ("Color 10", Color) = (1,1,1,1)
        _Color11 ("Color 11", Color) = (1,1,1,1)
        _Color12 ("Color 12", Color) = (1,1,1,1)
        _Color13 ("Color 13", Color) = (1,1,1,1)
        _Color14 ("Color 14", Color) = (1,1,1,1)
        _Color15 ("Color 15", Color) = (1,1,1,1)
        _Color16 ("Color 16", Color) = (1,1,1,1)
        _Color17 ("Color 17", Color) = (1,1,1,1)
        _Color18 ("Color 18", Color) = (1,1,1,1)
        _Color19 ("Color 19", Color) = (1,1,1,1)
        _Color20 ("Color 20", Color) = (1,1,1,1)
        _Color21 ("Color 21", Color) = (1,1,1,1)
        _Color22 ("Color 22", Color) = (1,1,1,1)
        _Color23 ("Color 23", Color) = (1,1,1,1)
        _Color24 ("Color 24", Color) = (1,1,1,1)
        _Color25 ("Color 25", Color) = (1,1,1,1)
        _Color26 ("Color 26", Color) = (1,1,1,1)
        _Color27 ("Color 27", Color) = (1,1,1,1)
        _Color28 ("Color 28", Color) = (1,1,1,1)
        _Color29 ("Color 29", Color) = (1,1,1,1)
        _Color30 ("Color 30", Color) = (1,1,1,1)
        _Color31 ("Color 31", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            // 32 color overrides
            float4 _Color0, _Color1, _Color2, _Color3, _Color4, _Color5, _Color6, _Color7;
            float4 _Color8, _Color9, _Color10, _Color11, _Color12, _Color13, _Color14, _Color15;
            float4 _Color16, _Color17, _Color18, _Color19, _Color20, _Color21, _Color22, _Color23;
            float4 _Color24, _Color25, _Color26, _Color27, _Color28, _Color29, _Color30, _Color31;

            float GetLuminance(float3 color)
            {
                return dot(color, float3(0.299, 0.587, 0.114));
            }

            float4 GetColorByIndex(int index)
            {
                if (index == 0) return _Color0;
                if (index == 1) return _Color1;
                if (index == 2) return _Color2;
                if (index == 3) return _Color3;
                if (index == 4) return _Color4;
                if (index == 5) return _Color5;
                if (index == 6) return _Color6;
                if (index == 7) return _Color7;
                if (index == 8) return _Color8;
                if (index == 9) return _Color9;
                if (index == 10) return _Color10;
                if (index == 11) return _Color11;
                if (index == 12) return _Color12;
                if (index == 13) return _Color13;
                if (index == 14) return _Color14;
                if (index == 15) return _Color15;
                if (index == 16) return _Color16;
                if (index == 17) return _Color17;
                if (index == 18) return _Color18;
                if (index == 19) return _Color19;
                if (index == 20) return _Color20;
                if (index == 21) return _Color21;
                if (index == 22) return _Color22;
                if (index == 23) return _Color23;
                if (index == 24) return _Color24;
                if (index == 25) return _Color25;
                if (index == 26) return _Color26;
                if (index == 27) return _Color27;
                if (index == 28) return _Color28;
                if (index == 29) return _Color29;
                if (index == 30) return _Color30;
                if (index == 31) return _Color31;

                return float4(1, 1, 1, 1); // fallback
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float4 sampled = tex2D(_MainTex, uv);
                float lum = GetLuminance(sampled.rgb);

                // Texture block layout: 8 x 4
                int blockX = (int)(uv.x * 8.0);
                int blockY = (int)(uv.y * 4.0);
                int index = blockY * 8 + blockX;

                float4 targetColor = GetColorByIndex(index);
                return float4(targetColor.rgb * lum, 1.0);
            }

            ENDHLSL
        }
    }
}
