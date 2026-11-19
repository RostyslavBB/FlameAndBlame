// Smoke gathering under the ceiling (URP, WebGL2-safe). Stacked transparent planes with domain-warped, slowly
// swirling noise give a volumetric look; scene depth softens the intersections with walls and furniture,
// and the layer fades near the camera so walking under it never shows a hard edge.
Shader "FB/SmokeLayer"
{
    Properties
    {
        _Color ("Smoke Color", Color) = (0.36, 0.34, 0.33, 1)
        _LightColor ("Lit Wisps", Color) = (0.62, 0.59, 0.56, 1)
        _Density ("Density (runtime)", Range(0, 1)) = 0
        _MaxAlpha ("Max Opacity", Range(0, 1)) = 0.92
        _NoiseScale ("Noise Scale", Float) = 0.55
        _Speed ("Swirl Speed", Float) = 0.06
        _SoftDistance ("Soft Intersection (m)", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent+10" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "Smoke"
            Tags { "LightMode" = "UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _LightColor;
                float _Density;
                float _MaxAlpha;
                float _NoiseScale;
                float _Speed;
                float _SoftDistance;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            float Hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float Noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(Hash(i), Hash(i + float2(1.0, 0.0)), u.x),
                    lerp(Hash(i + float2(0.0, 1.0)), Hash(i + float2(1.0, 1.0)), u.x), u.y);
            }

            float Fbm(float2 p)
            {
                return Noise(p) * 0.55 + Noise(p * 2.1 + 3.7) * 0.3 + Noise(p * 4.3 + 7.1) * 0.15;
            }

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS);
                output.screenPos = ComputeScreenPos(output.positionCS);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float t = _Time.y * _Speed;
                float2 p = input.positionWS.xz * _NoiseScale + input.positionWS.y * 1.7;
                float2 warp = float2(Fbm(p + t), Fbm(p - t + 5.2));
                float n = Fbm(p + warp * 1.6 + float2(t * 0.5, -t * 0.3));

                // More smoke = more coverage, not just more opacity.
                float coverage = smoothstep(0.62 - _Density * 0.45, 0.95, n + _Density * 0.15);

                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                float sceneDepth = LinearEyeDepth(SampleSceneDepth(screenUV), _ZBufferParams);
                float soft = saturate((sceneDepth - input.screenPos.w) / _SoftDistance);
                float nearFade = saturate((input.screenPos.w - 0.3) / 1.2);

                half4 color;
                color.rgb = lerp(_Color.rgb, _LightColor.rgb, saturate(n * 1.4 - 0.4));
                color.a = coverage * _Density * _MaxAlpha * soft * nearFade;
                return color;
            }
            ENDHLSL
        }
    }
}
