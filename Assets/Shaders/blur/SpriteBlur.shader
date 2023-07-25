Shader "Unlit/SpriteBlur"
{
    Properties
    {
        _Twisting("Twisting", Range(-3,3)) = 0
        _AlphaCutoff("Alpha Cutoff", Range(0.0,1.0))=0.5
        _BlurVector("Blur Size", Vector) = (0,0,0,0)
        _MainTex ("Texture", 2D) = "white" {}
        _CellSize("Cell Size", Float) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite off
        Cull off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityShaderVariables.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _AlphaCutoff;
            float4 _BlurVector;
            float _CellSize;

            float _Twisting;

            v2f vert (appdata v)
            {
                v2f o;
                //v.vertex.y*=2;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float angle = _Twisting*length(v.vertex);
                float cosLength, sinLength;
                sincos(angle, sinLength, cosLength);
                //o.vertex[0] = cosLength*v.vertex[0] - sinLength*v.vertex[1];
                //o.vertex[1] = sinLength*v.vertex[0] + cosLength*v.vertex[1];
                //o.vertex[0] = v.vertex[0];
                //o.vertex[1] = v.vertex[1];
                //o.vertex[2] = 0;
                //o.vertex[3] = 1;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                //o.vertex = UnityPixelSnap(o.vertex);
                return o;
            }
            fixed4 frag (v2f i) : SV_Target
            {   
                
                float2 cellSize = float2(_CellSize, _CellSize);
                float2 steppedUV = i.uv.xy;
                steppedUV /= cellSize;
                steppedUV = round(steppedUV);
                steppedUV *= cellSize;

                fixed4 col = tex2D(_MainTex, steppedUV)*i.color;

                
                //fixed4 col = tex2D(_MainTex, i.uv);
                //Process alpha cutoff before we do blur
                col.a = smoothstep(0,_AlphaCutoff, col.a);
                fixed4 blur = fixed4(0.0,0.0,0.0,0.0);
                blur += tex2D(_MainTex, float2(i.uv.x,i.uv.y)+_BlurVector.xy);
                
                fixed4 finalCol = lerp(col, blur, 0.5);
                return finalCol;
            }
            ENDCG
        }
    }
}
