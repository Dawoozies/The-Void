Shader "Unlit/Displacement"
{
    Properties
    {
        _AlphaCutoff("Alpha Cutoff", Range(0,1)) = 0.2
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"}
        //Blend DstColor One
        //Blend One One
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
                fixed4 color : COLOR0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 texUV : TEXCOORD1;
                float squareValue : Float;
                fixed4 color : COLOR0;
                fixed4 texColor : COLOR1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float random( float2 p )
            {
                return frac( cos( dot(p,float2(23.14069263277926,2.665144142690225))) * 12345.6789 );
            }

            v2f vert (appdata v)
            {
                v2f o;
                float4 objectPosition = v.vertex;
                
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.texUV = TRANSFORM_TEX(v.uv, _MainTex);
                //o.color = tex2D(_MainTex, v.uv);
                

                //float2 coordinates = mul(unity_ObjectToWorld, v.vertex);
                
                o.vertex = UnityObjectToClipPos(objectPosition);
                o.uv = mul(unity_ObjectToWorld, objectPosition.xyz);

                //Figure out how to make what you need using geometry buffer
                float squaresStep = step(0.1, random(floor(o.uv*5)));
                v.vertex.xyz += float3(0,1,0) * squaresStep * random(objectPosition.xy);
                o.squareValue = squaresStep;
                o.color = v.color;
                return o;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 output = fixed4(1,1,1,1);
                fixed4 output = tex2D(_MainTex, i.texUV);
                //float squares = step(0.1, random(floor(i.uv*5)));
                if(i.squareValue == 1)
                {
                    output = fixed4(1,0,0,1);
                    //output = tex2D(_MainTex, i.texUV)*0.5;
                }
                //if(i.uv.y < 0)
                //    output -= fixed4(0,0,0,squares);
                //fixed4 col = fixed4(0,0,0,1);
                //fixed4 col = tex2D(_MainTex, i.uv);
                //fixed4 col = tex2D(_MainTex, i.uv);

                //col = i.color;
                return output;
            }

            ENDCG
        }
    }
}
