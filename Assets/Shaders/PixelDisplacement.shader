Shader "Unlit/PixelDisplacement"
{
    Properties
    {
        _AlphaCutoff("Alpha Cutoff", Range(0,1)) = 0.2
        _SquareSize("Square Size", Float) = 5
        _SquareDensity("Square Density", Range(0,1)) = 0.1
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
                float2 uv : TEXCOORD0;
                float2 texUV : TEXCOORD1;
                fixed4 overlayColor : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _AlphaCutoff;
            float _SquareSize;
            float _SquareDensity;

            float random( float2 uv )
            {
                return frac( cos( dot(uv,float2(23.14069263277926,2.665144142690225))) * 12345.6789 );
            }

            v2f vert (appdata v)
            {
                v2f o;
                float4 objectPos = v.vertex;
                o.texUV = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertex = UnityObjectToClipPos(objectPos);
                o.uv = mul(unity_ObjectToWorld, objectPos.xyz);
                o.overlayColor = v.overlayColor;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 output = fixed4(1,1,1,1);
                fixed4 spriteOutput = tex2D(_MainTex, i.texUV);
                float squareValue = step(_SquareDensity, random(floor(i.uv*_SquareSize)));
                if(squareValue == 1)
                {
                    output = fixed4(0,0,0,0);
                }
                return output;
            }
            ENDCG
        }
    }
}
