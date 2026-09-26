Shader "SuperGenius/BrushedMetal"
{
    Properties
    {
        _BaseColor("Metal Color", Color) = (0.62,0.66,0.70,1)
        _NormalMap("Brush Normal Map", 2D) = "bump" {}
        _PackedResponseLUT("Historical Packed Response", 2D) = "white" {}
        _BrushTiling("Brush Tiling", Vector) = (6,24,0,0)
        _NormalStrength("Normal Strength", Range(0,2)) = 0.6
        _LegacySpecularPower("Legacy Specular Power", Range(1,256)) = 64
        _RoughnessT("Tangent Roughness", Range(0.02,1)) = 0.16
        _RoughnessB("Bitangent Roughness", Range(0.02,1)) = 0.48
        [Enum(HistoricalPacked,0,IsotropicGGX,1,AnisotropicGGX,2,AnisotropicScratches,3)] _Mode("Mode", Float) = 3
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "BrushedMetalLighting.hlsl"

            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
            TEXTURE2D(_PackedResponseLUT); SAMPLER(sampler_PackedResponseLUT);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BrushTiling;
                float _NormalStrength;
                float _LegacySpecularPower;
                float _RoughnessT;
                float _RoughnessB;
                float _Mode;
            CBUFFER_END

            struct Attributes { float4 positionOS:POSITION; float3 normalOS:NORMAL; float4 tangentOS:TANGENT; float2 uv:TEXCOORD0; };
            struct Varyings { float4 positionCS:SV_POSITION; float3 positionWS:TEXCOORD0; float3 normalWS:TEXCOORD1; float3 tangentWS:TEXCOORD2; float3 bitangentWS:TEXCOORD3; float2 uv:TEXCOORD4; };

            Varyings vert(Attributes i)
            {
                Varyings o; VertexPositionInputs p=GetVertexPositionInputs(i.positionOS.xyz); VertexNormalInputs n=GetVertexNormalInputs(i.normalOS,i.tangentOS);
                o.positionCS=p.positionCS; o.positionWS=p.positionWS; o.normalWS=normalize(n.normalWS); o.tangentWS=normalize(n.tangentWS); o.bitangentWS=normalize(n.bitangentWS); o.uv=i.uv; return o;
            }

            float3 BrushNormal(float2 uv, float3 N, float3 T, float3 B)
            {
                float3 tex = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uv * _BrushTiling.xy).xyz * 2.0 - 1.0;
                tex.xy *= _NormalStrength; tex = normalize(tex);
                return normalize(T*tex.x + B*tex.y + N*tex.z);
            }

            half4 frag(Varyings i):SV_Target
            {
                float3 N0=normalize(i.normalWS), T=normalize(i.tangentWS), B=normalize(i.bitangentWS);
                float3 V=normalize(GetWorldSpaceViewDir(i.positionWS));
                Light light=GetMainLight(TransformWorldToShadowCoord(i.positionWS)); float3 L=normalize(light.direction); float3 H=normalize(V+L);
                float shadow=light.shadowAttenuation*light.distanceAttenuation;

                float3 N = (_Mode > 2.5 || _Mode < 0.5) ? BrushNormal(i.uv,N0,T,B) : N0;
                float NoL=saturate(dot(N,L)), NoH=saturate(dot(N,H));
                float3 color=0;

                if (_Mode < 0.5)
                {
                    // Explicit reconstruction of the surviving page description:
                    // RGB stores N.L and alpha stores N.H.
                    float4 packed=SAMPLE_TEXTURE2D(_PackedResponseLUT,sampler_PackedResponseLUT,float2(NoL,NoH));
                    float3 body=_BaseColor.rgb*packed.rgb;
                    float3 spec=_BaseColor.rgb*pow(saturate(packed.a),_LegacySpecularPower);
                    color=body+spec;
                }
                else
                {
                    float ax = (_Mode < 1.5) ? 0.30 : _RoughnessT;
                    float ay = (_Mode < 1.5) ? 0.30 : _RoughnessB;
                    float3 brdf=SG_AnisotropicGGX(N,V,L,T,B,ax,ay,_BaseColor.rgb);
                    color=brdf*NoL;
                }

                color*=light.color*shadow;
                color += _BaseColor.rgb * 0.025;
                return half4(color,1);
            }
            ENDHLSL
        }
    }
}
