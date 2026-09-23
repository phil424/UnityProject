Shader "MiniCrawler/Environment/Tilemap World Lit"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [MainColor] _Color ("Tint", Color) = (1, 1, 1, 1)

        _AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.1
        _AmbientStrength ("Ambient Strength", Range(0, 2)) = 0.65
        _DirectLightStrength ("Direct Light Strength", Range(0, 2)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "AlphaTest"
            "RenderType" = "TransparentCutout"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "CanUseSpriteAtlas" = "True"
        }

        Pass
        {
            Name "ForwardLit"

            Tags
            {
                "LightMode" = "UniversalForward"
            }

            Cull Off
            ZWrite On

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _FORWARD_PLUS
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half _AlphaCutoff;
                half _AmbientStrength;
                half _DirectLightStrength;
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
                float3 positionWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
                half4 color : COLOR;
                half fogFactor : TEXCOORD2;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;

                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS);
                output.uv = input.uv;
                output.color = input.color * _Color;
                output.fogFactor = ComputeFogFactor(output.positionCS.z);

                return output;
            }

            half3 EvaluateLight(half3 normalWS, Light light)
            {
                half normalLight = saturate(dot(normalWS, normalize(light.direction)));

                return light.color *
                    normalLight *
                    light.distanceAttenuation *
                    light.shadowAttenuation;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half4 textureColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * input.color;

                clip(textureColor.a - _AlphaCutoff);

                half3 normalWS = half3(0, 1, 0);

                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.normalWS = normalWS;
                inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);

                half3 lighting = SampleSH(normalWS) * _AmbientStrength;

                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                Light mainLight = GetMainLight(shadowCoord);

                lighting += EvaluateLight(normalWS, mainLight) * _DirectLightStrength;

                #if defined(_ADDITIONAL_LIGHTS)

                    #if USE_FORWARD_PLUS

                        UNITY_LOOP for (
                            uint lightIndex = 0;
                            lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS);
                            lightIndex++
                        )
                        {
                            Light additionalLight =
                                GetAdditionalLight(lightIndex, inputData.positionWS, half4(1, 1, 1, 1));

                            lighting += EvaluateLight(normalWS, additionalLight) * _DirectLightStrength;
                        }

                    #endif

                    uint pixelLightCount = GetAdditionalLightsCount();

                    LIGHT_LOOP_BEGIN(pixelLightCount)

                        Light additionalLight =
                            GetAdditionalLight(lightIndex, inputData.positionWS, half4(1, 1, 1, 1));

                        lighting += EvaluateLight(normalWS, additionalLight) * _DirectLightStrength;

                    LIGHT_LOOP_END

                #endif

                half3 finalColor = textureColor.rgb * lighting;
                finalColor = MixFog(finalColor, input.fogFactor);

                return half4(finalColor, textureColor.a);
            }

            ENDHLSL
        }
    }
}