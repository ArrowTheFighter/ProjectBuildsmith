Shader "Custom/GradientPaletteURP"
{
    Properties
    {
        _MainTex ("Palette Texture", 2D) = "white" {}
        _Color0 ("Color Slot 0", Color) = (1,1,1,1)
        _Color1 ("Color Slot 1", Color) = (1,1,1,1)
        _Color2 ("Color Slot 2", Color) = (1,1,1,1)
        _Color3 ("Color Slot 3", Color) = (1,1,1,1)
        // Add more as needed
        _BlockCount ("Block Count (horizontal)", Float) = 4
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
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

            float4 _Color0;
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;
            float _BlockCount;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            float GetLuminance(float3 color)
            {
                return dot(color, float3(0.299, 0.587, 0.114));
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float4 sampled = tex2D(_MainTex, uv);
                float lum = GetLuminance(sampled.rgb);

                // Determine which block this UV falls into
                float2 blockUV = uv * _BlockCount;
                int blockIndex = (int)floor(blockUV.x) + (int)floor(blockUV.y) * (int)_BlockCount;

                float4 targetColor;
                if (blockIndex == 0) targetColor = _Color0;
                else if (blockIndex == 1) targetColor = _Color1;
                else if (blockIndex == 2) targetColor = _Color2;
                else if (blockIndex == 3) targetColor = _Color3;
                else targetColor = float4(1,1,1,1); // fallback

                return float4(targetColor.rgb * lum, 1.0);
            }

            ENDHLSL
        }
    }
}
