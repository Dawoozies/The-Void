Shader "Unlit/Blit"
{
    Properties
    {
        //Main texture here is not input
        //This is what gets actively written to by source
        _NewTexture("New Texture", 2D) = "white" {}
        _AlphaCutoff("Alpha Cutoff", Range(0,1)) = 0.2
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NewTexture;
            float4 _NewTexture_ST;
            float _AlphaCutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _NewTexture);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 output = tex2D(_NewTexture, i.uv);
                if(output.a < _AlphaCutoff)
                    discard;
                return output;
            }
            ENDCG
        }
    }
}
