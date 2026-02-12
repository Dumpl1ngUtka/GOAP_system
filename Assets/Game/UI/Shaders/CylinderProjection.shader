Shader "Custom/CylinderProjection"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,0.2)
        _IntersectColor ("Intersection Color", Color) = (1,0,0,1)
        _IntersectThreshold ("Intersection Threshold", Range(0.0, 2.0)) = 0.5
        _RingWidth ("Ring Width", Range(0.0, 0.5)) = 0.1
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            Stencil
            {
                Ref 1         
                Comp NotEqual  
                Pass Keep       
            }
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float3 positionOS : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _IntersectColor;
                float _IntersectThreshold;
                float _RingWidth;
                float4 _MainTex_ST;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.screenPos = ComputeScreenPos(output.positionCS);
                output.positionOS = input.positionOS.xyz;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                
                float dist = length(input.positionOS.xz);
                float innerRadius = 0.5 - _RingWidth;
                
                float ringMask = smoothstep(innerRadius - 0.01, innerRadius, dist);
                
                if (ringMask <= 0.001)
                {
                    discard;
                }

                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                
                // Sample scene depth
                float depth = SampleSceneDepth(screenUV);
                float sceneDepthLinear = LinearEyeDepth(depth, _ZBufferParams);
                
                // Fragment depth (linear)
                float surfaceDepthLinear = input.screenPos.w;
                
                float diff = sceneDepthLinear - surfaceDepthLinear;
                
                half4 finalColor = _BaseColor * texColor;
                finalColor.a *= ringMask;
                
                // Intersection highlight
                if (diff > 0 && diff < _IntersectThreshold)
                {
                    float t = diff / _IntersectThreshold;
                    float intersectionFactor = 1.0 - t;
                    finalColor = lerp(finalColor, _IntersectColor, intersectionFactor);
                    finalColor.a = max(finalColor.a, _IntersectColor.a);
                }
                
                return finalColor;
            }
            ENDHLSL
        }
    }
}