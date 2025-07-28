Shader "Skybox/NightDay Cubemap"
{
    Properties
    {
        _Tex1("Cubemap 1", Cube) = "white" {}
        _Tex2("Cubemap 2", Cube) = "white" {}
        _Blend("Blend", Range(0, 1)) = 0.5
        _Rotation("Rotation", Float) = 0
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float3 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            samplerCUBE _Tex1;
            samplerCUBE _Tex2;
            float _Blend;
            float _Rotation;

            v2f vert(appdata v)
            {
                v2f o;
                o.texcoord = v.vertex.xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Convert rotation to radians
                float rad = radians(_Rotation);
                float cosR = cos(rad);
                float sinR = sin(rad);

                // Rotate around Y axis
                float3 dir = i.texcoord;
                float3 rotatedDir;
                rotatedDir.x = cosR * dir.x - sinR * dir.z;
                rotatedDir.y = dir.y;
                rotatedDir.z = sinR * dir.x + cosR * dir.z;

                fixed4 colorTex1 = texCUBE(_Tex1, rotatedDir);
                fixed4 colorTex2 = texCUBE(_Tex2, rotatedDir);
                // Blend softly between 0.25–0.75
                float blendB = smoothstep(0.1, 0.9, _Blend);
                float blendA = 1.0 - blendB;

                return colorTex1 * blendA + colorTex2 * blendB;
            }
            ENDCG
        }
    }
}