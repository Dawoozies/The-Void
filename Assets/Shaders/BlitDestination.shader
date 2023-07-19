Shader "Unlit/BlitDestination"
{
    Properties
    {
        _BlitOutput("New Texture", 2D) = "white" {}
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "Blit"
            //
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // The Blit.hlsl file provides the vertex shader (Vert),
            // input structure (Attributes) and output strucutre (Varyings)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            TEXTURE2D_X(_BlitPassTexture);
            SAMPLER(sampler_BlitPassTexture);
            TEXTURE2D(_BlitOutput);
            SAMPLER(sampler_BlitOutput);
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _Intensity;

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                //float4 color = SAMPLE_TEXTURE2D_X(_BlitPassTexture, sampler_BlitPassTexture, input.texcoord);
                //float4 color = SAMPLE_TEXTURE2D(_BlitOutput, sampler_BlitOutput, input.texcoord);
                float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.texcoord);
                return color;
            }
            ENDHLSL
        }
    }
}
