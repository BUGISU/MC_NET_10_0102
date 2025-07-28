Shader "Custom/IceShaderURP"
{
    Properties
    {
        _IceTex ("Ice Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _Smoothness ("Smoothness", Range(0,1)) = 0.9
        _RefractionStrength ("Refraction Strength", Range(0, 1)) = 0.1
        _FresnelPower ("Fresnel Power", Range(0.5, 5)) = 2.5
        _Transparency ("Transparency", Range(0,1)) = 0.7
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 300
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldViewDir : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
            };

            sampler2D _IceTex;
            sampler2D _NormalMap;
            float _Smoothness;
            float _RefractionStrength;
            float _FresnelPower;
            float _Transparency;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.uv = IN.uv;
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                OUT.worldViewDir = normalize(_WorldSpaceCameraPos - worldPos);
                OUT.worldPos = worldPos;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Normal mapping (optional)
                float3 normal = normalize(IN.worldNormal);

                // Fresnel (edge glow effect)
                float fresnel = pow(1.0 - saturate(dot(normalize(IN.worldViewDir), normal)), _FresnelPower);

                // Ice base color
                float4 iceColor = tex2D(_IceTex, IN.uv);

                // Simulated refraction (fake screen distortion)
                float2 offsetUV = IN.uv + normal.xy * _RefractionStrength;
                float4 refractedColor = tex2D(_IceTex, offsetUV);

                // Final color mix
                float4 finalColor = lerp(iceColor, refractedColor, 0.5);
                finalColor.rgb += fresnel * 0.4; // edge light
                finalColor.a = _Transparency;

                return finalColor;
            }
            ENDHLSL
        }
    }
}
