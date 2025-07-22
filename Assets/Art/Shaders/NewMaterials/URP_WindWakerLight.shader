Shader "URP/Custom/WindWakerLight"
{
    Properties
    {
        _Color ("Light Color", Color) = (1,1,1,1)
        _Intensity ("Intensity", Float) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Name "WindWakerVolume"
            Tags { "LightMode"="UniversalForward" }

            ZWrite Off
            ZTest LEqual
            Cull Back
            Blend One One
            Stencil
            {
                Ref 1
                Comp Equal
                Pass Keep
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _Color;
            float _Intensity;

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                float4 posWS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionHCS = posWS;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                return _Color * _Intensity;
            }
            ENDHLSL
        }
    }
}
