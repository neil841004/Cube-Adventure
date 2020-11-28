Shader "Zebra/LWRP/MainActor_Ding"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" { }
		_FinalAddColor ("FinalAddColor", Color) =  (0, 0, 0, 0)
        _TintColor ("TintColor", Color) =  (1, 1, 1, 1)
		_PointLightPos("PointLightPos", Vector) = (0, 0, 0, 0)
		_PointLightColor("PointLightColor", Color) = (0, 0, 0, 0)

		[Space(10)]
		[Header(OutLine)]
        _EdgeThickness ("Outline粗細", Float) = 1
		_OutLineColor ("Outline Color", Color) = (0, 0, 0, 1)

		[Space(10)]
		[Header(Shadow)]
		_ShadowColor ("陰影顏色", Color) = (1, 1, 1, 1)
		[Header(Shadow_Lambert)]
		_CelValueMin ("_CelValueMin", Range(0, 1)) = 0.4
		_CelValueMax("_CelValueMax", Range(0, 1)) = 0.5
		
		[Toggle(ENABLE_FOG)] _Fog("Enable Fog", Float) = 0

		[Toggle(ENABLE_SCREEN_DOOR_TRANSPARENCY)] _ScreenDoorTransparency("Enable Screen Door Transparency", Float) = 0
		_Alpha("Alpha", Float) = 1

		[Enum(Off, 0, On, 1)] _ZWrite("ZWrite", Float) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Depth test", Float) = 4

			
    }
    SubShader
    {
		Tags 
		{ 
			"RenderType" = "Opaque"
			"RenderPipeline" = "LightweightPipeline"
		}
        Pass
        {
            Name "Toon"
			Tags
			{
				"LightMode" = "LightweightForward" 
			}

			ZTest [_ZTest]
            ZWrite [_ZWrite]
            Cull Back

            HLSLPROGRAM
			
			#pragma shader_feature_local ENABLE_SHARP_RIMLIGHT

            #pragma multi_compile_fog
			#pragma multi_compile __ HIDE_ON
			#pragma multi_compile_local __ ENABLE_NEGATIVE_COLOR
			#pragma multi_compile_local __ ENABLE_SCREEN_DOOR_TRANSPARENCY

			// Unity defined keywords
			#pragma shader_feature_local ENABLE_FOG

            #pragma target 4.5

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
            
			float _CelValueMin,_CelValueMax;
			half3 _ShadowColor;

			float _Alpha;
			float _Gray;
			float3 _FinalAddColor;
			float3 _TintColor;

			float4 _WorldSpaceLightPos0;

            struct a2v
            {
                float4 positionOS: POSITION;
                float2 uv: TEXCOORD0;
                float3 normalOS: NORMAL;
				float4 vertexColor: COLOR;
            };

            struct v2f
            {
                float2 uv: TEXCOORD0;
                float4 vertex: SV_POSITION;
                float3 normalWS: TEXCOORD1;
                float4 camDirWS: TEXCOORD2; 
                float4 shadowCoord: TEXCOORD3;
				float3 posWS: TEXCOORD6;
				float4 vertexColor: COLOR;
#if ENABLE_FOG
				float fogFactor : TEXCOORD4;
#endif
#if ENABLE_SCREEN_DOOR_TRANSPARENCY
				float4 screenPos: TEXCOORD5;
#endif
            };

            v2f vert(a2v i)
            {
                v2f o = (v2f)0;

                float3 normalWS = TransformObjectToWorldNormal(i.normalOS);
                float3 positionWS = TransformObjectToWorld(i.positionOS.xyz);
				o.posWS = positionWS;
                float4 positionCS = TransformObjectToHClip(i.positionOS.xyz);
				o.vertexColor = i.vertexColor;

                o.normalWS = normalWS;
                o.camDirWS.xyz = normalize(_WorldSpaceCameraPos - positionWS);
				//w存模型中心離攝影機的距離
				o.camDirWS.w = length(_WorldSpaceCameraPos - TransformObjectToWorld(float3(0,0,0)));

				
                o.vertex = positionCS;
#if ENABLE_SCREEN_DOOR_TRANSPARENCY
				o.screenPos = ComputeScreenPos(o.vertex);
#endif 
                o.uv.xy = TRANSFORM_TEX(i.uv, _BaseMap);
                o.shadowCoord = ComputeScreenPos(positionCS);


				#if ENABLE_FOG
					o.fogFactor = ComputeFogFactor(positionCS.z);
				#endif

                return o;
            }

			float Lambert(float3 lightDir, float3 normal)
			{
				half Ndot = saturate(dot(normal, lightDir));
				return Ndot;
			}

			float4 frag(v2f input) : SV_Target
			{
#if HIDE_ON
					discard;
#endif
#if ENABLE_SCREEN_DOOR_TRANSPARENCY
				const float4x4 thresholdMatrix =
				{
				1,   9,  3, 11,
				13,  5, 15,  7,
				4,  12,  2, 10,
				16,  8, 14,  6
				};
				float2 pixelPos = input.screenPos.xy / input.screenPos.w * _ScreenParams.xy;
				float threshold = thresholdMatrix[pixelPos.x % 4][pixelPos.y % 4] / 17;
				clip(_Alpha - threshold);
#endif

                float4 diffuseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                float3 combinedColor = diffuseColor;
				
				float3 cameraDir = float3(0, 0, 1);
                float normalDotEye = dot(normalize(input.normalWS), cameraDir);
				float falloffU = 1 - abs(normalDotEye);
                
			
				float attenuation = 1;
				float3 shadowColor = _ShadowColor.rgb * combinedColor;

                //JOJO蘭伯特
                Light mainLight = GetMainLight();
				half Ndot = saturate(dot(input.normalWS, _MainLightPosition.xyz));
                float lambert = Lambert(_WorldSpaceLightPos0.xyz, input.normalWS);
				
				attenuation = smoothstep(_CelValueMin, _CelValueMax, lambert);

				combinedColor = lerp(shadowColor, combinedColor, attenuation);

#if ENABLE_FOG
				combinedColor.rgb = MixFog(combinedColor, input.fogFactor);
				combinedColor.rgb = lerp(combinedColor, 1, input.fogFactor * 0.3);
#endif

				combinedColor.rgb += _FinalAddColor.rgb;
				
                return float4(combinedColor * _TintColor, 1);
            }
            ENDHLSL
            
        }

		Pass
		{
			Name "HalfTone Flag"

			Tags
			{
				"RenderType" = "Opaque"
				"LightMode" = "HalfToneFlag"
			}

			ZWrite Off
			Cull Back 

			HLSLPROGRAM

			#pragma target 4.5
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ HIDE_ON

			#include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"

			struct a2v
			{
				float4 positionOS: POSITION;
			};

			struct v2f
			{
				float4 vertex: SV_POSITION;
			};

			v2f vert(a2v v)
			{
				v2f o = (v2f)0;
				o.vertex = TransformObjectToHClip(v.positionOS);
				return o;
			}

			half frag(v2f input) : SV_Target
			{
				#if HIDE_ON
					discard;
				#endif
				return 1;
			}
			ENDHLSL

		}

		Pass
		{
			Name "Outline"
			Tags
			{
				"RenderType" = "Opaque"
				"LightMode" = "Outline"
			}
			Cull Front
			ZTest Less
			//Blend[_SourceBlend][_DestBlend]

			HLSLPROGRAM

			#pragma multi_compile __ HIDE_ON
			#pragma multi_compile __ HIDE_OUTLINE
			#pragma multi_compile_local __ ENABLE_SCREEN_DOOR_TRANSPARENCY
			#pragma target 4.5

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"


			float _EdgeThickness = 1.0;
			float4 _OutLineColor;
			float _Alpha;
			struct a2v
			{
				float4 positionOS: POSITION;
				float2 uv: TEXCOORD0;
				float3 normalOS: NORMAL;
				float4 color: COLOR;
			};

			struct v2f
			{
				float4 vertex: SV_POSITION;
				float2 uv: TEXCOORD0;
				float2 normal: TEXCOORD1;
				float4 screenPos: TEXCOORD5;
			};

			#define INV_EDGE_THICKNESS_DIVISOR 0.00285
			v2f vert(a2v input)
			{
				v2f output = (v2f)0;
				float shift = input.color.r;

				float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
				float3 positionWS = TransformObjectToWorld(input.positionOS.xyz) + _EdgeThickness * INV_EDGE_THICKNESS_DIVISOR * normalize(normalWS) * shift;
				float4 positionCS = TransformWorldToHClip(positionWS);

				output.uv.xy = TRANSFORM_TEX(input.uv, _BaseMap);

				half4 projSpaceNormal = normalize(TransformObjectToHClip(input.normalOS));
				output.normal = normalWS;
				half4 scaledNormal = _EdgeThickness * INV_EDGE_THICKNESS_DIVISOR * projSpaceNormal;

				//scaledNormal.z += 0.00001;
				output.vertex = positionCS + scaledNormal * shift;

#if ENABLE_SCREEN_DOOR_TRANSPARENCY
				output.screenPos = ComputeScreenPos(output.vertex);
#endif 

				return output;
			}

			half4 frag(v2f input) : SV_Target
			{
				#if ENABLE_SCREEN_DOOR_TRANSPARENCY
				const float4x4 thresholdMatrix =
				{
				1,   9,  3, 11,
				13,  5, 15,  7,
				4,  12,  2, 10,
				16,  8, 14,  6
				};
				float2 pixelPos = input.screenPos.xy / input.screenPos.w * _ScreenParams.xy;
				float threshold = thresholdMatrix[pixelPos.x % 4][pixelPos.y % 4] / 17;
				clip(_Alpha - threshold);
				#endif

				#if HIDE_ON || HIDE_OUTLINE
					discard;
				#endif

				float4 diffuseMapColor = _OutLineColor;
				return float4(diffuseMapColor.rgb , 1);
			}
			ENDHLSL

		}
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            Cull[_Cull]
            
            HLSLPROGRAM
            
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _GLOSSINESS_FROM_BASE_ALPHA

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL   
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask 0
            Cull[_Cull]
            
            HLSLPROGRAM
            
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _GLOSSINESS_FROM_BASE_ALPHA

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
            
        }
    }
    FallBack "Hidden/InternalErrorShader"
}
