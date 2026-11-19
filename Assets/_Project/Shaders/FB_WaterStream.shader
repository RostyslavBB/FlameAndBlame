// Fire-hose jet drawn on a LineRenderer (uv.x along the stream, uv.y across).
// Aerated look: dense white core, bubbles racing along it, turbulent edges that break up into spray towards the end.
Shader "FB/WaterStream"
{
    Properties
    {
        _Color ("Body Color", Color) = (0.78, 0.92, 1, 0.8)
        _CoreColor ("Aerated Core", Color) = (1, 1, 1, 0.95)
        _FlowSpeed ("Flow Speed", Float) = 10
        _NoiseScale ("Noise Scale", Float) = 4
        _Breakup ("Edge Breakup", Range(0, 1)) = 0.55
        _Bubbles ("Bubble Amount", Range(0, 1)) = 0.6
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "Stream"
            Tags { "LightMode" = "UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _CoreColor;
                float _FlowSpeed;
                float _NoiseScale;
                float _Breakup;
                float _Bubbles;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
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

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float along = input.uv.x;
                float across = abs(input.uv.y * 2.0 - 1.0);
                float t = _Time.y * _FlowSpeed;
                float flow = Noise(float2(along * _NoiseScale * 10.0 - t, input.uv.y * 3.0));
                float turbulence = Noise(float2(along * _NoiseScale * 28.0 - t * 1.6, input.uv.y * 8.0));

                // Solid near the nozzle, ragged and spraying towards the end.
                float breakup = _Breakup * (0.3 + along);
                float edge = 1.0 - smoothstep(0.5 - flow * breakup, 1.0 - turbulence * breakup * 0.6, across);
                float core = 1.0 - smoothstep(0.0, 0.45, across);

                float bubbleNoise = Noise(float2(along * 90.0 - t * 2.2, input.uv.y * 14.0));
                float bubbles = smoothstep(1.0 - _Bubbles * 0.35, 1.0, bubbleNoise);

                half4 color = lerp(_Color, _CoreColor, saturate(core * 0.75 + flow * 0.25));
                color.rgb = lerp(color.rgb, 1.0, bubbles);
                color.a = saturate(color.a + bubbles * 0.3);
                color.a *= edge * input.color.a * (1.0 - smoothstep(0.85, 1.0, along) * 0.6);
                return color;
            }
            ENDHLSL
        }
    }
}
