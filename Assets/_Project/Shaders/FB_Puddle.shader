// Puddles on the floor for URP (WebGL2-safe). Reads the simulated water grid (R = depth in meters, G = electrified):
// the vertex shader lifts the surface by the depth, so the water level visibly rises;
// calm surface, irregular soft edges, meniscus from the depth gradient, wet-floor darkening, sky reflection and
// sun glint, expanding ring ripples where the hose lands, foam at the impact, electric veins on live water.
Shader "FB/Puddle"
{
    Properties
    {
        [NoScaleOffset] _WaterTex ("Water Grid (set at runtime)", 2D) = "black" {}
        _WaterTint ("Water Tint", Color) = (0.1, 0.2, 0.28, 1)
        _ShallowAlpha ("Shallow Alpha", Range(0, 1)) = 0.35
        _DeepAlpha ("Deep Alpha", Range(0, 1)) = 0.6
        _EdgeLow ("Edge Start (m)", Range(0, 0.02)) = 0.0012
        _EdgeHigh ("Edge Full (m)", Range(0, 0.02)) = 0.003
        _EdgeNoise ("Edge Noise (m)", Range(0, 0.01)) = 0.001
        _DeepDepth ("Deep Water Depth (m)", Float) = 0.04
        _EdgeSlope ("Meniscus Strength", Float) = 400
        _MicroNormal ("Micro Normal", Range(0, 1)) = 0.08
        _SkyColor ("Reflection Color", Color) = (0.82, 0.9, 1, 1)
        _Reflection ("Reflection", Range(0, 1)) = 0.8
        _RimStrength ("Meniscus Highlight", Float) = 600
        _RimColor ("Meniscus Color", Color) = (1, 1, 1, 0.7)
        _Gloss ("Gloss", Float) = 300
        _SpecularIntensity ("Specular", Float) = 3
        _FresnelPower ("Fresnel Power", Float) = 2.5
        _RippleSpeed ("Ripple Speed", Float) = 0.9
        _RippleFrequency ("Ripple Frequency", Float) = 45
        _RippleStrength ("Ripple Strength", Float) = 0.9
        _RippleLife ("Ripple Life", Float) = 1.4
        _FoamColor ("Foam Color", Color) = (1, 1, 1, 0.9)
        _ElectricColor ("Electric Color", Color) = (0.7, 0.95, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent-20" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "Puddle"
            Tags { "LightMode" = "UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_WaterTex);
            SAMPLER(sampler_WaterTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _WaterTex_TexelSize;
                float4 _GridRect;
                float4 _Ripples[8];
                half4 _WaterTint;
                float _ShallowAlpha;
                float _DeepAlpha;
                float _EdgeLow;
                float _EdgeHigh;
                float _EdgeNoise;
                float _DeepDepth;
                float _EdgeSlope;
                float _MicroNormal;
                half4 _SkyColor;
                float _Reflection;
                float _RimStrength;
                half4 _RimColor;
                float _Gloss;
                float _SpecularIntensity;
                float _FresnelPower;
                float _RippleSpeed;
                float _RippleFrequency;
                float _RippleStrength;
                float _RippleLife;
                half4 _FoamColor;
                half4 _ElectricColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
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

            float DepthAt(float2 uv)
            {
                return SAMPLE_TEXTURE2D(_WaterTex, sampler_WaterTex, uv).r;
            }

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float2 uv = (output.positionWS.xz - _GridRect.xy) / _GridRect.zw;
                output.positionWS.y += SAMPLE_TEXTURE2D_LOD(_WaterTex, sampler_WaterTex, uv, 0).r;
                output.positionCS = TransformWorldToHClip(output.positionWS);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 xz = input.positionWS.xz;
                float2 uv = (xz - _GridRect.xy) / _GridRect.zw;
                half4 water = SAMPLE_TEXTURE2D(_WaterTex, sampler_WaterTex, uv);
                float depth = water.r;

                // Static noise breaks the bilinear grid into organic puddle outlines (no animation: calm water).
                float outline = Noise(xz * 5.0) * 0.65 + Noise(xz * 13.0) * 0.35;
                float edge = smoothstep(_EdgeLow, _EdgeHigh, depth + (outline - 0.5) * _EdgeNoise);
                clip(edge - 0.01);

                float2 texel = _WaterTex_TexelSize.xy;
                float dx = DepthAt(uv - float2(texel.x, 0.0)) - DepthAt(uv + float2(texel.x, 0.0));
                float dz = DepthAt(uv - float2(0.0, texel.y)) - DepthAt(uv + float2(0.0, texel.y));
                float3 normal = float3(dx * _EdgeSlope, 1.0, dz * _EdgeSlope);
                normal.xz += (float2(Noise(xz * 40.0), Noise(xz * 40.0 + 17.0)) - 0.5) * _MicroNormal;

                float foam = 0.0;
                for (int i = 0; i < 8; i++)
                {
                    float4 ripple = _Ripples[i];
                    float age = _Time.y - ripple.z;
                    if (ripple.w <= 0.0 || age < 0.0 || age > _RippleLife)
                    {
                        continue;
                    }

                    float2 offset = xz - ripple.xy;
                    float distance = length(offset);
                    float band = distance - age * _RippleSpeed;
                    float fade = ripple.w * exp(-age * 2.5) * saturate(1.0 - abs(band) * 5.0);
                    float2 direction = offset / max(distance, 0.001);
                    normal.xz += direction * cos(band * _RippleFrequency) * fade * _RippleStrength;
                    foam += ripple.w * saturate(1.0 - distance / 0.22) * exp(-age * 4.0);
                }

                normal = normalize(normal);
                Light mainLight = GetMainLight();
                float3 viewDir = normalize(GetWorldSpaceViewDir(input.positionWS));
                float3 halfDir = normalize(mainLight.direction + viewDir);
                float specular = pow(saturate(dot(normal, halfDir)), _Gloss) * _SpecularIntensity;
                float fresnel = pow(1.0 - saturate(dot(normal, viewDir)), _FresnelPower);

                half4 color = _WaterTint;
                color.a = lerp(_ShallowAlpha, _DeepAlpha, saturate(depth / _DeepDepth));
                color.rgb = lerp(color.rgb, _SkyColor.rgb, fresnel * _Reflection);
                color.rgb += specular * mainLight.color;
                color.a = saturate(color.a + fresnel * _Reflection * 0.5 + specular);

                // Thin bright line where the water surface curves down to the floor (meniscus catching the light).
                float rim = saturate(length(float2(dx, dz)) * _RimStrength) * (1.0 - edge * 0.5);
                color.rgb = lerp(color.rgb, _RimColor.rgb, rim * _RimColor.a);
                color.a = saturate(color.a + rim * _RimColor.a * 0.5);

                float foamMask = saturate(foam * (0.6 + Noise(xz * 30.0 + _Time.y) * 0.8));
                color.rgb = lerp(color.rgb, _FoamColor.rgb, foamMask);
                color.a = max(color.a, foamMask * _FoamColor.a);

                float electric = water.g;
                float t = _Time.y;
                float ridge = pow(1.0 - abs(Noise(xz * 4.0 + t * 2.5) * 2.0 - 1.0), 12.0);
                float flicker = 0.5 + 0.5 * sin(t * 40.0 + outline * 8.0);
                color.rgb = lerp(color.rgb, _ElectricColor.rgb, electric * flicker * 0.35);
                color.rgb += _ElectricColor.rgb * ridge * electric * 2.0;
                color.a = saturate(color.a + ridge * electric * 0.8);

                color.a *= edge;
                return color;
            }
            ENDHLSL
        }
    }
}
