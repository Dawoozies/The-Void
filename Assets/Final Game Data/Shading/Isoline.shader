Shader "Unlit/Isoline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (0,0,0,1)
        _BgColor("BgColor", Color) = (1,1,1,0)
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #pragma multi_compile _ DISTORTION
            #pragma multi_compile _ MODULATION_FRAC MODULATION_SIN MODULATION_NOISE

            #include "UnityCG.cginc"
            #include "SimplexNoise3D.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float2 _MainTex_TexelSize;
            float4 _MainTex_ST;

            sampler2D_float _CameraDepthTexture;

            half4 _Color;
            half _Blend;
            float _FallOffDepth;
            half4 _BgColor;

            float3 _Axis;
            float _Density;
            float3 _Offset;

            float _DistFreq;
            float _DistAmp;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            float GetPotential(float3 worldPos)
            {
                worldPos += _Offset;
                worldPos += snoise(worldPos * _DistFreq) * _DistAmp;
                return dot(worldPos, _Axis) * _Density;
            }
            fixed4 frag(v2f i) : SV_TARGET
            {
                float4 disp = float4(_MainTex_TexelSize.xy, -_MainTex_TexelSize.x, 0);

                float2 uv0 = i.uv;
                float2 uv1 = i.uv + disp.xy;
                float2 uv2 = i.uv + disp.xw;
                float2 uv3 = i.uv + disp.wy;
                
                float z0 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv0));

                return fixed4(1,1,1,1);
            }

            ENDCG
        }
    }
}
