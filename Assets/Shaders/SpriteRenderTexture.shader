Shader "Unlit/SpriteRenderTexture"
{
    Properties
    {
        _AlphaCutoff("Alpha Cutoff", Range(0,1)) = 0.2
        _RenderTexture("Render Texture (Given By BLIT)", 2D) = "white" {} //THE TEXTURE GIVEN BY THE BLIT
        _MainTex ("Texture", 2D) = "white" {} //I.E THE TEXTURE GIVEN BY SPRITE RENDERER
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _RenderTexture;
            float4 _RenderTexture_ST;
            float _AlphaCutoff;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 finalOutput = fixed4(1,1,1,1);
                fixed4 blitOutput = tex2D(_RenderTexture, i.uv);
                //fixed4 spriteOutput = tex2D(_MainTex, i.uv);
                return blitOutput;
            }
            ENDCG
        }
    }
}
