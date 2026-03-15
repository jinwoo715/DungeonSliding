Shader "Custom/SimpleTransparentURP"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BaseColor("Color (Alpha for Opacity)", Color) = (1,1,1,0.5)
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
            LOD 100

            Pass
            {
                // 이 부분이 반투명을 결정하는 핵심 설정입니다.
                Blend SrcAlpha OneMinusSrcAlpha
                ZWrite Off

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                struct Attributes {
                    float4 positionOS : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct Varyings {
                    float4 positionCS : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                sampler2D _MainTex;
                float4 _BaseColor;

                Varyings vert(Attributes IN) {
                    Varyings OUT;
                    OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                    OUT.uv = IN.uv;
                    return OUT;
                }

                half4 frag(Varyings IN) : SV_Target {
                    half4 texColor = tex2D(_MainTex, IN.uv);
                    // 텍스처 컬러와 설정한 색상(Alpha 포함)을 곱합니다.
                    return texColor * _BaseColor;
                }
                ENDHLSL
            }
        }
}