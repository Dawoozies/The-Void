Shader "Unlit/PixelDisplacement"
{
    Properties
    {
        _AlphaCutoff("Alpha Cutoff", Range(0,1)) = 0.2
        _EditedTexture ("Edited Texture", 2D) = "white" {}
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
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
                fixed4 overlayColor : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 mainUV : TEXCOORD0;
                float2 editUV : TEXCOORD1;
                fixed4 overlayColor : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _EditedTexture;
            float4 _EditedTexture_ST;
            float _AlphaCutoff;

            float random( float2 uv )
            {
                return frac( cos( dot(uv,float2(23.14069263277926,2.665144142690225))) * 12345.6789 );
            }

            v2f vert (appdata v)
            {
                v2f o;
                float4 objectPos = v.vertex;
                //float2 editUV = v.uv/8;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.mainUV = TRANSFORM_TEX(v.uv, _MainTex);
                o.editUV = TRANSFORM_TEX(v.uv, _EditedTexture);
                o.overlayColor = v.overlayColor;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 output = fixed4(1,1,1,1);
                //fixed4 spriteOutput = tex2D(_MainTex, i.texUV);
                //fixed4 spriteOutput = tex2D(_EditedTexture, i.texUV);
                //fixed4 editedOutput = tex2D(_EditedTexture, i.texUV);
                fixed4 spriteOutput = tex2D(_MainTex, i.mainUV);
                fixed4 editedOutput = tex2D(_EditedTexture, i.editUV);
                output = spriteOutput;
                if(editedOutput.r == 1)
                    output = fixed4(1,0,0,1);
                if(editedOutput.g == 1)
                    output = fixed4(0,1,0,1);
                if(spriteOutput.a < _AlphaCutoff)
                    discard;
                return output;
            }
            ENDCG
        }
    }
}
